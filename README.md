# Patient Appointment Network Data Application

This is my solution to the [Aire Logic Patient Appointment Backend Tech Test](https://github.com/airelogic/tech-test-portal/tree/main/Patient-Appointment-Backend)

## Approach

Given the requirements specified the following decisions have been made:

- Considering the client's past burnout with vendor lock-in and their love for smaller frameworks, I have chosen to use ASP.NET Core. It's open-source, cross-platform, and lightweight.
- To make the PANDA database-agnostic, Entity Framework Core (EF Core) has been used.
- SQLite has been used for development due to its simplicity, but for production it should be relatively painless to switch to another provider (e.g., PostgreSQL or SQL Server).
- Since they're eyeing foreign markets, I have used ASP.NET Core's in-built support for localization and globalization. By using resource files (.resx), we can store localized strings for different cultures.
- UTF-8 encoding has been used throughout the application and database, to ensure we support a wide range of international characters.
- Data Transfer Objects (DTOs) have been used to decouple the API layer and data layer, which adds a layer of security by controlling exposed data.
- Model validation has been used, to validate NHS Number and Postcode.
- Global Exception Handling middleware has been implemented to catch exceptions globally and return structured, meaningful error responses.
- Logging has been implemented using Serilog, which is important for debugging issues, and can be configured to send logs to different sources, depending on where the API is hosted.
- The application and database uses Coordinated Universal Time (UTC) for timestamps.
- Unit tests ensure the business logic is implemented correctly. These can be automated as part of a CI/CD pipeline.

## Assumptions

The following assumptions were made during development:

- There is no information in the spec about what details we need to record for Clinician, Department and Organisation, so for now I am just storing ID and Name for Clinician and Department, and ID and Postcode for Organisation (with the provision to also store Name and OrgCode/ODS Code).
- For now Duration is a string, but could be extracted to a lookup list in future.
- Clinician and Department include a property that lists missed appointments, although this isn't used anywhere in the code.
- I assume OpenAPI is sufficient to document the API. If I had more time I would have also provided a Postman collection.
- I have provided some basic unit tests, which could be built into a CI/CD pipeline for automated testing. Given more time I would increase the test coverage, as well as creating integration tests and automation tests using Postman.

## Next Steps

- Due to time constraints all functionality has been built into a single API. In the future patient and appointment functionality could be split into separate microservices.
- The API could be containerised (e.g., with Docker) to make the API consistent across environments.
- Use of CI/CD pipelines would allow to automation of testing and delivery.


## Setup Guide

### Prerequisites
Before you begin, ensure you have the following installed on your machine:
- [.NET SDK](https://dotnet.microsoft.com/download)
- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Step-by-Step Guide

#### 1. Clone the Repository
First, clone the repository.
```
bash
git clone https://github.com/matthewandrewthomas/PANDA.Api.git
cd your-repo-name
```

#### 2. Configure SQLite
Ensure that your project is set up to use SQLite. Update the connection string in the appsettings.json file:
```
"ConnectionStrings": {
  "DefaultConnection": "Data Source=PANDA.db"
}
```

#### 3. Apply Migrations
Use the Entity Framework Core tools to apply the latest migrations to your SQLite database.
```
dotnet ef database update
```

#### 4. Run the Application
Start the Web API using the .NET CLI.
```
dotnet run
```

## Interacting with the Web API

The API is documented using Swagger. Swagger UI provides a web-based interface to interact with your API, making it easy to explore and test the endpoints.

### Accessing the Swagger UI

Once your API is running, you can access the Swagger UI at the following URL:

[http://localhost:5225/swagger/index.html](http://localhost:5225/swagger/index.html)

### Using Swagger UI

1. **Navigate to the Swagger URL**:
   Open your browser and go to [http://localhost:5225/swagger/index.html](http://localhost:5225/swagger/index.html).

2. **Explore API Endpoints**:
   The Swagger UI will list all available API endpoints. You can expand each endpoint to see the details, including HTTP methods, request parameters, and response formats.

3. **Testing Endpoints**:
   Swagger UI allows you to test the endpoints directly from the interface:
   - Click on an endpoint to expand its details.
   - Fill in any required parameters.
   - Click the "Try it out" button to send a request to the API and see the response.

### Troubleshooting

If you encounter any issues with the Swagger UI, ensure that:
- Your API is running and accessible.
- The correct URL is being used (check for any typos).

For further assistance, refer to the official [Swagger Documentation](https://swagger.io/docs/).
