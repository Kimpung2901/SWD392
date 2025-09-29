using System.Security.Claims;
using System.Text;
using BLL.Services;
using BLL.Services.Jwt;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace WebNameProjectOfSWD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===== DbContext =====
            builder.Services.AddDbContext<DollDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ===== DI =====
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddScoped<SmtpEmailSender>();

            // nếu UserService cần repo, đăng ký ở đây (ví dụ):
            // builder.Services.AddScoped<UserRepository>();

            // ===== Controllers =====
            builder.Services.AddControllers();

            // ===== Swagger (Bearer button) =====
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebNameProjectOfSWD API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập: Bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Nếu lỡ có 2 action trùng route, chọn cái đầu để Swagger không crash
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            // ===== JWT AuthN/AuthZ =====
            var jwt = builder.Configuration.GetSection("Jwt")
                      ?? throw new Exception("Missing 'Jwt' section in appsettings.json");
            var key = Encoding.UTF8.GetBytes(jwt["Key"] ?? throw new Exception("Missing 'Jwt:Key'"));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero,

                        // Rất quan trọng để [Authorize] map đúng Name/Role từ claims bạn phát
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role
                    };

                    opt.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            Console.WriteLine("[JWT] AuthFailed: " + ctx.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnChallenge = ctx =>
                        {
                            Console.WriteLine("[JWT] Challenge: " + ctx.Error + " / " + ctx.ErrorDescription);
                            return Task.CompletedTask;
                        }
                    };
                });

            // ===== Email (SMTP) =====
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Smtp"));

            builder.Services.AddAuthorization();

            // ===== CORS =====
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // đổi port nếu frontend chạy khác
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5122); // chỉ HTTP
                                           // Nếu cần HTTPS thì uncomment:
                                           // options.ListenAnyIP(7122, listenOptions => listenOptions.UseHttps());
            });

            var app = builder.Build();

            // ===== Pipeline =====
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend"); // 👈 thêm vào pipeline

            app.UseAuthentication();   // phải trước UseAuthorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
