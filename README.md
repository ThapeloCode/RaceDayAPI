# RaceDay - Full-Stack Event Management Platform

## Project Overview
**RaceDay** is a full-stack, cloud-aware event management system designed specifically for the South African road running, walking, and cycling community. The platform empowers Event Organisers to create and manage sporting events, distance categories, routes, and timing results, while enabling Participants to browse upcoming events, enter races, track performance history, and view live race-day weather integrations.

## Technical Stack & Architecture

**Framework:** ASP.NET Core Web API (.NET 8)[cite: 8, 11]
**ORM / Database:** Entity Framework Core (Code-First)[cite: 11, 12] & Microsoft SQL Server (`RaceDayDb`)[cite: 7, 12]
**Authentication & Security:** JWT Bearer Authentication with Role-Based Access Control (`Organiser`, `Admin`, `Participant`) & BCrypt password hashing
**Testing Suite:** xUnit Test Framework with EF Core In-Memory Database Provider[cite: 11]
**Continuous Integration (CI/CD):** GitHub Actions Automated Build & Test Pipeline (`.github/workflows/ci.yml`)[cite: 8, 11]
**External Integration:** Live Weather Forecast API Integration (Open-Meteo)



## Repository Structure

```text
RaceDay/
├── .github/
│   └── workflows/
│       └── ci.yml
├── RaceDay/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   ├── appsettings.json
│   ├── Program.cs
│   └── RaceDay.csproj
├── RaceDay.Tests/
│   ├── EventsControllerTests.cs
│   ├── RegistrationsControllerTests.cs
│   ├── ResultsControllerTests.cs
│   └── RaceDay.Tests.csproj
└── RaceDay.sln                       <-- Visual Studio Solution File
