# Glossary App - Project Overview

Glossary App is a glossary management system that allows users to store, display, and validate terms and their definitions.
It is developed as a .NET 8 Web API with a React frontend, following the principles of three-layer architecture (API, Service, Data).

The system provides authentication, role-based access control, term publishing, archiving, and validation — including a forbidden words check.

---

## Technologies and Libraries

### Backend

- C# (.NET 8) — RESTful Web API
- Entity Framework Core — ORM for database interaction and migrations
- Microsoft Identity — User identity, authentication, and authorization
- Serilog — Structured logging for debugging and analysis
- Custom Exception Middleware — Global error handling and standardized API responses

### Frontend

- React
- Redux Toolkit — State management
- Bootstrap — UI styling
- JavaScript

### Database

- PostgreSQL

---

## Testing
- xUnit - Unit and integration tests
- Moq - Mocking dependencies for isolated testing

---

## Running the Application

### Backend
1. Clone the repository
2. Open the solution in Visual Studio
3. In the Glossary.API project, create an appsettings.json with the following content:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "GlossaryDb": "Host=localhost;Port=5432;Database=GlossaryDb;Username=postgres;Password=YOUR_PASSWORD_HERE"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:dd-MM HH:mm:ss} {Level:u3}] |{SourceContext}| {NewLine}{Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/Glossary-Api-.log",
          "rollingInterval": "Day",
          "rollOnFileSizeLimit": true,
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ]
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_HERE", 
    "Issuer": "Glossary.BackendAPI",
    "Audience": "Glossary.FrontendClient"
  },
  "GlossarySettings": {
    "MinDefinitionLength": 30
  },
  "AllowedOrigins": [
    "http://localhost:3000"
  ],
  "AllowedHosts": "*"
}

```
   
5. Ensure a PostgreSQL database named GlossaryDb exists.
6. Run the backend project
7. You can also access Swagger UI : https://localhost:7163/swagger/index.html

### Frontend

1. Go to the location where the frontend app is located
2. Open CMD and type: code .
3. In the VS Code terminal, run: npm install
4. To start the application: npm start
5. The app will run at: http://localhost:3000

---

## Notes

- appsettings.json is not included in the repository and must be created manually.
- When the application starts for the first time, seed data will be automatically inserted into the database if no users or terms or forbidden words exist.
- you can add your own value for jwt key and make sure to enter valid connection string in appsettings.json

---

## Functionalities

### Unauthenticated User
- Can view an alphabetically sorted list of published terms on the homepage (/).

### Authenticated Glossary Author 
- Sees all published terms + their own draft and archived terms
- Can create, publish, archive, or delete terms (only their own)
- Access control ensures authors can only modify their own entries

- ### Term Management Rules
  
| Action                    | Conditions                                                                                                      |
| ------------------------- | --------------------------------------------------------------------------------------------------------------- |
| **Create**                | New term is created with status **Draft**                                                                       |
| **Publish**               | Definition must have **≥30 characters**, term and definition required, and must not contain **forbidden words** |
| **Archive**               | Allowed only if the term is **Published**                                                                       |
| **Delete**                | Allowed only if the term is **Draft**                                                                           |
| **Forbidden Words Check** | Based on entries stored in the `ForbiddenWords` table in the database                                           |


- Forbidden words are stored in the database so that admins can manage them dynamically without redeploying the application.

--- 

## How to Test the Application

### Viewing Published Terms
- When the application starts, all published terms are displayed alphabetically.

### Login
- To log in, you can use one of the pre-seeded users, for example:

  - `username: author1`
  - `password: Author1123!

- After login, the homepage (/) shows all published terms and user's own drafts and archived terms.

### Signup

- Fill in: Email, Unique username, Password.
- After registration, the user is redirected to the homepage.

###  Homepage Overview
- Navbar: App name, login/logout links
- Card section: Add new term
- Table: List of terms with actions (archive, publish, delete)
- Pagination: Easy navigation between term pages

- Each action includes a confirmation alert.
- Errors (e.g., invalid definition) are displayed using React Toastify notifications.

---

## Backend Architecture
- API Layer
    - Handles HTTP requests and responses
    - Uses DTOs and AutoMapper for data transfer
    - Configures CORS for the frontend
- Service Layer
    - Contains business logic
    - Handles validation, authorization, and exception throwing
- Data Layer
    - Manages database operations via Entity Framework Core
    - Uses IdentityDbContext for user management

--- 

## Authentication & Authorization
- Authentication
    - Users authenticate using ASP.NET Identity.
    - On successful login, a JWT token is generated and attached to requests.
- Authorization
    - API validates JWT tokens on every protected route
    - Only term authors can manage (edit, publish, archive, delete) their own terms.

--- 

## Error Handling
- A centralized exception middleware globally handles all exceptions.
- It translates them into clear, standardized responses with appropriate HTTP status codes.
- Data validation is enforced with Data Annotation attributes, ensuring only valid inputs are processed.

---

## Future Improvements

1. Admin UI for Forbidden Words
   - A simple panel for managing forbidden words directly from the app instead of manually editing the database.
3. Caching
   - Implement caching for forbidden words to improve performance.
   - Decide whether cache refreshes happen periodically or on app startup.
5. Filters & Search
   - Add filtering by term status and searching by term name.
   - Consider hiding archived terms by default, with an optional “Show Archived” filter.
7. Authorization Refactoring
   - Current authorization logic repeats across multiple service methods:

     ```
       if (glossaryTerm.AuthorId != userId)
      throw new ForbidException();
     
     ```
    - Plan: Move this logic into a private helper method within GlossaryTermsService.
    - For future scalability, extract it into a shared AuthorizationHelper class.
    - This would reduce code duplication and simplify unit testing.


## Screenshots

### Login Form

<img width="552" height="453" alt="Image" src="https://github.com/user-attachments/assets/8c3de0bb-2503-4ff6-8086-93adb59787d6" />

---

### Register Form

<img width="677" height="620" alt="Image" src="https://github.com/user-attachments/assets/92aa5a02-51e3-41b0-8f78-76e052f4855f" />

---

### Unauthenticated User View

<img width="1441" height="437" alt="Image" src="https://github.com/user-attachments/assets/3533e405-5106-42e8-b4dc-d01c6f292aaa" />

---

### Authenticated User View

<img width="1394" height="593" alt="Image" src="https://github.com/user-attachments/assets/df2ecd12-f0c9-4f41-8d30-4b9cbcc5cd02" />

---

### Creating a Term

<img width="1330" height="748" alt="Image" src="https://github.com/user-attachments/assets/4c326a85-f00e-4ac7-8d2b-47b597bdfcbb" />

---

### Notification Message

<img width="964" height="552" alt="Image" src="https://github.com/user-attachments/assets/61c56d70-10f4-44fd-8f13-905f7eb9480d" />

