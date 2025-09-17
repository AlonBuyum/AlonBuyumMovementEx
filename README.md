# Alon Buyum Home Task

A .NET application with authentication, role based access control, and data management functionality.

## Quick Start

### Prerequisites
- Visual Studio
- Docker (for Redis)
- Git

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   ```

2. **Open the project**
   - Open the `.sln` file in Visual Studio

3. **Start Redis**
   ```bash
   docker run -p 6379:6379 --name redis -d redis
   ```

4. **Run the application**
   - Press the `Play` button in Visual Studio

5. **Test the API**
   - Use Swagger UI or Postman to test endpoints

## Authentication

The application uses a simple authentication system:

- **Login with any username except "Admin"** → Assigns `User` role
- **Login with "Admin"** → Assigns `Admin` role (case-sensitive)

### Authentication Flow
1. Use the `POST /api/Auth/Login` endpoint with your desired username
2. Copy the received token
3. Paste it in:
   - Swagger: Click "Authorize" and enter the token
   - Postman: Add to the "Auth" tab as Bearer token

## API Endpoints

### Authentication
```http
POST /api/Auth/Login
Content-Type: application/json

{
  "username": "your-username"
}
```
**Response:** `{"jwt-token"}`

### Data Management

| Method | Endpoint | Access | Description |
|--------|----------|---------|-------------|
| `GET` | `/api/Data/GetDataById/{id}` | User, Admin | Retrieve data by ID |
| `POST` | `/api/Data/AddData` | Admin only | Add new data |
| `PUT` | `/api/Data/UpdateData` | Admin only | Update existing data |

#### Add/Update Data Request Body
```json
{
  // dataModel properties here
}
```

## Storage

- **Database:** SQLite file (`Data.db`) stored in the working directory
- **Cache:** File-based cache stored in the `FileCache` directory

---
