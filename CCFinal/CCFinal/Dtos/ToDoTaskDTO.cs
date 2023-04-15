namespace CCFinal.Dtos;

public class ToDoTaskDTO {
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Created { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime Updated { get; set; } = DateTime.UtcNow;
    public TaskTypeDTO TaskType { get; set; }
    public bool IsFavorite {get; set; }
    public bool IsCompleted { get; set; }
}