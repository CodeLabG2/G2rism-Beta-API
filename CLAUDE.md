# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**G2rism Beta API** is a .NET 9.0 Web API for a tourism management system (Sistema de Turismo) by CodeLabG2. The system manages roles, permissions, users, clients (CRM), employees, providers, and travel services (airlines, flights).

## Technology Stack

- **Framework**: .NET 9.0 (net9.0)
- **Database**: MySQL 9.0 via Pomelo.EntityFrameworkCore.MySql
- **ORM**: Entity Framework Core 9.0.9
- **Validation**: FluentValidation 11.3.0
- **Mapping**: AutoMapper 12.0.1
- **Authentication**: BCrypt.Net-Next 4.0.3 (password hashing)
- **Documentation**: Swagger/OpenAPI 9.0.6

## Common Commands

### Build and Run
```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application (dev mode with Swagger at http://localhost:5000/)
dotnet run

# Run with watch (auto-reload on changes)
dotnet watch run
```

### Database Migrations
```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Apply migrations to database
dotnet ef database update

# Rollback to a specific migration
dotnet ef database update MigrationName

# Remove last migration (if not applied)
dotnet ef migrations remove

# List all migrations
dotnet ef migrations list
```

### Testing
```bash
# Run all tests (if test project exists)
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## Architecture

### Layered Architecture Pattern

The project follows a clean layered architecture:

1. **Models** (`Models/`) - Domain entities with EF Core configuration
2. **DTOs** (`DTOs/`) - Data Transfer Objects organized by module subdirectories
3. **Interfaces** (`Interfaces/`) - Abstraction contracts for repositories and services
4. **Repositories** (`Repositories/`) - Data access layer implementing repository pattern
5. **Services** (`Services/`) - Business logic layer
6. **Controllers** (`Controllers/`) - API endpoints following REST conventions
7. **Validators** (`Validators/`) - FluentValidation rules for DTOs
8. **Middleware** (`Middleware/`) - Global exception handler
9. **Helpers** (`Helpers/`) - Utilities (EmailHelper, TokenGenerator, PasswordHasher)
10. **Mappings** (`Mappings/`) - AutoMapper profile for Model ↔ DTO conversions
11. **Data** (`Data/`) - DbContext, DbInitializer (seeding), DbContextFactory

### Dependency Injection Flow

**Program.cs** registers services in this order:
- DbContext with MySQL connection
- AutoMapper
- Generic Repository
- Module-specific Repositories
- Module-specific Services
- FluentValidation
- Controllers and Swagger

### Modules and Features

The system is organized into distinct modules:

#### 1. Configuration Module (Roles & Permissions)
- **Models**: `Rol`, `Permiso`, `RolPermiso` (many-to-many)
- **Controllers**: `RolesController`, `PermisosController`
- **Features**: Hierarchical access levels, permission assignment, role management

#### 2. User Authentication Module
- **Models**: `Usuario`, `UsuarioRol` (many-to-many), `TokenRecuperacion`
- **Controllers**: `AuthController`, `UsuariosController`
- **Features**:
  - Registration, login, password recovery
  - BCrypt password hashing
  - Login attempt tracking and account locking
  - Token-based password reset

#### 3. CRM Module (Clients & Employees)
- **Models**: `CategoriaCliente`, `Cliente`, `PreferenciaCliente`, `Empleado`
- **Controllers**: `CategoriasClienteController`, `ClientesController`, `PreferenciasClienteController`, `EmpleadosController`
- **Features**:
  - Client segmentation with discount categories
  - Client preferences tracking
  - Employee hierarchy (self-referencing with `IdJefe`)
  - Computed properties (age, tenure, full name)

#### 4. Providers Module
- **Models**: `Proveedor`, `ContratoProveedor`
- **Controllers**: `ProveedoresController`, `ContratosProveedorController`
- **Features**:
  - Provider types (hotels, airlines, transport, services)
  - Contract management with expiration tracking
  - Provider rating system

#### 5. Services Module (Airlines & Flights)
- **Models**: `Aerolinea`, `Vuelo`
- **Controllers**: `AerolineasController`
- **Features**:
  - IATA/ICAO code validation
  - Baggage policy management
  - Airline-flight relationships

### Database Design Patterns

1. **Many-to-Many Relationships**: Explicit join tables with composite keys (`RolPermiso`, `UsuarioRol`)
2. **One-to-One Relationships**: `Cliente` ↔ `PreferenciaCliente` (cascade delete)
3. **Self-Referencing**: `Empleado.IdJefe` → `Empleado.Subordinados` (hierarchical structure)
4. **Soft Delete**: Most entities use `Estado` boolean field instead of hard deletes
5. **Audit Fields**: `FechaCreacion`, `FechaModificacion` on most entities
6. **Unique Constraints**: Enforced via unique indexes (`Username`, `Email`, `NIT`, `IATA codes`, etc.)
7. **Cascade Behavior**: `Restrict` for critical relationships, `Cascade` for dependent data

### Key Patterns and Conventions

1. **Repository Pattern with Generic Base**:
   - `IGenericRepository<T>` provides standard CRUD
   - Entity-specific repositories extend with custom queries
   - Example: `IRolRepository : IGenericRepository<Rol>`

2. **Service Layer Pattern**:
   - Services contain business logic and orchestrate repositories
   - Services use AutoMapper to convert Models ↔ DTOs
   - Services validate business rules before repository calls

3. **DTO Pattern**:
   - `CreateDto` - For creating new entities (excludes ID, timestamps)
   - `UpdateDto` - For updates (nullable fields for partial updates)
   - `ResponseDto` - For API responses (excludes sensitive data, includes computed fields)
   - Organized in subdirectories: `DTOs/Rol/`, `DTOs/Cliente/`, etc.

4. **Validation Strategy**:
   - FluentValidation validators in `Validators/` directory
   - Validators named `{DtoName}Validator.cs`
   - Auto-registered via `AddValidatorsFromAssembly()`
   - Custom validators for business rules (unique checks, date ranges, etc.)

5. **API Response Structure**:
   - Success: `ApiResponse<T>` with `Success`, `Message`, `Data`, `Timestamp`
   - Error: `ApiErrorResponse` with `Success`, `Message`, `Errors`, `StatusCode`, `Timestamp`
   - Global exception handling via `GlobalExceptionHandlerMiddleware`

6. **Computed Properties in Models**:
   - Use `[NotMapped]` for calculated fields (e.g., `Cliente.Edad`, `Empleado.NombreCompleto`)
   - Prevents EF Core from mapping to database columns
   - Allows rich domain models with business logic

### Database Connection

**Connection String Location**: `appsettings.json` → `ConnectionStrings:DefaultConnection`

**Default**: MySQL on localhost:3306, database `g2rism_beta_db`

**IMPORTANT**: When modifying `appsettings.json`, never commit credentials. The file contains sensitive data.

### Seeding Strategy

**DbInitializer.Initialize()** is called on application startup:
- Creates database if it doesn't exist
- Seeds initial roles, permissions, and admin user
- Idempotent (checks for existing data before seeding)

## Development Workflow

### Adding a New Module

When adding a new entity/module, follow this order:

1. **Model** (`Models/`) - Define entity with EF annotations
2. **DbContext** - Add `DbSet<T>` and configure relationships in `OnModelCreating`
3. **Migration** - Run `dotnet ef migrations add ModuleName`
4. **DTOs** - Create `CreateDto`, `UpdateDto`, `ResponseDto` in `DTOs/ModuleName/`
5. **AutoMapper** - Add mappings in `Mappings/MappingProfile.cs`
6. **Repository Interface** - Create `IModuleRepository` in `Interfaces/`
7. **Repository Implementation** - Implement in `Repositories/`
8. **Service Interface** - Create `IModuleService` in `Interfaces/`
9. **Service Implementation** - Implement in `Services/`
10. **Validators** - Create FluentValidation validators in `Validators/`
11. **Controller** - Create API controller in `Controllers/`
12. **Register in Program.cs** - Add repository and service to DI container
13. **Update Migration** - Run `dotnet ef database update`

### Making Changes to Existing Entities

1. Modify the Model class
2. Update AutoMapper mappings if DTOs changed
3. Create migration: `dotnet ef migrations add DescriptiveChangeName`
4. Review generated migration code
5. Apply migration: `dotnet ef database update`

### Controller Conventions

- Use `[ApiController]` and `[Route("api/[controller]")]`
- Return `ApiResponse<T>` or `ApiErrorResponse` consistently
- Use standard HTTP status codes (200, 201, 204, 400, 404, 500)
- Include XML comments for Swagger documentation

### Naming Conventions

- **Controllers**: Plural names (`RolesController`, `ClientesController`)
- **Services/Repositories**: Singular entity name + Service/Repository (`RolService`, `ClienteRepository`)
- **DTOs**: Entity name + purpose (`RolCreateDto`, `ClienteResponseDto`)
- **Validators**: DTO name + Validator (`RolCreateDtoValidator`)
- **Database Tables**: Plural Spanish names (`Roles`, `Clientes`, `Empleados`)
- **Foreign Keys**: `Id` + Entity name (`IdRol`, `IdCliente`)

## Important Notes

1. **CORS Policy**: Currently set to `AllowAll` for development. Restrict in production.

2. **Password Security**:
   - NEVER store plain text passwords
   - Use `PasswordHasher.HashPassword()` before saving
   - Verify with `BCrypt.Verify(plainText, hash)`

3. **Swagger**: Available at root (`http://localhost:5000/`) in Development mode only

4. **Middleware Order**: `GlobalExceptionHandlerMiddleware` MUST be first in pipeline

5. **Entity Relationships**:
   - Use `Include()` and `ThenInclude()` for eager loading
   - Be aware of circular references when serializing (AutoMapper handles this)
   - Use `DeleteBehavior.Restrict` for important data, `Cascade` only when safe

6. **Migration Safety**:
   - ALWAYS review migrations before applying
   - Never delete migrations that have been applied to production
   - Use descriptive migration names with dates/modules

7. **Generic Repository**: Use when entity has standard CRUD. Create custom repository methods for complex queries.

8. **Service Layer**: Business rules belong here, not in controllers or repositories.

9. **DTO Validation**: Use FluentValidation for all DTOs. Include custom validators for database-dependent rules (uniqueness, existence checks).

10. **Partial Updates**: `UpdateDto` classes support partial updates via nullable properties and AutoMapper condition mapping.
