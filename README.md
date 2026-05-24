# BallastLane Technical Test

## Overview

This project is a full-stack billing/invoicing application developed as part of a .NET technical assessment.

The application allows authenticated users to manage:

- Categories
- Products
- Customers
- Invoices
- Invoice details

The solution follows Clean Architecture principles, includes JWT authentication, validation layers, unit testing, and SQL Server integration without using Entity Framework, Dapper, or MediatR.

---

# Architecture

The solution is organized into the following layers:

## API
Responsible for:
- Controllers
- HTTP endpoints
- Middleware
- Dependency injection configuration
- Authentication configuration

## Application
Responsible for:
- Business logic
- DTOs
- Validators
- Application services
- Shared/common response models

## Infrastructure
Responsible for:
- SQL Server access
- ADO.NET repositories
- JWT token generation
- Password hashing
- Database initialization and seed scripts

## Domain
Responsible for:
- Core entities
- Domain models
- Exceptions

## Tests
Responsible for:
- Unit testing
- Service testing
- Validator testing

---

# Technologies

- ASP.NET Core 9
- SQL Server
- Angular
- JWT Authentication
- xUnit
- Moq
- ADO.NET
- Clean Architecture

---

# Features

- JWT Authentication
- User registration and login
- Authorized and non-authorized endpoints
- CRUD operations for:
  - Categories
  - Products
  - Customers
  - Invoices
- Global exception middleware
- Input validation layer
- Seeded database
- Transactional invoice creation

---

# Database

The application automatically:

1. Creates the database if it does not exist
2. Creates all required tables
3. Inserts seed/demo data

This process runs automatically on application startup.

---

# How to Run

## Backend

1. Open the solution in Visual Studio 2022
2. Configure SQL Server connection string in:

```json
appsettings.json