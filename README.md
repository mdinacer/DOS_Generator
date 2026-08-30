# DOS Generator

**Desktop application for generating and managing maritime Declarations of Security (DoS).**

DOS Generator is a Windows desktop application designed to streamline the preparation and management of **Declarations of Security** used in maritime and port operations.

The application models the operational entities involved in a declaration — including ships, ports, facilities, agencies, officers, and security levels — and provides a structured workflow for creating, storing, and processing declarations.

The project combines a **WPF desktop interface**, a layered C# architecture, **Entity Framework Core**, **SQLite**, document generation, and application licensing.

---

## Why It Exists

A Declaration of Security is an important document in port and ship/shore security operations.

Preparing these documents manually can involve repeatedly entering information about:

- Ships
- Port facilities
- Port operations
- Ship agencies
- Security officers
- Security levels
- Operation types
- Dates and times

DOS Generator turns this process into a structured application.

Instead of repeatedly preparing documents from scratch, the application keeps the underlying operational data in a local database and uses it to generate declarations.

---

## Core Workflow

The application is built around the declaration workflow:

```text
┌───────────────┐
│ Select /      │
│ register ship │
└───────┬───────┘
        │
        ▼
┌───────────────┐
│ Select port   │
│ & facility    │
└───────┬───────┘
        │
        ▼
┌───────────────┐
│ Select officer│
│ & agency      │
└───────┬───────┘
        │
        ▼
┌───────────────┐
│ Define        │
│ operation     │
│ & security    │
│ level         │
└───────┬───────┘
        │
        ▼
┌───────────────┐
│ Generate      │
│ declaration   │
└───────┬───────┘
        │
        ▼
┌───────────────┐
│ Track sent /  │
│ received state│
└───────────────┘
```

This makes the application more than a document template: the declaration is connected to structured operational data.

---

## Features

- Declaration of Security creation
- Ship management
- Port management
- Port facility management
- Officer management
- Agency management
- Mail server configuration
- Operation type management
- Security level management
- Local persistence
- Declaration history
- Declaration status tracking
- Document generation
- Word document templates
- Email-related configuration
- Application licensing
- Local SQLite database
- Desktop Material Design interface

---

## Technology Stack

### Application

- C#
- .NET 5
- WPF
- XAML

### Data

- Entity Framework Core 5
- SQLite
- EF Core migrations
- Repository pattern
- Unit of Work

### Documents

- Open XML SDK
- OpenXmlPowerTools
- Microsoft Word `.docx` templates

### Security

- BCrypt

### UI

- MaterialDesignInXaml
- Material Design Colors
- Montserrat fonts

### Architecture

- Layered architecture
- Repository pattern
- Unit of Work
- Separation of domain models and persistence
- WPF application layer

---

## Architecture

The solution is split into four projects:

```text
DOS_Generator
│
├── DOS_Generator.Core
│   ├── Models
│   ├── Repositories
│   └── IUnitOfWork
│
├── DOS_Generator.Data
│   ├── Entity Framework Core
│   ├── Migrations
│   └── Repository implementations
│
├── DOS_Generator.WPF
│   ├── Views
│   ├── ViewModels
│   ├── Services
│   ├── Resources
│   └── Application
│
└── DOS_Generator.License
    └── Licensing functionality
```

The dependency direction is intentionally separated:

```text
                    WPF
                     │
          ┌──────────┼──────────┐
          ▼          ▼          ▼
        Data       License     UI
          │
          ▼
         Core
```

The Core project contains domain-level models and repository abstractions, while Data provides their persistence implementation.

The WPF project acts as the desktop application layer.

---

## Core Domain

The application models the main entities involved in maritime security declarations.

### Ship

A ship contains information such as:

```text
Ship
├── IMO Number
├── Name
├── Port of Registry
├── Email
└── Agency
```

The ship entity can therefore be associated with its agency and used when creating a declaration.

### Declaration

The central domain object is the `Declaration`.

A declaration contains relationships to:

```text
Declaration
├── Ship
├── Officer
├── Port
├── Facility
├── Operation
└── Security Level
```

It also contains:

```text
Date
Start Date
End Date
Is Sent
Is Received
```

This allows the application to represent not only the declaration itself but also its operational state.

---

## Domain Model

A simplified representation of the domain is:

```text
                     Agency
                       │
                       │
                       ▼
                     Ship
                       │
                       │
                       ▼
                 Declaration
                /      │       \
               /       │        \
              ▼        ▼         ▼
          Officer     Port     Facility
                       │
                       │
                       ▼
                  Operation
                       │
                       ▼
                 Security Level
```

The declaration acts as the central aggregate connecting the entities required for a maritime security document.

---

## Persistence

The Data project uses **Entity Framework Core 5** with SQLite.

The project contains:

- DbContext
- Entity configurations
- Repository implementations
- EF Core migrations
- Database model snapshot

The database is stored locally, making the application suitable for desktop environments where a centralized database server may not be available.

---

## Repository Pattern

The Core project defines repository abstractions such as:

```text
IAgencyRepository
IDeclarationRepository
IFacilityRepository
IMailServerRepository
IOfficerRepository
IPortRepository
IShipRepository
IUserRepository
```

The Data project provides the concrete implementations.

This separates the application/domain layer from the underlying persistence mechanism.

The general flow is:

```text
WPF ViewModel
      │
      ▼
Repository / Unit of Work
      │
      ▼
Entity Framework Core
      │
      ▼
SQLite
```

---

## Unit of Work

The Core project exposes an `IUnitOfWork` abstraction.

This provides a common boundary for coordinating repository operations and persistence.

Conceptually:

```text
Application Operation
        │
        ▼
   Unit of Work
        │
   ┌────┼────┬────┐
   ▼    ▼    ▼    ▼
 Ship  Port  User  Declaration
 Repo  Repo  Repo      Repo
        │
        ▼
       Save
        │
        ▼
     SQLite
```

This keeps persistence coordination separate from the WPF presentation layer.

---

## Document Generation

One of the important responsibilities of DOS Generator is turning structured declaration data into a usable document.

The WPF project includes Word document resources:

```text
Resources
├── Message.docx
└── template.docx
```

The application uses the Open XML ecosystem to work with Word documents.

The relevant dependencies include:

- `DocumentFormat.OpenXml`
- `OpenXmlPowerTools`

This allows the application to use a document template rather than requiring the entire declaration to be constructed manually through UI controls.

The conceptual workflow is:

```text
Structured Declaration
        │
        ▼
Load Word Template
        │
        ▼
Populate Declaration Data
        │
        ▼
Generate Document
        │
        ▼
      .docx
```

---

## WPF Application

The desktop application is implemented using Windows Presentation Foundation.

The WPF project targets:

```text
net5.0-windows
```

and enables:

```xml
<UseWPF>true</UseWPF>
```

The application uses XAML for its presentation layer.

---

## UI Design

The application uses **MaterialDesignInXaml** for its visual design system.

The application defines shared resources for:

- Buttons
- Text fields
- Password fields
- Combo boxes
- Typography
- Colors
- Fonts

The interface uses Material Design components while applying custom Montserrat typography.

The application bundles:

```text
Montserrat Regular
Montserrat Light
Montserrat Bold
```

---

## ViewModel Architecture

The WPF application follows a View/ViewModel-oriented structure.

The project contains dedicated areas for:

```text
ViewModels
├── Dialogs
├── Controls
└── Wizard
```

and corresponding views.

This keeps UI behavior and application state separated from the XAML presentation layer.

The application also uses a `ViewModelLocator` to associate views with their corresponding view models.

---

## Wizard-Based Workflow

The WPF project contains dedicated wizard views and view models.

This is appropriate for a declaration workflow because creating a declaration requires collecting information from several related entities.

A wizard-style workflow can guide the user through:

```text
Step 1
Ship

   ↓

Step 2
Port / Facility

   ↓

Step 3
Officer / Agency

   ↓

Step 4
Operation / Security

   ↓

Step 5
Review

   ↓

Step 6
Generate
```

This reduces the cognitive load compared with presenting every field on a single large form.

---

## Licensing

The solution contains a dedicated:

```text
DOS_Generator.License
```

project.

Keeping licensing separate from the main application allows licensing concerns to remain isolated from the core domain and persistence layers.

The application therefore has a structure that distinguishes:

```text
Business Domain
Persistence
Desktop UI
Licensing
```

rather than putting all functionality into a single WPF project.

---

## Security

The application includes BCrypt for password/hash-related functionality.

The solution also separates security-sensitive functionality into appropriate application areas rather than treating authentication and licensing as UI-only concerns.

Any credentials or secrets used for mail-server configuration should be supplied through local configuration and should not be committed to source control.

---

## Database Migrations

Entity Framework Core migrations are included in:

```text
DOS_Generator.Data/Migrations
```

The repository contains an initial database migration and the corresponding model snapshot.

This allows the database schema to be versioned alongside the application code.

---

## Project Structure

```text
DOS_Generator
│
├── DOS_Generator.Core
│   │
│   ├── Models
│   │   ├── Agency.cs
│   │   ├── Declaration.cs
│   │   ├── Facility.cs
│   │   ├── MailServer.cs
│   │   ├── Officer.cs
│   │   ├── OperationType.cs
│   │   ├── Port.cs
│   │   ├── Ship.cs
│   │   └── User.cs
│   │
│   ├── Repositories
│   │   ├── IAgencyRepository.cs
│   │   ├── IDeclarationRepository.cs
│   │   ├── IFacilityRepository.cs
│   │   ├── IMailServerRepository.cs
│   │   ├── IOfficerRepository.cs
│   │   ├── IPortRepository.cs
│   │   ├── IShipRepository.cs
│   │   └── IUserRepository.cs
│   │
│   └── IUnitOfWork.cs
│
├── DOS_Generator.Data
│   │
│   ├── Migrations
│   └── Repositories
│
├── DOS_Generator.WPF
│   │
│   ├── Database
│   ├── Resources
│   │   ├── Fonts
│   │   ├── Message.docx
│   │   └── template.docx
│   ├── Services
│   ├── ViewModels
│   │   ├── Controls
│   │   ├── Dialogs
│   │   └── Wizard
│   └── Views
│       └── Wizard
│
└── DOS_Generator.License
```

---

## Getting Started

### Requirements

The application is a Windows desktop application and requires a Windows-compatible .NET development environment.

The project targets:

```text
.NET 5
net5.0-windows
```

You should therefore use a Visual Studio version/toolchain capable of building the .NET 5 WPF project.

---

## Clone the Repository

```bash
git clone https://github.com/mdinacer/DOS_Generator.git

cd DOS_Generator
```

---

## Build

Open:

```text
DOS_Generator.sln
```

in Visual Studio.

The solution contains the following projects:

```text
DOS_Generator.Core
DOS_Generator.Data
DOS_Generator.WPF
DOS_Generator.License
```

Build the solution using the desired configuration:

```text
Debug
Release
```

The solution also contains configurations for:

```text
Any CPU
x64
x86
```

---

## Run

Set:

```text
DOS_Generator.WPF
```

as the startup project and run the application from Visual Studio.

Because the application is a WPF desktop application, it must be run on Windows.

---

## Database

The application uses a local SQLite database.

The WPF project includes:

```text
Database/Dos.db
```

as a database resource.

Entity Framework Core migrations are included in the Data project.

---

## Configuration

The WPF application includes:

```text
appsettings.json
```

for application configuration.

Mail-server configuration is represented as part of the application domain through the `MailServer` model and repository.

Production credentials should never be committed to the repository.

---

## Engineering Highlights

DOS Generator demonstrates several areas of software engineering:

- C# application development
- WPF desktop development
- XAML UI development
- Layered application architecture
- Domain modeling
- Entity Framework Core
- SQLite persistence
- Repository pattern
- Unit of Work pattern
- Database migrations
- Document generation
- Open XML
- Word document templates
- Material Design UI
- ViewModel-based WPF architecture
- Wizard-based workflows
- Application licensing
- BCrypt
- Local-first desktop architecture

---

## Domain + Engineering

One of the defining aspects of this project is the combination of **domain knowledge and software engineering**.

The application is not a generic CRUD demonstration. Its domain model reflects real maritime and port operations:

```text
Ship
Port
Facility
Officer
Agency
Operation
Security Level
Declaration
```

This makes the project an example of building software around a specialized operational workflow rather than simply implementing generic entities.

---

## Project Context

DOS Generator was developed as a domain-specific software application for maritime/port operations.

The project reflects an intersection between:

```text
Maritime Operations
        +
Software Engineering
        +
Desktop Application Development
        +
Document Automation
```

This combination is particularly useful when demonstrating the ability to translate real operational requirements into a working software system.

---

## Repository

GitHub:

https://github.com/mdinacer/DOS_Generator

---

## License

No explicit open-source license is currently defined for this repository.
