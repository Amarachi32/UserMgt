# UserManagementSystem

## Overview

The User Management System is a comprehensive application for managing user accounts and profiles. This system includes user registration, authentication, profile management, and administrative capabilities. This README provides an overview of the system, explains how to run it, and includes information about seeded identity data and unit tests.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Seeded Identity Data](#seeded-identity-data)
- [Unit Tests](#xunit-tests)
- [Contributing](#contributing)
- [License](#license)

## Prerequisites

Before running the User Management System, ensure that you have the following prerequisites installed on your development environment:

- .NET Core SDK (version 6 or higher)
- Visual Studio or Visual Studio Code (optional but recommended for development)
- SQL Server (for database storage)

## Getting Started
In this Project I used Asp.netCore Identity for managing users accounts to ensure security and authenticity of the user.

Follow these steps to set up and run the User Management System:

   ```bash
   git clone https://github.com/yourusername/user-management-system.git
   cd user-management-system
   dotnet build
   dotnet restore
   dotnet ef database update
   dotnet run
```

## Asp.NetIdentity

## Seeded Identity Data

The User Management System includes seeded identity data to help you get started. Below is a table showing some of the seeded user accounts and their roles:

| Username          | Email                   | Role     | Password  |
| ----------------- | --------------------    | -------- | --------  |
| admin             | useradmin@gmail.com     | Admin    | @Admin123 |


These seeded accounts can be used for testing and administrative purposes.

# Unit Tests

To ensure the correctness of the User Management System, unit tests have been provided using the Fake It Easy library. These tests cover various endpoints and functionalities of the system, helping to identify and prevent potential issues. You can run these tests to verify that everything is functioning as expected.


