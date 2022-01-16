using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SampleIdentity.Core.Common.Configurations;
using SampleIdentity.Core.Common.Utilities;
using SampleIdentity.Core.Entities.ApplicationUserAggregate;
using SampleIdentity.Core.Repositories;
using SampleIdentity.Core.Repositories.Base;
using SampleIdentity.Core.Repositories.Interfaces;
using SampleIdentity.Core.Repositories.Interfaces.Base;
using SampleIdentity.Core.Services.Account;
using SampleIdentity.Core.Services.Account.Interfaces;
using SampleIdentity.Infrastructure.Data.Context;
using SampleIdentity.Infrastructure.Data.Repositories;
using SampleIdentity.Infrastructure.Repositories;
using SampleIdentity.WebAPI.Filters;
using SampleIdentity.WebAPI.Providers;
using SampleIdentity.WebAPI.Providers.Interfaces;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

namespace SampleIdentity.WebAPI
{
    /// <summary>
    /// 
    /// </summary>
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            ConfigureDatabase(services);
            ConfigureSwagger(services);
            CofigureAuthentication(services);
            ConfigureIdentityOption(services);
            ConfigureDpContainer(services);
            ConfigureMediatR(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //ConfigureCors(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            UserSwagger(app);

            var enableSwagger = Configuration.GetValue<bool>("EnableSwagger");
            if (enableSwagger)
                app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCaching();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        #region Private Methods

        private void ConfigureSwagger(IServiceCollection services)
        {
            var enableSwagger = Configuration.GetValue<bool>("EnableSwagger");
            if (!enableSwagger)
                return;

            var virtualDirectory = Configuration["VirtualDirectory"];
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SampleIdentity Web API",
                    Version = "v1",
                    //TermsOfService = new Uri("www.SampleIdentity.com")
                });
                c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}\\SampleIdentity.WebApi.xml");
                //c.DescribeAllEnumsAsStrings();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" }
                        }, new List<string>() }
                });

                c.CustomSchemaIds(type => type.ToString());
            });
        }

        private void UserSwagger(IApplicationBuilder app)
        {
            var enableSwagger = Configuration.GetValue<bool>("EnableSwagger");
            if (!enableSwagger)
                return;

            var virtualDirectory = Configuration["VirtualDirectory"];
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(virtualDirectory + "swagger/v1/swagger.json", "SampleIdentity Web API V1");
                c.OAuthClientId("SampleIdentityWebClient");
                c.OAuthClientSecret(HashingUtility.GetHash("123456@abc"));
                c.DocExpansion(DocExpansion.None);
                c.EnableFilter();
                c.DefaultModelRendering(ModelRendering.Model);
                c.DisplayRequestDuration();
                c.ShowExtensions();
                c.InjectStylesheet("../swagger-ui/custom.css");
            });

        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            const string databaseConnectionKeyName = "DatabaseConnection";

            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(databaseConnectionKeyName), s =>
                {
                    s.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                //options.ConfigureWarnings(w => w.Throw(RelationalEventId));
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();
        }

        private void CofigureAuthentication(IServiceCollection services)
        {
            var tokenSecretKey = Configuration["Tokens:SecretKey"];
            var tokenIssuer = Configuration["Tokens:Issuer"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenIssuer,
                    ValidAudience = tokenIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecretKey))
                };

                config.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        private void ConfigureIdentityOption(IServiceCollection services)
        {
            var defaultLockoutMinutes = int.Parse(Configuration["IdentityOptions:DefaultLockoutMinutes"]);
            var maxFailedAccessAttempts = int.Parse(Configuration["IdentityOptions:MaxFailedAccessAttempts"]);

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(defaultLockoutMinutes);
                options.Lockout.MaxFailedAccessAttempts = maxFailedAccessAttempts;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = false;
            });
        }

        private void ConfigureMediatR(IServiceCollection services)
        {
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        }


        private void ConfigureDpContainer(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
            services.AddScoped(typeof(IRepositoryReadonly<>), typeof(RepositoryBaseReadOnly<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IUserAccountService, UserAccountService>();


            services.AddScoped<ITokenGeneratorProvider, TokenGeneratorProvider>();
            services.AddScoped<IOAuthAuthorizationProvider, OAuthAuthorizationProvider>();
            services.AddScoped<ClientAuthenticationFilterAttribute>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
 
            //// Web Configuration
            var config = new ConfigurationsManager();
            Configuration.Bind(config);services.AddSingleton(config);

            //// Add memory cache services
            services.AddMemoryCache();

            // Add Auto Mapper 
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var corsPolicy = Configuration.GetSection("CorsPolicyConfiguration").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins(corsPolicy)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

       

            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

            services.AddMvc().AddJsonOptions(opts =>
            {
                //var enumConverter = new JsonStringEnumConverter();
                //opts.JsonSerializerOptions.Converters.Add(enumConverter);
            });

        }

        #endregion

    }
}
