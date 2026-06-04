# EventEase – Venue Booking System

**Student:** Mpumelelo Chonco (st10449316)  
**Module:** CLVD6211 – Cloud Development  
**Parts:** 1, 2 & 3 (Complete POE)

---

##  Project Overview

EventEase is an ASP.NET Core MVC web application for managing venue bookings. It allows booking specialists to:

- Manage venues, events, and bookings (CRUD)
- Upload venue/event images to Azure Blob Storage
- Prevent double bookings (overlapping date/time)
- Search and filter bookings by ID, event name, customer, event type, date range, and availability
- Run locally (LocalDB + Azurite) and deploy to live Azure Cloud (Azure SQL, Blob Storage, App Service)

---

##  Features (All Parts)

| Part | Features |

| **Part 1** | • CRUD for Venues, Events, Bookings<br>• SQL LocalDB persistence<br>• Double‑booking prevention (unique constraint)<br>• Basic Bootstrap UI<br>• GitHub & localhost screenshots |
| **Part 2** | • Azure Blob Storage (Azurite emulator & live)<br>• Image upload for venues & events<br>• Consolidated booking view (JOIN Venue + Event)<br>• Search by Booking ID / Event Name / Customer<br>• Success/error `TempData` alerts |
| **Part 3** | • `EventType` lookup table (5 categories)<br>• Advanced filters: event type, date range, future‑only<br>• Migration to Azure SQL Database<br>• Deployment to Azure App Service (live)<br>• Reflective technical report |

---

## Technologies

- **Backend:** ASP.NET Core MVC (.NET 8)
- **Database:** SQL LocalDB (dev) → Azure SQL (prod)
- **ORM:** Entity Framework Core (Code First + Migrations)
- **Storage:** Azure Blob Storage (Azurite local, live cloud)
- **Hosting:** Azure App Service (Free F1 tier)
- **Frontend:** Bootstrap 5, Bootstrap Icons, jQuery
- **Version Control:** Git + GitHub

---

## Prerequisites (Local Development)

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with ASP.NET & Azure development workloads)
- [.NET 8 SDK](https://dotnet.microsoft.com/)
- [SQL Server Express LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (comes with Visual Studio)
- [Azure Storage Emulator (Azurite)](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) (for local blob testing)
- [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/) (optional – view local emulator containers)

---

```bash
git clone https://github.com/Modies20/EventsEase.git
cd EventsEase
