Feature: Manage Users – Internal & External User Handling
Objective

Enhance the Manage Users module in the UAM portal to support Internal Users and External Users using Microsoft Graph API while ensuring the existing application architecture, APIs, and services remain unchanged.

Only UI behavior and integration logic should be introduced without affecting the existing architecture.

User Classification Model

Users will be categorized based on the Azure Entra ID userType property.

Mapping rule:

Entra User Type	UAM Category
Guest	External Users
Member	Internal Users
Graph API Integration
Important Requirement

The system must make ONLY ONE call to Microsoft Graph API to retrieve users.

Example Graph API call:

GET https://graph.microsoft.com/v1.0/users?$select=id,displayName,mail,userType

After retrieving users:

Application logic should separate users into two collections.

Pseudo logic:

foreach user in graphUsers
    if user.userType == "Guest"
        add to ExternalUsers
    else
        add to InternalUsers

This approach prevents multiple API calls and improves performance.

Manage Users UI Structure

Page layout:

Manage Users

[ Internal Users ]  [ External Users ]

Two tabs must exist:

Internal Users Tab

Displays all users where:

userType = Member

These represent organization users from Entra ID.

External Users Tab

Displays all users where:

userType = Guest

These represent external partner or guest users invited to the tenant.

Filters (Current Requirement)

Filters must exist in the UI but remain disabled.

Reason:

Filtering may be implemented later once the system integrates additional metadata or large user datasets.

Example UI:

Filter by Role   [ Disabled ]
Filter by App    [ Disabled ]

No filtering logic should be implemented now.

Add External User

When adding an External User, the user must be associated with an Application Access option.

Instead of CAP values, use predefined application names.

Application dropdown options:

Olympus
Nova
Kingfisher

Example form layout:

Add External User

First Name
Last Name
Email

Application Access
[ Olympus ▼ ]

Role
[ Select Role ]

Status
[ Active ]
Application Access Model

External users may have access to one or more applications.

Initial supported applications:

Olympus
Nova
Kingfisher

The system should be designed so this list can be extended in the future without major UI changes.

Possible future sources:

Configuration file
Database table
Application settings
User Grid

Common columns:

User
Email
Role
Application
Status

External users must display the Application column.

Internal users may not require the application column if they are organization members.

UI Behavior
Tabs
Internal Users
External Users

Each tab displays the corresponding list generated from the single Graph API response.

Future Enhancements (Not in Current Scope)

The following features are planned but must NOT be implemented now.

Graph API Filtering

Possible future optimization:

GET /users?$filter=userType eq 'Guest'

But for now only one call must be made and filtering happens inside the application.

Filter Enablement

Future UI filters:

Application
Role
Status
User Type

Currently disabled.

Application Management

Future enhancement may allow:

Dynamic application registration
Role mapping per application
Permission-based access
Technical Constraints

The following must not be modified:

Application architecture
Existing services
Controller logic
Database schema
Authentication
Authorization
API contracts
Middleware

Allowed changes:

UI layout
UI theme
Graph API integration logic
Frontend classification logic
Dropdown configuration
Expected Result

After implementation:

Only one Graph API call retrieves all users.
Users are categorized internally using userType.
UI displays two tabs:
Internal Users
External Users
Filters remain disabled.
External user creation includes Application Access dropdown.
Application options include:
Olympus
Nova
Kingfisher