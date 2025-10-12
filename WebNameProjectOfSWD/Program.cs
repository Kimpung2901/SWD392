using BLL.Helper;
using BLL.IService;
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

var builder = WebApplication.CreateBuilder(args);

// ===== Firebase Initialization =====
try
{
    var firebaseCredPath = builder.Configuration["Firebase:CredentialPath"] ?? "firebase-adminsdk.json";
    var fullPath = Path.Combine(builder.Environment.ContentRootPath, firebaseCredPath);
    
    if (File.Exists(fullPath))
    {
        var firebaseApp = FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(fullPath)
        });
        Console.WriteLine("✅ Firebase initialized successfully");
    }
    else
    {
        Console.WriteLine($"⚠️ Warning: Firebase credential file not found at: {fullPath}");
        Console.WriteLine("⚠️ Google Login will not work without Firebase credentials");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Firebase initialization failed: {ex.Message}");
    Console.WriteLine("⚠️ Google Login will not work");
}

// ===== Controllers & Swagger =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

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

// ===== DI services =====

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
builder.Services.AddScoped<ICharacterOrderRepository, CharacterOrderRepository>();
builder.Services.AddScoped<ICharacterOrderService, CharacterOrderService>();

// CharacterPackage - THÊM 2 DÒNG NÀY
builder.Services.AddScoped<ICharacterPackageRepository, CharacterPackageRepository>();
builder.Services.AddScoped<ICharacterPackageService, CharacterPackageService>();

// Payment
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Payment Providers
builder.Services.AddScoped<IPaymentProvider, MoMoProvider>();
builder.Services.AddScoped<IPaymentProvider, VNPayProvider>();

// Payment Options
builder.Services.Configure<PaymentRootOptions>(builder.Configuration.GetSection("Payments"));

// Email sender
builder.Services.AddScoped<BLL.IService.IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IOtpService, OtpService>();

// ===== AuthN / AuthZ =====
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = JwtTokenService.GetValidationParameters(builder.Configuration);
    });

// Policies
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

// ===== HTTPS redirect: CHỈ Dev =====
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// ===== Route gốc =====
app.MapGet("/", () => Results.Json(new
{
    ok = true,
    message = "API running",
    env = app.Environment.EnvironmentName,
    swagger = swaggerEnabled
}));

app.MapControllers();

app.Run();
