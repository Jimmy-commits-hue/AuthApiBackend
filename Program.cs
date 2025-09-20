using AuthApi.BackgroudService;
using AuthApi.Database;
using AuthApi.DTOs;
using AuthApi.Infrastructure;
using AuthApi.Interfaces;
using AuthApi.Repository;
using AuthApi.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using AuthApi.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<GenerateJwtToken>();  
builder.Services.AddSingleton<UserCustomNumberEnrich>();
builder.Services.AddHostedService<CleanUpDatabaseBackgroundService>();
builder.Services.AddHostedService<SendVerificationEmails>();
builder.Services.AddHostedService<CleanUpUnActiveUsers>();

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{

    builder.Host.UseSerilog((context, service, config) =>
    {

        config.ReadFrom.Configuration(context.Configuration).
        Enrich.FromLogContext().WriteTo.Console().
        Enrich.With(service.GetRequiredService<UserCustomNumberEnrich>()).
        WriteTo.Seq(context.Configuration["Serilog:serverUrl"] ?? "http://localhost:5341");

    });

    Log.Information("Application is starting up");

    Env.Load();
    string Password = Environment.GetEnvironmentVariable("DB_PASSWORD")!;
    string password = Password.Split('\\')[1];

    string connectionString = $"Server={builder.Configuration["connectionString:Server"]};Port={builder.Configuration["connectionString:Port"]};User={builder.
        Configuration["connectionString:User"]};Database={builder.Configuration["connectionString:Database"]};Password={password};";

    builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options =>
     {

         options.RequireHttpsMetadata = false;
         options.SaveToken = true;
         options.TokenValidationParameters = new TokenValidationParameters
         {

             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidAudience = builder.Configuration["JWT:Audience"],
             ValidIssuer = builder.Configuration["JWT:Issuer"],
             IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Environment.GetEnvironmentVariable("JWT_KEY")!))

         };

         options.Events = new JwtBearerEvents
         {

             OnAuthenticationFailed = context =>
             {

                 Console.WriteLine($"Authentication failed: {context.Exception.Message}");

                 return Task.CompletedTask;

             },

             OnMessageReceived = context =>
             {

                 if (context.HttpContext.Request.Cookies.ContainsKey("accessToken"))
                 {

                     context.Token = context.HttpContext.Request.Cookies["accessToken"];

                 }
                 

                 return Task.CompletedTask;

             },

             OnTokenValidated = context =>
             {

                 Console.WriteLine("Token validated successfully");

                 return Task.CompletedTask;

             },

             OnForbidden = async context =>
             {

                 context.Response.StatusCode = 401;
                 context.Response.ContentType = "application/json";
                 await context.Response.WriteAsync("{\"message\": \"You are not authorized to access this resource.\"}");

             }

         };

     });

 
    builder.Services.AddDbContext<AuthApiDbContext>(options =>
    {

        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

    });

    builder.Services.AddScoped<IEmailVerifRepo, EmaiLVerificationRepo>();
    builder.Services.AddScoped<ICustomNumberRepo, CustomerNumberRepo>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEmailService, EmailVerificationService>();
    builder.Services.AddScoped<ICustomNumberService, GenerateCustomerNumber>();
    builder.Services.AddTransient<ISendEmailService, SendEmailService>();
    builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
    builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    builder.Services.AddScoped<GenerateJwtToken>();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("CookieAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Name = "accessToken",
            In = Microsoft.OpenApi.Models.ParameterLocation.Cookie,
            Description = "JWT stored in the accessToken cookie"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "CookieAuth"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddAuthorization();
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {

        app.UseSwagger();
        app.UseSwaggerUI();

    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{

    Log.Fatal(ex, "Application failed to start");

}
finally
{

    Log.CloseAndFlush();

}