# Azure WCF Service

This repository contains a WCF service implemented with Azure Table and Blob Storage. The service includes the following methods:

- `Create(login, password)`: Creates a new user.
- `Login(login, password)`: Verifies if the user exists and checks the password. If valid, it creates a new session identifier and returns it.
- `Logout(login)`: Deletes the session for the specified user.
- `Put(name, content, session_id)`: Creates a file (BLOB) with the specified name and content, if the session ID is valid.
- `Get(name, session_id)`: Retrieves the content of the file with the specified name, if the session ID is valid.

## Project Structure

- `Uzytkownik.cs`: Defines the `Uzytkownik` class used for Azure Table Storage entities.
- `IService1.cs`: Defines the service contract for the WCF service.
- `Service1.svc.cs`: Implements the WCF service methods.

## Usage

### Create a User

```csharp
bool result = service.Create("username", "password");
```

### Login

```csharp
Guid sessionId = service.Login("username", "password");
```

### Logout

```csharp
bool result = service.Logout("username");
```

### Put content

```csharp
bool result = service.Put("filename.txt", "file content", sessionId);
```

### Get content

```csharp
string content = service.Get("filename.txt", sessionId);
```
