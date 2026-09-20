# Changelog

All notable changes to Mindora will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- Docker-based deployment for public demo
- Production database migration path (SQL Server → managed instance)
- Comprehensive unit and integration test coverage
- Full API documentation via OpenAPI

## [1.0.0] — 2026-09-21

### Added
- Initial production-ready release of Mindora
- **ASP.NET Core MVC** web application targeting **.NET 10**
- **Clean Architecture** solution layout:
  - `Mindora.Domain` — entities, enums, domain contracts
  - `Mindora.Application` — DTOs, service interfaces, repository abstractions
  - `Mindora.Infrastructure` — EF Core, repositories, unit of work, external services
  - `Mindora.AI` — AI abstraction layer
  - `Mindora.Web` — MVC, Identity, SignalR, Razor views
- **ASP.NET Core Identity** with `User`/`Role` entities and full account lifecycle
- **Entity Framework Core** with **SQL Server** provider and **27 migrations**
- **Repository Pattern** and **Unit of Work**
- **SignalR** real-time layer:
  - `/hubs/notifications` — live notifications
  - `/hubs/chat` — one-on-one chat
  - `/hubs/presence` — online/offline tracking
- **Google Gemini AI** integration (`gemini-3.6-flash`) for AI wellness coaching
- **SSLCommerz** payment gateway integration (sandbox mode)
- Feature modules:
  - Habit recovery and tracking
  - Journal and mood tracking
  - Assessment engine
  - Community (posts, groups, reactions, moderation)
  - Notifications (in-app, realtime, templates, preferences)
  - Professional support and consultation
  - AI wellness coach
  - Payment, subscriptions, coupons, refunds, payouts
  - Admin dashboard, analytics, and reporting
- **Health check** endpoint at `/health`
- Cross-editor code style via `.editorconfig`
- Cross-platform line-ending rules via `.gitattributes`
- Playwright-based route audit tests

### Security
- Secrets managed via **.NET User Secrets** (development) and environment variables (production)
- No credentials, connection strings, or API keys committed to the repository
- All configuration templates contain placeholders only

[Unreleased]: https://github.com/The-Musafir/Mindora/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/The-Musafir/Mindora/releases/tag/v1.0.0