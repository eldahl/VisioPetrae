using inference_registry.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Register the InferenceServerRegistry as a singleton
builder.Services.AddSingleton<InferenceServerRegistry>();

// Logger
builder.Services.AddLogging(logging => {
    logging.AddConsole();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Heartbeat
app.MapGet("/heartbeat", () => "ok");

// CORS
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()); 

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
