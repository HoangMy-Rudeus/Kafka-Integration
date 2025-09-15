# Quick Start Script for Kafka Microservices Demo
# This script helps you get the demo up and running quickly

param(
    [switch]$Infrastructure,
    [switch]$Services,
    [switch]$Stop,
    [switch]$Clean
)

function Show-Help {
    Write-Host "Kafka Microservices Demo - Quick Start" -ForegroundColor Green
    Write-Host "=====================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\start-demo.ps1                  # Start everything (infrastructure + services)"
    Write-Host "  .\start-demo.ps1 -Infrastructure  # Start only Kafka infrastructure"
    Write-Host "  .\start-demo.ps1 -Services        # Start only microservices"
    Write-Host "  .\start-demo.ps1 -Stop            # Stop all services"
    Write-Host "  .\start-demo.ps1 -Clean           # Clean up Docker resources"
    Write-Host ""
    Write-Host "Services:" -ForegroundColor Cyan
    Write-Host "  - Kafka UI:           http://localhost:8080"
    Write-Host "  - Order Service:      http://localhost:5001"
    Write-Host "  - Inventory Service:  http://localhost:5002"
    Write-Host "  - Notification Service: http://localhost:5003"
}

function Start-Infrastructure {
    Write-Host "🚀 Starting Kafka infrastructure..." -ForegroundColor Green
    docker-compose up -d zookeeper kafka kafka-ui
    
    Write-Host "⏳ Waiting for Kafka to be ready..." -ForegroundColor Yellow
    $maxAttempts = 30
    $attempt = 0
    
    do {
        Start-Sleep -Seconds 2
        $attempt++
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:8080" -TimeoutSec 5 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                Write-Host "✅ Kafka infrastructure is ready!" -ForegroundColor Green
                return $true
            }
        }
        catch {
            Write-Host "." -NoNewline -ForegroundColor Yellow
        }
    } while ($attempt -lt $maxAttempts)
    
    Write-Host ""
    Write-Host "❌ Kafka infrastructure failed to start properly" -ForegroundColor Red
    return $false
}

function Start-Services {
    Write-Host "🚀 Starting microservices..." -ForegroundColor Green
    docker-compose up -d order-service inventory-service notification-service
    
    Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
    
    $services = @(
        @{ Name = "Order Service"; Url = "http://localhost:5001/api/orders" },
        @{ Name = "Inventory Service"; Url = "http://localhost:5002/api/inventory" },
        @{ Name = "Notification Service"; Url = "http://localhost:5003/api/notifications/test" }
    )
    
    $allReady = $true
    foreach ($service in $services) {
        try {
            $response = Invoke-RestMethod -Uri $service.Url -Method GET -TimeoutSec 10
            Write-Host "✅ $($service.Name) is ready" -ForegroundColor Green
        }
        catch {
            Write-Host "❌ $($service.Name) failed to start" -ForegroundColor Red
            $allReady = $false
        }
    }
    
    return $allReady
}

function Stop-Services {
    Write-Host "🛑 Stopping all services..." -ForegroundColor Yellow
    docker-compose down
    Write-Host "✅ All services stopped" -ForegroundColor Green
}

function Clean-Resources {
    Write-Host "🧹 Cleaning up Docker resources..." -ForegroundColor Yellow
    docker-compose down -v --remove-orphans
    docker system prune -f
    Write-Host "✅ Cleanup completed" -ForegroundColor Green
}

function Show-Status {
    Write-Host "📊 Service Status:" -ForegroundColor Cyan
    docker-compose ps
    Write-Host ""
    Write-Host "🌐 Access URLs:" -ForegroundColor Cyan
    Write-Host "  - Kafka UI:           http://localhost:8080" -ForegroundColor White
    Write-Host "  - Order Service:      http://localhost:5001" -ForegroundColor White
    Write-Host "  - Inventory Service:  http://localhost:5002" -ForegroundColor White
    Write-Host "  - Notification Service: http://localhost:5003" -ForegroundColor White
}

# Main execution
if ($Stop) {
    Stop-Services
    exit 0
}

if ($Clean) {
    Clean-Resources
    exit 0
}

if ($Infrastructure -and $Services) {
    Write-Host "❌ Cannot use both -Infrastructure and -Services flags together" -ForegroundColor Red
    Show-Help
    exit 1
}

if (-not $Infrastructure -and -not $Services) {
    # Start everything
    Write-Host "🚀 Starting complete Kafka Microservices Demo..." -ForegroundColor Green
    
    if (Start-Infrastructure) {
        if (Start-Services) {
            Write-Host ""
            Write-Host "🎉 Demo is ready!" -ForegroundColor Green
            Show-Status
            Write-Host ""
            Write-Host "💡 Next steps:" -ForegroundColor Cyan
            Write-Host "   1. Run the demo: .\demo-complete-flow.ps1" -ForegroundColor White
            Write-Host "   2. Check Kafka UI: http://localhost:8080" -ForegroundColor White
            Write-Host "   3. View logs: docker-compose logs -f" -ForegroundColor White
        }
    }
} elseif ($Infrastructure) {
    if (Start-Infrastructure) {
        Write-Host ""
        Write-Host "🎉 Kafka infrastructure is ready!" -ForegroundColor Green
        Write-Host "💡 You can now run services locally with 'dotnet run'" -ForegroundColor Cyan
    }
} elseif ($Services) {
    if (Start-Services) {
        Write-Host ""
        Write-Host "🎉 Microservices are ready!" -ForegroundColor Green
        Show-Status
    }
}
