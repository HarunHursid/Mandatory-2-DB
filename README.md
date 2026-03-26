# 🧠 DISC Profiles API

A full-stack **multi-database ASP.NET Core Web API** for managing DISC personality profiles across an organization — built as a mandatory database exam project. The system stores and mirrors data across three different database engines simultaneously: **SQL Server**, **MongoDB**, and **Neo4j**.

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│                  ASP.NET Core API                   │
│  ┌────────────┐  ┌─────────────┐  ┌──────────────┐ │
│  │ SQL Server │  │   MongoDB   │  │    Neo4j     │ │
│  │  (EF Core) │  │   (Atlas)   │  │   (Graph)    │ │
│  └────────────┘  └─────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────┘
         │                                  │
  DiscProfilesMigrator              GraphMigrator
  (SQL → MongoDB sync)           (SQL → Neo4j sync)
```

---

## ✨ Features

- 🔐 **JWT Authentication** — register, login, role-based authorization (`Admin` / `Employee`)
- 🗄️ **SQL Server** — full relational model with Entity Framework Core
- 🍃 **MongoDB Atlas** — mirrored document store for all entities
- 🔗 **Neo4j** — graph database mirroring employee relationships, projects, DISC profiles
- 🔍 **Cross-database search** — unified `/api/search` endpoint querying SQL, MongoDB and Neo4j simultaneously
- 🔄 **Data migration tools** — standalone migrators to sync SQL → MongoDB and SQL → Neo4j
- 🖥️ **Built-in frontend** — static HTML pages (login, employee dashboard, admin panel, profile page)
- 📄 **Swagger UI** — interactive API documentation with JWT support

---

## 📦 Tech Stack

| Layer | Technology |
|-------|------------|
| API Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core |
| Relational DB | SQL Server |
| Document DB | MongoDB Atlas |
| Graph DB | Neo4j |
| Auth | JWT Bearer Tokens |
| Mapping | AutoMapper |
| API Docs | Swagger / OpenAPI |
| Env Config | DotNetEnv |

---

## 🗂️ Project Structure

```
Mandatory-2-DB/
├── DiscProfilesApi/           # Main Web API
│   ├── Controllers/
│   │   ├── SQL/               # SQL Server controllers (19 endpoints)
│   │   ├── MongoControllers/  # MongoDB mirror controllers
│   │   ├── Graph/             # Neo4j graph controllers
│   │   └── SearchController   # Unified cross-DB search
│   ├── Models/                # EF Core entity models
│   ├── MongoDocuments/        # MongoDB document models
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Repositories/          # Generic repository pattern
│   ├── Services/              # Business logic & graph services
│   ├── Mappings/              # AutoMapper profiles
│   └── wwwroot/               # Static frontend (HTML pages)
├── DiscProfilesMigrator/      # SQL → MongoDB migration tool
└── GraphMigrator/             # SQL → Neo4j migration tool
```

---

## 📋 Domain Entities

| Entity | Description |
|--------|-------------|
| `Company` | Organisation with location and business field |
| `Department` | Company department |
| `Employee` | Works at a company, has a DISC profile |
| `Person` | Personal info linked to an employee |
| `Position` | Job title/role |
| `DiscProfile` | D/I/S/C personality type with colour and description |
| `Project` | Organisational project |
| `Task` | Work item linked to a project |
| `DailyTaskLog` | Daily time tracking per task |
| `TaskEvaluation` | Evaluation record for a completed task |
| `StressMeasure` | Employee stress tracking entry |
| `SocialEvent` | Team event recommendations based on DISC type |
| `Education` | Employee educational background |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or Azure)
- MongoDB Atlas account
- Neo4j instance (local or Aura)

### Environment Variables

Create a `.env` file in `DiscProfilesApi/`:

```env
CONNECTION_STRING=Server=...;Database=DiscProfilesDB;...
MONGO_CONNECTION_STRING_ATLAS=mongodb+srv://...
MONGO_DATABASE_NAME_ATLAS=DiscProfilesDB
NEO4J_URI=bolt://localhost:7687
NEO4J_USER=neo4j
NEO4J_PASSWORD=yourpassword
NEO4J_DATABASE=neo4j
```

### Run the API

```bash
cd DiscProfilesApi
dotnet run
```

Swagger UI available at: `http://localhost:<port>/swagger`

### Migrate Data

```bash
# SQL → MongoDB
cd DiscProfilesMigrator
dotnet run

# SQL → Neo4j
cd GraphMigrator
dotnet run
```

---

## 🔍 API Endpoints (highlights)

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive JWT token |

### Search (cross-database)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/search/sql` | Search all SQL entities by text |
| GET | `/api/search/mongo` | Search MongoDB documents |
| GET | `/api/search/neo` | Search Neo4j graph nodes |

### Core Resources (SQL + MongoDB + Neo4j mirrors)
All major entities expose full CRUD via three sets of controllers:
- `GET/POST/PUT/DELETE /api/{entity}` — SQL Server
- `GET/POST/PUT/DELETE /api/mongo/{entity}` — MongoDB
- `GET/POST/PUT/DELETE /api/graph/{entity}` — Neo4j

---

## 🖥️ Frontend Pages

Served as static files from `/wwwroot`:

| Page | Route | Description |
|------|-------|-------------|
| Login | `/login.html` | Authenticate and receive token |
| Employee | `/employee.html` | Employee self-service dashboard |
| Admin | `/admin.html` | Admin management panel |
| My Profile | `/me.html` | View your own DISC profile |

---

## 🔒 Security

- Passwords hashed before storage
- JWT tokens with configurable issuer, audience and expiry
- Role-based authorization (`Admin` / `Employee`)
- All sensitive configuration via environment variables (never committed)

---

## 📚 About DISC

**DISC** is a behaviour assessment model identifying four personality types:

| Type | Colour | Trait |
|------|--------|-------|
| **D** — Dominance | 🔴 Red | Results-oriented, direct |
| **I** — Influence | 🟡 Yellow | Enthusiastic, social |
| **S** — Steadiness | 🟢 Green | Supportive, calm |
| **C** — Conscientiousness | 🔵 Blue | Analytical, detail-oriented |

This system helps organisations match employees to projects and social events based on their personality type.

---

## 👤 Author

**Harun Hursid** — [@HarunHursid](https://github.com/HarunHursid)
