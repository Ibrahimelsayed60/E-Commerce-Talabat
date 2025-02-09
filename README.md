# Talabat-Ecommerce System

## Table of Contents
- [Overview](#overview)
- [Architecture](#architecture)
- [Modules](#modules)
- [Features](#features)
- [Technologies](#technologies)
- [Setup and Installation](#setup-and-installation)
- [Running the Application](#running-the-application)

## Overview
This project is a food delivery system inspired by Talabat, designed to connect restaurants with customers for online ordering and delivery services. The system includes user authentication, restaurant management, order processing, and a RESTful API to handle frontend integration.

## Architecture
The system follows the Onion Architecture approach, ensuring modularity, scalability, and separation of concerns. This design makes the application easier to maintain and extend.
 - **Presentation Layer (API Layer)**
    - Built using ASP.NET Core Web API.
    - Handles HTTP requests and response formatting.
    - Implements JWT Authentication for secure access.
    - Uses Swagger for API documentation.
    - Controllers interact with the Application Layer to process requests.
 - **Application Layer (Business Logic Layer)**
    - Contains the core business logic and use cases.
    - Uses Service Interfaces and Implementations to enforce SOLID principles.
    - AutoMapper is used for data transformation between DTOs and models.
 - **Data Access Layer (DAL)**
    - Uses Entity Framework Core (EF Core) to interact with SQL Server.
    - Implements Repository Pattern to manage data operations efficiently.
    - Handles database migrations and query optimizations.
 - **Authentication & Security**
    - JWT (JSON Web Token) Authentication secures API endpoints.
    - Role-Based Access Control (RBAC) ensures restricted access.
    - Uses ASP.NET Core Identity for user authentication and authorization.

## Modules
 - **Authentication & Authorization Module**
    - Handles user registration, login, and JWT authentication.
    - Manages user roles (Customer, Restaurant Owner, Admin, Delivery Partner).
    - Implements ASP.NET Core Identity and JWT token authentication.
 - **Restaurant Management Module**
    - Allows restaurant owners to add, update, and delete restaurants.
    - Manages restaurant details such as name, menu, and location.
 - **Menu & Product Management Module**
    - Handles menu items, pricing, categories, and availability.
    - Allows restaurant owners to manage their menu dynamically.
 - **Ordering & Checkout Module**
    - Enables customers to place orders, update, cancel, or track them.
    - Manages the order processing workflow, including payment and delivery.
 - **Payment Module**
    - Integrates payment gateways like Stripe.
    - Handles order payment processing and transactions.
 - **Delivery & Tracking Module**
    - Assigns orders to delivery personnel.
    - Tracks order status (Preparing, Out for Delivery, Delivered).
 - **Customer Management Module**
    - Allows customers to update profiles, view order history, and save addresses.
    - Handles customer preferences and notifications.
 - **Middleware & API Enhancement Module**
    - Centralized error handling
    - API response caching for performance optimization
    -  Integrated Swagger UI for API documentation

## Features
 - **JWT-Based Authentication & Role Management** – Implements JSON Web Tokens (JWT) with r   role-based access for Admins, Customers, Restaurant Owners, and Delivery Agents.
 - **Restaurant & Menu Management** – Supports restaurant registration, product listings, pricing updates, and availability settings.
 - **Order & Basket Management** – Customers can add items to baskets, place orders, and receive real-time updates. Includes an optimized checkout process.
 - **Secure Payment Processing** – Integrates Stripe to handle transactions securely, with an order validation and status tracking system.
 - **Admin Dashboard & Reporting** – Admins can manage users, orders, and restaurant data with insightful analytics and logs.
 - **Optimized API Development** – Implements RESTful API best practices with Swagger for documentation, API versioning, and global error handling via custom middleware.
 - **Caching & Performance Optimization** – Utilizes Redis/In-Memory Caching to reduce database load and optimize response times.
 - **Scalable & Maintainable Architecture** – Follows Clean Architecture principles:
    - **Domain Layer**: Business logic (Entities, Specifications)
    - **Application Layer**: Use cases and services
    - **Infrastructure Layer**: Data access (EF Core, Repositories, Unit of Work)
    - **Presentation Layer**: Web API with controllers and DTOs

## Technologies:
 - **Backend**: ASP.NET Core Web API (C#), Entity Framework Core
 - **Database**: Microsoft SQL Server, EF Core Code-First Migrations
 - **Authentication & Security**: JWT Authentication, Identity Management, Role-Based Authorization
 - **API Development**: RESTful APIs, Swagger UI, Middleware for logging & exception handling
 - **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Presentation)
 - **Caching & Performance**: Redis, Memory Cache, Entity Framework Core Optimizations
 - **Payment Integration**: Stripe for secure transactions
 - **Version Control**: Git & GitHub for collaboration and source code management

## Setup and Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/Ibrahimelsayed60/E-Commerce-Talabat.git
   cd E-Commerce-Talabat
2. Restore the .NET dependencies:
    ```bash
    dotnet restore
3. Configure the database connection string in appsettings.json:
    ```bash
    "ConnectionStrings": {
        "DefaultConnection": "Server = .; Database = Talabat.Ecommerce.APIs; Trusted_Connection=true;encrypt=false; MultipleActiveResultSets = true",
        "IdentityConnection": "Server = .; Database = Talabat.Ecommerce.Identity.APIs; Trusted_Connection=true;encrypt=false; MultipleActiveResultSets = true",
        "Redis": "localhost"
  },
    

## Running the Application
- To run the application locally:
    ```bash
    dotnet run
