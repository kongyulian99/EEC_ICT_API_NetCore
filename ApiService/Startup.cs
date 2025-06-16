using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiService.Entity;
using ApiService.Business;
using ApiService.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiService.Common;
using Autofac;
using ApiService.Core.RedisHelper.Configurations;
using Microsoft.OpenApi.Models;

namespace ApiService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
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

            services.AddSingleton(Configuration.GetSection(nameof(AppSetting)).Get<AppSetting>());            
            ConfigurationHelper.Initialize(Configuration);
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null; // 👈 giữ nguyên tên property
            });

            services.AddAuthentication(option=> {
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
                     ValidIssuer = Configuration.GetSection("AppSetting:Jwt:Issuer").Get<string>(),
                     ValidAudience = Configuration.GetSection("AppSetting:Jwt:Issuer").Get<string>(),
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSetting:Jwt:Key").Get<string>()))                     
                 };
             });

            services.AddScoped<AuthService>();
            services.AddScoped<ServiceFactory>();
            services.AddScoped<MyActionFilter>();
            services.AddMemoryCache();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.Register(p => Configuration.GetSection(nameof(RedisClientSetting)).Get<RedisClientSetting>()).SingleInstance();
            //builder.RegisterModule(new RedisClientModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<HttpRequestMidleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
            }

            if (env.IsDevelopment()) // Nếu đang ở môi trường Development
            {
                app.UseCors("AllowAngularClient"); // Áp dụng chính sách cho localhost
            }
            else // Môi trường Production (trên Render.com)
            {
                app.UseCors("AllowAngularClientDeploy"); // Áp dụng chính sách cho frontend đã deploy
            }
            //app.UseCors("AllowAngularClient");

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            if (Configuration.GetSection("AppSetting:SwaggerAllow").Get<bool>())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
                });
            }
        }
    }
}
