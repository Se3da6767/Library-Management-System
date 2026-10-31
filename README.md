# 📚 Library Management System

A web application for managing a library's books, categories, publishers, members and borrowings —
built as my final project for the **ITI Web Development using .NET** track.

**Stack:** ASP.NET Core MVC (.NET 9) · C# · Entity Framework Core (Code-First) · SQL Server · Razor Views · Bootstrap 5

## Features

- **Dashboard** with live statistics, recent borrowings and latest books
- **Full CRUD** for Books, Categories, Publishers, Members and Borrowings
- **One-to-many relationships** loaded with `Include` / `ThenInclude`
- **Search & sorting** for books (title, author, price) built on `IQueryable`
- **Cover image upload** with live preview, type and size checks (jpg/png/webp, max 2 MB)
- **Validation** with Data Annotations, custom `ModelState` rules and client-side jQuery validation
- **Borrowing rules**
  - only available books and active members can be selected
  - due date defaults to two weeks
  - overdue tracking with filters (All / On loan / Overdue / Returned)
  - one-click return with an automatic late fee (5 per late day)
- **Safe deletes** — categories, publishers, books and members with related records are protected
- Code-first **migrations** and **seed data**, `DbContext` registered through dependency injection

## Getting started

1. Install the **.NET 9 SDK** and **SQL Server** (Express is fine).
2. Clone the repository and open `LibrarySystem.sln` in Visual Studio.
3. Update the connection string in `appsettings.json` with your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=LibrarySystem;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```
4. Create the database from the existing migrations (Package Manager Console):
   ```powershell
   Update-Database
   ```
5. Run the project (`Ctrl + F5`). The app opens on the dashboard.

## Project structure

```
Controllers/   Books, Categories, Publishers, Members, Borrowings, Home
Models/        Entities with validation attributes
Data/          AppDbContext + seed data
Migrations/    EF Core migration history
Helpers/       Display helpers used by the views
Views/         Razor views, shared layout and partial views
wwwroot/       CSS, JS and uploaded book covers
```

## Acknowledgements

Thanks to **ITI** and the instructors of the Web Development using .NET track.
