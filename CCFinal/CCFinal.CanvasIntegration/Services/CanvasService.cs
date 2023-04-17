using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using CCFinal.CanvasIntegration.Database;
using CCFinal.CanvasIntegration.Entities;
using CCFinal.CanvasIntegration.PublishEvents;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.CanvasIntegration.Services;

public interface ICanvasService {
    Task<List<UserInformation>> GetUsers(TimeSpan? updateFrequency = null);
    Task ProcessUser(UserInformation userInfo, CancellationToken stoppingToken = default);
}

public class CanvasService : ICanvasService {
    private readonly IServiceProvider service;
    private readonly CanvasIntegrationDbContext _dbContext;
    private readonly TimeSpan _defaultUpdateFrequency = TimeSpan.FromMinutes(5);
    private readonly ILogger<CanvasService> _logger;
    private readonly HttpClient _client;
    private readonly ICapPublisher _capPublisher;

    private readonly string _requestString = """
        {
          "query": "query Integration { allCourses { id name updatedAt assignmentsConnection { nodes { createdAt updatedAt dueAt id name unlockAt description submissionTypes htmlUrl } } }}"
        }
        """;

    public CanvasService(IServiceProvider service,
        ILogger<CanvasService> logger,
        HttpClient client,
        ICapPublisher capPublisher, CanvasIntegrationDbContext dbContext) {
        this.service = service;
        _logger = logger;
        _client = client;
        _capPublisher = capPublisher;
        _dbContext = dbContext;
    }

    public async Task<List<UserInformation>> GetUsers(TimeSpan? updateFrequency = null) {
        var timeOffset = DateTime.UtcNow.Subtract(updateFrequency ?? _defaultUpdateFrequency);

        List<UserInformation> output = new();

        if (await _dbContext.Information.AnyAsync())
            output = await _dbContext.Information.Where(x => x.LastRunDateTime < timeOffset)
                .ToListAsync();

        return output;
    }

    public async Task ProcessUser(UserInformation userInfo, CancellationToken stoppingToken = default) {
        // Preparing and sending external API request
        var request = new HttpRequestMessage(HttpMethod.Post, "graphql");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userInfo.Token);
        request.Content = new StringContent(_requestString, null, MediaTypeNames.Application.Json);

        var response = await _client.SendAsync(request, stoppingToken);

        // Handling API request
        if (!response.IsSuccessStatusCode)
            _logger.LogDebug(
                $"""
                            User ID: {userInfo.UserID} 
                            Response code: {response.StatusCode} 
                            Reason: {response.ReasonPhrase ?? ""} 
                            Content: {await response.Content.ReadAsStringAsync(stoppingToken)}
                        """);

        Course[] courses = Array.Empty<Course>();
        if (response.IsSuccessStatusCode) {
            var data = await response.Content.ReadFromJsonAsync<Data>((JsonSerializerOptions?)default, stoppingToken);
            if (data is null)
                _logger.LogDebug("Unable to unpack request");
            courses = data.data.allCourses ?? Array.Empty<Course>();
            userInfo.LastRunDateTime = DateTime.UtcNow;
        }

        // Logging updated courses
        var maxUpdate = new DateTime();
        foreach (var course in courses) {
            if (course.UpdatedAt > userInfo.LastCanvasUpdateDateTime)
                foreach (var assignment in course.AssignmentsConnection.Nodes) {
                    if (assignment.UpdatedAt > userInfo.LastCanvasUpdateDateTime) {
                        _capPublisher.Publish("TaskUpsert",
                            new ToDoTaskIntegrationDto(default,
                                assignment.Id,
                                assignment.CreatedAt,
                                assignment.DueAt,
                                userInfo.UserID,
                                assignment.SubmissionTypes.ParseTaskType(),
                                false,
                                false,
                                $"{course.Name} : {assignment.Name}",
                                assignment.Description,
                                assignment.UpdatedAt));

                        maxUpdate = maxUpdate.MaxDateTime(assignment.UpdatedAt);
                        _logger.LogInformation($"""
                        Updated Course Name: {course.Name}
                            Assignment Name: {assignment.Name}
                            Assignment Created: {assignment.CreatedAt}
                            Assignment Updated: {assignment.UpdatedAt}
                            Assignment Due: {assignment.DueAt}
                        """);
                    }
                }
        }

        //Sending last run update to DB
        userInfo.LastCanvasUpdateDateTime = maxUpdate;
        await _dbContext.SaveChangesAsync(stoppingToken);
    }
}
