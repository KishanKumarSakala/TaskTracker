using Xunit;
using TaskTracker.API.Controllers;
using TaskTracker.API.Models;
using TaskTracker.API.Validators;

namespace TaskTracker.Tests;

public class TaskValidatorTests
{
    #region Create Task Validation Tests

    [Fact]
    public void ValidateCreateTask_WithValidData_ReturnsValid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Valid Task",
            Description = "A valid task",
            Status = TaskStatus.Todo,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var result = TaskValidator.ValidateCreateTask(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateCreateTask_WithEmptyTitle_ReturnsInvalid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "",
            Description = "A task without title"
        };

        // Act
        var result = TaskValidator.ValidateCreateTask(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Title is required", result.Errors);
    }

    [Fact]
    public void ValidateCreateTask_WithWhitespaceTitle_ReturnsInvalid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "   ",
            Description = "A task with only whitespace"
        };

        // Act
        var result = TaskValidator.ValidateCreateTask(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Title is required", result.Errors);
    }

    [Fact]
    public void ValidateCreateTask_WithTitleExceeding100Chars_ReturnsInvalid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = new string('a', 101)
        };

        // Act
        var result = TaskValidator.ValidateCreateTask(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Title cannot exceed 100 characters", result.Errors);
    }

    [Fact]
    public void ValidateCreateTask_WithTitle100Chars_ReturnsValid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = new string('a', 100)
        };

        // Act
        var result = TaskValidator.ValidateCreateTask(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateCreateTask_WithDoneStatusAndEmptyTitle_ReturnsInvalid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "",
            Status = TaskStatus.Done
        };

        // Act
        var result = TaskValidator.ValidateCreateTask(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("A task cannot be marked as Done if the Title is empty or whitespace", result.Errors);
    }

    [Fact]
    public void ValidateCreateTask_WithDoneStatusAndWhitespaceTitle_ReturnsInvalid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "  \t\n  ",
            Status = TaskStatus.Done
        };

        // Act
        var result = TaskValidator.ValidateCreateTask(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("A task cannot be marked as Done if the Title is empty or whitespace", result.Errors);
    }

    [Fact]
    public void ValidateCreateTask_WithDoneStatusAndValidTitle_ReturnsValid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Completed Task",
            Status = TaskStatus.Done
        };

        // Act
        var result = TaskValidator.ValidateCreateTask(request);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region Update Task Validation Tests

    [Fact]
    public void ValidateUpdateTask_WithValidData_ReturnsValid()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = "Updated Task",
            Description = "Updated description",
            Status = TaskStatus.InProgress
        };

        // Act
        var result = TaskValidator.ValidateUpdateTask(request, 1);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateUpdateTask_WithEmptyTitle_ReturnsInvalid()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = ""
        };

        // Act
        var result = TaskValidator.ValidateUpdateTask(request, 1);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Title is required", result.Errors);
    }

    [Fact]
    public void ValidateUpdateTask_WithDoneStatusAndEmptyTitle_ReturnsInvalid()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = "",
            Status = TaskStatus.Done
        };

        // Act
        var result = TaskValidator.ValidateUpdateTask(request, 1);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("A task cannot be marked as Done if the Title is empty or whitespace", result.Errors);
    }

    [Fact]
    public void ValidateUpdateTask_WithDoneStatusAndValidTitle_ReturnsValid()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = "Completed Task",
            Status = TaskStatus.Done
        };

        // Act
        var result = TaskValidator.ValidateUpdateTask(request, 1);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateUpdateTask_WithTitleExceeding100Chars_ReturnsInvalid()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = new string('a', 101)
        };

        // Act
        var result = TaskValidator.ValidateUpdateTask(request, 1);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Title cannot exceed 100 characters", result.Errors);
    }

    #endregion
}
