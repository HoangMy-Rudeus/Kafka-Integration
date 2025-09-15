# Simple Notification Demo Script
# Demonstrates notification functionality without complex health checks

param(
    [string]$CustomerId = "DEMO_CUSTOMER_001"
)

Write-Host "📧 Kafka Microservices Notification Demo" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Customer ID: $CustomerId" -ForegroundColor Yellow
Write-Host ""

# Service URLs
$orderServiceUrl = "http://localhost:5001"
$notificationServiceUrl = "http://localhost:5003"
$inventoryServiceUrl = "http://localhost:5002"

function Show-Notifications {
    param([string]$CustomerId)
    
    Write-Host "📋 Notifications for customer: $CustomerId" -ForegroundColor Cyan
    
    try {
        $notifications = Invoke-RestMethod -Uri "$notificationServiceUrl/api/notifications/$CustomerId" -Method GET -TimeoutSec 10
        
        if ($notifications.Count -eq 0) {
            Write-Host "   📭 No notifications found" -ForegroundColor Yellow
        }
        else {
            Write-Host "   📬 Found $($notifications.Count) notification(s):" -ForegroundColor Green
            $notifications | ForEach-Object {
                $timestamp = [DateTime]::Parse($_.timestamp).ToString("yyyy-MM-dd HH:mm:ss")
                Write-Host "   [$timestamp] $($_.type)" -ForegroundColor Magenta
                Write-Host "   └─ $($_.message)" -ForegroundColor White
                Write-Host ""
            }
        }
    }
    catch {
        Write-Host "   ❌ Error retrieving notifications: $($_.Exception.Message)" -ForegroundColor Red
    }
}

function Send-TestNotification {
    param([string]$CustomerId)
    
    Write-Host "🧪 Sending Test Order Confirmation Notification" -ForegroundColor Cyan
    
    $testOrderId = [Guid]::NewGuid()
    $testAmount = 199.99
    
    $testRequest = @{
        CustomerId = $CustomerId
        OrderId = $testOrderId
        TotalAmount = $testAmount
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri "$notificationServiceUrl/api/notifications/test-order-confirmation" -Method POST -Body $testRequest -ContentType "application/json" -TimeoutSec 10
        Write-Host "   ✅ $response" -ForegroundColor Green
        Write-Host "   📧 Test Order ID: $testOrderId" -ForegroundColor White
        Write-Host "   💰 Amount: $($testAmount.ToString('C'))" -ForegroundColor White
    }
    catch {
        Write-Host "   ❌ Failed to send test notification: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

function Create-SampleOrder {
    param([string]$CustomerId)
    
    Write-Host "🛒 Creating Sample Order" -ForegroundColor Cyan
    
    $orderRequest = @{
        CustomerId = $CustomerId
        Items = @(
            @{
                ProductId = "PROD001"
                ProductName = "Gaming Laptop"
                Quantity = 1
                UnitPrice = 1299.99
            },
            @{
                ProductId = "PROD002"
                ProductName = "Wireless Mouse"
                Quantity = 2
                UnitPrice = 49.99
            }
        )
    } | ConvertTo-Json -Depth 3
    
    try {
        Write-Host "   📦 Creating order with 2 products..." -ForegroundColor Gray
        $response = Invoke-RestMethod -Uri "$orderServiceUrl/api/orders" -Method POST -Body $orderRequest -ContentType "application/json" -TimeoutSec 10
        
        Write-Host "   ✅ Order created successfully!" -ForegroundColor Green
        Write-Host "   📝 Order ID: $($response.id)" -ForegroundColor White
        Write-Host "   📊 Status: $($response.status)" -ForegroundColor White
        Write-Host "   💰 Total: $($response.totalAmount.ToString('C'))" -ForegroundColor White
        
        return $response.id
    }
    catch {
        Write-Host "   ❌ Failed to create order: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Main Demo Flow
Write-Host "🎬 Starting Notification Demo..." -ForegroundColor Magenta
Write-Host ""

# Step 1: Check initial notifications
Write-Host "Step 1: Check existing notifications" -ForegroundColor DarkCyan
Show-Notifications $CustomerId

# Step 2: Send test notification (direct API call)
Write-Host "Step 2: Test direct notification API" -ForegroundColor DarkCyan
Send-TestNotification $CustomerId

Write-Host "⏳ Waiting 2 seconds for notification to be processed..." -ForegroundColor Gray
Start-Sleep -Seconds 2

Show-Notifications $CustomerId

# Step 3: Create real order (triggers Kafka events)
Write-Host "Step 3: Create order (triggers Kafka workflow)" -ForegroundColor DarkCyan
$orderId = Create-SampleOrder $CustomerId

if ($orderId) {
    Write-Host ""
    Write-Host "⏳ Waiting 5 seconds for Kafka events to be processed..." -ForegroundColor Gray
    Write-Host "   (OrderCreated → InventoryReserved → Notifications)" -ForegroundColor Gray
    Start-Sleep -Seconds 5
    
    Write-Host ""
    Write-Host "Step 4: Check notifications after order creation" -ForegroundColor DarkCyan
    Show-Notifications $CustomerId
}

Write-Host ""
Write-Host "✨ Demo Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "💡 What just happened:" -ForegroundColor Yellow
Write-Host "1. 🧪 Sent test notification via direct API call" -ForegroundColor White
Write-Host "2. 🛒 Created order via Order Service API" -ForegroundColor White
Write-Host "3. 📤 Order Service published OrderCreated event to Kafka" -ForegroundColor White
Write-Host "4. 📦 Inventory Service consumed event, reserved inventory" -ForegroundColor White
Write-Host "5. 📤 Inventory Service published InventoryReserved event to Kafka" -ForegroundColor White
Write-Host "6. 📧 Notification Service consumed both events automatically" -ForegroundColor White
Write-Host "7. 💾 Notifications were saved to database" -ForegroundColor White
Write-Host ""
Write-Host "🌐 Explore more:" -ForegroundColor Yellow
Write-Host "- Order Service Swagger: http://localhost:5001/swagger" -ForegroundColor Cyan
Write-Host "- Inventory Service Swagger: http://localhost:5002/swagger" -ForegroundColor Cyan
Write-Host "- Notification Service Swagger: http://localhost:5003/swagger" -ForegroundColor Cyan
Write-Host "- Kafka UI: http://localhost:8080" -ForegroundColor Cyan
Write-Host ""
Write-Host "🔄 Re-run anytime with: .\scripts\simple-notification-demo.ps1" -ForegroundColor Yellow
