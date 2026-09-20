# ==============================================================================
# Script Name : setup_mindora_folders.ps1
# Description : Idempotent PowerShell script to create missing enterprise Clean 
#               Architecture directory structures for the Mindora Solution.
# Architecture: Clean Architecture / Domain-Driven Design
# ==============================================================================

# Ensure script halts on unhandled critical errors
$ErrorActionPreference = "Stop"

# List of required directory paths relative to the Solution Root
$directories = @(
    # --------------------------------------------------------------------------
    # Mindora.Web
    # --------------------------------------------------------------------------
    "Mindora.Web/Views/Home",
    "Mindora.Web/Views/Dashboard",
    "Mindora.Web/Views/HabitRecovery",
    "Mindora.Web/Views/Assessment",
    "Mindora.Web/Views/AICoach",
    "Mindora.Web/Views/Community",
    "Mindora.Web/Views/Journal",
    "Mindora.Web/Views/Professional",
    "Mindora.Web/Views/WellnessTools",
    "Mindora.Web/Views/Settings",
    "Mindora.Web/Views/Admin",
    "Mindora.Web/Views/Shared",
    "Mindora.Web/ViewModels",
    "Mindora.Web/Helpers",
    "Mindora.Web/wwwroot/css",
    "Mindora.Web/wwwroot/js",
    "Mindora.Web/wwwroot/images",
    "Mindora.Web/wwwroot/icons",
    "Mindora.Web/wwwroot/uploads",

    # --------------------------------------------------------------------------
    # Mindora.API
    # --------------------------------------------------------------------------
    "Mindora.API/Controllers/Auth",
    "Mindora.API/Controllers/Habits",
    "Mindora.API/Controllers/Assessments",
    "Mindora.API/Controllers/AI",
    "Mindora.API/Controllers/Community",
    "Mindora.API/Controllers/Journal",
    "Mindora.API/Controllers/Professionals",
    "Mindora.API/Controllers/Appointments",
    "Mindora.API/Controllers/Notifications",
    "Mindora.API/Controllers/Admin",
    "Mindora.API/Middlewares",
    "Mindora.API/Extensions",

    # --------------------------------------------------------------------------
    # Mindora.Application
    # --------------------------------------------------------------------------
    "Mindora.Application/Features/Auth",
    "Mindora.Application/Features/Habits",
    "Mindora.Application/Features/Assessments",
    "Mindora.Application/Features/AI",
    "Mindora.Application/Features/Community",
    "Mindora.Application/Features/Journal",
    "Mindora.Application/Features/Professionals",
    "Mindora.Application/Features/Appointments",
    "Mindora.Application/Features/Notifications",
    "Mindora.Application/Features/Admin",
    "Mindora.Application/Interfaces",
    "Mindora.Application/DTOs",
    "Mindora.Application/Services",
    "Mindora.Application/Mappings",
    "Mindora.Application/Validators",

    # --------------------------------------------------------------------------
    # Mindora.Domain
    # --------------------------------------------------------------------------
    "Mindora.Domain/Entities",
    "Mindora.Domain/Enums",
    "Mindora.Domain/Common",
    "Mindora.Domain/Constants",

    # --------------------------------------------------------------------------
    # Mindora.Infrastructure
    # --------------------------------------------------------------------------
    "Mindora.Infrastructure/Persistence/DbContext",
    "Mindora.Infrastructure/Persistence/Migrations",
    "Mindora.Infrastructure/Repositories",
    "Mindora.Infrastructure/Identity",
    "Mindora.Infrastructure/Services",
    "Mindora.Infrastructure/Configurations",

    # --------------------------------------------------------------------------
    # Mindora.AI
    # --------------------------------------------------------------------------
    "Mindora.AI/Services",
    "Mindora.AI/Models",
    "Mindora.AI/Prompts/HabitRecovery",
    "Mindora.AI/Prompts/Motivation",
    "Mindora.AI/Prompts/MoodAnalysis",
    "Mindora.AI/Prompts/WellnessCoach"
)

# Counters and Lists for Summary Output
$createdCount = 0
$existingCount = 0
$createdFolders = [System.Collections.Generic.List[string]]::new()
$existingFolders = [System.Collections.Generic.List[string]]::new()

Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host "       MINDORA SOLUTION ARCHITECTURE FOLDER CREATION SCRIPT       " -ForegroundColor Cyan
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host "Processing directories relative to: $(Get-Location)" -ForegroundColor Yellow
Write-Host ""

# Process each directory idempotently
foreach ($dir in $directories) {
    if (Test-Path -Path $dir) {
        $existingCount++
        $existingFolders.Add($dir)
        Write-Host "[EXISTS]  $dir" -ForegroundColor DarkGray
    } else {
        try {
            $null = New-Item -ItemType Directory -Path $dir -Force
            $createdCount++
            $createdFolders.Add($dir)
            Write-Host "[CREATED] $dir" -ForegroundColor Green
        } catch {
            Write-Host "[ERROR]   Failed to create $dir - $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

# --------------------------------------------------------------------------
# Execution Summary
# --------------------------------------------------------------------------
Write-Host ""
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host "                        EXECUTION SUMMARY                         " -ForegroundColor Cyan
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host "Created Folders : $createdCount" -ForegroundColor Green
Write-Host "Existing Folders: $existingCount" -ForegroundColor Yellow
Write-Host "Total Count     : $($directories.Count)" -ForegroundColor White
Write-Host "------------------------------------------------------------------" -ForegroundColor Cyan

if ($createdCount -gt 0) {
    Write-Host "`nNewly Created Folders Details:" -ForegroundColor Green
    foreach ($folder in $createdFolders) {
        Write-Host " + $folder" -ForegroundColor Green
    }
}

Write-Host "`nFolder setup completed successfully." -ForegroundColor Cyan