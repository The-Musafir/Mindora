param([string]$BaseUrl = "http://localhost:5000")

$ErrorActionPreference = "Continue"

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  MINDORA SMOKE TEST" -ForegroundColor Cyan
Write-Host "  Target: $BaseUrl" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

$routes = @(
    @{Path="/";                     Name="Landing" },
    @{Path="/Home/About";           Name="About" },
    @{Path="/Home/Privacy";         Name="Privacy" },
    @{Path="/Home/Terms";           Name="Terms" },
    @{Path="/Home/Contact";         Name="Contact" },
    @{Path="/Identity/Account/Login";    Name="Login" },
    @{Path="/Identity/Account/Register"; Name="Register" },
    @{Path="/health";               Name="Health" },
    @{Path="/Habit";                Name="Habits" },
    @{Path="/Journal";              Name="Journal" },
    @{Path="/Assessment";           Name="Assessments" },
    @{Path="/Community";            Name="Community" },
    @{Path="/Notification";         Name="Notifications" },
    @{Path="/Chat";                 Name="AI Chat" },
    @{Path="/Professional";         Name="Professionals" },
    @{Path="/Consultation";         Name="Consultations" },
    @{Path="/Boredom";              Name="Boredom" },
    @{Path="/Payment/MySubscription"; Name="Billing" },
    @{Path="/UserAnalytics";        Name="Analytics" },
    @{Path="/Profile";              Name="Profile" },
    @{Path="/Admin";                Name="Admin" },
    @{Path="/AdminDashboard";       Name="AdminDash" },
    @{Path="/Report";               Name="Reports" }
)

$passed = 0
$failed = 0
$failures = @()

Write-Host ("{0,-30} {1,-8} {2}" -f "Route", "Status", "Result")
Write-Host "----------------------------------------------"

foreach ($route in $routes) {
    $url = "$BaseUrl$($route.Path)"
    try {
        $r = Invoke-WebRequest -Uri $url -UseBasicParsing -MaximumRedirection 5 -TimeoutSec 10 -ErrorAction Stop
        $code = $r.StatusCode
        if ($code -ge 200 -and $code -lt 400) {
            Write-Host ("{0,-30} {1,-8} OK" -f $route.Name, $code) -ForegroundColor Green
            $passed++
        }
    }
    catch {
        $code = 0
        if ($_.Exception.Response) {
            $code = [int]$_.Exception.Response.StatusCode
        }
        
        if ($code -eq 302 -or $code -eq 401 -or $code -eq 403) {
            Write-Host ("{0,-30} {1,-8} AUTH-REDIRECT" -f $route.Name, $code) -ForegroundColor Yellow
            $passed++
        }
        elseif ($code -eq 404) {
            Write-Host ("{0,-30} {1,-8} NOT FOUND" -f $route.Name, $code) -ForegroundColor Red
            $failed++
            $failures += "$($route.Name) - 404"
        }
        elseif ($code -eq 500) {
            Write-Host ("{0,-30} {1,-8} SERVER ERROR" -f $route.Name, $code) -ForegroundColor Red
            $failed++
            $failures += "$($route.Name) - 500"
        }
        else {
            Write-Host ("{0,-30} {1,-8} ERROR" -f $route.Name, $code) -ForegroundColor Red
            $failed++
            $failures += "$($route.Name) - $code"
        }
    }
}

Write-Host "----------------------------------------------"
Write-Host ""
Write-Host "Passed:  $passed" -ForegroundColor Green
Write-Host "Failed:  $failed" -ForegroundColor Red
Write-Host "Total:   $($routes.Count)" -ForegroundColor Cyan
Write-Host ""

if ($failures.Count -gt 0) {
    Write-Host "FAILURES:" -ForegroundColor Red
    foreach ($f in $failures) {
        Write-Host "  X $f" -ForegroundColor Red
    }
}
else {
    Write-Host "ALL TESTS PASSED!" -ForegroundColor Green
}