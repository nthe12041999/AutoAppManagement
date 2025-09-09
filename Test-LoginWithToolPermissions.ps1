# Test Login with Tool Permissions
# Kiểm tra API Login đã trả về đầy đủ thông tin quyền tool

$BaseUrl = "https://localhost:7000"
$Email = "customer@email.com"
$Password = "password123"

Write-Host "🧪 Testing Login API with Tool Permissions" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# Test Login API
Write-Host "`n🔐 Testing Login API..." -ForegroundColor Yellow

$loginRequest = @{
    emailOrPhone = $Email
    password = $Password
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$BaseUrl/api/Account/Login" `
        -Method POST `
        -ContentType "application/json" `
        -Body $loginRequest `
        -SkipCertificateCheck

    if ($loginResponse.isSuccess) {
        Write-Host "✅ Login successful!" -ForegroundColor Green
        Write-Host "   Message: $($loginResponse.message)" -ForegroundColor White
        
        $data = $loginResponse.data
        
        # Check account info
        if ($data.account) {
            Write-Host "`n👤 Account Information:" -ForegroundColor Cyan
            Write-Host "   ID: $($data.account.id)" -ForegroundColor White
            Write-Host "   Username: $($data.account.userName)" -ForegroundColor White
            Write-Host "   Email: $($data.account.email)" -ForegroundColor White
            Write-Host "   Name: $($data.account.name)" -ForegroundColor White
        }
        
        # Check license info
        if ($data.licenseInfo) {
            Write-Host "`n📋 License Information:" -ForegroundColor Cyan
            Write-Host "   License ID: $($data.licenseInfo.licenseId)" -ForegroundColor White
            Write-Host "   License Key: $($data.licenseInfo.licenseKey)" -ForegroundColor White
            Write-Host "   License Name: $($data.licenseInfo.licenseName)" -ForegroundColor White
            Write-Host "   License Type: $($data.licenseInfo.licenseType)" -ForegroundColor White
            Write-Host "   Status: $($data.licenseInfo.status)" -ForegroundColor White
            Write-Host "   Days Remaining: $($data.licenseInfo.daysRemaining)" -ForegroundColor White
        }
        
        # Check tool permissions/resources
        if ($data.availableResources) {
            Write-Host "`n🔧 Available Tool Resources:" -ForegroundColor Cyan
            
            $totalResources = $data.availableResources.Count
            $enabledResources = ($data.availableResources | Where-Object { $_.isEnabled }).Count
            $availableResources = ($data.availableResources | Where-Object { $_.status -eq "available" }).Count
            $limitedResources = ($data.availableResources | Where-Object { $_.status -eq "limited" }).Count
            $exhaustedResources = ($data.availableResources | Where-Object { $_.status -eq "exhausted" }).Count
            
            Write-Host "   Total Resources: $totalResources" -ForegroundColor White
            Write-Host "   Enabled: $enabledResources" -ForegroundColor Green
            Write-Host "   Available: $availableResources" -ForegroundColor Green
            Write-Host "   Limited: $limitedResources" -ForegroundColor Yellow
            Write-Host "   Exhausted: $exhaustedResources" -ForegroundColor Red
            
            Write-Host "`n   📊 Resource Details:" -ForegroundColor Cyan
            foreach ($resource in $data.availableResources) {
                $statusColor = switch ($resource.status) {
                    "available" { "Green" }
                    "limited" { "Yellow" }
                    "exhausted" { "Red" }
                    "disabled" { "Gray" }
                    default { "White" }
                }
                
                $usageInfo = if ($resource.usageLimit) { 
                    "$($resource.usedCount)/$($resource.usageLimit) (remaining: $($resource.remainingCount))" 
                } else { 
                    "$($resource.usedCount)/unlimited" 
                }
                
                Write-Host "     ⚡ $($resource.featureName) [$($resource.featureCode)]" -ForegroundColor White
                Write-Host "        Tool: $($resource.toolName)" -ForegroundColor Gray
                Write-Host "        Status: $($resource.status) | Usage: $usageInfo" -ForegroundColor $statusColor
                if ($resource.warningMessage) {
                    Write-Host "        ⚠️  $($resource.warningMessage)" -ForegroundColor Yellow
                }
            }
        } else {
            Write-Host "ℹ️ No tool resources found in response" -ForegroundColor Gray
        }
        
        # Check token
        if ($data.token) {
            Write-Host "`n🔑 Authentication Token:" -ForegroundColor Cyan
            Write-Host "   Token: $($data.token.Substring(0, 50))..." -ForegroundColor White
            Write-Host "   Expiry: $($data.tokenExpiry)" -ForegroundColor White
        }
        
        Write-Host "`n✅ Login API now returns full tool permissions!" -ForegroundColor Green
        
    } else {
        Write-Host "❌ Login failed: $($loginResponse.message)" -ForegroundColor Red
    }
}
catch {
    Write-Host "❌ Login API test failed: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode
        Write-Host "   Status Code: $statusCode" -ForegroundColor Red
    }
}

Write-Host "`n🎯 Test completed!" -ForegroundColor Cyan
