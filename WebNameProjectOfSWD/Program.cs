using BLL.IService;
using BLL.Services;
using BLL.Services.Jwt;
using BLL.Services.MailService;
using DAL.IRepo;
using DAL.Models;
using DAL.Repo;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// ===== Controllers & Swagger =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Gộp còn 1 lần AddSwaggerGen, kèm JWT Bearer
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DollAI Store API", Version = "v1" });
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

builder.Services.AddMemoryCache();

builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddScoped<IDollTypeRepository, DollTypeRepository>();
builder.Services.AddScoped<IDollTypeService, DollTypeService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IDollModelRepository, DollModelRepository>();
builder.Services.AddScoped<IDollModelService, DollModelService>();
builder.Services.AddScoped<IDollVariantRepository, DollVariantRepository>();
builder.Services.AddScoped<IDollVariantService, DollVariantService>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterService, CharacterService>();

// Email sender
builder.Services.AddScoped<BLL.IService.IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IOtpService, OtpService>();


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
