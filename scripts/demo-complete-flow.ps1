# Demo Script: Complete Order Flow
# This script demonstrates the complete microservices flow

Write-Host "🚀 Starting Kafka Microservices Demo" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

# Step 1: Check if services are running
Write-Host "`n📋 Step 1: Checking service health..." -ForegroundColor Yellow

$services = @(
    @{ Name = "Order Service"; Url = "http://localhost:5001/api/orders" },
    @{ Name = "Inventory Service"; Url = "http://localhost:5002/api/inventory" },
    @{ Name = "Notification Service"; Url = "http://localhost:5003/api/notifications/CUST001" }
)

foreach ($service in $services) {
    try {
        $response = Invoke-RestMethod -Uri $service.Url -Method GET -TimeoutSec 5
        Write-Host "✅ $($service.Name) is running" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ $($service.Name) is not responding" -ForegroundColor Red
        Write-Host "   Please start the services first: docker-compose up" -ForegroundColor Red
        exit 1
    }
}

# Step 2: View initial inventory
Write-Host "`n📦 Step 2: Viewing initial inventory..." -ForegroundColor Yellow
try {
    $inventory = Invoke-RestMethod -Uri "http://localhost:5002/api/inventory" -Method GET
    Write-Host "Current Inventory:" -ForegroundColor Cyan
    $inventory | ForEach-Object {
        Write-Host "  - $($_.productName) ($($_.productId)): $($_.availableQuantity) available, $($_.reservedQuantity) reserved" -ForegroundColor White
    }
}
catch {
    Write-Host "❌ Failed to retrieve inventory" -ForegroundColor Red
}

# Step 3: Create a sample order
Write-Host "`n🛒 Step 3: Creating a sample order..." -ForegroundColor Yellow

$orderData = @{
    CustomerId = "CUST001"
    Items = @(
        @{
            ProductId = "PROD001"
            ProductName = "Laptop"
            Quantity = 2
            Price = 999.99
        },
        @{
            ProductId = "PROD002"
            ProductName = "Mouse"
            Quantity = 1
            Price = 29.99
        }
    )
} | ConvertTo-Json -Depth 3

try {
    Write-Host "Sending order request..." -ForegroundColor Cyan
    $order = Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method POST -Body $orderData -ContentType "application/json"
    Write-Host "✅ Order created successfully!" -ForegroundColor Green
    Write-Host "   Order ID: $($order.id)" -ForegroundColor White
    Write-Host "   Customer: $($order.customerId)" -ForegroundColor White
    Write-Host "   Total Amount: $$$($order.totalAmount)" -ForegroundColor White
    Write-Host "   Status: $($order.status)" -ForegroundColor White
    
    $orderId = $order.id
}
catch {
    Write-Host "❌ Failed to create order: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 4: Wait for event processing
Write-Host "`n⏳ Step 4: Waiting for event processing..." -ForegroundColor Yellow
Write-Host "   (In a real system, this happens asynchronously)" -ForegroundColor Gray
Start-Sleep -Seconds 3

# Step 5: Check updated inventory
Write-Host "`n📦 Step 5: Checking inventory after order..." -ForegroundColor Yellow
try {
    $updatedInventory = Invoke-RestMethod -Uri "http://localhost:5002/api/inventory" -Method GET
    Write-Host "Updated Inventory:" -ForegroundColor Cyan
    $updatedInventory | ForEach-Object {
        Write-Host "  - $($_.productName) ($($_.productId)): $($_.availableQuantity) available, $($_.reservedQuantity) reserved" -ForegroundColor White
    }
}
catch {
    Write-Host "❌ Failed to retrieve updated inventory" -ForegroundColor Red
}

# Step 6: Check notifications
Write-Host "`n📨 Step 6: Checking customer notifications..." -ForegroundColor Yellow
try {
    $notifications = Invoke-RestMethod -Uri "http://localhost:5003/api/notifications/CUST001" -Method GET
    if ($notifications.Count -gt 0) {
        Write-Host "Customer Notifications:" -ForegroundColor Cyan
        $notifications | ForEach-Object {
            Write-Host "  - [$($_.type)] $($_.message)" -ForegroundColor White
            Write-Host "    Sent: $($_.timestamp)" -ForegroundColor Gray
        }
    } else {
        Write-Host "No notifications found for customer CUST001" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "❌ Failed to retrieve notifications" -ForegroundColor Red
}

# Step 7: Verify order status
Write-Host "`n📋 Step 7: Verifying order status..." -ForegroundColor Yellow
try {
    $orderStatus = Invoke-RestMethod -Uri "http://localhost:5001/api/orders/$orderId" -Method GET
    Write-Host "Order Status:" -ForegroundColor Cyan
    Write-Host "  - Order ID: $($orderStatus.id)" -ForegroundColor White
    Write-Host "  - Status: $($orderStatus.status)" -ForegroundColor White
    Write-Host "  - Created: $($orderStatus.createdAt)" -ForegroundColor White
}
catch {
    Write-Host "❌ Failed to retrieve order status" -ForegroundColor Red
}

Write-Host "`n🎉 Demo completed successfully!" -ForegroundColor Green
Write-Host "=====================================`n" -ForegroundColor Green

Write-Host "💡 What happened:" -ForegroundColor Cyan
Write-Host "   1. Order Service received the order and published OrderCreatedEvent to Kafka" -ForegroundColor White
Write-Host "   2. Inventory Service consumed the event and reserved inventory" -ForegroundColor White
Write-Host "   3. Notification Service consumed events and sent customer notifications" -ForegroundColor White
Write-Host "   4. All services logged their activities (check Docker logs)" -ForegroundColor White

Write-Host "`n🔍 Next steps:" -ForegroundColor Cyan
Write-Host "   - Check Kafka UI at http://localhost:8080 to see message flow" -ForegroundColor White
Write-Host "   - View service logs: docker-compose logs [service-name]" -ForegroundColor White
Write-Host "   - Try the other demo scripts in this folder" -ForegroundColor White
