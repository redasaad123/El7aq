# 🚍 El7aq  
A Smart Ride-Sharing & Transportation Management Platform  

**.NET 8 · ASP.NET Core MVC · Entity Framework Core · PayPal API · SQL Server · Bootstrap**

---

## 🎯 Project Overview
**El7aq** is a multi-layered ride-sharing and trip management system built with ASP.NET Core.  
It connects **drivers** and **passengers**, manages **trips & bookings**, and supports **payments, notifications, and administration**.  

The solution is structured into **clean layers** (`Core`, `Infrastructure`, `Web`) and demonstrates real-world integration with **PayPal API** and **SMTP email services**.

---

## ✨ Key Features
### 👤 Passengers
- Search trips by origin & destination.  
- Book seats instantly with PayPal payments.  
- Manage bookings (view, edit, cancel).  
- Receive email & in-app notifications.  
- Update personal profile & preferences.  

### 🚗 Drivers
- Create and manage trips.  
- Define routes between stations.  
- Manage passenger bookings.  
- Maintain driver & vehicle profile.  

### 🧑‍💼 Staff
- Manage transportation hubs (stations).  
- Configure routes.  

### ⚙️ Admin
- Manage all system users (drivers, passengers, staff).  
- Oversee authentication & roles.  

---

## 🏗️ Solution Architecture
The project follows a **Clean Architecture** approach:
```
El7aq.sln
│
├── Core/
│   ├── Models/             # Entity models
│   ├── Enums/
│   └── Interfaces/
├── Infrastructure/          # Data access, EF Core, Repositories
│   ├── repositories/            # Generic Repository
│   ├── Services/                # Business Layer
│   ├── Migrations/              # Database migrations
│   └── Unit Of Work/       
└── Web/                         # ASP.NET Core MVC (UI, APIs, Controllers)
    ├── Controllers/                 # MVC Controllers
    ├── View Models/                 # Models for views
    └── Views/                       # Razor Views
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

## 🚀 Future Enhancements
- **GPS Integration** → Real-time trip tracking.  
- **Advanced Filters** → Price, vehicle type, amenities.  
- **Maps API Integration** → Route optimization.  
- **Mobile Apps** → Native Android/iOS.  

---

## 🛠️ Getting Started
### Prerequisites
- .NET 8 SDK  
- SQL Server 
- Visual Studio 2022 / VS Code  
- PayPal developer account (sandbox for testing)  

### Installation
Clone the repository:

```bash
git clone https://github.com/redasaad123/El7aq.git
cd El7aq
```

## 👥 Team Members

- **Ahmed Qassem** – Backend Developer  
  - Implemented core booking services and business logic  
  - Worked on integrating the Unit of Work pattern  

- **Reda Saad** – Backend Developer
  - Worked on authentication and role management  
  - Developed trip management services (TripService, BookingService)  
  - Integrated PayPal payment gateway  

- **Ahmed Abdelfattah** – Backend Developer  
  - Built notification
  - Profile Services
  - Built Razor Views with Bootstrap  

- **Basmala Mohamed** – Database Designer  
  - Developed trip management services (TripService, BookingService)  
  - Integrated PayPal payment gateway  

- **Khaled Eldawly** – Backend Developer  
  - Implemented driver-side features (trip creation, route management)  
  - Developed APIs for passenger-driver interactions  

- **Hassan Atwa** – Database Designer
  - Managed database migrations with EF Core
  - Supported data modeling and relationships (users, trips, bookings)

- **Aya Majar** – Frontend Developer  
  - Built Razor Views with Bootstrap  
  - Implemented user-friendly booking & trip search UI  

- **Esraa Saad** – Database Designer  
  - Created seed data and handled initial database population  
  - Supported schema evolution during development  

## 📚 Additional Documentation
- [API Documentation](docs/api.md)  
- [ERD Diagram](docs/erd.md)  
- [System Design](docs/system-design.md)  

## 🙏 Acknowledgments
- **Microsoft** → .NET & ASP.NET Core framework  
- **PayPal** → Secure payment gateway  
- **SQL Server** → Reliable database engine  
- **SMTP (Gmail)** → Email notifications  


