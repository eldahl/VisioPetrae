using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services for controllers
builder.Services.AddControllers();

// Register services
builder.Services.AddSingleton<ProfileService>();
builder.Services.AddSingleton<IMongoDBContext, MongoDBContext>();
builder.Services.AddSingleton<JwtService>();

// JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(bearerOpt => {
    
    var jwtConfig = builder.Configuration.GetSection("JWT");
    var issuers = jwtConfig.GetSection("Issuers").Get<string[]>();
    var audiences = jwtConfig.GetSection("Audiences").Get<string[]>();
    var signingKey = jwtConfig["SigningKey"];

    bearerOpt.TokenValidationParameters = new TokenValidationParameters
    {
        // Set proper values for JWT validation
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:JWTSigningKey"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        // Make sure token expires exactly at token expiration time
        ClockSkew = TimeSpan.Zero,
    };
});

// Logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Heartbeat
app.MapGet("/heartbeat", () => "ok");

// Map controllers
app.MapControllers();

app.Run();
