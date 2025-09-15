# Debugging Helper Script for Kafka Microservices
# This script provides debugging utilities and health checks

param(
    [switch]$Health,
    [switch]$Logs,
    [switch]$Kafka,
    [switch]$Topics,
    [switch]$Reset,
    [string]$Service = ""
)

function Show-Help {
    Write-Host "Kafka Microservices Debug Helper" -ForegroundColor Green
    Write-Host "===============================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\debug-helper.ps1 -Health           # Check health of all services"
    Write-Host "  .\debug-helper.ps1 -Logs             # Show logs for all services"
    Write-Host "  .\debug-helper.ps1 -Logs -Service order-service # Show logs for specific service"
    Write-Host "  .\debug-helper.ps1 -Kafka            # Show Kafka status and topics"
    Write-Host "  .\debug-helper.ps1 -Topics           # List and describe Kafka topics"
    Write-Host "  .\debug-helper.ps1 -Reset            # Reset all services and data"
    Write-Host ""
}

function Check-Health {
    Write-Host "🏥 Health Check Results:" -ForegroundColor Cyan
    Write-Host "======================" -ForegroundColor Cyan
    
    # Check Docker containers
    Write-Host "`n📦 Docker Containers:" -ForegroundColor Yellow
    docker-compose ps
    
    # Check service endpoints
    Write-Host "`n🌐 Service Endpoints:" -ForegroundColor Yellow
    
    $services = @(
        @{ Name = "Order Service"; Url = "http://localhost:5001/api/orders"; Port = 5001 },
        @{ Name = "Inventory Service"; Url = "http://localhost:5002/api/inventory"; Port = 5002 },
        @{ Name = "Notification Service"; Url = "http://localhost:5003/api/notifications/test"; Port = 5003 },
        @{ Name = "Kafka UI"; Url = "http://localhost:8080"; Port = 8080 }
    )
    
    foreach ($service in $services) {
        try {
            $response = Invoke-RestMethod -Uri $service.Url -Method GET -TimeoutSec 5
            Write-Host "  ✅ $($service.Name) - Healthy" -ForegroundColor Green
        }
        catch {
            Write-Host "  ❌ $($service.Name) - Not responding" -ForegroundColor Red
            
            # Check if port is in use
            $portInUse = netstat -an | Select-String ":$($service.Port)"
            if ($portInUse) {
                Write-Host "     Port $($service.Port) is in use" -ForegroundColor Yellow
            } else {
                Write-Host "     Port $($service.Port) is not in use" -ForegroundColor Red
            }
        }
    }
    
    # Check Kafka connectivity
    Write-Host "`n📊 Kafka Status:" -ForegroundColor Yellow
    try {
        $kafkaTopics = docker exec kafka kafka-topics --bootstrap-server localhost:9092 --list 2>$null
        if ($kafkaTopics) {
            Write-Host "  ✅ Kafka broker is accessible" -ForegroundColor Green
            Write-Host "  📋 Available topics: $($kafkaTopics.Count)" -ForegroundColor White
        }
    }
    catch {
        Write-Host "  ❌ Kafka broker is not accessible" -ForegroundColor Red
    }
}

function Show-Logs {
    if ($Service) {
        Write-Host "📝 Logs for $Service :" -ForegroundColor Cyan
        docker-compose logs --tail=50 $Service
    } else {
        Write-Host "📝 Recent logs for all services:" -ForegroundColor Cyan
        Write-Host "===============================" -ForegroundColor Cyan
        
        $services = @("order-service", "inventory-service", "notification-service", "kafka")
        
        foreach ($svc in $services) {
            Write-Host "`n--- $svc ---" -ForegroundColor Yellow
            docker-compose logs --tail=10 $svc
        }
    }
}

function Show-KafkaStatus {
    Write-Host "📊 Kafka Cluster Status:" -ForegroundColor Cyan
    Write-Host "========================" -ForegroundColor Cyan
    
    try {
        # Check if Kafka container is running
        $kafkaContainer = docker ps --filter "name=kafka" --format "table {{.Names}}\t{{.Status}}"
        Write-Host "`n📦 Kafka Container:" -ForegroundColor Yellow
        Write-Host $kafkaContainer
        
        # List topics
        Write-Host "`n📋 Kafka Topics:" -ForegroundColor Yellow
        $topics = docker exec kafka kafka-topics --bootstrap-server localhost:9092 --list 2>$null
        if ($topics) {
            $topics | ForEach-Object {
                Write-Host "  - $_" -ForegroundColor White
            }
        } else {
            Write-Host "  No topics found or Kafka not accessible" -ForegroundColor Red
        }
        
        # Show consumer groups
        Write-Host "`n👥 Consumer Groups:" -ForegroundColor Yellow
        $consumerGroups = docker exec kafka kafka-consumer-groups --bootstrap-server localhost:9092 --list 2>$null
        if ($consumerGroups) {
            $consumerGroups | ForEach-Object {
                Write-Host "  - $_" -ForegroundColor White
            }
        } else {
            Write-Host "  No consumer groups found" -ForegroundColor Gray
        }
        
    }
    catch {
        Write-Host "❌ Failed to get Kafka status: $($_.Exception.Message)" -ForegroundColor Red
    }
}

function Show-Topics {
    Write-Host "📋 Kafka Topics Details:" -ForegroundColor Cyan
    Write-Host "========================" -ForegroundColor Cyan
    
    try {
        $topics = docker exec kafka kafka-topics --bootstrap-server localhost:9092 --list 2>$null
        
        if ($topics) {
            foreach ($topic in $topics) {
                Write-Host "`n📄 Topic: $topic" -ForegroundColor Yellow
                $topicDetails = docker exec kafka kafka-topics --bootstrap-server localhost:9092 --describe --topic $topic 2>$null
                Write-Host $topicDetails -ForegroundColor White
                
                # Show recent messages (first 5)
                Write-Host "`n💬 Recent messages (last 5):" -ForegroundColor Gray
                $messages = docker exec kafka timeout 5 kafka-console-consumer --bootstrap-server localhost:9092 --topic $topic --from-beginning --max-messages 5 2>$null
                if ($messages) {
                    $messages | ForEach-Object {
                        Write-Host "  $($_)" -ForegroundColor White
                    }
                } else {
                    Write-Host "  No messages or timeout reached" -ForegroundColor Gray
                }
            }
        } else {
            Write-Host "No topics found" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "❌ Failed to get topic details: $($_.Exception.Message)" -ForegroundColor Red
    }
}

function Reset-Environment {
    Write-Host "🔄 Resetting Kafka Microservices Environment..." -ForegroundColor Yellow
    Write-Host "===============================================" -ForegroundColor Yellow
    
    Write-Host "`n🛑 Stopping all services..." -ForegroundColor Red
    docker-compose down -v
    
    Write-Host "`n🧹 Cleaning Docker resources..." -ForegroundColor Yellow
    docker system prune -f
    
    Write-Host "`n🚀 Starting fresh environment..." -ForegroundColor Green
    docker-compose up -d
    
    Write-Host "`n⏳ Waiting for services to be ready..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    Write-Host "`n✅ Environment reset complete!" -ForegroundColor Green
    Write-Host "Run health check to verify: .\debug-helper.ps1 -Health" -ForegroundColor Cyan
}

# Main execution
if (-not $Health -and -not $Logs -and -not $Kafka -and -not $Topics -and -not $Reset) {
    Show-Help
    exit 0
}

if ($Health) {
    Check-Health
}

if ($Logs) {
    Show-Logs
}

if ($Kafka) {
    Show-KafkaStatus
}

if ($Topics) {
    Show-Topics
}

if ($Reset) {
    $confirmation = Read-Host "This will reset all data and restart services. Continue? (y/N)"
    if ($confirmation -eq 'y' -or $confirmation -eq 'Y') {
        Reset-Environment
    } else {
        Write-Host "Reset cancelled." -ForegroundColor Yellow
    }
}
