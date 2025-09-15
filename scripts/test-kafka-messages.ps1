# Test Kafka Message Production Script
# This script sends test orders to demonstrate Kafka messaging

Write-Host "🚀 Testing Kafka Message Production" -ForegroundColor Green
Write-Host ""

# First, let's check if the services are running
Write-Host "📋 Checking service status..." -ForegroundColor Yellow
$orderServiceHealth = try { 
    $response = Invoke-RestMethod -Uri "http://localhost:5001/health" -Method Get -ErrorAction Stop
    "✅ Order Service is running"
} catch {
    "❌ Order Service is not responding"
}

Write-Host $orderServiceHealth

Write-Host ""
Write-Host "📦 Creating test orders to generate Kafka messages..." -ForegroundColor Yellow

# Create a few test orders
$testOrders = @(
    @{
        customerId = "customer-001"
        customerName = "John Doe"
        items = @(
            @{
                productId = "550e8400-e29b-41d4-a716-446655440000"
                productName = "Laptop"
                quantity = 1
                price = 999.99
            }
        )
    },
    @{
        customerId = "customer-002"
        customerName = "Jane Smith"
        items = @(
            @{
                productId = "550e8400-e29b-41d4-a716-446655440001"
                productName = "Mouse"
                quantity = 2
                price = 25.50
            },
            @{
                productId = "550e8400-e29b-41d4-a716-446655440002"
                productName = "Keyboard"
                quantity = 1
                price = 75.00
            }
        )
    },
    @{
        customerId = "customer-003"
        customerName = "Bob Wilson"
        items = @(
            @{
                productId = "550e8400-e29b-41d4-a716-446655440003"
                productName = "Monitor"
                quantity = 1
                price = 299.99
            }
        )
    }
)

$orderCount = 1
foreach ($order in $testOrders) {
    Write-Host "Creating order $orderCount..." -ForegroundColor Cyan
    
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method Post -Body ($order | ConvertTo-Json -Depth 3) -ContentType "application/json"
        Write-Host "✅ Order created successfully: $($response.id)" -ForegroundColor Green
        
        # Wait a moment between orders
        Start-Sleep -Seconds 1
    }
    catch {
        Write-Host "❌ Failed to create order: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    $orderCount++
}

Write-Host ""
Write-Host "🔍 How to view messages in Kafka UI:" -ForegroundColor Magenta
Write-Host "1. Open your browser and go to: http://localhost:8080" -ForegroundColor White
Write-Host "2. You should see the Kafka UI dashboard" -ForegroundColor White
Write-Host "3. Click on 'Topics' in the sidebar" -ForegroundColor White
Write-Host "4. Look for topics like 'order-created', 'inventory-updated', etc." -ForegroundColor White
Write-Host "5. Click on a topic to see the messages" -ForegroundColor White
Write-Host "6. Click on 'Messages' tab to see the actual message content" -ForegroundColor White
Write-Host ""
Write-Host "📊 Available topics should include:" -ForegroundColor Yellow
Write-Host "- order-created: Messages when new orders are created" -ForegroundColor White
Write-Host "- inventory-updated: Messages when inventory changes" -ForegroundColor White
Write-Host "- notifications: Messages for user notifications" -ForegroundColor White
Write-Host ""
Write-Host "✨ Open Kafka UI now: http://localhost:8080" -ForegroundColor Green
