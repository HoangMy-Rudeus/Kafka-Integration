using KafkaMicroservices.NotificationService.Services;
using KafkaMicroservices.Shared.Configuration;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Kafka settings
var kafkaSettings = new KafkaSettings();
builder.Configuration.GetSection("Kafka").Bind(kafkaSettings);
builder.Services.AddSingleton(kafkaSettings);

// Register services
builder.Services.AddScoped<INotificationService, KafkaMicroservices.NotificationService.Services.NotificationService>();
builder.Services.AddSingleton<IKafkaConsumer<BaseEvent>, KafkaConsumerService>();
builder.Services.AddHostedService<KafkaMicroservices.NotificationService.Services.EventHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
