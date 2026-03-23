CAP Portal – Theme & Home Dashboard Implementation
Objective

Implement a CAP Portal UI theme and Home Dashboard that follows the same UI structure, typography, and layout used in the existing UAM portal, but with a green theme instead of blue.

This implementation must ONLY modify UI/theming and routing for the dashboard tiles.

The existing system architecture, authentication flow, and backend functionality must NOT be modified or broken.

Key Design Principle

The CAP Portal must visually align with the UAM Portal design system:

Same font family
Same layout
Same sidebar structure
Same header style
Same component spacing
Same tile/card style

The ONLY visual difference must be:

Color Theme = Green (CAP Portal)
Color Theme = Blue (Internal UAM Portal)
System Constraints (Very Important)

Copilot must NOT change or refactor:

Authentication flow
Controllers
Services
Existing APIs
Routing structure
Application architecture
Database schema
Middleware
Authorization logic

Allowed modifications:

CSS
SCSS
Theme variables
Dashboard UI layout
Dummy navigation pages
Tile click navigation
Sidebar Navigation

CAP Portal sidebar should be minimal.

Only one navigation item
Home

No additional menu items should be created.

Layout Structure

Same structure as UAM portal:

-------------------------------------
| Sidebar | Header                   |
|         |--------------------------|
|         | Dashboard Content        |
|         |                          |
-------------------------------------

Sidebar width, typography, and icon styling should match the existing UAM portal UI framework.

Theme Colors (CAP Portal)

The CAP Portal must use a green-based theme.

Primary color:

#2FBF71

Secondary green shade:

#1FA463

Light background green:

#E8F5EC

Neutral background:

#F5F7FA

Text color:

#2C3E50
Font

Use exact same font as UAM portal.

Do not introduce new fonts.

Typography hierarchy should remain identical.

Header / Welcome Banner

After authentication, the Home page should display a welcome banner.

Example:

Welcome, {UserName}

This banner should appear in the top section of the dashboard.

The banner can optionally include a background image similar to the provided design.

Layout example:

---------------------------------
Home
Welcome, Joe
---------------------------------
Home Dashboard

The dashboard should contain three tiles/cards.

Centered in the page.

Tiles
Talent Insights
Skills Intelligence
Decision Support

Tile layout example:

        Talent Insights        Skills Intelligence

                Decision Support

Each tile should appear as a rounded card.

Card styling:

border-radius: 16px
soft shadow
hover elevation
green accent border
Tile Styling

Example style structure:

Card background: #FFFFFF
Border: Green accent
Hover: Slight scale effect
Shadow: Soft

Text alignment:

center
vertical middle
Tile Navigation

Each tile should navigate to a dummy placeholder page.

Routing behavior:

Talent Insights  -> /talent-insights
Skills Intelligence -> /skills-intelligence
Decision Support -> /decision-support
Dummy Pages

Each page should contain a simple welcome message.

Example page structure:

Talent Insights

Welcome to Talent Insights Module
Skills Intelligence

Welcome to Skills Intelligence Module
Decision Support

Welcome to Decision Support Module

No functional logic should be implemented in these pages.

Footer

The footer may contain branding text similar to the design.

Example:

Talent Solutions platform powered by ManpowerGroup

Footer styling should follow the same layout as UAM portal.

Responsiveness

The dashboard must be responsive.

Expected behavior:

Desktop → 3 tile layout
Tablet → 2 tiles per row
Mobile → 1 tile per row
Sidebar Footer Section

Include:

Account and Settings
Help and Support
Sign Out

These should reuse existing functionality if already implemented.

Do not modify logout logic.

Visual Design Summary

CAP Portal UI must maintain:

Same UI components as UAM
Same layout grid
Same typography
Same spacing
Same component styling

Difference:

Blue theme → UAM Portal
Green theme → CAP Portal
Expected Result

After implementation:

CAP Portal loads after authentication.
Sidebar shows only Home menu.
Home dashboard displays 3 tiles.
Tiles navigate to dummy welcome pages.
Portal theme is green instead of blue.
No existing system functionality is broken.