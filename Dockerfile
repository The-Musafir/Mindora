# Mindora - Production Dockerfile (multi-stage, .NET 10, Alpine)

# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY src/Mindora.Domain/Mindora.Domain.csproj               src/Mindora.Domain/
COPY src/Mindora.Application/Mindora.Application.csproj     src/Mindora.Application/
COPY src/Mindora.AI/Mindora.AI.csproj                       src/Mindora.AI/
COPY src/Mindora.Infrastructure/Mindora.Infrastructure.csproj src/Mindora.Infrastructure/
COPY src/Mindora.Web/Mindora.Web.csproj                     src/Mindora.Web/

RUN dotnet restore src/Mindora.Web/Mindora.Web.csproj

COPY . .

WORKDIR /src/src/Mindora.Web
RUN dotnet publish Mindora.Web.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

RUN addgroup -S mindora && adduser -S mindora -G mindora

COPY --from=build /app/publish .

RUN chown -R mindora:mindora /app
USER mindora

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_NOLOGO=true

EXPOSE 8080

ENTRYPOINT ["dotnet", "Mindora.Web.dll"]
