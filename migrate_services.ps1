# Migration script for services to use generic repository pattern

$servicesPath = "AutoAppManagement.Service\Services"

# Service to Repository mapping
$serviceRepoMap = @{
    "AccountService" = "IAccountsRepository"
    "AdminAccountService" = "IAdminAccountRepository" 
    "LicenseService" = "ILicenseRepository"
    "NotificationService" = "INotificationsRepository"
    "PermissionService" = "IRoleAccountRepository"
}

foreach ($service in $serviceRepoMap.Keys) {
    $filePath = "$servicesPath\$service.cs"
    $repository = $serviceRepoMap[$service]
    
    Write-Host "Migrating $service to use $repository..."
    
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Add repository import
        $content = $content -replace '(using AutoAppManagement\.Repository\.Repositories\.Base;)', '$1`nusing AutoAppManagement.Repository.Repositories;'
        
        # Update class declaration
        $content = $content -replace "BaseBusinessService<([^,]+), ([^>]+)>", "BaseBusinessService<`$1, `$2, $repository>"
        
        # Replace Repository.Update with Update helper method
        $content = $content -replace 'Repository\.Update\(([^)]*)\);', 'Update($1); // Use helper method from BaseBusinessService'
        
        # Replace Repository.Insert with Insert helper method
        $content = $content -replace 'Repository\.Insert\(([^)]*)\);', 'await Insert($1); // Use helper method from BaseBusinessService'
        
        # Replace Repository.Any with Any helper method
        $content = $content -replace 'Repository\.Any\(([^)]*)\)', 'await Any($1) // Use helper method from BaseBusinessService'
        
        # Save the file
        $content | Set-Content $filePath -NoNewline
        
        Write-Host "✅ Migrated $service"
    } else {
        Write-Host "❌ File not found: $filePath"
    }
}

Write-Host "`n🎉 Migration completed! Please review and test the changes."
