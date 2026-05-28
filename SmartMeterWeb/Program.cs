
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using Serilog;
using SmartMeterWeb.Configs;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Middlewares;
using SmartMeterWeb.Services;
using SmartMeterWeb.Validators;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;



namespace SmartMeterWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //  Setup Serilog logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "Logs/smartmeter-log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7, // Keep 7 days of logs
                    shared: true)
                .CreateLogger();
            builder.Host.UseSerilog();

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IMailService, MailService>();

            // Add services to the container.
            builder.Services.AddScoped<ITariffService, TariffService>();
            builder.Services.AddScoped<IMonthlyTariffReportService, MonthlyTariffReportService>();
            builder.Services.AddScoped<IConsumerTariffService, ConsumerTariffService>();

            builder.Services.AddScoped<ICustomerCareService, CustomerCareService>();


            builder.Services.AddScoped<ILogService, LogService>();

            builder.Services.AddScoped<IUserReportService, UserReportService>();

            builder.Services.AddScoped<IPdfService, PdfService>();



            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IConsumerService, ConsumerService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();


            builder.Services.AddHostedService<RabbitmqReadingService>();

            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
            });

            builder.Services.AddControllers();


            builder.Services.AddValidatorsFromAssemblyContaining<CustomerCareDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<HistoricalConsumptionRequestValidator>();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            builder.Services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                var jwt = builder.Configuration.GetSection("Jwt");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]))
                };
            });

            

            builder.Services.AddAuthorization();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT EF Demo", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer <token>'"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();
            builder.Services.AddScoped<IBillingService, BillingService>();

            

            builder.Services.AddHttpContextAccessor();
            QuestPDF.Settings.License = LicenseType.Community;

            var app = builder.Build();
            app.UseDeveloperExceptionPage();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
