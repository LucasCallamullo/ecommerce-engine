
# E-Commerce Engine (.NET & React)

High-performance, modular backend API and modern frontend built with C# .NET and React, adhering to Clean Architecture principles, automated pipelines, and containerized deployment.

---

## Tech Stack

### **Backend**
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQLite / PostgreSQL](https://img.shields.io/badge/Database-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Mapster](https://img.shields.io/badge/Mapster-F1502F?style=for-the-badge&logo=git&logoColor=white)

* **Core:** .NET / ASP.NET Core Web API (C#)
* **Architecture:** Clean Architecture + Modular Monolith
* **Persistence (ORM):** Entity Framework Core (Code-First, Fluent API, Migrations)
* **Mapping:** Mapster (High-performance DTO projections)
* **Database:** SQLite (Development) / PostgreSQL (Production ready)
* **Testing & Tooling:** REST Client (`.http`), EF Core Design-Time Factory

### **Frontend**
![React](https://img.shields.io/badge/React-61DAFB?style=for-the-badge&logo=react&logoColor=black)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?style=for-the-badge&logo=typescript&logoColor=white)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)

* **Framework / Library:** React
* **Language:** TypeScript
* **Styling:** Tailwind CSS
* **Build Tooling:** Vite

---

## Roadmap & Planned Infrastructure

- [ ] **Containerization:** Full Dockerization of API & Frontend using Multi-stage `Dockerfile`.
- [ ] **Orchestration:** `docker-compose.yml` for unified local stack environment.
- [ ] **Gateway & Reverse Proxy:** NGINX / YARP configuration for static asset serving and `/api/*` routing.
- [ ] **Distributed Cache:** Redis integration for response caching and session state.

---

## Architecture & Infrastructure (C4 Model - Level 2)

The system follows a **Modular Monolith** pattern with strict boundary separation, orchestrated via Docker containers and exposed through a single entry gateway.

```mermaid
graph TD
    Client[Browser / Admin Client] -->|HTTP / HTTPS| NGINX[NGINX / Reverse Proxy]
    
    subgraph Docker Network
        NGINX -->|Serve SPA Assets| React[React + Tailwind UI]
        NGINX -->|Proxy /api/*| API[ASP.NET Core Web Host]
        
        subgraph Modular Monolith Engine
            API --> Shared[Shared Kernel]
            API --> Products[Products Module]
            API --> Cart[Cart & Orders Module]
            
            Shared -->|EF Core DbContext| DB[(Database / SQLite / Postgres)]
            Shared -->|Cache / Session| Redis[(Redis Cache)]
        end
    end
```



## Architecture & Infrastructure (C4 Model - Level 2)

The application follows a Modular Monolith pattern decoupled into independent domain layers, orchestrated via Docker Compose and routed through a reverse proxy.

```mermaid
erDiagram
    CATEGORY ||--o{ CATEGORY : "has subcategories"
    CATEGORY ||--o{ PRODUCT : "primary category"
    BRAND ||--o{ PRODUCT : "manufactures"
    PRODUCT ||--o{ PRODUCT_VARIANT : "has"
    PRODUCT ||--o{ PRODUCT_IMAGE : "has gallery"
    PRODUCT_VARIANT ||--o{ PRODUCT_IMAGE : "has specific images"

    CATEGORY {
        int Id PK
        string Name
        string Slug
        int ParentCategoryId FK
        boolean IsActive
    }

    BRAND {
        int Id PK
        string Name
        string Slug
        boolean IsActive
    }

    PRODUCT {
        int Id PK
        string Name
        string Description
        int CategoryId FK
        int BrandId FK
        boolean IsActive
        boolean IsDeleted
    }

    PRODUCT_VARIANT {
        int Id PK
        int ProductId FK
        string SKU UK
        decimal PriceArs
        decimal ComparisonPriceArs
        int Stock
        string Size
        string Color
        string HexColor
        boolean IsDeleted
    }

    PRODUCT_IMAGE {
        int Id PK
        int ProductId FK
        int ProductVariantId FK
        string Url
        boolean MainImage
        int DisplayOrder
    }
```