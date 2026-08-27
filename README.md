# MultiBranch.StockTransfer API

A backend REST API for managing products, stock, shelves, employees, stores, and inter-store product transfers within a multi-branch retail network.

The project focuses on data integrity, stock consistency, traceability, and the controlled management of stock transfers between physical stores.

## Table of Contents

* [Project Purpose](#project-purpose)
* [Technologies](#technologies)
* [Architecture](#architecture)
* [Project Structure](#project-structure)
* [Business Domain](#business-domain)
* [Business Rules](#business-rules)
* [Transfer Lifecycle](#transfer-lifecycle)
* [Stock Movement Types](#stock-movement-types)
* [Database](#database)
* [DTOs and Validation](#dtos-and-validation)
* [API and Swagger](#api-and-swagger)
* [Setup and Installation](#setup-and-installation)
* [Database Configuration](#database-configuration)
* [EF Core Migrations](#ef-core-migrations)
* [Running the Application](#running-the-application)
* [Testing and Verification](#testing-and-verification)
* [Git and GitHub](#git-and-github)
* [Conclusion](#conclusion)

## Project Purpose

The purpose of this project is to develop a backend service for a multi-branch retail network that manages:

* Physical stores
* Store employees
* Products and product categories
* Suppliers
* Shelves and physical product locations
* Current stock quantities
* Stock movement history
* Inter-store product transfers

The system places a strong focus on the following topics:

* Data integrity
* Stock consistency
* Full stock traceability
* Soft delete
* Shelf capacity control
* Minimum stock warnings
* Employee/store isolation
* Transaction management
* Inter-store transfer lifecycle management

## Technologies

The project uses the following technologies:

* **C#**
* **.NET 8**
* **ASP.NET Core Minimal APIs**
* **Clean Architecture**
* **Entity Framework Core**
* **PostgreSQL**
* **EF Core Code First**
* **FluentValidation**
* **Swagger / OpenAPI**
* **Git**
* **GitHub**

## Architecture

The project follows **Clean Architecture** principles and is divided into four main layers:

```text
MultiBranch.StockTransfer
│
├── Domain
├── Application
├── Infrastructure
└── API
```

The dependency direction follows:

```text
API
 ↓
Application
 ↓
Infrastructure

Domain
 ↑
Referenced by the other layers
```

### Domain Layer

The Domain layer contains the core business entities and enumerations.

Main entity groups include:

* Category
* Employee
* Product
* Shelf
* ShelfStock
* StockMovement
* Store
* Supplier
* Transfer
* TransferItem

Main enumerations include:

* `StockMovementType`
* `TransferStatus`

The Domain layer does not depend on infrastructure or API-specific implementation details.

### Application Layer

The Application layer contains application-level business workflows and abstractions.

It includes:

* DTOs
* Interfaces
* Services
* Validators

Important services include:

* `ShelfStockService`
* `TransferService`

Important abstractions include:

* `IApplicationDbContext`
* `IShelfStockService`
* `ITransferService`

The Application layer is responsible for operations such as:

* Creating transfers
* Validating transfer requests
* Removing stock
* Adding stock
* Checking stock levels
* Producing minimum-stock warnings
* Managing transfer-related stock operations

### Infrastructure Layer

The Infrastructure layer handles persistence and external implementation details.

It includes:

* Entity Framework Core
* PostgreSQL integration
* `ApplicationDbContext`
* Entity configurations
* Persistence configuration
* Dependency injection

The Infrastructure layer implements the persistence abstractions required by the Application layer.

### API Layer

The API layer exposes the application functionality through HTTP endpoints using ASP.NET Core Minimal APIs.

It is responsible for:

* HTTP endpoint configuration
* Request handling
* Dependency injection configuration
* Swagger/OpenAPI integration

## Project Structure

The project is organized into the following main layers:

```text
MultiBranch.StockTransfer.API/
│
├── MultiBranch.StockTransfer.Domain/
│   ├── Entities/
│   └── Enums/
│       ├── StockMovementType.cs
│       └── TransferStatus.cs
│
├── MultiBranch.StockTransfer.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   └── Validators/
│
├── MultiBranch.StockTransfer.Infrastructure/
│   └── Persistence/
│       ├── Configurations/
│       └── ApplicationDbContext.cs
│
├── MultiBranch.StockTransfer.API/
│   └── Program.cs
│
├── MultiBranch.StockTransfer.sln
├── .gitignore
└── README.md
```

Generated build directories such as `bin/` and `obj/` are excluded from Git tracking through `.gitignore`.

## Business Domain

### Store and Employee Management

The system supports multiple physical stores.

Each employee is associated with a store and store-specific operations are controlled according to this relationship.

### Product and Catalog Management

Products contain their basic information and can be associated with:

* Categories
* Suppliers

Products can be uniquely identified through their product code or barcode-related identifier.

Products also contain a `MinimumStockLevel` used for stock-level warning logic.

### Physical Location and Current Stock

Products are physically located on shelves inside stores.

The system can track:

* Store
* Shelf
* Product
* Current quantity

This allows the application to determine how much stock of a product is currently available at a particular shelf.

### Stock Movement Auditing

Stock changes are recorded through the `StockMovements` structure.

The system is designed to keep a traceable history of stock-related operations such as:

* Stock entering a store
* Sales
* Waste
* Shelf relocation
* Inter-store transfers

Stock movement records provide information about the type and time of stock operations.

### Inter-store Logistics

Products can be transferred between different stores.

A transfer does not immediately add stock to the destination store.

Instead, the transfer follows an operational lifecycle:

```text
Source Store
     │
     │ Transfer created
     ▼
Stock removed from source shelf
     │
     │ TransferOut
     ▼
  InTransit
     │
     ├───────────────┐
     │               │
     ▼               ▼
 Completed        Cancelled
     │               │
     ▼               ▼
Stock added       Stock returned
to destination    to source shelf
shelf
```

## Business Rules

### 1. Soft Delete

Records are not physically deleted from the database.

Instead, records are deactivated by setting:

```text
IsActive = false
```

List operations return active records only.

The system therefore avoids physical deletion of business records.

### 2. Negative Stock Protection

A shelf's stock quantity cannot become negative.

Before removing stock, the available quantity is checked.

If the requested quantity is greater than the available stock, the operation is rejected.

This prevents invalid stock states such as:

```text
Quantity = -5
```

### 3. Shelf Capacity Limit

Every shelf has a capacity value.

When stock is added to a shelf, the resulting quantity cannot exceed the shelf capacity.

For example:

```text
Shelf Capacity = 25
Current Stock  = 20
Incoming Stock = 5

Result = 25
```

An operation that would exceed the capacity is rejected.

### 4. Minimum Stock Warning

Products have a `MinimumStockLevel`.

If an operation successfully reduces the stock below the defined minimum level, the operation can still succeed, but the API response contains a warning.

Example:

```text
MinimumStockLevel = 10
Current Stock     = 15
Removed Quantity  = 8

Remaining Stock   = 7
```

The stock operation succeeds, but the response contains a minimum-stock warning.

### 5. Append-only Stock Movements

Stock movement history is designed to be append-only.

Historical stock movement records must not be deleted or modified.

New stock operations create new movement records instead.

This preserves the historical sequence of stock changes.

### 6. Inter-store Transfer Lifecycle

Transfers have three main statuses:

* `InTransit`
* `Completed`
* `Cancelled`

The transfer does not immediately increase stock at the destination.

#### InTransit

When a transfer is created:

1. The source shelf stock is reduced.
2. A `TransferOut` movement is recorded.
3. The transfer remains in the `InTransit` state.

```text
Source Shelf
    │
    │ Remove Stock
    ▼
TransferOut
    │
    ▼
InTransit
```

#### Completed

When the destination store completes the transfer:

1. Stock is added to the destination shelf.
2. A `TransferIn` movement is recorded.
3. The transfer is marked as `Completed`.

```text
InTransit
    │
    │ Destination approval
    ▼
Destination Shelf
    │
    │ Add Stock
    ▼
TransferIn
    │
    ▼
Completed
```

#### Cancelled

When a transfer is cancelled:

1. Previously deducted stock is returned to the original source shelf.
2. The corresponding stock movement is recorded.
3. The transfer is marked as `Cancelled`.

```text
InTransit
    │
    │ Cancel
    ▼
Return Stock
    │
    ▼
Original Source Shelf
```

### 7. Employee and Store Isolation

Employees are associated with a store.

A transfer operation must use a source shelf belonging to the employee's own store.

An employee cannot initiate a transfer by directly operating on another store's shelf.

This rule prevents unauthorized cross-store stock operations.

### 8. Transaction Management

Operations involving stock changes and stock movement records must be handled as a single transactional operation.

Conceptually:

```text
Stock Change
     +
StockMovement Record
     │
     ▼
Transaction
     │
     ├── Success → Commit
     │
     └── Error   → Rollback
```

If an operation fails, the related database changes should not be partially persisted.

## Transfer Lifecycle

The complete transfer lifecycle is:

```text
             Create Transfer
                   │
                   ▼
              InTransit
              /        \
             /          \
            ▼            ▼
       Completed      Cancelled
            │              │
            ▼              ▼
    Add stock to       Return stock
 destination shelf    to source shelf
            │              │
            ▼              ▼
       TransferIn      Stock Return
        movement        movement
```

### Transfer Creation

The transfer request contains information such as:

* Source store
* Target store
* Employee
* Transfer items

Each transfer item contains:

* Product
* Source shelf
* Target shelf
* Quantity

### Transfer Validation

The application validates transfer requests before processing them.

Important validation rules include:

* Source store is required.
* Target store is required.
* Source and target stores must be different.
* Employee is required.
* At least one transfer item is required.
* Product is required.
* Source shelf is required.
* Target shelf is required.
* Quantity must be greater than zero.

## Stock Movement Types

The system defines the following stock movement types:

| Value | Movement Type   | Description                       |
| ----: | --------------- | --------------------------------- |
|     1 | `StockIn`       | Stock entering the inventory      |
|     2 | `Sale`          | Product sold                      |
|     3 | `Waste`         | Product disposed of or wasted     |
|     4 | `RelocationOut` | Stock moved out of a shelf        |
|     5 | `RelocationIn`  | Stock moved into a shelf          |
|     6 | `TransferOut`   | Stock sent to another store       |
|     7 | `TransferIn`    | Stock received from another store |

These movement types allow stock changes to remain traceable.

## Database

The application uses:

* **PostgreSQL**
* **Entity Framework Core**
* **Code First**

The main database entities are:

* `Categories`
* `Products`
* `Suppliers`
* `Employees`
* `Stores`
* `Shelves`
* `ShelfStocks`
* `StockMovements`
* `Transfers`
* `TransferItems`

### Main Relationships

The database is designed to support relationships such as:

```text
Category
   │
   └── Products

Supplier
   │
   └── Products

Store
   │
   ├── Employees
   │
   └── Shelves
          │
          └── ShelfStocks
                 │
                 └── Products

Store ─────── Transfer ─────── Store
                  │
                  └── TransferItems
                         │
                         └── Products

StockMovements
   │
   └── Records stock-related operations
```

## DTOs and Validation

The API uses Data Transfer Objects instead of exposing database entities directly for request and response models.

DTO groups include:

* Product DTOs
* Category DTOs
* Supplier DTOs
* Employee DTOs
* Store DTOs
* Shelf DTOs
* ShelfStock DTOs
* Transfer DTOs
* TransferItem DTOs

### FluentValidation

Transfer creation uses FluentValidation.

The validation rules include:

```text
SourceStoreId
    → Required

TargetStoreId
    → Required
    → Must be different from SourceStoreId

EmployeeId
    → Required

TransferItems
    → At least one item required

TransferItem.ProductId
    → Required

TransferItem.SourceShelfId
    → Required

TransferItem.TargetShelfId
    → Required

TransferItem.Quantity
    → Must be greater than 0
```

This prevents invalid transfer requests from reaching the business logic.

## API and Swagger

The application uses ASP.NET Core Minimal APIs.

Swagger/OpenAPI is used to document and test the API.

The available API functionality covers the main domain resources, including:

* Categories
* Products
* Suppliers
* Employees
* Stores
* Shelves
* Shelf stocks
* Transfers

Swagger can be used during development to send requests and inspect API responses.

### Stock Operations

Stock addition and removal are handled internally by the application services as part of the business workflows.

They are not exposed as separate public stock-operation endpoints.

This keeps stock modification under the relevant business workflows.

## Setup and Installation

### Prerequisites

Before running the project, make sure the following are installed:

* .NET 8 SDK
* PostgreSQL
* Git

### Clone the Repository

```bash
git clone https://github.com/amineacar/MultiBranch.StockTransfer.API.git
```

Move into the project directory:

```bash
cd MultiBranch.StockTransfer.API
```

### Restore Dependencies

```bash
dotnet restore
```

### Build the Solution

```bash
dotnet build
```

A successful build should complete without errors.

## Database Configuration

The application uses PostgreSQL.

Configure the PostgreSQL connection string in the application's configuration.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=MultiBranchStockTransfer;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

Replace the placeholder values with the PostgreSQL configuration on your local machine.

Do not commit real passwords or other sensitive credentials to GitHub.

## EF Core Migrations

The project follows the EF Core Code First approach.

After configuring the database connection, EF Core migrations can be applied with:

```bash
dotnet ef database update
```

If the project is executed from the repository root and the startup/project paths need to be specified explicitly, use:

```bash
dotnet ef database update --project .\MultiBranch.StockTransfer.Infrastructure\MultiBranch.StockTransfer.Infrastructure.csproj --startup-project .\MultiBranch.StockTransfer.API\MultiBranch.StockTransfer.API.csproj
```

The exact command may depend on the location of the solution and project files in the local environment.

## Running the Application

Run the API project with:

```bash
dotnet run --project .\MultiBranch.StockTransfer.API\MultiBranch.StockTransfer.API.csproj
```

After the application starts, the terminal displays the local application address.

Open the Swagger endpoint using the address provided by the application.

Swagger can then be used to test the API endpoints.

## Testing and Verification

During development, the following areas were tested and verified through the API and database:

### Build Verification

* Solution builds successfully.
* Build completed with 0 errors.
* Build completed with 0 warnings.

### Stock Tests

* Product creation
* Store creation
* Shelf creation
* Shelf stock creation
* Stock addition
* Stock removal
* Negative stock protection
* Shelf capacity protection
* Minimum stock warning

### Transfer Tests

* Transfer creation
* Source stock deduction
* `TransferOut` movement creation
* `InTransit` status
* Transfer completion
* Destination stock addition
* `TransferIn` movement creation
* Transfer cancellation
* Stock return to the original shelf

### Database Verification

Database checks were also performed to verify:

* No negative stock quantities
* Shelf capacity limits
* Correct transfer status
* Correct stock movement records
* Correct stock return after cancellation
* Correct stock addition after completion
* Active/inactive record behavior

## Git and GitHub

The complete development process was managed through Git and GitHub.

Repository:

https://github.com/amineacar/MultiBranch.StockTransfer.API

The project uses meaningful and descriptive commit messages.

Examples:

```text
feat: add product entity and configuration
feat: implement shelf stock service
feat: add transfer service
fix: prevent negative stock
fix: enforce shelf capacity
chore: stop tracking build artifacts
```

### Branch Structure

The project development included the following branches:

```text
main
│
├── feature/domain-layer
├── infrastructure-layer
├── feature/application-layer
├── feature/api-layer
└── integration/full-project
```

The development process included separate work on the Domain, Infrastructure, Application, and API layers.

The latest versions of the layers were brought together through:

```text
feature/domain-layer
        │
        ├──────────────┐
        │              │
infrastructure-layer  │
        │              │
        ├──────────────┤
        │              │
feature/application-layer
        │              │
feature/api-layer
        │              │
        ▼              │
integration/full-project
        │
        │ Pull Request
        ▼
       main
```

The integrated project was merged into the `main` branch through a Pull Request.

## Conclusion

MultiBranch.StockTransfer API is a backend service for managing stock and inter-store transfers in a multi-branch retail environment.

The project combines Clean Architecture, ASP.NET Core Minimal APIs, Entity Framework Core, PostgreSQL, and FluentValidation to provide a structured backend solution.

The implementation focuses on the most important requirements of the project:

* Multi-store management
* Product and catalog management
* Shelf-based stock tracking
* Stock movement auditing
* Soft delete
* Negative stock protection
* Shelf capacity control
* Minimum stock warnings
* Append-only stock movement history
* Employee/store isolation
* Transaction-based stock operations
* Inter-store transfer lifecycle management

The project was developed and maintained using Git and GitHub with separate architectural layers and meaningful commit history.
