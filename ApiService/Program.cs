using ApiService.Business;
using ApiService.Common;
using ApiService.Core;
using ApiService.Core.RedisHelper.Configurations;
using ApiService.Entity;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//// Đảm bảo ứng dụng lắng nghe cả HTTP và HTTPS
//if (builder.Environment.IsDevelopment())
//{
//    builder.WebHost.UseUrls("http://localhost:40484", "https://localhost:5001", "http://localhost:5000");
//}

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Sử dụng builder.Logging thay vì ConfigureLogging
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddLog4Net(@"log4net.config");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
    options.AddPolicy("AllowAngularClientDeploy", policy =>
    {
        policy.WithOrigins("https://eec-ict-api-netcore.onrender.com/")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(AppSetting)).Get<AppSetting>() ?? new AppSetting());
ConfigurationHelper.Initialize(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // 👈 giữ nguyên tên property
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("AppSetting:Jwt:Issuer").Get<string>(),
        ValidAudience = builder.Configuration.GetSection("AppSetting:Jwt:Issuer").Get<string>(),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSetting:Jwt:Key").Get<string>() ?? string.Empty))
    };
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ServiceFactory>();
builder.Services.AddScoped<MyActionFilter>();
builder.Services.AddMemoryCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// ConfigureContainer được thực hiện trong phần Host.ConfigureContainer 
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    //builder.Register(p => Configuration.GetSection(nameof(RedisClientSetting)).Get<RedisClientSetting>()).SingleInstance();
    //builder.RegisterModule(new RedisClientModule());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<HttpRequestMidleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors("AllowAngularClient"); // Áp dụng chính sách cho localhost
}
else
{
    app.UseCors("AllowAngularClientDeploy"); // Áp dụng chính sách cho frontend đã deploy
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Luôn hiển thị Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Đặt Swagger là trang mặc định
    
    // Hỗ trợ làm việc ở cả môi trường phát triển và sản xuất
    c.OAuthUsePkce();
});

// Thêm dữ liệu vào cache trước khi chạy ứng dụng
app.AddDataToCache();

app.Run();
