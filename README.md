# Event Manager 2.0

CST8359 Assignment 2 - an ASP.NET Core MVC application for managing events and their attendees.

**Student:** Mateusz Gumienny  
**Student number:** 041033057  
**Username:** Gumi0002  
**Student email:** gumi0002@algonquinlive.com

## Assignment requirements implemented

- Entity Framework Core with SQL Server and an Azure SQL-ready connection string
- Database initializer with three events and two attendees per event
- Full CRUD for events
- Full nested CRUD for attendees
- Attribute routes such as `/events/1/attendees` and `/events/1/attendees/create`
- Banner upload on event creation and editing
- Public Azure Blob Storage uploads with the resulting URL saved in the event record
- Local banner storage fallback when Azure Storage is not configured
- Responsive event list, details, forms, and attendee-management pages
- Validation, anti-forgery protection, file type/size checks, and cascade deletion

## Local setup

The app uses its in-memory fallback when a SQL connection string is not configured, so it runs without local credentials and remains deployable for review. From the repository root:

```powershell
dotnet restore EventManager.slnx
dotnet run --project EventManager/EventManager.csproj
```

To test real Azure services locally, keep credentials out of `appsettings.json` and use .NET user-secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your Azure SQL connection string>" --project EventManager/EventManager.csproj
dotnet user-secrets set "AzureBlobStorage:ConnectionString" "<your Azure Storage connection string>" --project EventManager/EventManager.csproj
dotnet user-secrets set "AzureBlobStorage:ContainerName" "event-banners" --project EventManager/EventManager.csproj
```

## Azure App Service configuration

Configure these application settings in Azure rather than committing credentials:

- `ConnectionStrings__DefaultConnection`
- `AzureBlobStorage__ConnectionString`
- `AzureBlobStorage__ContainerName=event-banners`

The application applies its EF Core migration and seeds data at startup. The storage service creates the banner container with public blob access when needed.

## AI use disclosure

OpenAI Codex was used to review the assignment specification, refactor the supplied reference implementation, improve validation and security, implement Azure Blob upload support, and refine the responsive UI. All generated code was reviewed and tested as part of the submission workflow.
