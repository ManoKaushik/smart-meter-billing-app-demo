# Smart Meter Management System

A backend application for managing smart meter users, meter readings, billing, notifications, and simulated meter data.

## Features

- Consumer management
- Smart meter reading processing
- Bill generation workflow
- Email notification support
- Meter simulator integration
- RabbitMQ-based message communication
- File/photo upload support

## Tech Stack

- ASP.NET Core
- C#
- Entity Framework Core
- PostgreSQL / SQL Database
- RabbitMQ
- SMTP Email Service

## Project Structure

```text
SmartMeter/
├── SmartMeterWeb/
├── Meter_Simulator/
└── README.md
```

## Configuration

Create appsettings.json from appsettings.example.json

## How To Run

- dotnet restore
- dotnet build
- dotnet run

To run the simulator
- python simulator.py

## Notes

This is a cleaned portfolio version. Personal notes, test credentials, local logs, photos, and sensitive configuration files are excluded.
