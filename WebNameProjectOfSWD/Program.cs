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

var builder = WebApplication.CreateBuilder(args);

// ===== Firebase Initialization =====
try
{
    var firebaseCredPath = builder.Configuration["Firebase:CredentialPath"] ?? builder.Configuration["Firebase__CredentialPath"] ?? "firebase-adminsdk.json";
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
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Firebase initialization failed: {ex.Message}");
}

// ===== Controllers & Swagger =====
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
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
});

// ===== DbContext =====
builder.Services.AddDbContext<DollDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== DI services =====
builder.Services.AddMemoryCache();
builder.Services.AddScoped<JwtTokenService>();

// ✅ Thêm AutoMapper
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
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
// Thêm vào phần DI registration
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

// Options
builder.Services.Configure<PaymentRootOptions>(builder.Configuration.GetSection("Payment"));

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
});

var app = builder.Build();

// ===== Swagger - Luôn bật =====
var enableSwagger = app.Environment.IsDevelopment()
    || string.Equals(Environment.GetEnvironmentVariable("ENABLE_SWAGGER"), "true", StringComparison.OrdinalIgnoreCase);

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect "/" về Swagger để có cái hiển thị
app.MapGet("/", () => Results.Redirect("/swagger"));

// Chỉ redirect HTTPS trong Development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
