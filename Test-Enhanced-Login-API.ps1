# 🧪 Test Enhanced Login API with Tool Permissions

# Kiểm tra API Login đã được cập nhật với tool permissions

Write-Host "🚀 Testing Enhanced Login API..." -ForegroundColor Cyan

# Environment variables
$baseUrl = "https://localhost:7000"
$loginUrl = "$baseUrl/api/Account/Login"

# Test credentials
$credentials = @{
    emailOrPhone = "customer@email.com"
    password = "password123"
} | ConvertTo-Json

Write-Host "📧 Testing login with credentials:" -ForegroundColor Yellow
Write-Host "Email: customer@email.com" -ForegroundColor Gray
Write-Host "Password: ********" -ForegroundColor Gray

try {
    # Make login request
    Write-Host "`n🔄 Making login request..." -ForegroundColor Yellow
    
    $headers = @{
        "Content-Type" = "application/json"
        "Accept" = "application/json"
    }
    
    $response = Invoke-RestMethod -Uri $loginUrl -Method Post -Body $credentials -Headers $headers -ErrorAction Stop
    
    if ($response.isSuccess) {
        Write-Host "✅ Login successful!" -ForegroundColor Green
        
        # Display account info
        if ($response.data.account) {
            Write-Host "`n👤 Account Information:" -ForegroundColor Cyan
            Write-Host "  ID: $($response.data.account.id)" -ForegroundColor White
            Write-Host "  Username: $($response.data.account.userName)" -ForegroundColor White
            Write-Host "  Email: $($response.data.account.email)" -ForegroundColor White
            Write-Host "  Name: $($response.data.account.name)" -ForegroundColor White
        }
        
        # Display license info
        if ($response.data.licenseInfo) {
            Write-Host "`n🎫 License Information:" -ForegroundColor Cyan
            Write-Host "  License Key: $($response.data.licenseInfo.licenseKey)" -ForegroundColor White
            Write-Host "  License Type: $($response.data.licenseInfo.licenseType)" -ForegroundColor White
            Write-Host "  Status: $($response.data.licenseInfo.status)" -ForegroundColor White
            Write-Host "  Expiry Date: $($response.data.licenseInfo.expiryDate)" -ForegroundColor White
        }
        
        # Display available resources (tool permissions)
        if ($response.data.availableResources) {
            $resourceCount = $response.data.availableResources.Count
            Write-Host "`n🔧 Available Tool Resources ($resourceCount):" -ForegroundColor Cyan
            
            foreach ($resource in $response.data.availableResources) {
                Write-Host "  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
                Write-Host "  🛠️  Feature: $($resource.featureName)" -ForegroundColor Yellow
                Write-Host "     Code: $($resource.featureCode)" -ForegroundColor Gray
                Write-Host "     Category: $($resource.category)" -ForegroundColor Gray
                Write-Host "     Type: $($resource.featureType)" -ForegroundColor Gray
                Write-Host "     Enabled: $($resource.isEnabled)" -ForegroundColor $(if ($resource.isEnabled) { "Green" } else { "Red" })
                
                if ($resource.resourceLimits) {
                    Write-Host "     Limits: $($resource.resourceLimits)" -ForegroundColor Magenta
                }
                
                if ($resource.usageQuota) {
                    Write-Host "     Quota: $($resource.usageQuota)" -ForegroundColor Magenta
                }
            }
        } else {
            Write-Host "`n⚠️  No tool resources found in response" -ForegroundColor Yellow
        }
        
        # Display token info
        if ($response.data.token) {
            $tokenLength = $response.data.token.Length
            $tokenPreview = $response.data.token.Substring(0, [Math]::Min(50, $tokenLength)) + "..."
            Write-Host "`n🔑 Authentication Token:" -ForegroundColor Cyan
            Write-Host "  Length: $tokenLength characters" -ForegroundColor White
            Write-Host "  Preview: $tokenPreview" -ForegroundColor Gray
        }
        
        # Test a protected endpoint with the token
        Write-Host "`n🧪 Testing protected endpoint with token..." -ForegroundColor Yellow
        
        $authHeaders = @{
            "Authorization" = "Bearer $($response.data.token)"
            "Accept" = "application/json"
        }
        
        try {
            $profileResponse = Invoke-RestMethod -Uri "$baseUrl/api/Account/GetById/$($response.data.account.id)" -Method Get -Headers $authHeaders -ErrorAction Stop
            Write-Host "✅ Protected endpoint access successful!" -ForegroundColor Green
        } catch {
            Write-Host "❌ Protected endpoint access failed: $($_.Exception.Message)" -ForegroundColor Red
        }
        
    } else {
        Write-Host "❌ Login failed!" -ForegroundColor Red
        Write-Host "Message: $($response.message)" -ForegroundColor Red
        if ($response.errors) {
            Write-Host "Errors: $($response.errors | ConvertTo-Json)" -ForegroundColor Red
        }
    }
    
} catch {
    Write-Host "❌ Request failed: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "Status Code: $statusCode" -ForegroundColor Red
        
        try {
            $errorStream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($errorStream)
            $errorContent = $reader.ReadToEnd()
            Write-Host "Error Content: $errorContent" -ForegroundColor Red
        } catch {
            Write-Host "Could not read error content" -ForegroundColor Red
        }
    }
}

Write-Host "`n" -NoNewline
Write-Host "🏁 Test completed!" -ForegroundColor Cyan
Write-Host "📚 Check the Updated_Postman_Collection_README.md for more testing options" -ForegroundColor Gray

# Pause to keep window open
Write-Host "`nPress any key to continue..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
