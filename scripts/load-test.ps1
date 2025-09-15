# Load Testing Script for Kafka Microservices
# This script creates multiple orders to test the system under load

param(
    [int]$OrderCount = 10,
    [int]$DelayMs = 1000
)

Write-Host "🚀 Starting Load Test for Kafka Microservices" -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green
Write-Host "Orders to create: $OrderCount" -ForegroundColor Yellow
Write-Host "Delay between orders: $DelayMs ms" -ForegroundColor Yellow
Write-Host ""

# Product catalog for random order generation
$products = @(
    @{ Id = "PROD001"; Name = "Laptop"; Price = 999.99 },
    @{ Id = "PROD002"; Name = "Mouse"; Price = 29.99 },
    @{ Id = "PROD003"; Name = "Keyboard"; Price = 79.99 },
    @{ Id = "PROD004"; Name = "Monitor"; Price = 299.99 }
)

$customers = @("CUST001", "CUST002", "CUST003", "CUST004", "CUST005")

# Check if Order Service is running
try {
    Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method GET -TimeoutSec 5 | Out-Null
    Write-Host "✅ Order Service is running" -ForegroundColor Green
}
catch {
    Write-Host "❌ Order Service is not responding. Please start the services first." -ForegroundColor Red
    exit 1
}

$successCount = 0
$failCount = 0
$orderIds = @()

Write-Host "🛒 Creating orders..." -ForegroundColor Cyan

for ($i = 1; $i -le $OrderCount; $i++) {
    try {
        # Generate random order
        $customerId = $customers | Get-Random
        $itemCount = Get-Random -Minimum 1 -Maximum 4
        $items = @()
        
        for ($j = 1; $j -le $itemCount; $j++) {
            $product = $products | Get-Random
            $quantity = Get-Random -Minimum 1 -Maximum 5
            $items += @{
                ProductId = $product.Id
                ProductName = $product.Name
                Quantity = $quantity
                Price = $product.Price
            }
        }
        
        $orderData = @{
            CustomerId = $customerId
            Items = $items
        } | ConvertTo-Json -Depth 3
        
        Write-Host "Creating order $i/$OrderCount for customer $customerId..." -ForegroundColor Yellow
        
        $order = Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method POST -Body $orderData -ContentType "application/json"
        
        Write-Host "  ✅ Order $($order.id) created - Total: $$$($order.totalAmount)" -ForegroundColor Green
        $orderIds += $order.id
        $successCount++
        
        if ($i -lt $OrderCount) {
            Start-Sleep -Milliseconds $DelayMs
        }
    }
    catch {
        Write-Host "  ❌ Failed to create order $i : $($_.Exception.Message)" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "📊 Load Test Results:" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan
Write-Host "Total orders attempted: $OrderCount" -ForegroundColor White
Write-Host "Successful orders: $successCount" -ForegroundColor Green
Write-Host "Failed orders: $failCount" -ForegroundColor Red
Write-Host "Success rate: $([math]::Round(($successCount / $OrderCount) * 100, 2))%" -ForegroundColor Yellow

if ($successCount -gt 0) {
    Write-Host ""
    Write-Host "⏳ Waiting for event processing..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5
    
    Write-Host ""
    Write-Host "📦 Checking inventory status..." -ForegroundColor Cyan
    try {
        $inventory = Invoke-RestMethod -Uri "http://localhost:5002/api/inventory" -Method GET
        $inventory | ForEach-Object {
            Write-Host "  - $($_.productName): $($_.availableQuantity) available, $($_.reservedQuantity) reserved" -ForegroundColor White
        }
    }
    catch {
        Write-Host "❌ Failed to retrieve inventory status" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "📨 Checking notification counts..." -ForegroundColor Cyan
    $totalNotifications = 0
    foreach ($customer in $customers) {
        try {
            $notifications = Invoke-RestMethod -Uri "http://localhost:5003/api/notifications/$customer" -Method GET
            if ($notifications.Count -gt 0) {
                Write-Host "  - $customer : $($notifications.Count) notifications" -ForegroundColor White
                $totalNotifications += $notifications.Count
            }
        }
        catch {
            Write-Host "  - $customer : Unable to retrieve notifications" -ForegroundColor Red
        }
    }
    Write-Host "  Total notifications sent: $totalNotifications" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "💡 Performance Tips:" -ForegroundColor Cyan
Write-Host "  - Monitor Kafka UI at http://localhost:8080" -ForegroundColor White
Write-Host "  - Check service logs: docker-compose logs [service-name]" -ForegroundColor White
Write-Host "  - Increase DelayMs parameter to reduce load" -ForegroundColor White
Write-Host "  - Monitor Docker resource usage" -ForegroundColor White

Write-Host ""
Write-Host "🎉 Load test completed!" -ForegroundColor Green
