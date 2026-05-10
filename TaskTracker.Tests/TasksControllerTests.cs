using Xunit;
using Microsoft.EntityFrameworkCore;
using TaskTracker.API.Controllers;
using TaskTracker.API.Data;
using TaskTracker.API.Models;
using Microsoft.Extensions.Logging;

namespace TaskTracker.Tests;

public class TasksControllerTests
{
    private TaskTrackerDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TaskTrackerDbContext(options);
    }

    private ILogger<TasksController> CreateMockLogger()
    {
        return new MockLogger<TasksController>();
    }

    #region Create Task Tests

    [Fact]
    public async Task CreateTask_WithValidData_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var controller = new TasksController(dbContext, CreateMockLogger());
        var request = new CreateTaskRequest
        {
            Title = "Test Task",
            Description = "Test Description",
            Status = TaskStatus.Todo,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var result = await controller.CreateTask(request);

        // Assert
        var createdResult = result.Result as Microsoft.AspNetCore.Mvc.CreatedAtActionResult;
        Assert.NotNull(createdResult);
        Assert.Equal(nameof(TasksController.GetTask), createdResult.ActionName);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);

        var returnedDto = createdResult.Value as TaskItemDto;
        Assert.NotNull(returnedDto);
        Assert.Equal("Test Task", returnedDto.Title);
        Assert.Equal("Test Description", returnedDto.Description);
        Assert.Equal("Todo", returnedDto.Status);

        // Verify it's in the database
        var savedTask = await dbContext.Tasks.FirstAsync();
        Assert.Equal("Test Task", savedTask.Title);
    }

    [Fact]
    public async Task CreateTask_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var controller = new TasksController(dbContext, CreateMockLogger());
        var request = new CreateTaskRequest
        {
            Title = "",
            Description = "Test Description"
        };

        // Act
        var result = await controller.CreateTask(request);

        // Assert
        var badRequestResult = result.Result as Microsoft.AspNetCore.Mvc.BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WithDoneStatusAndEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var controller = new TasksController(dbContext, CreateMockLogger());
        var request = new CreateTaskRequest
        {
            Title = "",
            Status = TaskStatus.Done
        };

        // Act
        var result = await controller.CreateTask(request);

        // Assert
        var badRequestResult = result.Result as Microsoft.AspNetCore.Mvc.BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    #endregion

    #region Update Task Tests

    [Fact]
    public async Task UpdateTask_WithValidData_ReturnsOkWithUpdatedTask()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var existingTask = new TaskItem
        {
            Title = "Original Title",
            Description = "Original Description",
            Status = TaskStatus.Todo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.Tasks.Add(existingTask);
        await dbContext.SaveChangesAsync();

        var controller = new TasksController(dbContext, CreateMockLogger());
        var request = new UpdateTaskRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Status = TaskStatus.InProgress
        };

        // Act
        var result = await controller.UpdateTask(existingTask.Id, request);

        // Assert
        var okResult = result.Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        var returnedDto = okResult.Value as TaskItemDto;
        Assert.NotNull(returnedDto);
        Assert.Equal("Updated Title", returnedDto.Title);
        Assert.Equal("Updated Description", returnedDto.Description);
        Assert.Equal("InProgress", returnedDto.Status);

        // Verify it's updated in the database
        var updatedTask = await dbContext.Tasks.FirstAsync();
        Assert.Equal("Updated Title", updatedTask.Title);
        Assert.Equal(TaskStatus.InProgress, updatedTask.Status);
    }

    [Fact]
    public async Task UpdateTask_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var existingTask = new TaskItem
        {
            Title = "Original Title",
            Status = TaskStatus.Todo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.Tasks.Add(existingTask);
        await dbContext.SaveChangesAsync();

        var controller = new TasksController(dbContext, CreateMockLogger());
        var request = new UpdateTaskRequest
        {
            Title = ""
        };

        // Act
        var result = await controller.UpdateTask(existingTask.Id, request);

        // Assert
        var badRequestResult = result.Result as Microsoft.AspNetCore.Mvc.BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_WithDoneStatusAndValidTitle_ReturnsOk()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var existingTask = new TaskItem
        {
            Title = "Task to Complete",
            Status = TaskStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.Tasks.Add(existingTask);
        await dbContext.SaveChangesAsync();

        var controller = new TasksController(dbContext, CreateMockLogger());
        var request = new UpdateTaskRequest
        {
            Title = "Task to Complete",
            Status = TaskStatus.Done
        };

        // Act
        var result = await controller.UpdateTask(existingTask.Id, request);

        // Assert
        var okResult = result.Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        var returnedDto = okResult.Value as TaskItemDto;
        Assert.NotNull(returnedDto);
        Assert.Equal("Done", returnedDto.Status);
    }

    [Fact]
    public async Task UpdateTask_WithNonexistentId_ReturnsNotFound()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var controller = new TasksController(dbContext, CreateMockLogger());
        var request = new UpdateTaskRequest
        {
            Title = "Updated Title"
        };

        // Act
        var result = await controller.UpdateTask(999, request);

        // Assert
        var notFoundResult = result.Result as Microsoft.AspNetCore.Mvc.NotFoundObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    #endregion

    #region Get Tasks Tests

    [Fact]
    public async Task GetAllTasks_WithMultipleTasks_ReturnsAllTasks()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var tasks = new[]
        {
            new TaskItem { Title = "Task 1", Status = TaskStatus.Todo, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new TaskItem { Title = "Task 2", Status = TaskStatus.InProgress, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new TaskItem { Title = "Task 3", Status = TaskStatus.Done, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        dbContext.Tasks.AddRange(tasks);
        await dbContext.SaveChangesAsync();

        var controller = new TasksController(dbContext, CreateMockLogger());

        // Act
        var result = await controller.GetAllTasks();

        // Assert
        var okResult = result.Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.NotNull(okResult);
        var returnedTasks = okResult.Value as List<TaskItemDto>;
        Assert.NotNull(returnedTasks);
        Assert.Equal(3, returnedTasks.Count);
    }

    [Fact]
    public async Task GetTask_WithValidId_ReturnsTask()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var task = new TaskItem
        {
            Title = "Test Task",
            Status = TaskStatus.Todo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();

        var controller = new TasksController(dbContext, CreateMockLogger());

        // Act
        var result = await controller.GetTask(task.Id);

        // Assert
        var okResult = result.Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.NotNull(okResult);
        var returnedDto = okResult.Value as TaskItemDto;
        Assert.NotNull(returnedDto);
        Assert.Equal(task.Id, returnedDto.Id);
        Assert.Equal("Test Task", returnedDto.Title);
    }

    [Fact]
    public async Task GetTask_WithNonexistentId_ReturnsNotFound()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var controller = new TasksController(dbContext, CreateMockLogger());

        // Act
        var result = await controller.GetTask(999);

        // Assert
        var notFoundResult = result.Result as Microsoft.AspNetCore.Mvc.NotFoundObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    #endregion

    #region Delete Task Tests

    [Fact]
    public async Task DeleteTask_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var task = new TaskItem
        {
            Title = "Task to Delete",
            Status = TaskStatus.Todo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();

        var controller = new TasksController(dbContext, CreateMockLogger());

        // Act
        var result = await controller.DeleteTask(task.Id);

        // Assert
        var noContentResult = result as Microsoft.AspNetCore.Mvc.NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);

        // Verify it's deleted from the database
        var deletedTask = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async Task DeleteTask_WithNonexistentId_ReturnsNotFound()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var controller = new TasksController(dbContext, CreateMockLogger());

        // Act
        var result = await controller.DeleteTask(999);

        // Assert
        var notFoundResult = result as Microsoft.AspNetCore.Mvc.NotFoundObjectResult;
        Assert.NotNull(notFoundResult);
    }

    #endregion
}

// Simple mock logger implementation
public class MockLogger<T> : ILogger<T>
{
    public IDisposable BeginScope<TState>(TState state) => null!;
    public bool IsEnabled(LogLevel logLevel) => true;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        // No-op for testing
    }
}
