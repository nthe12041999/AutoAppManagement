# Test LoginWithResources API
# PowerShell script để test API LoginWithResources

param(
    [Parameter(Mandatory=$false)]
    [string]$BaseUrl = "https://localhost:7000",
    
    [Parameter(Mandatory=$false)]
    [string]$Email = "admin@example.com",
    
    [Parameter(Mandatory=$false)]
    [string]$Password = "Admin123!"
)

Write-Host "=== Testing LoginWithResources API ===" -ForegroundColor Green
Write-Host "Base URL: $BaseUrl" -ForegroundColor Yellow
Write-Host "Testing with Email: $Email" -ForegroundColor Yellow

# Ignore SSL certificate errors for localhost testing
Add-Type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy

# Test data
$loginRequest = @{
    emailOrPhone = $Email
    password = $Password
} | ConvertTo-Json

Write-Host "`n1. Testing Regular Login API..." -ForegroundColor Cyan

try {
    $regularLoginResponse = Invoke-RestMethod -Uri "$BaseUrl/api/Account/Login" `
        -Method POST `
        -ContentType "application/json" `
        -Body $loginRequest `
        -ErrorAction Stop
    
    Write-Host "✅ Regular Login successful" -ForegroundColor Green
    Write-Host "Response: $($regularLoginResponse | ConvertTo-Json -Depth 3)" -ForegroundColor Gray
}
catch {
    Write-Host "❌ Regular Login failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Response: $($_.Exception.Response)" -ForegroundColor Gray
}

Write-Host "`n2. Testing LoginWithResources API..." -ForegroundColor Cyan

try {
    $loginWithResourcesResponse = Invoke-RestMethod -Uri "$BaseUrl/api/Account/LoginWithResources" `
        -Method POST `
        -ContentType "application/json" `
        -Body $loginRequest `
        -ErrorAction Stop
    
    Write-Host "✅ LoginWithResources successful" -ForegroundColor Green
    
    if ($loginWithResourcesResponse.isSuccess) {
        $data = $loginWithResourcesResponse.data
        
        Write-Host "`n📊 Login Information:" -ForegroundColor Yellow
        Write-Host "Account: $($data.account.name) ($($data.account.email))" -ForegroundColor White
        Write-Host "Login Time: $($data.loginTime)" -ForegroundColor White
        Write-Host "Token Expiry: $($data.tokenExpiry)" -ForegroundColor White
        
        if ($data.licenseInfo) {
            Write-Host "`n📄 License Information:" -ForegroundColor Yellow
            Write-Host "License Name: $($data.licenseInfo.licenseName)" -ForegroundColor White
            Write-Host "License Type: $($data.licenseInfo.licenseType)" -ForegroundColor White
            Write-Host "Status: $($data.licenseInfo.status)" -ForegroundColor White
            Write-Host "Days Remaining: $($data.licenseInfo.daysRemaining)" -ForegroundColor White
            Write-Host "Start Date: $($data.licenseInfo.startDate)" -ForegroundColor White
            Write-Host "End Date: $($data.licenseInfo.endDate)" -ForegroundColor White
        }
        
        if ($data.availableResources -and $data.availableResources.Count -gt 0) {
            Write-Host "`n🛠️ Available Resources:" -ForegroundColor Yellow
            
            foreach ($resource in $data.availableResources) {
                $statusColor = switch ($resource.status) {
                    "available" { "Green" }
                    "limited" { "Yellow" }
                    "exhausted" { "Red" }
                    "disabled" { "Gray" }
                    default { "White" }
                }
                
                Write-Host "  📌 $($resource.featureName) ($($resource.featureCode))" -ForegroundColor White
                Write-Host "     Tool: $($resource.toolName)" -ForegroundColor Gray
                Write-Host "     Status: $($resource.status)" -ForegroundColor $statusColor
                Write-Host "     Enabled: $($resource.isEnabled)" -ForegroundColor Gray
                
                if ($resource.usageLimit) {
                    Write-Host "     Usage: $($resource.usedCount)/$($resource.usageLimit) (Remaining: $($resource.remainingCount))" -ForegroundColor Gray
                } else {
                    Write-Host "     Usage: $($resource.usedCount) (Unlimited)" -ForegroundColor Gray
                }
                
                if ($resource.periodStart -and $resource.periodEnd) {
                    Write-Host "     Period: $($resource.periodStart) to $($resource.periodEnd)" -ForegroundColor Gray
                }
                
                if ($resource.warningMessage) {
                    Write-Host "     ⚠️ Warning: $($resource.warningMessage)" -ForegroundColor Yellow
                }
                Write-Host ""
            }
            
            # Summary statistics
            $totalResources = $data.availableResources.Count
            $enabledResources = ($data.availableResources | Where-Object { $_.isEnabled }).Count
            $availableResources = ($data.availableResources | Where-Object { $_.status -eq "available" }).Count
            $limitedResources = ($data.availableResources | Where-Object { $_.status -eq "limited" }).Count
            $exhaustedResources = ($data.availableResources | Where-Object { $_.status -eq "exhausted" }).Count
            
            Write-Host "📈 Summary:" -ForegroundColor Cyan
            Write-Host "  Total Resources: $totalResources" -ForegroundColor White
            Write-Host "  Enabled: $enabledResources" -ForegroundColor Green
            Write-Host "  Available: $availableResources" -ForegroundColor Green
            Write-Host "  Limited: $limitedResources" -ForegroundColor Yellow
            Write-Host "  Exhausted: $exhaustedResources" -ForegroundColor Red
        } else {
            Write-Host "ℹ️ No resources available for this account" -ForegroundColor Gray
        }
        
        # Save token for further testing
        $global:AuthToken = $data.token
        Write-Host "`n🔑 Token saved to `$global:AuthToken for further testing" -ForegroundColor Cyan
        
    } else {
        Write-Host "❌ Login failed: $($loginWithResourcesResponse.message)" -ForegroundColor Red
    }
}
catch {
    Write-Host "❌ LoginWithResources failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Gray
    }
}

Write-Host "`n3. Testing Feature Access Check (if token available)..." -ForegroundColor Cyan

if ($global:AuthToken) {
    try {
        $headers = @{
            "Authorization" = "Bearer $global:AuthToken"
            "Content-Type" = "application/json"
        }
        
        # Test feature access check
        $featureCheckRequest = @{
            accountId = 1
            featureCode = "AI_TEXT_GEN"
        } | ConvertTo-Json
        
        $featureCheckResponse = Invoke-RestMethod -Uri "$BaseUrl/api/FeatureAccess/CheckAccess" `
            -Method POST `
            -Headers $headers `
            -ContentType "application/json" `
            -Body $featureCheckRequest `
            -ErrorAction Stop
        
        Write-Host "✅ Feature access check successful" -ForegroundColor Green
        Write-Host "Response: $($featureCheckResponse | ConvertTo-Json -Depth 3)" -ForegroundColor Gray
    }
    catch {
        Write-Host "⚠️ Feature access check not available or failed: $($_.Exception.Message)" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠️ No token available for testing feature access" -ForegroundColor Yellow
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Green

# Additional utility functions
Write-Host "`n📋 Available utility functions:" -ForegroundColor Cyan
Write-Host "  Test-FeatureAccess -FeatureCode 'FEATURE_CODE' -AccountId 1" -ForegroundColor Gray
Write-Host "  Get-UserResources" -ForegroundColor Gray

function Test-FeatureAccess {
    param(
        [Parameter(Mandatory=$true)]
        [string]$FeatureCode,
        
        [Parameter(Mandatory=$true)]
        [long]$AccountId,
        
        [Parameter(Mandatory=$false)]
        [string]$BaseUrl = "https://localhost:7000"
    )
    
    if (-not $global:AuthToken) {
        Write-Host "❌ No auth token available. Please run login test first." -ForegroundColor Red
        return
    }
    
    try {
        $headers = @{
            "Authorization" = "Bearer $global:AuthToken"
            "Content-Type" = "application/json"
        }
        
        $request = @{
            accountId = $AccountId
            featureCode = $FeatureCode
        } | ConvertTo-Json
        
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/FeatureAccess/CheckAccess" `
            -Method POST `
            -Headers $headers `
            -ContentType "application/json" `
            -Body $request
        
        Write-Host "✅ Feature access check for '$FeatureCode':" -ForegroundColor Green
        Write-Host "Has Access: $($response.data.hasAccess)" -ForegroundColor $(if($response.data.hasAccess) {"Green"} else {"Red"})
        if ($response.data.reason) {
            Write-Host "Reason: $($response.data.reason)" -ForegroundColor Yellow
        }
        
        return $response
    }
    catch {
        Write-Host "❌ Feature access check failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

function Get-UserResources {
    param(
        [Parameter(Mandatory=$false)]
        [string]$BaseUrl = "https://localhost:7000"
    )
    
    if (-not $global:AuthToken) {
        Write-Host "❌ No auth token available. Please run login test first." -ForegroundColor Red
        return
    }
    
    Write-Host "🔄 Fetching current user resources..." -ForegroundColor Cyan
    
    # Re-login to get fresh resource data
    $loginRequest = @{
        emailOrPhone = $Email
        password = $Password
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/Account/LoginWithResources" `
            -Method POST `
            -ContentType "application/json" `
            -Body $loginRequest
        
        if ($response.isSuccess -and $response.data.availableResources) {
            return $response.data.availableResources
        } else {
            Write-Host "⚠️ No resources found" -ForegroundColor Yellow
            return $null
        }
    }
    catch {
        Write-Host "❌ Failed to get resources: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

Write-Host "`nExample usage:" -ForegroundColor Cyan
Write-Host "  Test-FeatureAccess -FeatureCode 'AI_TEXT_GEN' -AccountId 1" -ForegroundColor Gray
Write-Host "  `$resources = Get-UserResources" -ForegroundColor Gray
