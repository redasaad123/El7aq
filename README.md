# 🚍 El7aq – ERD

#  Project Overview
      Our platform facilitates seamless connections between drivers offering rides and passengers seeking transportation. The system manages bookings, payments, notifications, and provides real-time trip management capabilities.


# Key Features
  # 1. For Passengers
   - Trip Search: Search for available trips by origin and destination stations.
   - Real-time Booking: Book available seats with instant confirmation.
   - Secure Payments: Integrated PayPal payment system.
   - Booking Management: View, modify, and cancel bookings.
   - Profile Management: Maintain personal profile and preferences.
   - Notification System: Receive updates about booking status and trip changes.
     
  # 2. For Drivers
   - Trip Creation: Create and manage trips with flexible scheduling.
   - Route Management: Define routes between stations.
   - Booking Oversight: View and manage passenger bookings.
   - Profile Management: Maintain driver credentials and vehicle information.

  # 3. For Staffs
   - Station Management: Manage transportation hubs and stations.
   - Route Administration: Configure available routes between stations.

  # 4. For Admin
   - User Management: Oversee driver and passenger accounts.


## Architecture

 # Backend Structure
  # 1. Core Services
   - BookingService: Handles all booking operations and validations.
   - TripService: Manages trip creation, updates, and searches.
   - PayPalService: Processes secure payments through PayPal integration.
   - EmailService: Sends notifications and confirmations.
   - NotificationService: Manages in-app notifications.

  # 2. Design Patterns Used
   - Repository Pattern: Abstraction layer for data access.
   - Unit of Work Pattern: Manages transactions and data consistency.
   - Service Layer Pattern: Encapsulates business logic.
   - Dependency Injection: Promotes loose coupling and testability.
     

 ## Technology Stack
   # 1. Backend
   - Framework: ASP.NET Core 8+
   - Database: Entity Framework Core with SQL Server
   - Authentication: ASP.NET Core Identity
   - Payment Processing: PayPal API Integration
   - Email Service: SMTP with Gmail integration

   # 2. Frontend
   - Framework: ASP.NET Core MVC
   - Styling: Bootstrap (responsive design)
   - JavaScript: jQuery for dynamic interactions
     
   # 3. Database Design
   - Users: Role-based authentication (Driver, Passenger, Staff, Manager)
   - Trips: Trip scheduling and management
   - Bookings: Reservation system with status tracking
   - Payments: Secure transaction processing
   - Notifications: Real-time user communications



## Key Functionalities
  # 1. Trip Search & Booking Flow
   - Passengers search for trips by selecting origin and destination.
   - System displays available trips with seat availability.
   - Passengers can view trip details including driver information.
   - Secure booking process with PayPal integration.
   - Email confirmations sent to both parties.
   - Real-time notification system keeps users updated.

  # 2. Driver Trip Management
   - Drivers create trips by selecting routes and schedules.
   - System validates trip details and availability.
   - View passenger bookings and contact information.
   - Cancel trips when necessary.

  # 3. Payment Processing
   - Secure PayPal integration for all transactions.
   - Automatic refund processing for cancellations.



##  Future Enhancements 
   1. Real-time GPS Tracking: Live trip tracking for passengers.
   2. Advanced Search Filters: Price range, vehicle type, amenities.
   3. Integration with Maps APIs: Route optimization and navigation.





     
