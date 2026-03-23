UAM Portal UI Theme Update (Non-Breaking)
Objective

Update the User Access Management (UAM) Portal UI Theme to match the provided design reference.

The update must ONLY modify UI, styling, layout, and theming.

The following must NOT be changed:

Existing architecture
Backend APIs
Controllers
Service layer
Routing
Authentication
Authorization
Data models
Existing business logic

All existing functionality must work exactly the same after the UI update.

UI Design Reference

The theme should follow the design provided in the reference screenshot.

Main characteristics:

Left vertical navigation panel
Collapsible sidebar
Soft blue gradient theme
Minimalist dashboard layout
Rounded UI components
User avatar initials
Light neutral background
Layout Structure
-----------------------------------------------------
| Sidebar | Top Header                               |
|        |--------------------------------------------|
|        | Dashboard / Page Content                   |
|        |                                            |
|        |                                            |
-----------------------------------------------------
Sidebar Navigation
Position

Left side fixed vertical navigation.

Behaviour
Collapsible
Expand / Collapse toggle
Should persist state during navigation
Sidebar Sections
1. Logo Section

Top area contains:

Talent Solutions
ManpowerGroup

with logo icon.

2. User Profile Section

Display logged-in user:

[ RN ]  Super Admin  ▼

Dropdown contains:

Profile
Logout
3. Navigation Menu

Main navigation groups:

Home
Home
Administration

Collapsible section.

Contains:

Administration
   • Manage Users
   • Manage Permissions
   • System Messages
Client Management
Client Management
Productivity Enablers
Productivity Enablers
Sidebar Theme Toggle

At bottom of sidebar:

Dark Mode / Light Mode Toggle
Dashboard (Home Page)

The Home page should act as a dashboard.

Suggested widgets:

Total Users
Active Users
Inactive Users
Recent Activity
Pending Approvals

Cards should have:

Rounded corners
Soft shadows
Minimal icon + number layout

Example:

+---------------------+
| Total Users         |
|  128                |
+---------------------+
User Management Page
Page Title
Manage Users

Tabs:

Active Users | Inactive Users
User Grid

Table columns:

[ ]  User        Email                Role         User Group

Example row:

[ ]  AS  A, Shalini      shalini.a@company.com   Internal User   Internal
Avatar Initials

User name should show initials in a circular badge.

Example:

AS
AM
RN
Role Tags

Use pill style tags:

Internal User
Admin User
External User
User Management Controls

Above the grid:

Role Type

Toggle buttons:

Core | Functional
Filter by Role

Dropdown:

Select Role ▼
Manage Role Button

Right side action button:

Manage Role
Add User Page

Should contain:

Form fields:

First Name
Last Name
Email
Role
User Group
Status

Buttons:

Cancel | Save

Layout:

2 column responsive form
Styling Guidelines
Color Palette

Primary sidebar color:

#5FA8D3

Light background:

#F5F7FA

Primary button:

#2F80ED

Text:

#2C3E50

Border:

#E0E6ED
UI Component Standards
Buttons

Rounded

border-radius: 8px
Cards
border-radius: 12px
box-shadow: soft
Tables
Sticky header
Row hover highlight
Checkbox selection
Responsiveness

Must support:

Desktop
Laptop
Tablet

Sidebar collapses automatically on small screens.

Technical Constraints

The UI update must not modify:

API endpoints
DTOs
Services
Controllers
Database logic
Authentication configuration
Authorization policies
Middleware

Only allowed changes:

CSS
SCSS
Layout components
HTML structure
UI components
Theme configuration
Expected Outcome

After applying this theme:

Portal UI matches provided design style
Left navigation collapsible
Dashboard added
User management UI redesigned
No backend functionality affected