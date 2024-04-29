# SmartVillages

## Web Application for Digital Marketplace

The SmartVillages web application is a major project done entirely independently as part of college studies. It was submitted as a final thesis and treated as such from the very beginning. From designing the application and all its functionalities to designing the database.

The purpose of the application is to allow sellers to add their homemade products to the "Marketplace", where customers can then place orders for them. Once the product is marked as delivered, the customer can rate and comment on it. The application also integrates real-time messaging between customers and sellers.

### Technologies used in development:

- **Blazor - WebAssembly** in ASP.NET Core
- **Entity Framework Core** for creating and migrating the database
- **SignalR** for sending real-time notifications and messages
- **MudBlazor** - Blazor Component Library for design
- **Microsoft SQL Server** for working with the database

[![My Skills](https://skillicons.dev/icons?i=wasm,dotnet,js,bootstrap,sqlite)](https://skillicons.dev)

The application includes a simple _sign-in/registration_ process with email verification. Real-time messaging is integrated with read/unread options.

---

### When a farmer logs in to the application, they can:

- Search and sort the marketplace.
- Add new products to the marketplace.
- Order products from other farmers in the marketplace.
- View their shopping cart.
- Update and delete their products.
- View their orders, orders for their products from other customers, and completed orders.
- Mark their product orders as delivered.
- Provide ratings and comments for completed orders.
- Send messages to customers who ordered their products.
- View their profile.
- Modify user data for their profile.

### When a regular user logs in to the application, they can:

- Search and sort the marketplace.
- Order products from the marketplace.
- View their shopping cart.
- View their ongoing and completed orders.
- Provide ratings and comments for completed orders.
- Send messages to sellers of the products they ordered.
- View their profile.
- Modify user data for their profile.

---

### Project Setup

Create the database and run migrations by using `Package Manager Console` and executing the following command:

```sh
update-database
```

Then, insert the following data into the `UserTypes` table:

| UserTypeId | UserTypeName    |
| ---------- | --------------- |
| 1          | Korisnik        |
| 2          | Poljoprivrednik |

Also, insert a location of your choice into the `Places` table:

| Id  | Name       | PostalCode |
| --- | ---------- | ---------- |
| 1   | Virovitica | 33000      |

- To avoid manually entering data into the tables, a SQL file with prepared data for the database (_Quick Start_) should be created in the future.

Finally, run the application with the `Server` as the only `Startup Project`. This is because the `Server` project has a reference to the `Client` project, and they run on the same port. Then, register as either a user or a farmer following the instructions.
