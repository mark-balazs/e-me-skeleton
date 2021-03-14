﻿using System;
using System.Text;
using System.Threading.Tasks;
using e_me.Model.Repositories;
using e_me.Mvc.Auth;
using e_me.Mvc.Auth.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace e_me.Mvc.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authSettingsSection = configuration.GetSection("AuthSettings");
            services.Configure<AuthSettings>(authSettingsSection);
            var authSettings = authSettingsSection.Get<AuthSettings>();
            var signinSecretKey = Encoding.ASCII.GetBytes(authSettings.SecretKey);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(signinSecretKey),
                        ValidateIssuer = true,
                        ValidIssuer = authSettings.Issuer,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddTransient<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IJwtTokenRepository, JwtTokenRepository>();
            services.AddTransient<IResetPasswordTokenRepository, ResetPasswordTokenRepository>();
            services.AddTransient<ISecurityRoleRepository, SecurityRoleRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserSecurityRoleRepository, UserSecurityRoleRepository>();
            services.AddTransient<IApplicationSettingRepository, ApplicationSettingRepository>();
            services.AddTransient<IUserAvatarRepository, UserAvatarRepository>();
        }
    }
}