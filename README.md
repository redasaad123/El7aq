# 🚍 El7aq – ERD

```mermaid
erDiagram
    User {
        int Id
        string FullName
        string Email
        string Phone
        string PasswordHash
        enum Role
    }

    PassengerProfile {
        int Id
        int UserId
    }

    DriverProfile {
        int Id
        int UserId
        string LicenseNumber
        string CarNumber
    }

    StaffProfile {
        int Id
        int UserId
        int StationId
    }

    Station {
        int Id
        string Name
        string City
    }

    Route {
        int Id
        int StartStationId
        int EndStationId
        decimal Price
    }

    Trip {
        int Id
        int RouteId
        int DriverId
        datetime DepartureTime
        int AvailableSeats
    }

    Booking {
        int Id
        int PassengerId
        int TripId
        datetime BookingDate
        enum Status
    }

    Payment {
        int Id
        int BookingId
        decimal Amount
        datetime PaymentDate
        enum Status
        enum Method
        string TransactionReference
    }

    %% Relationships
    User ||--o| PassengerProfile : has
    User ||--o| DriverProfile : has
    User ||--o| StaffProfile : has

    PassengerProfile ||--o{ Booking : makes
    DriverProfile ||--o{ Trip : drives
    StaffProfile }o--|| Station : works_at

    Station ||--o{ Route : has
    Route ||--o{ Trip : defines
    Trip ||--o{ Booking : includes
    Booking ||--o{ Payment : has
