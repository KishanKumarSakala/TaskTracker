# TaskTracker - .NET Web API

A simple yet robust Task Tracker API built with .NET 6, Entity Framework Core, and SQLite.

## Features

- ✅ Full CRUD operations for tasks
- ✅ Task status management (Todo, InProgress, Done)
- ✅ Comprehensive input validation
- ✅ SQLite persistence with Entity Framework Core
- ✅ Business rule enforcement (task cannot be marked Done without a title)
- ✅ Unit tests covering validation, business rules, and CRUD operations
- ✅ RESTful API design with appropriate HTTP status codes

## Project Structure

```
TaskTracker/
├── TaskTracker.API/           # Main API project
│   ├── Models/                # Data models
│   ├── Controllers/           # API endpoints
│   ├── Data/                  # Entity Framework Core DbContext
│   ├── Validators/            # Input validation logic
│   └── Program.cs             # Startup configuration
├── TaskTracker.Tests/         # Unit tests
└── TaskTracker.sln            # Solution file
```

## Getting Started

### Prerequisites

- .NET 6 SDK or later
- Visual Studio, Visual Studio Code, or any text editor

### Running the API

```bash
cd TaskTracker.API
dotnet run
```

The API will be available at `https://localhost:7000` (or `http://localhost:5000`).

### Running Tests

```bash
cd TaskTracker.Tests
dotnet test
```

## API Endpoints

### Create a Task
```
POST /api/tasks
Content-Type: application/json

{
  "title": "Complete project",
  "description": "Finish the TaskTracker API",
  "status": "Todo",
  "dueDate": "2026-12-31"
}
```

### Get All Tasks
```
GET /api/tasks
```

### Get Task by ID
```
GET /api/tasks/{id}
```

### Update a Task
```
PUT /api/tasks/{id}
Content-Type: application/json

{
  "title": "Updated title",
  "description": "Updated description",
  "status": "InProgress",
  "dueDate": "2026-12-31"
}
```

### Delete a Task
```
DELETE /api/tasks/{id}
```

## Data Model

### TaskItem

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | int | Yes | Auto-generated primary key |
| Title | string | Yes | Max 100 characters |
| Description | string | No | Optional field |
| Status | enum | Yes | Values: Todo, InProgress, Done |
| DueDate | DateTime | No | Optional field |
| CreatedAt | DateTime | Yes | Auto-set on creation |
| UpdatedAt | DateTime | Yes | Auto-updated on modifications |

## Business Rules

1. **Title is Required**: A task cannot be created or updated without a title.
2. **Title Length**: Title must not exceed 100 characters.
3. **Done Status Validation**: A task cannot be marked as "Done" if the title is empty or whitespace.
4. **Status Values**: Only Todo, InProgress, and Done are valid statuses.

## Design Decisions

### Architecture

- **Layered Architecture**: The project follows a simple layered architecture with Controllers, Services/Validators, and Data Access layers.
- **Entity Framework Core**: Provides a robust ORM for database operations and migrations.
- **SQLite**: Lightweight, file-based database perfect for this use case, no server setup required.

### Validation

- **Input Validation**: Implemented at the controller level using custom validators and data annotations.
- **Business Rule Validation**: The "Done status" rule is enforced both at the model level and in the business logic.
- **Error Responses**: All validation errors return HTTP 400 (Bad Request) with descriptive messages.

### Testing

- **Unit Tests**: Using xUnit and Moq for comprehensive test coverage.
- **Test Coverage**: Tests include:
  - Validation failure scenarios
  - Status transition business rules
  - Successful CRUD operations
  - Edge cases (empty titles, special characters, etc.)

### Database

- **SQLite**: File-based database (`tasktracker.db`) for easy setup and portability.
- **Entity Framework Core Code-First**: Database schema is managed through entity configurations.
- **Automatic Timestamps**: CreatedAt and UpdatedAt are automatically managed.

## HTTP Status Codes

- `200 OK`: Successful GET, PUT requests
- `201 Created`: Successful POST request
- `204 No Content`: Successful DELETE request
- `400 Bad Request`: Validation errors or business rule violations
- `404 Not Found`: Task not found
- `500 Internal Server Error`: Unexpected errors

## Future Enhancements

- Pagination for the GET /tasks endpoint
- Filtering and sorting capabilities
- User authentication and authorization
- Task categories or tags
- Due date reminders
- API documentation with Swagger/OpenAPI
- Logging and monitoring

## License

MIT License - Feel free to use this project for learning and development.
