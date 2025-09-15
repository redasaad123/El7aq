Table Users {
  Id int [pk, increment]
  Name nvarchar(100)
  Email nvarchar(100) [unique]
  Role nvarchar(50)
}

Table Trips {
  Id int [pk, increment]
  DriverId int [ref: > Users.Id]
  RouteId int [ref: > Routes.Id]
  StartTime datetime
  EndTime datetime
  AvailableSeats int
}

Table Bookings {
  Id int [pk, increment]
  TripId int [ref: > Trips.Id]
  PassengerId int [ref: > Users.Id]
  Status nvarchar(50)
}

Table Payments {
  Id int [pk, increment]
  BookingId int [ref: > Bookings.Id]
  Amount decimal
  Status nvarchar(50)
}

Table Notifications {
  Id int [pk, increment]
  UserId int [ref: > Users.Id]
  Message nvarchar(255)
  CreatedAt datetime
}

Table Stations {
  Id int [pk, increment]
  Name nvarchar(100)
}

Table Routes {
  Id int [pk, increment]
  FromStationId int [ref: > Stations.Id]
  ToStationId int [ref: > Stations.Id]
}
