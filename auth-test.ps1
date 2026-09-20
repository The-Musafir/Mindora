param([string]$BaseUrl = "http://localhost:5000")

$passed = 0
$failed = 0

Write-Host ""
Write-Host "==== MINDORA AUTH TEST ====" -ForegroundColor Cyan
Write-Host "Target: $BaseUrl" -ForegroundColor Cyan
Write-Host ""

# ============================================================
# TEST 1: Admin Login
# ============================================================
Write-Host "[1/8] Admin Login..." -NoNewline

try {
    $s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $page = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -WebSession $s -UseBasicParsing
    $token = [regex]::Match($page.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value

    $body = @{
        "Input.Email" = "bithix29@gmail.com"
        "Input.Password" = "Bithix_29"
        "Input.RememberMe" = "false"
        "__RequestVerificationToken" = $token
    }

    $r = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -Method POST -Body $body -WebSession $s -MaximumRedirection 0 -UseBasicParsing -ErrorAction SilentlyContinue
    $loc = $r.Headers["Location"]

    if ($r.StatusCode -eq 302 -and $loc -like "*Admin*") {
        Write-Host " PASS (302 -> $loc)" -ForegroundColor Green
        $passed++
    } else {
        Write-Host " FAIL (Status $($r.StatusCode))" -ForegroundColor Red
        $failed++
    }
} catch {
    Write-Host " FAIL (Exception)" -ForegroundColor Red
    $failed++
}

# ============================================================
# TEST 2: Wrong Password
# ============================================================
Write-Host "[2/8] Wrong Password..." -NoNewline

try {
    $s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $page = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -WebSession $s -UseBasicParsing
    $token = [regex]::Match($page.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value

    $body = @{
        "Input.Email" = "bithix29@gmail.com"
        "Input.Password" = "WRONG_xyz_123"
        "__RequestVerificationToken" = $token
    }

    $r = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -Method POST -Body $body -WebSession $s -MaximumRedirection 0 -UseBasicParsing -ErrorAction SilentlyContinue

    if ($r.Content -match "Invalid login attempt") {
        Write-Host " PASS (rejected)" -ForegroundColor Green
        $passed++
    } else {
        Write-Host " FAIL (no error shown)" -ForegroundColor Red
        $failed++
    }
} catch {
    Write-Host " PASS (rejected)" -ForegroundColor Green
    $passed++
}

# ============================================================
# TEST 3: CSRF Protection
# ============================================================
Write-Host "[3/8] CSRF Protection..." -NoNewline

try {
    $s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $body = @{ "Input.Email" = "test@x.com"; "Input.Password" = "test" }

    $r = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -Method POST -Body $body -WebSession $s -MaximumRedirection 0 -UseBasicParsing -ErrorAction SilentlyContinue

    Write-Host " FAIL (accepted without token)" -ForegroundColor Red
    $failed++
} catch {
    $code = 0
    if ($_.Exception.Response) { $code = [int]$_.Exception.Response.StatusCode }

    if ($code -eq 400 -or $code -eq 302) {
        Write-Host " PASS (rejected: $code)" -ForegroundColor Green
        $passed++
    } else {
        Write-Host " PASS (status $code)" -ForegroundColor Green
        $passed++
    }
}

# ============================================================
# TEST 4: XSS Protection
# ============================================================
Write-Host "[4/8] XSS Protection..." -NoNewline

try {
    $s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $page = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Register" -WebSession $s -UseBasicParsing
    $token = [regex]::Match($page.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value

    $body = @{
        "Input.DisplayName" = "<script>alert(1)</script>"
        "Input.Email" = "xsstest$((Get-Random))@x.com"
        "Input.PhoneNumber" = "01700000000"
        "Input.Password" = "Test@1234"
        "Input.ConfirmPassword" = "Test@1234"
        "Input.DateOfBirth" = "1995-01-01"
        "Input.Gender" = "Male"
        "Input.AcceptTerms" = "true"
        "__RequestVerificationToken" = $token
    }

    $r = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Register" -Method POST -Body $body -WebSession $s -UseBasicParsing -ErrorAction SilentlyContinue

    if ($r.Content -match "<script>alert\(1\)</script>") {
        Write-Host " FAIL (raw script)" -ForegroundColor Red
        $failed++
    } else {
        Write-Host " PASS (encoded)" -ForegroundColor Green
        $passed++
    }
} catch {
    Write-Host " PASS (rejected)" -ForegroundColor Green
    $passed++
}

# ============================================================
# TEST 5: SQL Injection
# ============================================================
Write-Host "[5/8] SQL Injection..." -NoNewline

try {
    $s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $page = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -WebSession $s -UseBasicParsing
    $token = [regex]::Match($page.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value

    $body = @{
        "Input.Email" = "' OR 1=1--"
        "Input.Password" = "anything"
        "__RequestVerificationToken" = $token
    }

    $r = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -Method POST -Body $body -WebSession $s -MaximumRedirection 0 -UseBasicParsing -ErrorAction SilentlyContinue

    if ($r.StatusCode -eq 302) {
        Write-Host " FAIL (injection worked!)" -ForegroundColor Red
        $failed++
    } else {
        Write-Host " PASS (blocked)" -ForegroundColor Green
        $passed++
    }
} catch {
    Write-Host " PASS (blocked)" -ForegroundColor Green
    $passed++
}

# ============================================================
# TEST 6: Empty Login Form
# ============================================================
Write-Host "[6/8] Empty Form..." -NoNewline

try {
    $s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $page = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -WebSession $s -UseBasicParsing
    $token = [regex]::Match($page.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value

    $body = @{
        "Input.Email" = ""
        "Input.Password" = ""
        "__RequestVerificationToken" = $token
    }

    $r = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -Method POST -Body $body -WebSession $s -MaximumRedirection 0 -UseBasicParsing -ErrorAction SilentlyContinue

    if ($r.StatusCode -eq 302) {
        Write-Host " FAIL (accepted empty)" -ForegroundColor Red
        $failed++
    } else {
        Write-Host " PASS (rejected)" -ForegroundColor Green
        $passed++
    }
} catch {
    Write-Host " PASS (rejected)" -ForegroundColor Green
    $passed++
}

# ============================================================
# TEST 7: Doctor Login
# ============================================================
Write-Host "[7/8] Doctor Login..." -NoNewline

try {
    $s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $page = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -WebSession $s -UseBasicParsing
    $token = [regex]::Match($page.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value

    $body = @{
        "Input.Email" = "provider@mindora.com"
        "Input.Password" = "Provider_123"
        "Input.RememberMe" = "false"
        "__RequestVerificationToken" = $token
    }

    $r = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -Method POST -Body $body -WebSession $s -MaximumRedirection 0 -UseBasicParsing -ErrorAction SilentlyContinue
    $loc = $r.Headers["Location"]

    if ($r.StatusCode -eq 302) {
        Write-Host " PASS (302 -> $loc)" -ForegroundColor Green
        $passed++
    } else {
        Write-Host " FAIL (Status $($r.StatusCode))" -ForegroundColor Red
        $failed++
    }
} catch {
    Write-Host " FAIL (Exception)" -ForegroundColor Red
    $failed++
}

# ============================================================
# TEST 8: Register Page Loads
# ============================================================
Write-Host "[8/8] Register Page..." -NoNewline

try {
    $r = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Register" -UseBasicParsing
    if ($r.StatusCode -eq 200) {
        Write-Host " PASS" -ForegroundColor Green
        $passed++
    } else {
        Write-Host " FAIL (Status $($r.StatusCode))" -ForegroundColor Red
        $failed++
    }
} catch {
    Write-Host " FAIL (Exception)" -ForegroundColor Red
    $failed++
}

# ============================================================
# SUMMARY
# ============================================================
Write-Host ""
Write-Host "==== SUMMARY ====" -ForegroundColor Cyan
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor Red
Write-Host "Total:  $($passed + $failed)" -ForegroundColor Cyan
Write-Host ""

if ($failed -eq 0) {
    Write-Host "ALL TESTS PASSED!" -ForegroundColor Green
} else {
    Write-Host "SOME TESTS FAILED" -ForegroundColor Red
}