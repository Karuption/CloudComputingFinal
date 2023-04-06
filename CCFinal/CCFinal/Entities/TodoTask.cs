namespace CCFinal.Entities;

public class ToDoTask {
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Created { get; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;
    public TaskType TaskType { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsFavorite { get; set; }
    public int UserID {get; set; }
    public Guid IntegrationId {get; set; }
}