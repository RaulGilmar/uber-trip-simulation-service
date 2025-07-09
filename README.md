# Aplicación de Viajes (Ride App)

Esta es una aplicación de ejemplo en **ASP.NET Core MVC** que simula el flujo de solicitud de viajes entre clientes y choferes, utilizando un repositorio en memoria (sin base de datos).

---

## Requisitos

- .NET 7 SDK o superior  
- Visual Studio, Visual Studio Code o cualquier editor compatible

---

## Instalación y ejecución

1. Clona el repositorio:

   ```bash
   git clone https://github.com/RaulGilmar/uber-trip-simulation-service.git
   cd tu-repo
   ```

2. Restaura los paquetes y ejecuta la aplicación:

   ```bash
   dotnet restore
   dotnet run --project .\uber-trip-simulation-service\

   ```

3. Abre tu navegador en `http://localhost:5163/Trip/Welcome/` o `https://localhost:7024/Trip/Welcome/`.

---

## Uso

- Al iniciar, selecciona un cliente y haz clic en **"Iniciar viaje"**.
- Ingresa el destino y sigue los pasos del flujo:
  - Solicitud de viaje
  - Asignación de chofer  
  - Viaje en curso
  - Finalización del recorrido
  - Calificación
- La aplicación simula el proceso completo, mostrando alertas en cada etapa.


---

# Ride App

This is a sample **ASP.NET Core MVC** application that simulates a ride request flow between customers and drivers, using an in-memory repository (no database).

---

## Requirements

- .NET 7 SDK or higher  
- Visual Studio, Visual Studio Code, or any compatible editor

---

## Installation and Run

1. Clone the repository:

   ```bash
   git clone https://github.com/RaulGilmar/uber-trip-simulation-service.git
   cd your-repo
   ```

2. Restore packages and run the app:

   ```bash
   dotnet restore
   dotnet run --project .\uber-trip-simulation-service\

   ```

3. Open your browser at `http://localhost:5163/Trip/Welcome/` or `https://localhost:7024/Trip/Welcome/`.

---

## Usage

- On start, select a customer and click **"Start trip"**.
- Enter the destination and follow the flow:
  - Trip request
  - Driver assignment
  - Trip in progress
  - Route finalization
  - Rating
- The app simulates the full process, showing alerts at each stage.

---
