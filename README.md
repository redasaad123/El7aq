# 🚍 El7aq - Smart Ride-Sharing Platform

[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-8.0-green.svg)](https://docs.microsoft.com/en-us/ef/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## 🎯 Project Overview

**El7aq** is a multi-layered ride-sharing platform that facilitates seamless transportation services across Egypt. The system supports multiple user roles with comprehensive trip management capabilities, secure payment processing, and real-time notifications.

### Key Highlights
- **Clean Architecture** with separation of concerns across Core, Infrastructure, and Web layers
- **Role-based Authentication** using ASP.NET Core Identity with granular permissions
- **Secure Payment Processing** via PayPal API integration with automatic refunds
- **Real-time Notifications** through email and in-app alert systems
- **Docker Support** for easy deployment and scalability
- **Responsive UI** built with Bootstrap 5 for optimal user experience

### Technology Stack
- **Backend**: .NET 8, ASP.NET Core MVC, Entity Framework Core
- **Database**: SQL Server with Entity Framework migrations
- **Frontend**: Razor Views, Bootstrap 5, JavaScript
- **Authentication**: ASP.NET Core Identity
- **Payments**: PayPal API Integration
- **Deployment**: Docker, Nginx, SSL certificates

---

## ✨ Features

### 👤 Passenger Features
- **Trip Search & Booking**: Advanced search by origin, destination, date, and passenger count
- **Secure Payments**: PayPal-integrated payment processing with automatic refunds
- **Booking Management**: Complete booking lifecycle with view, edit, and cancel capabilities
- **Real-time Notifications**: Email and in-app notifications for booking updates
- **Profile Management**: Comprehensive user profiles with preferences and history
- **Trip History**: Complete booking history with status tracking

### 🚗 Driver Features
- **Trip Creation**: Create and manage transportation routes with scheduling
- **Route Management**: Define routes between stations with distance calculation
- **Booking Management**: Monitor and manage passenger bookings with real-time updates
- **Driver Profile**: Maintain vehicle information and driver credentials
- **Earnings Tracking**: Monitor trip earnings and payment status
- **Vehicle Management**: Manage vehicle details and amenities

### 🧑‍💼 Staff Features
- **Station Management**: Manage transportation hubs and pickup points
- **Route Configuration**: Set up and maintain route networks
- **System Monitoring**: Oversee platform operations and user activity
- **Support Tools**: Handle customer inquiries and system issues

### ⚙️ Administrator Features
- **User Management**: Comprehensive user administration across all roles
- **Role Management**: Configure user roles and permissions
- **System Configuration**: Platform-wide settings and configurations
- **Analytics & Reports**: System usage and performance metrics
- **Content Management**: Manage system content and configurations

---
## 🏗️ Solution Architecture
The project follows a **Clean Architecture** approach:
```
El7aq.sln
│
├── Core/                   # Contain Data models, Enums for data models, System interfaces
│   ├── Models/                  # Entity models
│   ├── Enums/                   # Enumns for models
│   └── Interfaces/              # System interfaces
├── Infrastructure/          # Data access, EF Core, Repositories
│   ├── repositories/            # Generic Repository
│   ├── Services/                # Business Layer
│   ├── Migrations/              # Database migrations
│   └── Unit Of Work/       
└── Web/                      # ASP.NET Core MVC (UI, APIs, Controllers)
    ├── Controllers/             # MVC Controllers
    ├── View Models/             # Models for views
    └── Views/                   # Razor Views
```

### Design Patterns Used
- **Repository Pattern** → Data access abstraction.  
- **Unit of Work** → Transaction consistency.  
- **Service Layer** → Encapsulated business logic.  
- **Dependency Injection** → Loose coupling & testability.  

---

## 🗄️ Database Design
- **Users** → Role-based authentication (Passenger, Driver, Staff, Admin).  
- **Trips** → Scheduling, route, driver assignment.  
- **Bookings** → Reservation system with status tracking.  
- **Payments** → PayPal-secured transactions.  
- **Notifications** → Email + in-app.  

---

## 🚦 Core Workflows
### Passenger Booking Flow
1. Passenger searches for trips.  
2. System displays availability & driver details.  
3. Passenger books with PayPal.  
4. Email + in-app notifications sent.  

### Driver Management Flow
1. Driver creates trip with route & schedule.  
2. System validates details.  
3. Driver monitors & manages bookings.  
4. Trips can be canceled if needed.  

### Payment Flow
- Secure PayPal integration.  
- Automatic refunds for cancellations.  

---

