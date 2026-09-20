# Contributing to Mindora

Thanks for your interest in Mindora! This guide explains how to contribute effectively.

## Code of Conduct

By participating, you agree to abide by our [Code of Conduct](CODE_OF_CONDUCT.md).

## Ways to Contribute

- Report bugs
- Suggest features
- Improve documentation
- Add tests
- Submit pull requests

## Getting Started

### Prerequisites

- **.NET 10 SDK** - https://dotnet.microsoft.com/download
- **SQL Server / LocalDB** (Windows) or SQL Server in Docker (cross-platform)
- **Node.js 20+** (only for Playwright tests)
- **Git**

### Local Setup

    git clone https://github.com/The-Musafir/Mindora.git
    cd Mindora
    dotnet restore Mindora.slnx
    dotnet build Mindora.slnx -c Debug
    cd src/Mindora.Web
    dotnet user-secrets init
    dotnet user-secrets set "Gemini:ApiKey" "<your-gemini-key>"
    cd ../..
    dotnet ef database update --project src/Mindora.Infrastructure --startup-project src/Mindora.Web
    dotnet run --project src/Mindora.Web

The app will be available at the URL shown in the console (typically https://localhost:7268).

## Development Workflow

1. Fork the repository
2. Create a branch from main: `git checkout -b feat/your-feature`
3. Make changes following our conventions
4. Build and test locally
5. Commit using Conventional Commits
6. Push and open a Pull Request

## Coding Standards

- Follow Clean Architecture boundaries (Domain -> Application -> Infrastructure -> Web)
- Follow the .editorconfig rules (auto-applied by most editors)
- Never commit secrets, connection strings, or API keys
- Keep DTOs in Mindora.Application/DTOs
- Keep interfaces in Mindora.Application/Interfaces
- Keep implementations in Mindora.Infrastructure/Services

## Commit Messages

We follow Conventional Commits: https://www.conventionalcommits.org/

| Prefix | Use for |
|--------|---------|
| feat: | A new feature |
| fix: | A bug fix |
| docs: | Documentation only |
| style: | Formatting, no code change |
| refactor: | Code change that neither fixes nor adds |
| test: | Adding or updating tests |
| chore: | Build, tooling, dependencies |
| perf: | Performance improvement |

Example:

    feat(habit): add streak recovery after relapse

    - Introduce RecoveryStreak entity with auto-reset logic
    - Wire up SignalR notification on streak break
    - Add unit tests for edge cases

## Pull Request Checklist

- [ ] Branch is up to date with main
- [ ] Code builds (dotnet build Mindora.slnx)
- [ ] No secrets, API keys, or credentials added
- [ ] Public APIs documented
- [ ] Commit messages follow Conventional Commits
- [ ] PR description explains the why, not just the what

## Review Process

1. A maintainer reviews your PR
2. Changes may be requested
3. Once approved, the maintainer merges

## Questions?

Open a discussion or email the maintainer at habiburrahman10224@gmail.com.