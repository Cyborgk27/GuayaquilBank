# Guayaquil Bank - ERP Modular Multi-Tenant

Este proyecto consiste en un sistema ERP robusto basado en una arquitectura de Monolito Modular. Cuenta con un backend desarrollado en **.NET 8 Web API** utilizando **SQLite** como base de datos persistente, y un frontend moderno maquetado en **Angular**. Todo el entorno se encuentra completamente contenedorizado mediante **Docker**.

---

## 🛠️ Requisitos Previos

Antes de levantar el proyecto, asegúrate de tener instalado lo siguiente en tu máquina:

* [Docker Desktop](https://www.docker.com/products/docker-desktop/) (con soporte para contenedores Linux y WSL2 habilitado).
* [Node.js (Versión 20+)](https://nodejs.org/) 
* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) 

---

## 🚀 Arquitectura y Despliegue con Docker

El entorno está configurado para reutilizar la base de datos local ubicada en la raíz (`./Data/GuayaquilBankDb.db`) mapeándola directamente dentro del contenedor mediante un *Bind Mount*, asegurando la persistencia y manipulación inmediata de los datos existentes.

### Paso 1: Inicialización del Frontend (Scaffolding de la API)

El frontend utiliza `openapi-generator-cli` para autogenerar los servicios de comunicación a partir del Swagger de .NET. Para evitar problemas de red durante la construcción de Docker, la generación del cliente TypeScript se realiza localmente desde el host.

Si se modifica endpoints del backend, abre una terminal en la carpeta `GuayaquilBank.Frontend` y ejecuta:

```bash
npm install

npm run generate:api

```

### Paso 2: Construcción y Encendido del Entorno

Para limpiar cualquier configuración corrupta previa, eliminar volúmenes redundantes e iniciar el ecosistema completo (Backend + Frontend + Base de Datos), ejecuta el siguiente comando en la raíz del proyecto (donde se encuentra el archivo `compose.yml`):

```powershell
docker compose down -v && docker compose up --build

```

Una vez que termine el proceso, podrás acceder a las aplicaciones a través de las siguientes URLs:

* **Frontend (Angular + Nginx):** http://localhost (Puerto 80)
* **Backend (Web API .NET 8):** http://localhost:5000
* **Swagger UI (Documentación de la API):** http://localhost:5000/swagger/index.html

---

## 📁 Estructura del Repositorio

```text
GUAYAQUILBANK/
├── .github/                  # Flujos de trabajo de CI/CD
├── Data/                     # Almacenamiento local de la base de datos
│   └── GuayaquilBankDb.db    # Archivo físico de SQLite (Persistente)
├── GuayaquilBank.Backend/    # Solución de backend modular en .NET 8
│   └── GuayaquilBank.WebApi/ # Proyecto de entrada de la API y Dockerfile
├── GuayaquilBank.Frontend/   # Aplicación cliente en Angular, Nginx y Dockerfile
├── compose.yml               # Orquestación de servicios de Docker
└── README.md                 # Documentación del proyecto

```