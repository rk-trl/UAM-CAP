# Olympus Identity POC – Application Context for GitHub Copilot

## Project Objective

Build a **Proof of Concept (POC)** using **.NET 8** to demonstrate:

* **Single Sign-On (SSO)**
* **OAuth2 / OpenID Connect Authentication**
* **Internal and External User Management**
* **Invitation based onboarding**

The system will contain two applications:

1. **UAM Portal (User Access Management Portal)**
2. **CAP Portal (Customer Access Portal)**

Both applications will authenticate using **Microsoft Entra ID**.

Internal users will login using **Enterprise Entra ID** and external users will login using **External Entra ID authentication**.

The goal is to demonstrate authentication and authorization flows, not full UI development.

---

# Authentication Requirements

Authentication must use:

Microsoft OAuth 2.0
OpenID Connect
SSO

Library:

Microsoft.Identity.Web

---

## Internal Users

Internal users are administrators accessing the **UAM Portal**.

Authentication:

Microsoft Entra ID SSO

Flow:

1 Admin opens UAM Portal
2 Redirect to Microsoft login page
3 Admin signs in using corporate account
4 OAuth token returned
5 Application creates authenticated session

---

## External Users

External users access **CAP Portal**.

Authentication:

Microsoft Entra External Identity.

Flow:

1 User receives invitation email
2 User clicks invitation link
3 User registers
4 User authenticates using Microsoft identity
5 OAuth token returned to application

---

# Solution Structure

Create the following solution structure.

Olympus.Identity.POC

```
UamPortal
CapPortal
SharedInfrastructure
```

---

# Technology Stack

Framework

.NET 8

IDE

Visual Studio Code

Authentication

Microsoft.Identity.Web

Database Access

Dapper

Email Service

SendGrid

Database

Local SQL Server instance (SQL Server Express or LocalDB)

---

Authentication Validation Logic

After SSO login, both portals validate the authenticated user against the Users table.
CAP Portal: Only registered users can access the dashboard. Unregistered users are denied access.
UAM Portal: Only users with "Admin" role and "Active" status can access admin features. Others are denied access.
---------

Authentication Enforcement & Admin Features

All main pages require SSO authentication.
Admins see menu options for User Management and Invite User.
User Management page lists all users.
Invite page allows admins to invite external users, generating tokens and sending emails.
Only authenticated admins can access these features.

# Local Database Setup

For the POC use **Local SQL Server instance**.

Connection string example:

Server=(localdb)\MSSQLLocalDB;
Database=OlympusIdentityPOC;
Trusted_Connection=True;
MultipleActiveResultSets=true

Database should be automatically created if it does not exist.

---

# Database Schema

Users Table

Fields

Id uniqueidentifier
Email nvarchar(200)
DisplayName nvarchar(200)
Role nvarchar(50)
Status nvarchar(50)
AzureObjectId nvarchar(200)
CreatedDate datetime

---

Invitations Table

Fields

Id uniqueidentifier
Email nvarchar(200)
Token nvarchar(200)
Status nvarchar(50)
ExpiryDate datetime
CreatedDate datetime

---

# SharedInfrastructure Project

Purpose

Provide shared services for both portals.

Components

Database access

Models

Repositories

Email services

---

Classes

User

Invitation

UserRepository

InvitationRepository

DatabaseConnectionFactory

SendGridEmailService

Database access must use **Dapper**.

---

# UAM Portal

Purpose

Internal **User Access Management portal** used by administrators.

Authentication

SSO via Microsoft Entra ID.

Protocol

OAuth2 + OpenID Connect.

Library

Microsoft.Identity.Web.

---

# UAM Portal Features

Admin login using Microsoft SSO.

Dashboard.

User management page.

Invite new users.

Send email invitation to personal email IDs.

View invited users.

View registered users.

---

# User Management Page

The UAM Portal must include a **User Management page**.

Functions

View users

Add new user

Send invitation

View invitation status

---

## User Management Page UI

Columns

Email

Display Name

Role

Status

Invitation Status

Created Date

Actions

Invite

Resend Invitation

Deactivate

---

# Invite User Flow

1 Admin logs into UAM Portal using SSO.

2 Admin navigates to **User Management page**.

3 Admin enters:

User Email
Display Name
Role

4 System performs:

Generate invitation token using GUID.

Save invitation record in database.

Send invitation email using SendGrid.

---

# Invitation Email

The invitation email should contain a registration link.

Example link

https://localhost:5002/register?token={token}

The email should be sent to the user's **personal email ID** using SendGrid.

---

# CAP Portal

Purpose

Portal for **external users**.

Authentication

Microsoft Entra External Identity using OAuth2.

---

# CAP Portal Features

Accept invitation.

Register account.

Login using SSO.

Access dashboard.

---

# Registration Flow

External user receives invitation email.

User clicks invitation link.

Example

https://localhost:5002/register?token=abc123

Steps

1 System validates invitation token.

2 If token is valid:

Display registration page.

3 User registers.

4 System saves user in Users table.

5 Invitation status updated to Completed.

6 User logs into CAP Portal using SSO.

---

# CAP Portal Pages

Register Page

Login Page

Dashboard Page

---

# Controllers

UAM Portal Controllers

HomeController

DashboardController

UserController

InvitationController

---

CAP Portal Controllers

RegisterController

AccountController

DashboardController

---

# SendGrid Configuration

Email invitations must be sent using SendGrid.

Configuration should be stored in appsettings.json.

SendGridSettings

ApiKey
FromEmail

---

# Authentication Configuration

Both applications must use **Microsoft.Identity.Web**.

Example configuration.

AzureAd

Instance https://login.microsoftonline.com/

TenantId <tenant-id>

ClientId <client-id>

CallbackPath /signin-oidc

---

# Application Ports

UAM Portal

https://localhost:5001

CAP Portal

https://localhost:5002

---

# Security Requirements (POC Level)

Use HTTPS.

Validate invitation tokens.

Expire invitation tokens after 48 hours.

Store minimal user information.

Use OAuth tokens issued by Microsoft.

---

# UI Requirements

Use simple Razor views.

No frontend frameworks required.

Pages required

Login

Dashboard

User management

Invite user form

Registration page

---

# Copilot Development Instructions

When generating code:

Use dependency injection.

Use async methods.

Use repository pattern for database access.

Keep code simple for POC demonstration.

Focus on authentication, authorization, and invitation workflow.

---

# Expected Result

Two working .NET applications.

UAM Portal

SSO login using Microsoft Entra ID

User management page

Invite users

Send invitation emails

---

CAP Portal

Accept invitation

Register user

Login using Microsoft SSO

Access dashboard

---

Both applications share the same local SQL database and infrastructure project.
