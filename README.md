# Brain Wave Time Management App

A cross-platform time management system designed for **students and professionals** to improve productivity.

## Features
- ✅ Task management (create, update, delete)
- ✅ Reminders & notifications (email + in-app)
- ✅ Offline functionality with sync
- ✅ Collaboration tools (shared tasks, team projects)
- ✅ Gamification (badges & rewards)
- ✅ Export (PDF, Excel)

## Tech Stack
- **Frontend**: .NET MAUI (C#)
- **Backend**: ASP.NET Core Web API
- **Database**: SQLite (offline),Supabase (cloud sync)
- **Auth**: Role-based access (Admin, User)
- **Export**: Apache POI / iText for PDF

## Project Setup
```bash
git clone https://github.com/your-org/brainwave.git
cd brainwave
dotnet restore
dotnet build

## Project Architecture
BrainWave/
│
├── backend/                      # API + Database Layer
│   ├── BrainWave.Api/            # ASP.NET Core Web API (task sync, reminders, collab)
│   │   ├── Controllers/          # REST controllers (Tasks, Users, Notifications)
│   │   ├── Models/               # Data models (Task, User, Badge, Reminder)
│   │   ├── DTOs/                 # Transfer objects
│   │   ├── Services/             # Business logic (TaskService, AuthService, ExportService)
│   │   ├── Data/                 # EF Core / Firebase integration
│   │   └── Program.cs
│   └── BrainWave.Tests/          # Unit + Integration tests
│
├── frontend/                       # .NET MAUI Cross-platform App
│   ├── BrainWave.App/            
│   │   ├── Views/                # XAML pages (TaskListPage, TaskDetailPage, ProfilePage)
│   │   ├── ViewModels/           # MVVM bindings
│   │   ├── Models/               # Local models (mirroring API)
│   │   ├── Services/             # API client, Offline sync service
│   │   ├── Resources/            # Images, Styles
│   │   ├── Platforms/            # Android/iOS/Desktop specific files
│   │   └── App.xaml.cs
│
├── docs/                         # Project documentation
│   ├── requirements.md           # Pulled from CMPG213 Phase 2
│   ├── architecture.md           # System architecture diagrams
│   ├── usecases.md               # Use-case glossary
│   └── pert-gantt.md             # Scheduling
│
├── scripts/                      # DevOps / Automation
│   ├── seed-db.ps1               # Seed DB with test data
│   ├── build.ps1                 # Build automation
│   └── deploy.ps1                # Deployment script
│
├── .gitignore
├── README.md
└── BrainWave.sln

