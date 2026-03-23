Dashboard Metrics Update – Internal, External and Pending Invitations
Objective

Update the UAM Portal Dashboard user metrics to reflect the new user classification model and the invitation workflow maintained in the application database.

The current dashboard shows:

Total Users
Active Users
Inactive Users

This model must be replaced with metrics aligned to Internal users, External users, and Pending invitations.

Updated Dashboard Tiles

The dashboard must display the following tiles.

Tile 1
Total Users

Definition:

Total number of users retrieved from Azure Entra ID.

Calculation:

Total Users = Internal Users + External Users
Tile 2
Internal Users

Definition:

Users where:

userType = Member

These represent internal employees or organization members.

Tile 3
External Users

Definition:

Users where:

userType = Guest

These represent external users invited to the tenant.

Tile 4
Pending Invitations

Definition:

Users who have been invited through the UAM Portal invitation process but have not yet completed onboarding.

Important clarification:

Pending invitations cannot be identified from Microsoft Graph API.

The status is maintained by the application database.

Data Sources

The dashboard metrics must come from two different sources.

Source 1 – Microsoft Graph API

Used to determine:

Internal Users
External Users
Total Users

Example API call:

GET https://graph.microsoft.com/v1.0/users?$select=id,displayName,mail,userType

User classification logic:

Member → Internal Users
Guest → External Users
Source 2 – Application Database

Used to determine:

Pending Invitations

The application must query the Invitations table.

Example logic:

SELECT COUNT(*) 
FROM Invitations 
WHERE Status = 'Pending'

This count should populate the Pending Invitations dashboard tile.

Dashboard Layout

The dashboard should now display:

Total Users
Internal Users
External Users
Pending Invitations

Example layout:

----------------------------------
Total Users          Internal Users

External Users       Pending Invitations
----------------------------------
Important Constraints

These updates must not modify existing architecture or authentication logic.

Do not change:

Authentication flow
Authorization policies
Controllers
Services
Database schema
Existing APIs
Graph integration architecture

Allowed changes:

Dashboard UI tiles
Tile labels
Tile count calculation logic
Frontend display
Expected Result

After implementation:

Internal users are calculated from Graph API where userType = Member.
External users are calculated from Graph API where userType = Guest.
Total users equals the sum of internal and external users.
Pending invitations are retrieved from the Invitations database table where Status = Pending.
Dashboard tiles reflect the new user metrics without affecting existing system functionality.