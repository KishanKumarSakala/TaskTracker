using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.API.Data;
using TaskTracker.API.Models;
using TaskTracker.API.Validators;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskTrackerDbContext _dbContext;
    private readonly ILogger<TasksController> _logger;

    public TasksController(TaskTrackerDbContext dbContext, ILogger<TasksController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskItemDto>> CreateTask([FromBody] CreateTaskRequest request)
    {
        var validationResult = TaskValidator.ValidateCreateTask(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning($"Task creation validation failed: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(new { errors = validationResult.Errors });
        }

        var taskItem = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status ?? TaskStatusInfo.Todo,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Tasks.Add(taskItem);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Task created with ID: {taskItem.Id}");

        return CreatedAtAction(nameof(GetTask), new { id = taskItem.Id }, MapToDto(taskItem));
    }

    /// <summary>
    /// Get all tasks
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTasks()
    {
        var tasks = await _dbContext.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return Ok(tasks.Select(MapToDto).ToList());
    }

    /// <summary>
    /// Get a task by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> GetTask(int id)
    {
        var task = await _dbContext.Tasks.FindAsync(id);
        if (task == null)
        {
            _logger.LogWarning($"Task with ID {id} not found");
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        return Ok(MapToDto(task));
    }

    /// <summary>
    /// Update a task
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
    {
        var task = await _dbContext.Tasks.FindAsync(id);
        if (task == null)
        {
            _logger.LogWarning($"Task with ID {id} not found for update");
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        var validationResult = TaskValidator.ValidateUpdateTask(request, task.Id);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning($"Task update validation failed for ID {id}: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(new { errors = validationResult.Errors });
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status ?? task.Status;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        _dbContext.Tasks.Update(task);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Task {id} updated successfully");

        return Ok(MapToDto(task));
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _dbContext.Tasks.FindAsync(id);
        if (task == null)
        {
            _logger.LogWarning($"Task with ID {id} not found for deletion");
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Task {id} deleted successfully");

        return NoContent();
    }

    private static TaskItemDto MapToDto(TaskItem task)
    {
        return new TaskItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatusInfo? Status { get; set; }
    public DateTime? DueDate { get; set; }
}

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatusInfo? Status { get; set; }
    public DateTime? DueDate { get; set; }
}

public class TaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Todo";
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
