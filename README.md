# Olympus Identity POC

## Overview
This Proof of Concept demonstrates:
- Single Sign-On (SSO) with Microsoft Entra ID (Azure AD) and Azure AD B2C
- Internal and external user management
- Invitation-based onboarding
- Two portals: UAM (internal) and CAP (external)

---

## Solution Structure
- `UamPortal` – Internal admin portal (SSO via Azure AD/Entra ID)
- `CapPortal` – External user portal (SSO via Azure AD B2C)
- `SharedInfrastructure` – Shared models, repositories, services (Dapper, SendGrid)

---

## Prerequisites
- .NET 8 SDK
- Visual Studio Code
- Local SQL Server (Express or LocalDB)
- Azure Subscription
- SendGrid account

---

## 1. Azure Resource Registration & Configuration

### A. Register Azure AD App for UAM Portal
1. Go to [Azure Portal](https://portal.azure.com/)
2. Azure Active Directory > App registrations > New registration
3. Name: `TalentSolution-UAM-Portal`
4. Supported account types: Single tenant (or as needed)
5. Redirect URI: `https://localhost:5001/signin-oidc`
6. Register
7. Note the `Application (client) ID` and `Directory (tenant) ID`
8. Certificates & secrets > New client secret (save value)
9. API permissions > Add Microsoft Graph > User.Read
10. Authentication > ID tokens (checked)

### B. Register Azure AD B2C Tenant & App for CAP Portal
1. [Create an Azure AD B2C tenant](https://learn.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant) if not already done
2. Switch to B2C tenant in Azure Portal
3. Azure AD B2C > App registrations > New registration
4. Name: `Olympus-CAP-Portal`
5. Redirect URI: `https://localhost:5002/signin-oidc`
6. Register
7. Note the `Application (client) ID` and `Directory (tenant) ID`
8. Certificates & secrets > New client secret (save value)
9. User flows > New user flow > Sign up and sign in (e.g., `B2C_1_SignUpSignIn`)
10. API permissions > Add Microsoft Graph > User.Read
11. Authentication > ID tokens (checked)

### C. Configure SendGrid
1. [Create a SendGrid account](https://sendgrid.com/)
2. Create an API key
3. Note the API key and sender email

---

## 2. Local Configuration

### A. Update `appsettings.json` in UamPortal
```
"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "Domain": "<your-tenant-domain>",
  "TenantId": "<your-tenant-id>",
  "ClientId": "<your-client-id>",
  "CallbackPath": "/signin-oidc"
},
"SendGridSettings": {
  "ApiKey": "<your-sendgrid-api-key>",
  "FromEmail": "<your-from-email>"
}
```

### B. Update `appsettings.json` in CapPortal
```
"AzureAdB2C": {
  "Instance": "https://<your-b2c-tenant-name>.b2clogin.com/",
  "Domain": "<your-b2c-tenant-domain>",
  "TenantId": "<your-b2c-tenant-id>",
  "ClientId": "<your-b2c-client-id>",
  "CallbackPath": "/signin-oidc",
  "SignUpSignInPolicyId": "B2C_1_SignUpSignIn"
},
"SendGridSettings": {
  "ApiKey": "<your-sendgrid-api-key>",
  "FromEmail": "<your-from-email>"
}
```

### C. Database Connection
- Ensure SQL Server is running locally
- The default connection string uses LocalDB:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OlympusIdentityPOC;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
- Update if using a different SQL Server instance

---

## 3. Running the Applications

1. Open the solution in VS Code
2. Build the solution: `dotnet build Olympus.Identity.POC.sln`
3. Run UamPortal:
   - `cd UamPortal`
   - `dotnet run`
   - Access: https://localhost:5001
4. Run CapPortal:
   - `cd CapPortal`
   - `dotnet run`
   - Access: https://localhost:5002

---

## 4. SSO Authentication Flow

### UAM Portal (Internal)
- Uses Microsoft Entra ID (Azure AD)
- SSO via OpenID Connect
- Admins log in with corporate credentials

### CAP Portal (External)
- Uses Azure AD B2C
- SSO via OpenID Connect
- External users register and log in with personal Microsoft accounts

---

## 5. Invitation & Registration Flow

1. Admin logs into UAM Portal
2. Admin invites user (email, display name, role)
3. System generates invitation token, saves to DB, sends email via SendGrid
4. User receives email, clicks registration link (CAP Portal)
5. CAP Portal validates token, allows registration
6. User registers, logs in via Azure AD B2C

---

## 6. Additional Notes
- Both portals share the same database and infrastructure
- All authentication is SSO via Microsoft Identity
- All sensitive keys/secrets should be stored securely (not in source control)
- For production, configure HTTPS and secure settings

---

## References
- [Microsoft.Identity.Web Docs](https://learn.microsoft.com/azure/active-directory/develop/web-app-quickstart?tabs=netcore-cli)
- [Azure AD B2C Docs](https://learn.microsoft.com/azure/active-directory-b2c/overview)
- [SendGrid Docs](https://docs.sendgrid.com/)
