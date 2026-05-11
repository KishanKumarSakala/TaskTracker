using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public TaskStatusInfo Status { get; set; } = TaskStatusInfo.Todo;

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public enum TaskStatusInfo
{
    Todo = 0,
    InProgress = 1,
    Done = 2
}