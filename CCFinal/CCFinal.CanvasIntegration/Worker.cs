using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using CCFinal.CanvasIntegration.Database;

namespace CCFinal.CanvasIntegration;

public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _client;
    private readonly CanvasIntegrationDbContext _dbContext;

    private readonly string _requestString = """
        {
          "query": "query Integration { allCourses { id name updatedAt assignmentsConnection { nodes { createdAt updatedAt dueAt id name unlockAt description submissionTypes htmlUrl } } }}"
        }
        """;

    public Worker(ILogger<Worker> logger, HttpClient client, CanvasIntegrationDbContext dbContext) {
        _logger = logger;
        _client = client;
        _dbContext = dbContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Worker Started");

        //Get User metadata
        var currentUser = _dbContext.Information.First();
        var lastRunTime = currentUser.LastRunDateTime
                          ?? DateTime.UtcNow.Subtract(TimeSpan.FromDays(7)); //default to the last week

        // Preparing and sending external API request
        var request = new HttpRequestMessage(HttpMethod.Post, "graphql");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);
        request.Content = new StringContent(_requestString, null, MediaTypeNames.Application.Json);

        var response = await _client.SendAsync(request, stoppingToken);

        // Handling API request
        if (!response.IsSuccessStatusCode)
            _logger.LogDebug(
                $"""
                            User ID: {currentUser.UserID} 
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
        }

        // Logging updated courses
        var maxUpdate = new DateTime();
        foreach (var course in courses) {
            if (course.UpdatedAt > lastRunTime)
                foreach (var assignment in course.AssignmentsConnection.Nodes) {
                    if (assignment.UpdatedAt > lastRunTime) {
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
        currentUser.LastRunDateTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(stoppingToken);

        if (stoppingToken.IsCancellationRequested)
            _logger.LogInformation("Cancellation requested");

        Console.ReadKey();
    }
}