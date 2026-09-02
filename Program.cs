using System.Text;
using System.Text.Json.Serialization;
using cron_que.Data;
using cron_que.hubs;
using cron_que.Service;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<ItemsService>();
builder.Services.AddScoped<LikesService>();
builder.Services.AddScoped<NotificationsService>();
builder.Services.AddScoped<TokenService>();

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
              
});

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(connectionString)));

var queueOptions = new BackgroundJobServerOptions
{
    Queues = new[]
    {
        "general-queue",
        "very-fast-queue",
        "a-long-running-queue"
    }
};

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHangfireServer(queueOptions);
app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseCors("FrontendDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/hubs/notifications");

app.MapControllers();

app.Run();
