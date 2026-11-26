
using Backend;
using Backend.BLL;
using Backend.DAL;
using Backend.MODEL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Backend.Service;
using Backend.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký Controller và OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 2. Đọc Connection String
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Không tìm thấy ConnectionStrings:DefaultConnection.");
}

// 3. Đăng ký DbContext
builder.Services.AddDbContext<connect>(options =>
    options.UseSqlServer(connectionString));

// 4. Đăng ký DAL
builder.Services.AddScoped<AccountDAL>();
builder.Services.AddScoped<RoleDAL>();
builder.Services.AddScoped<UserDAL>();
builder.Services.AddScoped<PasswordResetTokenDAL>();

// 5. Đăng ký BLL
builder.Services.AddScoped<AccountBLL>();
builder.Services.AddScoped<RoleBLL>();
builder.Services.AddScoped<UserBLL>();
builder.Services.AddScoped<PasswordResetTokenBLL>();

// 6. Đăng ký PasswordHasher
builder.Services.AddScoped<
    IPasswordHasher<Account>,
    PasswordHasher<Account>>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

builder.Services.AddScoped<IEmailService, EmailService>();


// Đọc cấu hình JWT
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

// Đăng ký JwtService
builder.Services.AddScoped<JwtService>();

// Cấu hình xác thực JWT
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

if (jwtSettings == null ||
    string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    throw new InvalidOperationException(
        "Chưa cấu hình JwtSettings."
    );
}

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme
    )
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                ),

                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateLifetime = true,

                ClockSkew = TimeSpan.FromSeconds(30),

                NameClaimType = ClaimTypes.Email,
                RoleClaimType = ClaimTypes.Role
            };
    });


// 7. Cấu hình CORS cho ReactJS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// jwt


builder.Services.AddScoped<EmailVerificationService>();
builder.Services.AddScoped<OtpService>();
var app = builder.Build();
// 8. Cấu hình môi trường Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 9. Middleware
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("ReactFrontend");

// Khi bổ sung JWT, đặt UseAuthentication()
// trước UseAuthorization().

app.UseAuthorization();

app.MapControllers();

app.Run();
