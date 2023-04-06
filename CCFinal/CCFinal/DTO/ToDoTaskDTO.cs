namespace CCFinal.DTO;

public class ToDoTaskDTO {
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Created { get; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;
    public TaskTypeDTO TaskType { get; set; }
    public bool IsCompleted { get; set; }
}