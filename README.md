# 🎟️ Evently — Event Management Application

**Evently** is a full-stack web application for creating, viewing, and managing events.  
Built with **.NET 8 Web API**, **Angular**, and **PostgreSQL**, fully containerized with Docker.

---

## 🧰 Tech Stack

- ASP.NET Core 8 Web API (C#)
- Angular + TypeScript
- Entity Framework Core + PostgreSQL
- Docker & Docker Compose
- JWT Authentication using ASP.NET Identity
- Tailwind CSS for UI styling

---

## ⚙️ Requirements

Make sure you have the following installed:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) *(optional for local backend builds)*
- [Node.js 20+](https://nodejs.org/en) *(optional for local frontend builds)*
- [Angular CLI](https://angular.io/cli) *(optional, for development mode)*

---

## 🚀 Getting Started

### 1️⃣ Clone the repository
```bash
git clone https://github.com/vladyslavplus/Application.git
cd Application
```

---

### 2️⃣ Create a `.env` file
In the project root, copy the example file:

```bash
cp .env.example .env
```

Then adjust environment variables if needed:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=evently_db

JWT__KEY=this_is_a_very_long_and_secure_secret_key_1234567890
JWT__ISSUER=Evently.Api
JWT__AUDIENCE=Evently.Client

ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

CONNECTIONSTRINGS__DEFAULTCONNECTION=Host=evently-db;Port=5432;Database=evently_db;Username=postgres;Password=postgres
```

---

### 3️⃣ Run the application
From the project root:

```bash
docker-compose up --build
```

This will:
- Start the PostgreSQL database  
- Wait until it's healthy  
- Launch both the **Evently API** and **Angular UI** containers

---

### 4️⃣ Access the Application

#### 🌐 Frontend (Angular)
- [http://localhost:4200](http://localhost:4200)

#### ⚙️ Backend (API + Swagger)
- [http://localhost:5000/swagger](http://localhost:5000/swagger)
- Base API URL → `http://localhost:5000/api`

---

### 5️⃣ Test with default credentials

**Admin Account:**
- Email: `admin@example.com`
- Password: `Admin@1234`

**Regular User:**
- Email: `user@example.com`
- Password: `User@1234`

---

### 6️⃣ Stop containers
To shut everything down:

```bash
docker-compose down
```

---

## 🧾 License
This project was created for educational purposes.