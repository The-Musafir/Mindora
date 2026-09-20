# Security Policy

## Reporting a Vulnerability

The Mindora project takes security seriously. If you discover a security issue, please **do not** open a public GitHub issue.

Instead, report it privately:

- **Email:** habiburrahman10224@gmail.com
- **Subject:** `[SECURITY] Mindora — <brief description>`

Please include:
- A description of the issue
- Steps to reproduce (if possible)
- Potential impact
- Any suggested mitigation

You can expect an acknowledgement within **72 hours**. A more detailed response will follow within **7 days**.

## Scope

In-scope:
- `src/Mindora.Web` — primary web application
- `src/Mindora.Infrastructure` — data layer, EF Core, repositories
- `src/Mindora.AI` — AI integration
- Authentication, authorization, session handling
- Payment flow (SSLCommerz integration)

Out of scope:
- Sandbox credentials in `.env.example` (placeholders only)
- Third-party dependencies (report directly to their maintainers)
- Vulnerabilities in the demo deployment's free-tier infrastructure

## Supported Versions

| Version | Supported |
|---------|-----------|
| 1.x     | ✅ Yes    |

## Security Practices in This Repository

- Secrets are **never** committed (development uses .NET User Secrets; production uses environment variables)
- All configuration templates contain placeholders only
- Dependency updates are tracked via NuGet
- The project follows the principle of least privilege for all service accounts

## Disclosure

We follow **coordinated disclosure**. Once a fix is available, we will credit the reporter (unless anonymity is preferred) in the release notes.