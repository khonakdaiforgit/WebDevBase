# Professional Restaurant Management System
**Full-featured Backend for Independent Restaurants & Cafés**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-5C2D91?logo=.net&logoColor=white)](https://dotnet.microsoft.com)
[![MongoDB](https://img.shields.io/badge/MongoDB-6%2B-47A248?logo=mongodb&logoColor=white)](https://www.mongodb.com)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean/Onion-FF6F61)](https://jasontaylor.dev/clean-architecture/)

## Project Overview

A clean / Onion Architecture Restaurant Management System, owner-centric backend system built specifically for **one independent restaurant or café** (single-tenant).  
-based owners who want full control over their menu, gallery, news, newsletter, and customer communication — without relying on expensive third-party platforms.

This repository contains the **complete backend + domain layer** written with Clean/Onion Architecture in .NET 8 and MongoDB.


## Implemented Features (All Fully Working)

| Module             | Features
|---------------------|---------------------------------------------------------------|
| **Restaurant**      | Create & update profile, working hours, location (lat/lng), real-time open/closed status
| **Menu Management** | Categories + items, pricing, images, descriptions, availability toggle, (move up/down)
| **Gallery**         | Upload images, captions, show/hide, delete, pagination
| **News & Events**   | Create, edit, publish/unpublish, schedule-ready
| **Newsletter**      | Draft campaigns, send instantly via real SMTP, track status
| **Subscribers**     | Subscribe, email confirmation, unsubscribe (token), list active emails
| **Contact Form**    | Submit messages, mark as read, unread counter, pagination

## Project Structure (Clean / Onion Architecture)

## Project Structure (Clean / Onion Architecture)

```text
src/
├── MyApp.Domain/          (Entities, Value Objects, Interfaces, Domain Logic - pure C#)
├── MyApp.Application/     (DTOs, Service Interfaces, Use Cases - thin layer)
├── MyApp.Infrastructure/  (MongoDB Generic + Specific Repositories, services)
├── MyApp.API/             (Controllers, Middleware, Swagger, Program.cs)
└── MyApp.WebMVC/          (MVC site + Admin panel - ViewModel-based)
```




## Technology Stack

| Layer                  | Technology                                 |
|------------------------|--------------------------------------------|
| Framework              | .NET 8 (LTS)                               |
| Web UI                 | ASP.NET Core MVC + Razor Views + Bootstrap |
| API                    | ASP.NET Core Web API + Swagger             |
| Architecture           | Clean / Onion Architecture                 |
| Database               | MongoDB                                    |
| Repository Pattern     | Generic + Specific Repositories            |
| Authentication         | JWT + Refresh Tokens (ready)               |
