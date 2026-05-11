using TaskTracker.API.Controllers;
using TaskTracker.API.Models;

namespace TaskTracker.API.Validators;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

public static class TaskValidator
{
    /// <summary>
    /// Validates a create task request
    /// </summary>
    public static ValidationResult ValidateCreateTask(CreateTaskRequest request)
    {
        var result = new ValidationResult { IsValid = true };

        // Validate Title
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            result.Errors.Add("Title is required");
            result.IsValid = false;
        }
        else if (request.Title.Length > 100)
        {
            result.Errors.Add("Title cannot exceed 100 characters");
            result.IsValid = false;
        }

        // Validate Status
        if (request.Status.HasValue && !Enum.IsDefined(typeof(TaskStatus), request.Status))
        {
            result.Errors.Add("Invalid status value");
            result.IsValid = false;
        }

        // Validate Done Status Rule
        if (request.Status == TaskStatusInfo.Done && string.IsNullOrWhiteSpace(request.Title))
        {
            result.Errors.Add("A task cannot be marked as Done if the Title is empty or whitespace");
            result.IsValid = false;
        }

        return result;
    }

    /// <summary>
    /// Validates an update task request
    /// </summary>
    public static ValidationResult ValidateUpdateTask(UpdateTaskRequest request, int taskId)
    {
        var result = new ValidationResult { IsValid = true };

        // Validate Title
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            result.Errors.Add("Title is required");
            result.IsValid = false;
        }
        else if (request.Title.Length > 100)
        {
            result.Errors.Add("Title cannot exceed 100 characters");
            result.IsValid = false;
        }

        // Validate Status
        if (request.Status.HasValue && !Enum.IsDefined(typeof(TaskStatus), request.Status))
        {
            result.Errors.Add("Invalid status value");
            result.IsValid = false;
        }

        // Validate Done Status Rule
        if (request.Status == TaskStatusInfo.Done && string.IsNullOrWhiteSpace(request.Title))
        {
            result.Errors.Add("A task cannot be marked as Done if the Title is empty or whitespace");
            result.IsValid = false;
        }

        return result;
    }
}
