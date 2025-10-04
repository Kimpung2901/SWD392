using BLL.Services.UsersService;
using BLL.Services.Jwt;
using BLL.Services.MailService;
using DAL;
using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== Controllers & Swagger =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Gộp còn 1 lần AddSwaggerGen, kèm JWT Bearer
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ===== DbContext =====
builder.Services.AddDbContext<DollDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== DI services (giữ nguyên của bạn) =====
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddScoped<DollTypeRepository>();
builder.Services.AddScoped<DollTypeService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddSingleton<SmtpEmailSender>();

// ===== AuthN / AuthZ =====
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = JwtTokenService.GetValidationParameters(builder.Configuration);
    });

// Policies cho 3 role: admin/manager/customer
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminOnly", p => p.RequireRole("admin"));
    o.AddPolicy("ManagerOnly", p => p.RequireRole("manager"));
    o.AddPolicy("CustomerOnly", p => p.RequireRole("customer"));
    o.AddPolicy("AdminOrManager", p => p.RequireRole("admin", "manager"));
});

var app = builder.Build();

// ===== Swagger: bật khi Development HOẶC có env var =====
// Hỗ trợ cả "Swagger:Enabled" (appsettings) lẫn "Swagger__Enabled" (env Render)
var swaggerEnabled =
    app.Environment.IsDevelopment()
    || builder.Configuration.GetValue<bool>("Swagger:Enabled")
    || builder.Configuration.GetValue<bool>("Swagger__Enabled");

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===== HTTPS redirect: CHỈ Dev (Render đã TLS ở phía ngoài) =====
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// ===== Route gốc để test/health =====
app.MapGet("/", () => Results.Json(new
{
    ok = true,
    message = "API running",
    env = app.Environment.EnvironmentName,
    swagger = swaggerEnabled
}));

app.MapControllers();

app.Run();
