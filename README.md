# Online Examination

An **ASP.NET Core Blazor Interactive Server** web application for creating and taking online examinations. Built with C#, Entity Framework Core, and SQL Server.

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core 8.0 |
| UI | Blazor Interactive Server (Server-Side Rendering + SignalR) |
| Language | C# 12, HTML/Razor, CSS |
| Database | SQL Server |
| ORM | Entity Framework Core 8.0 |
| Authentication | ASP.NET Core Identity (Role-Based: Admin / Student) |
| Charts | Chart.js (CDN) |
| Email | Gmail SMTP |

### NuGet Packages

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 8.0.22
- `Microsoft.AspNetCore.Identity.UI` 8.0.22
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.22
- `Microsoft.AspNetCore.Components.QuickGrid.EntityFrameworkAdapter` 8.0.22
- `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore` 8.0.22

---

## Features

### Admin
- **Dashboard** with analytics charts (Chart.js)
- **Create Exams** — title, description, time limit (1–180 min), access code, education level, subject
- **Manage Questions** — multiple-choice (4 options A–D), correct answer, optional image upload, optional reading passage
- **Manage Students** — view, edit, delete student accounts
- **Global Exam History** — view all student attempts across all exams
- **Automatic Grading** — score calculated instantly upon submission

### Student
- **Join Exam** via unique 8-character access code
- **Take Exam** — timed exam interface with question-by-question or full-page views
- **Exam History** — view past attempts with scores
- **Mock Tests** — practice exams filtered by education level (PSLE / N-Level / O-Level / Poly / JC)
- **Auto-generated Math Questions** — procedural generation across 4 difficulty levels (Easy → Expert)

### General
- **Role-based Navigation** — different menus for Admin and Student
- **Registration & Login** with email verification
- **Forgot / Reset Password** via Gmail SMTP

---

## Architecture

```
┌──────────────────────────────────────────────┐
│               Blazor Components (UI)          │
│   Pages, Layouts, NavMenu, Razor Components   │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│            Controllers (REST API)             │
│       POST /api/login, /api/auth/*            │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│              Services (Business Logic)         │
│   ExamService, StudentService, UserSession,   │
│   GmailEmailSender, LocalMathGenerator        │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│          Data / Entity Framework Core          │
│    Online_ExaminationContext, Migrations      │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│              SQL Server Database              │
└──────────────────────────────────────────────┘
```

---

## Domain Models

```
Online_ExaminationUser (extends IdentityUser)
    │
    ├── CreatedExams (1:N) ──→ Exam
    │       │
    │       ├── Questions (1:N) ──→ Question
    │       │     • Text, Options A–D, CorrectAnswer
    │       │     • Optional: ImageUrl, ReadingPassage
    │       │
    │       └── Attempts (1:N) ──→ Attempt
    │             • User FK, Exam FK, Score
    │
    └── Attempts (1:N)
```

**BaseDomainModel** — abstract base class with `Id`, `DateCreated`, `DateUpdated`, `CreatedBy`, `UpdatedBy`.

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express, or Developer edition)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or any code editor

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/BoooSAMA/Online-Examination.git
cd Online-Examination
```

### 2. Configure the Connection String

Edit `Online Examination/appsettings.json` to point to your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=OnlineExaminationDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Configure Gmail SMTP (for password reset)

Edit `Online Examination/Services/GmailEmailSender.cs` and replace the credentials:

```csharp
// Replace with your own Gmail + App Password
private const string Email = "your-email@gmail.com";
private const string AppPassword = "your-app-password";
```

> **Security Note**: In production, store credentials in environment variables or Azure Key Vault — never in source code.

### 4. Run Database Migrations

```bash
cd "Online Examination"
dotnet ef database update
```

Or via Package Manager Console in Visual Studio:

```
Update-Database
```

### 5. Run the Application

```bash
dotnet run
```

Or press `F5` in Visual Studio.

The app will start at `https://localhost:5001` (or the port specified in `launchSettings.json`).

---

## Default Accounts

On first run, the database seeder creates these accounts automatically:

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@test.com` | `Admin123` |
| Student | `student@test.com` | `Student123` |

---

## Project Structure

```
Online-Examination/
├── Online Examination.slnx                    # Solution file
├── README.md
│
└── Online Examination/                        # Main project
    ├── Program.cs                             # Entry point, DI, middleware
    ├── appsettings.json                       # Configuration
    │
    ├── Domain/                                # Entity models
    │   ├── BaseDomainModel.cs
    │   ├── Exam.cs
    │   ├── Question.cs
    │   ├── Attempt.cs
    │   └── Online_ExaminationUser.cs
    │
    ├── Data/                                  # EF Core DbContext + Seeder
    │   ├── Online_ExaminationContext.cs
    │   └── DatabaseSeeder.cs
    │
    ├── Migrations/                            # EF Core migrations
    │
    ├── Controllers/                           # REST API endpoints
    │   ├── LoginController.cs
    │   └── AuthController.cs
    │
    ├── Services/                              # Business logic
    │   ├── ExamService.cs
    │   ├── StudentService.cs
    │   ├── UserSession.cs
    │   ├── GmailEmailSender.cs
    │   └── QuestionGenerators/
    │       └── LocalMathGenerator.cs
    │
    ├── Components/                            # Blazor UI
    │   ├── Layout/
    │   │   ├── MainLayout.razor
    │   │   └── NavMenu.razor
    │   ├── Pages/
    │   │   ├── Home.razor
    │   │   ├── Login.razor
    │   │   ├── AdminDashboard.razor
    │   │   ├── UserDashboard.razor
    │   │   ├── ExamCreate.razor
    │   │   ├── ExamPage.razor
    │   │   ├── ExamResult.razor
    │   │   ├── JoinExam.razor
    │   │   ├── ExamHistory.razor
    │   │   ├── Payment.razor
    │   │   └── ...
    │   └── Account/                           # Identity scaffolded pages
    │
    └── wwwroot/                               # Static assets
        ├── app.css
        ├── css/
        │   ├── exams-page.css
        │   ├── indexstyle.css
        │   └── site.css
        └── pics/
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/login` | User login |
| POST | `/api/auth/forgot-password` | Send password reset email |
| POST | `/api/auth/reset-password` | Reset password with token |

---

## UI Pages

| Route | Page | Access |
|-------|------|--------|
| `/` | Home | Public |
| `/about` | About Us | Public |
| `/Account/Login` | Login | Public |
| `/Account/Register` | Registration | Public |
| `/forgot-password` | Forgot Password | Public |
| `/admin-dashboard` | Admin Dashboard | Admin |
| `/admin/exam-create` | Create Exam | Admin |
| `/exam-index` | Manage Exams | Admin |
| `/modify-students` | Manage Students | Admin |
| `/modify-exam/{id}` | Edit Exam | Admin |
| `/admin/global-history` | Global History | Admin |
| `/user-dashboard` | Student Dashboard | Student |
| `/student/join-exam` | Join Exam | Student |
| `/exams` | Available Exams | Student |
| `/take-exam/{id}` | Take Exam | Student |
| `/exam-history` | My History | Student |
| `/exam-result/{id}` | View Result | Both |
| `/payment` | Payment | Student |
| `/mock-test/{level}` | Mock Test | Student |

---

## Database Migrations

| Migration | Date | Change |
|-----------|------|--------|
| `InitialCreate` | Jan 16, 2026 | Base schema |
| `AddAccessCodeToExam` | Jan 18, 2026 | Unique exam access codes |
| `AddEducationLevelToExam` | Jan 18, 2026 | Education level field |
| `AddExamSubject` | Jan 21, 2026 | Subject categorization |
| `AddJCLevel` | Jan 21, 2026 | Junior College level |

---

## Security Notes

- ⚠️ **Email credentials are hardcoded** in `GmailEmailSender.cs`. Before deploying to production, move them to environment variables or a secrets manager.
- Password policy is intentionally relaxed for development. Tighten `PasswordOptions` in `Program.cs` for production use.
- The `LoginController` endpoint has `[IgnoreAntiforgeryToken]` — evaluate if this is necessary for your deployment.

---

## License

This project is open source. Add a license file if you intend to distribute it.
