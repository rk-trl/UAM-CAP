# CAP Portal - Microsoft Entra External Identities Integration

## Authentication Architecture
- Uses Microsoft Entra ID (External Identities) for authentication
- OpenID Connect via Microsoft.Identity.Web
- No Azure AD B2C or b2clogin.com endpoints

## External Identity Login Flow
1. User receives invitation email with a link: `/register?token=xxxx`
2. User clicks the link, CAP validates the token
3. CAP stores invitation info in session
4. CAP redirects to Microsoft Entra login
5. User authenticates (Google, Microsoft, or Email OTP)
6. Entra redirects to `/register/callback`
7. CAP reads user claims, validates invitation
8. CAP creates user, marks invitation complete
9. User is logged in and redirected to dashboard

## Invitation-Based Onboarding Flow
- Invitation tokens are single-use, validated for existence, expiry, and status
- Email from Entra claim must match invitation email
- User is provisioned in the database upon successful registration

## Required Azure Portal Configuration
- Register CAP Portal as an app in Microsoft Entra ID
- Enable External Identities (Google, Microsoft, Email OTP)
- Configure redirect URI: `/signin-oidc`
- Set up client secret and permissions

## Project Structure
- `Controllers/RegisterController.cs`: Handles registration and callback
- `Services/InvitationValidationService.cs`: Validates invitation tokens
- `Services/UserProvisioningService.cs`: Provisions users
- `Repositories/InvitationRepository.cs`, `UserRepository.cs`: Data access
- `Models/Invitation.cs`, `User.cs`: Entity models

## Security
- Tokens are single-use, validated for expiry and status
- Email match is enforced
- No B2C policies or b2clogin.com endpoints used
