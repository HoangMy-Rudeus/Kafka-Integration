# Quick Notification Examples
# Simple examples showing notification functionality

Write-Host "📧 Quick Notification Examples" -ForegroundColor Green
Write-Host "==============================" -ForegroundColor Green
Write-Host ""

$notificationServiceUrl = "http://localhost:5003"

function Show-Notifications {
    param([string]$CustomerId)
    
    Write-Host "📋 Notifications for: $CustomerId" -ForegroundColor Cyan
    
    try {
        $notifications = Invoke-RestMethod -Uri "$notificationServiceUrl/api/notifications/$CustomerId" -Method GET
        
        if ($notifications.Count -eq 0) {
            Write-Host "   📭 No notifications" -ForegroundColor Yellow
        } else {
            Write-Host "   📬 $($notifications.Count) notification(s):" -ForegroundColor Green
            $notifications | ForEach-Object {
                $time = [DateTime]::Parse($_.timestamp).ToString("HH:mm:ss")
                Write-Host "   [$time] $($_.type): $($_.message)" -ForegroundColor White
            }
        }
    }
    catch {
        Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

function Send-TestNotification {
    param([string]$CustomerId, [decimal]$Amount = 99.99)
    
    $orderId = [Guid]::NewGuid()
    $request = @{
        CustomerId = $CustomerId
        OrderId = $orderId
        TotalAmount = $Amount
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri "$notificationServiceUrl/api/notifications/test-order-confirmation" -Method POST -Body $request -ContentType "application/json"
        Write-Host "✅ Sent notification to $CustomerId - Order: $orderId, Amount: $($Amount.ToString('C'))" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Failed to send notification to $CustomerId" -ForegroundColor Red
    }
}

# Example 1: Check notifications for different customers
Write-Host "Example 1: Check existing notifications" -ForegroundColor DarkCyan
Show-Notifications "DEMO_USER_123"
Show-Notifications "DEMO_USER_456"
Show-Notifications "NEW_CUSTOMER"

# Example 2: Send test notifications
Write-Host "Example 2: Send test notifications" -ForegroundColor DarkCyan
Send-TestNotification "CUSTOMER_A" 159.99
Send-TestNotification "CUSTOMER_B" 299.50
Send-TestNotification "CUSTOMER_A" 75.25

Write-Host "⏳ Waiting 2 seconds..." -ForegroundColor Gray
Start-Sleep -Seconds 2

# Example 3: Check notifications after sending
Write-Host "Example 3: Check notifications after sending" -ForegroundColor DarkCyan
Show-Notifications "CUSTOMER_A"
Show-Notifications "CUSTOMER_B"

Write-Host "🎯 Key Points:" -ForegroundColor Yellow
Write-Host "• Each customer has their own notification list" -ForegroundColor White
Write-Host "• Notifications are stored in PostgreSQL database" -ForegroundColor White
Write-Host "• REST API provides easy access to notification data" -ForegroundColor White
Write-Host "• In production, Kafka events would trigger notifications automatically" -ForegroundColor White
Write-Host ""
Write-Host "🌐 API Endpoints:" -ForegroundColor Yellow
Write-Host "• GET  /api/notifications/{customerId} - Get customer notifications" -ForegroundColor Cyan
Write-Host "• POST /api/notifications/test-order-confirmation - Send test notification" -ForegroundColor Cyan
