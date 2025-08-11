# LibrarySystem

A .NET 9 solution for managing a library system, including API, gRPC, and shared logic.
This project uses SQLite for data storage and MSTest for testing.

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- (Optional) Visual Studio 2022
- No manual SQLite setup required (database is auto-created)

---

## Running the Application

The solution consists of two main services:

- **LibrarySystem.GrpcService** (gRPC backend, handles data and business logic)
- **LibrarySystem.API** (REST API, communicates with the gRPC service)

**1. Start the gRPC Service**

		dotnet run --project LibrarySystem.GrpcService/LibrarySystem.Grpc.csproj

This will initialize the SQLite database (`library.db`), create tables, and seed initial data.

**2. Start the API Service**

In a new terminal:

		dotnet run --project LibrarySystem.API/LibrarySystem.API.csproj

The API will connect to the gRPC service (by default at `https://localhost:7165`). 

The API will start listening on https://localhost:7077.


---

## API Endpoints

### BooksController (`/books`)

- `GET /books`  
  Returns all books.

- `GET /books/most-borrowed`  
  Returns the most borrowed books.

- `GET /books/book-stats/{bookId}`  
  Returns statistics for a specific book (total copies, borrowed count, available copies).

- `GET /books/book-same-people/{bookId}`  
  Returns other books borrowed by the same people who borrowed the specified book.

- `GET /books/reading-estimate/{bookId}`  
  Returns the average reading rate (pages per day) for the specified book.

### UsersController (`/users`)

- `GET /users/top-users?startDate={startDate}&endDate={endDate}`  
  Returns the top users within the specified date range.

- `GET /users/user-books?id={userId}&startDate={startDate}&endDate={endDate}`  
  Returns the books borrowed by a specific user within the specified date range.

---

## Testing

Automated tests are located in the `LibrarySystem.TestProject`:

- **Unit and functional tests** (controllers, services)
- **Integration tests** (repositories, database)
- **System tests** (end-to-end, API + gRPC)

To run all tests:
dotnet test LibrarySystem.TestProject/LibrarySystem.TestProject.csproj


Test output will be shown in the console. MSTest is used as the test framework.

---

## Notes

- The database is automatically created and seeded on first run.
- All services target .NET 9.
- For development, ensure both the gRPC and API services are running for full functionality.

---