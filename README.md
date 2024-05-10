# Garage Version 3
This project is about a web application in ASP.NET MVC to manage parked vehicles in a garage. The aim is to develop a user-friendly interface allowing users to park, retrieve, and edit vehicles, as well as display overviews and details of parked vehicles.


   
## Database

- A relational database management system spporting SQL Server Management Studio (SSMS) [More Info](https://learn.microsoft.com/en-us/sql/ssms/sql-server-management-studio-ssms?view=sql-server-ver16)
- We used the tool [MSSQL](https://www.microsoft.com/en-us/sql-server/sql-server-downloads), which allowed us to view relationships, tables, and values within the tables.

In the application to configure to the database, you need to use: 
```
update-database

```

## Controller

| HTTP Verbs | Endpoints | Action |
| --------- | --------- | --------- |
| **Home-Controller** | | |
| GET | / | To the home index page |
| GET | /Home/Statistics | To the statistics page|
| **Users-Controller** | | |
| GET | /Users | To the users index page |
| GET | /Users/Creates | To the user create page |
| GET | /Users/Details/:id | To the user details page |
| GET | /Users/Delete/:id | To the user delete page |
| GET | /Users/Edit/:id | To the user edit page |
| **Vehicles-Controller** | | |
| GET | /Vehicles | To the vehicles index page |
| GET | /Vehicles/Creates | To the vehicles create page |
| GET | /Vehicles/Details/:id | To the vehicles details page |
| GET | /Vehicles/Delete/:id | To the vehicles delete page |
| GET | /Vehicles/Edit/:id | To the vehicles edit page |
| **ParkingLot-Controller** | | |
| GET | /ParkingLot | To the parking lot index page |
| GET | /ParkingLot/Creates | To the parking lot check-in page |
| GET | /ParkingLot/Details/:id | To the parking lot details page |
| GET | /ParkingLot/Delete/:id | To the parking lot check out page |
| **Receipts-Controller** | | |
| GET | /Receipts | To the receipts index page |
| GET | /ParkingLot/Details/:id | To the receipts details page |
| **VehicleType-Controller** | | |
| GET | /VehicleType/Create | To the vehicle type create page |

## Authors
[Emre Demirel](https://github.com/98emre)

[Annica Alexandra Andersson](https://github.com/Annisson)

[Sally Kristina Resch](https://github.com/SallyResch)

[Victor Terje Härström](https://github.com/ViceSyndicate)




