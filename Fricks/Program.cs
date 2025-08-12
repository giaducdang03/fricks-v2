using AspNetCoreRateLimit;
using FirebaseAdmin;
using Fricks;
using Fricks.Middlewares;
using Fricks.Repository.Entities;
using Fricks.Service.Settings;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Brute Force Settings
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

// Add Mail Settings
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSettings"));

// Add Vnpay Settings
builder.Services.Configure<VnpaySetting>(builder.Configuration.GetSection("Vnpay"));

// Add PayOS Settings
using StreamReader reader = new("exe201-8080a-payos.json");
var json = reader.ReadToEnd();
PayOSSetting payos = JsonConvert.DeserializeObject<PayOSSetting>(json);
builder.Services.Configure<PayOSSetting>(options =>
{
    options.ClientId = payos.ClientId;
    options.ApiKey = payos.ApiKey;
    options.ChecksumKey = payos.ChecksumKey;
    builder.Configuration.GetSection("PayOSSettings").Bind(options);
});

// Add AutomMapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(AutoMapperSetting).Assembly);

// Add Dependency Injection
builder.Services.AddWebAPIService();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fricks API", Version = "v.10.24" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token!",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("app-cors",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .WithExposedHeaders("X-Pagination")
            .AllowAnyMethod();
        });
});

// add DBContext

// ===================== FOR LOCAL DB =======================

builder.Services.AddDbContext<FricksContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("FricksVps"));
});

// ==========================================================



// ===================== FOR AZURE DB =======================

//var connection = String.Empty;
//if (builder.Environment.IsDevelopment())
//{
//    connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
//}
//else
//{
//    connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
//}

//builder.Services.AddDbContext<FricksContext>(options =>
// options.UseSqlServer(connection));

// ==================== NO EDIT OR REMOVE COMMENT =======================

// setup firebase
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("exe201-9459a-firebase-adminsdk-bryk3-482b1ccba6.json")
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseStatusCodePages();

app.UseExceptionHandler();

app.UseMiddleware<PerformanceMiddleware>();

app.UseHttpsRedirection();

app.UseCors("app-cors");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
