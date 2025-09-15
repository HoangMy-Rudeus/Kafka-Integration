using KafkaMicroservices.NotificationService.Data;
using KafkaMicroservices.NotificationService.Services;
using KafkaMicroservices.Shared.Configuration;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Notification Service API", 
        Version = "v1",
        Description = "Microservice for managing notifications in Kafka demo with PostgreSQL"
    });
});

// Configure PostgreSQL
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Kafka settings
var kafkaSettings = new KafkaSettings();
builder.Configuration.GetSection("Kafka").Bind(kafkaSettings);
builder.Services.AddSingleton(kafkaSettings);

// Register services
builder.Services.AddScoped<INotificationService, KafkaMicroservices.NotificationService.Services.NotificationService>(); // Changed back to Scoped for EF
builder.Services.AddSingleton<IKafkaConsumer<BaseEvent>, KafkaConsumerService>();
builder.Services.AddHostedService<KafkaMicroservices.NotificationService.Services.EventHandler>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    await context.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
