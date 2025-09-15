# Notification Service Demo Script
# This script demonstrates the notification functionality in the Kafka microservices system

param(
    [string]$CustomerId = "DEMO_CUSTOMER_001",
    [int]$OrderCount = 3,
    [int]$DelayMs = 2000
)

Write-Host "📧 Notification Service Demo" -ForegroundColor Green
Write-Host "============================" -ForegroundColor Green
Write-Host "Customer ID: $CustomerId" -ForegroundColor Yellow
Write-Host "Orders to create: $OrderCount" -ForegroundColor Yellow
Write-Host "Delay between operations: $DelayMs ms" -ForegroundColor Yellow
Write-Host ""

# Base URLs for services
$orderServiceUrl = "http://localhost:5001"
$notificationServiceUrl = "http://localhost:5003"
$inventoryServiceUrl = "http://localhost:5002"

# Product catalog
$products = @(
    @{ Id = "PROD001"; Name = "Gaming Laptop"; Price = 1299.99 },
    @{ Id = "PROD002"; Name = "Wireless Mouse"; Price = 49.99 },
    @{ Id = "PROD003"; Name = "Mechanical Keyboard"; Price = 129.99 },
    @{ Id = "PROD004"; Name = "4K Monitor"; Price = 399.99 }
)

function Test-ServiceHealth {
    param([string]$ServiceName, [string]$Url, [string]$TestPath = "")
    
    try {
        $testUrl = if ($TestPath) { "$Url$TestPath" } else { "$Url/swagger/index.html" }
        $response = Invoke-WebRequest -Uri $testUrl -Method GET -TimeoutSec 5 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ $ServiceName is running" -ForegroundColor Green
            return $true
        }
    }
    catch {
        Write-Host "❌ $ServiceName is not responding at $Url" -ForegroundColor Red
        return $false
    }
}

function Show-Notifications {
    param([string]$CustomerId)
    
    Write-Host "📋 Checking notifications for customer: $CustomerId" -ForegroundColor Cyan
    
    try {
        $notifications = Invoke-RestMethod -Uri "$notificationServiceUrl/api/notifications/$CustomerId" -Method GET
        
        if ($notifications.Count -eq 0) {
            Write-Host "   No notifications found for this customer" -ForegroundColor Yellow
        }
        else {
            Write-Host "   Found $($notifications.Count) notification(s):" -ForegroundColor Green
            foreach ($notification in $notifications) {
                $timestamp = [DateTime]::Parse($notification.timestamp).ToString("yyyy-MM-dd HH:mm:ss")
                Write-Host "   [$timestamp] $($notification.type): $($notification.message)" -ForegroundColor White
            }
        }
    }
    catch {
        Write-Host "   Error retrieving notifications: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

function Create-Order {
    param([string]$CustomerId, [int]$OrderNumber)
    
    Write-Host "🛒 Creating Order #$OrderNumber for customer $CustomerId" -ForegroundColor Cyan
    
    # Generate random order items
    $itemCount = Get-Random -Minimum 1 -Maximum 3
    $items = @()
    $totalAmount = 0
    
    for ($i = 1; $i -le $itemCount; $i++) {
        $product = $products | Get-Random
        $quantity = Get-Random -Minimum 1 -Maximum 3
        $items += @{
            ProductId = $product.Id
            ProductName = $product.Name
            Quantity = $quantity
            UnitPrice = $product.Price
        }
        $totalAmount += $product.Price * $quantity
    }
    
    $orderRequest = @{
        CustomerId = $CustomerId
        Items = $items
    } | ConvertTo-Json -Depth 3
    
    try {
        Write-Host "   Creating order with $itemCount item(s), total: $($totalAmount.ToString('C'))" -ForegroundColor Gray
        
        $response = Invoke-RestMethod -Uri "$orderServiceUrl/api/orders" -Method POST -Body $orderRequest -ContentType "application/json"
        
        Write-Host "   ✅ Order created successfully!" -ForegroundColor Green
        Write-Host "   Order ID: $($response.id)" -ForegroundColor White
        Write-Host "   Status: $($response.status)" -ForegroundColor White
        Write-Host "   Total: $($response.totalAmount.ToString('C'))" -ForegroundColor White
        
        return $response
    }
    catch {
        Write-Host "   ❌ Failed to create order: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

function Test-OrderConfirmationNotification {
    param([string]$CustomerId)
    
    Write-Host "🧪 Testing Order Confirmation Notification (Direct API)" -ForegroundColor Cyan
    
    $testOrderId = [Guid]::NewGuid()
    $testAmount = 299.99
    
    $testRequest = @{
        CustomerId = $CustomerId
        OrderId = $testOrderId
        TotalAmount = $testAmount
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri "$notificationServiceUrl/api/notifications/test-order-confirmation" -Method POST -Body $testRequest -ContentType "application/json"
        Write-Host "   ✅ Test notification sent: $response" -ForegroundColor Green
        Write-Host "   Test Order ID: $testOrderId" -ForegroundColor White
        Write-Host "   Test Amount: $($testAmount.ToString('C'))" -ForegroundColor White
    }
    catch {
        Write-Host "   ❌ Failed to send test notification: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

function Get-InventoryStatus {
    param([string]$ProductId)
    
    try {
        $inventory = Invoke-RestMethod -Uri "$inventoryServiceUrl/api/inventory/$ProductId" -Method GET
        return $inventory
    }
    catch {
        Write-Host "   ⚠️ Could not get inventory for product $ProductId" -ForegroundColor Yellow
        return $null
    }
}

# Main execution
Write-Host "🔍 Checking service health..." -ForegroundColor Cyan

$orderServiceOk = Test-ServiceHealth "Order Service" $orderServiceUrl
$notificationServiceOk = Test-ServiceHealth "Notification Service" $notificationServiceUrl
$inventoryServiceOk = Test-ServiceHealth "Inventory Service" $inventoryServiceUrl

if (-not ($orderServiceOk -and $notificationServiceOk -and $inventoryServiceOk)) {
    Write-Host "❌ One or more services are not running. Please start all services first." -ForegroundColor Red
    Write-Host "Run: docker-compose up --build -d" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Show initial notifications
Write-Host "📋 Initial notification check..." -ForegroundColor Magenta
Show-Notifications $CustomerId

# Test direct notification API
Test-OrderConfirmationNotification $CustomerId
Start-Sleep -Milliseconds ($DelayMs / 2)
Show-Notifications $CustomerId

# Create orders and demonstrate the full flow
Write-Host "🔄 Starting Order Creation Flow..." -ForegroundColor Magenta

for ($i = 1; $i -le $OrderCount; $i++) {
    Write-Host ""
    Write-Host "--- Order Creation #$i ---" -ForegroundColor DarkCyan
    
    # Show available inventory before order
    Write-Host "📦 Checking inventory status..." -ForegroundColor Gray
    foreach ($product in $products) {
        $inventory = Get-InventoryStatus $product.Id
        if ($inventory) {
            Write-Host "   $($product.Name): $($inventory.availableQuantity) units available" -ForegroundColor Gray
        }
    }
    
    # Create order (this will trigger Kafka events and notifications)
    $order = Create-Order $CustomerId $i
    
    if ($order) {
        Write-Host "   ⏳ Waiting for Kafka events to be processed..." -ForegroundColor Gray
        Start-Sleep -Milliseconds $DelayMs
        
        # Show notifications after order creation
        Show-Notifications $CustomerId
    }
    
    if ($i -lt $OrderCount) {
        Write-Host "   ⏸️ Waiting before next order..." -ForegroundColor Gray
        Start-Sleep -Milliseconds $DelayMs
    }
}

Write-Host ""
Write-Host "📊 Final Summary" -ForegroundColor Magenta
Write-Host "================" -ForegroundColor Magenta

# Show final notification count
Show-Notifications $CustomerId

# Show inventory status after all orders
Write-Host "📦 Final inventory status:" -ForegroundColor Cyan
foreach ($product in $products) {
    $inventory = Get-InventoryStatus $product.Id
    if ($inventory) {
        Write-Host "   $($product.Name): $($inventory.availableQuantity) units available" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "✨ Notification Demo Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "💡 What happened:" -ForegroundColor Yellow
Write-Host "1. Direct notification test was sent via REST API" -ForegroundColor White
Write-Host "2. Orders were created via Order Service REST API" -ForegroundColor White
Write-Host "3. Order Service published 'OrderCreated' events to Kafka" -ForegroundColor White
Write-Host "4. Inventory Service consumed events and published 'InventoryReserved' events" -ForegroundColor White
Write-Host "5. Notification Service consumed both event types from Kafka" -ForegroundColor White
Write-Host "6. Notifications were automatically created and stored in database" -ForegroundColor White
Write-Host ""
Write-Host "🌐 Access points:" -ForegroundColor Yellow
Write-Host "- Order Service: http://localhost:5001/swagger" -ForegroundColor White
Write-Host "- Inventory Service: http://localhost:5002/swagger" -ForegroundColor White
Write-Host "- Notification Service: http://localhost:5003/swagger" -ForegroundColor White
Write-Host "- Kafka UI: http://localhost:8080" -ForegroundColor White
