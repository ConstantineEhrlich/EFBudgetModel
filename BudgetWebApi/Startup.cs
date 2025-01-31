using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BudgetModel.Models;
using BudgetServices;
using BudgetServices.Cache;
using BudgetServices.Reports;

using BudgetWebApi.Sockets;
using BudgetWebApi.UserUpdateConsumer;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace BudgetWebApi;

public class Startup
{
    public IConfiguration Configuration { get; init; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Configuration["JWT_KEY"] = Environment.GetEnvironmentVariable("JWT_KEY") ??
                                   throw new KeyNotFoundException("JWT_KEY variable not set");
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Cors allow requesting the API from the domain that is different than API domain
        services.AddCors(options =>
        {
            options.AddPolicy("DynamicCorsPolicy", builder =>
            {
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(_ => true); // allow any origin
            });
        });
        

        
        services.AddControllers(opts => opts.Filters.Add<Controllers.ExceptionFilter>());
        
        // Data access services
        services.AddDbContext<BudgetModel.Context>(options =>
            options.UseNpgsql(BudgetModel.Context.GetPostgresConnectionString()));
        
        // Redis caching service
        ConfigurationOptions redisOpts = ConfigurationOptions.Parse(Configuration.GetSection("RedisSettings")["Hostname"]);
        redisOpts.Password = Environment.GetEnvironmentVariable("REDIS_PASSWORD");
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOpts.ToString()));

        services.AddScoped<UserService>();
        services.AddScoped<ICacheService<User>, RedisCacheService<User>>();
        services.AddScoped<BudgetFileService>();
        services.AddScoped<ICacheService<BudgetFile>, RedisCacheService<BudgetFile>>();
        services.AddScoped<CategoryService>();
        services.AddScoped<TransactionService>();
        
        services.AddScoped<IReportFactory, ReportFactory>();
        
        // User creation notification consumer
        services.Configure<UpdateConsumerSettings>(Configuration.GetSection("UpdateConsumer"));
        services.AddHostedService<UpdateConsumer>();        
            
            
        // Read JWT key from the config
        string jwtKey = Configuration["JWT_KEY"];
        byte[] byteKey = Encoding.ASCII.GetBytes(jwtKey);
        
        // Authentication service
        services.AddAuthentication(options =>
        {
            // Set the default scheme for authentication to JwtBearer
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
            {
                // Require authentication through https
                // disabled since the app runs in container
                options.RequireHttpsMetadata = false;
                // Save the token for later use in the request pipeline
                options.SaveToken = true;
                // Set up the token validation parameters
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    // Resolve the jwtKey that is stored in the config
                    IssuerSigningKey = new SymmetricSecurityKey(byteKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                // Custom event handling for the JwtBearer middleware
                options.Events = new JwtBearerEvents
                {
                    // This event is invoked when the middleware receives a message (in this case, an HTTP request)
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("access_token", out string? token))
                        {
                            // If the token is found in the cookie, use it for authentication
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
                
                
                
            });
        
        
        // Password hasher
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        // Updates socket manager
        services.AddSingleton<BudgetUpdateManager>();
        // Socket cleanup service - runs in background
        services.AddHostedService<CleanupService<BudgetUpdateManager>>();
        
        services.AddSwaggerGen();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseCors("DynamicCorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API V1");
                c.RoutePrefix = string.Empty; // To serve Swagger UI at the app's root
            });
        }
        else
        {
            app.UseCors("DynamicCorsPolicy");
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        
        
        // Add websocket for transaction updates
        app.UseWebSockets(new WebSocketOptions()
        {
            // These are defaults that might be changed
            KeepAliveInterval = TimeSpan.FromMinutes(2),
            // AllowedOrigins = {  }
        });
            
            
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(ep =>
        {
            ep.MapControllers();
        });
    }
}