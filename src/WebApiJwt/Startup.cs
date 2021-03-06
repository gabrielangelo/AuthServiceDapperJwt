﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using DOMAIN;
using DAL;
using System.Security.Claims;

namespace WebApiJwt
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
            // ===== Add our DbContext ========
            //services.AddDbContext<ApplicationDbContext>();
            services.AddTransient<UsersDAL>();
            // ===== Add Jwt Authentication ========
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"]))
                    };                
                });

            services.AddAuthorization(options =>
                {
                    options.AddPolicy("IsRefreshToken", policy =>
                        policy.RequireAssertion(context =>
                            context.User.HasClaim(c =>
                                (c.Type == ClaimTypes.AuthorizationDecision && c.Value == "is_refresh_token"))));
                    
                    options.AddPolicy("IsNotRefreshToken", policy =>
                        policy.RequireAssertion(context =>
                            !context.User.HasClaim(c =>
                                (c.Type == ClaimTypes.AuthorizationDecision && c.Value == "is_refresh_token"))));
                });

            // ===== Add MVC ========
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // ===== Use Authentication ======
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
