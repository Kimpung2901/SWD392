using BLL.Helper;
using BLL.IService;
using BLL.Options;
using BLL.Services;
using BLL.Services.Jwt;
using BLL.Services.MailService;
using BLL.Services.UsersService;
using DAL.IRepo;
using DAL.Models;
using DAL.Repo;
using DAL.Repositories;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;

var builder = WebApplication.CreateBuilder(args);

// 1. Thêm d?ch v? CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173",
           "https://doll-sales-system-fe.vercel.app",
           "http://localhost:5174"
           )
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Firebase is ready for push notifications
try
{
    GoogleCredential? credential = null;

    // Ưu tiên: đọc từ biến môi trường / App Settings (dùng cho Azure)
    var firebaseJson = builder.Configuration["FIREBASE_CREDENTIALS"];

    if (!string.IsNullOrEmpty(firebaseJson))
    {
        credential = GoogleCredential.FromJson(firebaseJson);
        Console.WriteLine("✅ Firebase: using credentials from FIREBASE_CREDENTIALS.");
    }
    else
    {
        // Fallback: đọc từ file local (dùng khi dev trên máy)
        var firebaseCredPath =
            builder.Configuration["Firebase:CredentialPath"]
            ?? builder.Configuration["Firebase__CredentialPath"]
            ?? Path.Combine("Configs", "firebase-adminsdk.json");

        var fullPath = Path.IsPathRooted(firebaseCredPath)
            ? firebaseCredPath
            : Path.Combine(builder.Environment.ContentRootPath, firebaseCredPath);

        if (File.Exists(fullPath))
        {
            credential = GoogleCredential.FromFile(fullPath);
            Console.WriteLine("✅ Firebase: using local file " + fullPath);
        }
        else
        {
            Console.WriteLine("⚠️ Firebase credential not found. File missing at: " + fullPath);
        }
    }

    if (credential != null && FirebaseApp.DefaultInstance == null)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = credential
        });

        Console.WriteLine("🚀 Firebase initialized successfully.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("❌ Firebase initialization failed: " + ex);
    // Không throw để API vẫn chạy bình thường
}

// ===== Controllers & Swagger =====
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // ? THÊM: C?u hình DateTime format t? d?ng
        // M?c d?nh .NET 8 dã serialize DateTime theo ISO 8601
        // Ch? c?n d?m b?o dùng DateTime.UtcNow trong code
    });

builder.Services.AddEndpointsApiExplorer();

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

    c.TagActionsBy(api =>
    {
        if (api.ActionDescriptor is ControllerActionDescriptor cad)
        {
            if (string.Equals(cad.ControllerName, "Order", StringComparison.OrdinalIgnoreCase))
            {
                return new[] { "Doll Order" };
            }

            return new[] { cad.ControllerName };
        }

        return new[] { api.GroupName ?? api.RelativePath ?? "Endpoints" };
    });
});

// ===== DbContext =====
builder.Services.AddDbContext<DollDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== DI services =====
builder.Services.AddMemoryCache();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ? Thêm AutoMapper
builder.Services.AddAutoMapper(typeof(Mapping));

// Doll services
builder.Services.AddScoped<IDollTypeRepository, DollTypeRepository>();
builder.Services.AddScoped<IDollTypeService, DollTypeService>();
builder.Services.AddScoped<IDollModelRepository, DollModelRepository>();
builder.Services.AddScoped<IDollModelService, DollModelService>();
builder.Services.AddScoped<IDollVariantRepository, DollVariantRepository>();
builder.Services.AddScoped<IDollVariantService, DollVariantService>();

// User & Auth services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Character services
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
// Character Package
builder.Services.AddScoped<ICharacterPackageRepository, CharacterPackageRepository>();
builder.Services.AddScoped<ICharacterPackageService, CharacterPackageService>();

// Order services
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
// Thêm vào ph?n DI registration
builder.Services.AddScoped<IOwnedDollRepository, OwnedDollRepository>();
builder.Services.AddScoped<IOwnedDollService, OwnedDollService>();
// UserCharacter services
builder.Services.AddScoped<IUserCharacterRepository, UserCharacterRepository>();
builder.Services.AddScoped<IUserCharacterService, UserCharacterService>();

// CharacterOrder services
builder.Services.AddScoped<ICharacterOrderRepository, CharacterOrderRepository>();
builder.Services.AddScoped<ICharacterOrderService, CharacterOrderService>();

// DollCharacterLink services
builder.Services.AddScoped<IDollCharacterLinkRepository, DollCharacterLinkRepository>();
builder.Services.AddScoped<IDollCharacterLinkService, DollCharacterLinkService>();

// MoMo Payment Services
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();

// Options
builder.Services.Configure<PaymentRootOptions>(builder.Configuration.GetSection("Payment"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Smtp"));

builder.Services.AddHttpClient<IPaymentProvider, MoMoProvider>();

// Email sender
builder.Services.AddScoped<BLL.IService.IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IOtpService, OtpService>();

// ===== AuthN / AuthZ =====
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = JwtTokenService.GetValidationParameters(builder.Configuration);
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminOnly", p => p.RequireRole("admin"));
    o.AddPolicy("ManagerOnly", p => p.RequireRole("manager"));
    o.AddPolicy("CustomerOnly", p => p.RequireRole("customer"));
    o.AddPolicy("AdminOrManager", p => p.RequireRole("admin", "manager"));
    o.AddPolicy("AdminOrCustomer", p => p.RequireRole("admin", "customer"));
});

var app = builder.Build();

// ===== Swagger - Luôn b?t =====
var enableSwagger = app.Environment.IsDevelopment()
    || string.Equals(Environment.GetEnvironmentVariable("ENABLE_SWAGGER"), "true", StringComparison.OrdinalIgnoreCase);

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect "/" v? Swagger d? có cái hi?n th?
app.MapGet("/", () => Results.Redirect("/swagger"));

// Ch? redirect HTTPS trong Development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// 2. S? d?ng CORS middleware (d?t tru?c UseAuthorization)
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();











