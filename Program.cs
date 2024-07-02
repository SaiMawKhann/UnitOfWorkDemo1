using kzy_entities.DBContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UnitOfWorkDemo.Repositories;
using UnitOfWorkDemo1.BL;
using kzy_entities.Common;
using UnitOfWorkDemo1.MapperProfiles;
using UnitOfWorkDemo1.Services;
using kzy_entities.Enums;
using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

// Add configuration files
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Determine the environment
var env = builder.Environment;

//Add Versioning
builder.Services.AddApiVersioning();

// Add services to the container based on the environment
if (env.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("WriterConnection")));
    builder.Services.AddDbContext<ReaderDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ReaderConnection")));
}
else if (env.IsStaging())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("WriterConnectionStag")));
    builder.Services.AddDbContext<ReaderDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ReaderConnectionStag")));
}
else
{
    // Add production or other environment configurations here
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("WriterConnectionProd")));
    builder.Services.AddDbContext<ReaderDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ReaderConnectionProd")));
}

// Register other services
builder.Services.AddTransient<IUnitOfWork<ApplicationDbContext, ReaderDbContext>, UnitOfWork<ApplicationDbContext, ReaderDbContext>>();
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IProductBL, ProductBL>();
builder.Services.AddTransient<IOnboardingBL, OnboardingBL>();
builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();
builder.Services.AddTransient<IErrorCodeProvider, ErrorCodeProvider>();  // Register IErrorCodeProvider
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

#region JWT Token & External Authentications
var jwtSettings = builder.Configuration.GetSection("Jwt");
var aa = jwtSettings["Keys:OTPToken"];

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddCookie(ConstantStrings.COOKIE_AUTHENTICATION, (options) =>
  {
      options.Cookie.Name = ConstantStrings.AUTH_COOKIE;
      options.SlidingExpiration = true;
      options.ExpireTimeSpan = TimeSpan.FromDays(30);
      options.Events.OnRedirectToLogin = (context) =>
      {
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          return Task.CompletedTask;
      };

      options.Cookie.HttpOnly = true;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
      // Only use this when the sites are on different domains
      options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
  })
  .AddCookie(ConstantStrings.COOKIE_REFRESH_AUTHENTICATION, (options) =>
  {
      options.Cookie.Name = ConstantStrings.REFRESH_COOKIE;
      options.SlidingExpiration = true;
      options.ExpireTimeSpan = TimeSpan.FromDays(30);
      options.Events.OnRedirectToLogin = (context) =>
      {
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          return Task.CompletedTask;
      };

      options.Cookie.HttpOnly = true;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
      // Only use this when the sites are on different domains
      options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
  })
  .AddJwtBearer(ConstantStrings.OTPTOKENAUTH, options =>
  {
      var oTPTokenKey = jwtSettings["Keys:OTPToken"]; 
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings["Issuer"],
          ValidAudience = jwtSettings["Audience"],
          ValidateIssuer = Boolean.Parse(jwtSettings["ValidateIssuer"]),
          ValidateAudience = Boolean.Parse(jwtSettings["ValidateAudience"]),
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(oTPTokenKey)),
          ClockSkew = TimeSpan.Zero
      };
      options.Events = new JwtBearerEvents
      {
          OnChallenge = async context =>
          {
              context.Response.StatusCode = 401;
              var response = new { error_code = 401, error_msg = "Invalid OTP Token or Token Expire!" };
              context.HttpContext.Response.ContentType = "application/json";
              context.HttpContext.Response.Headers.Add("error-code", "401");
              await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(response)));
              context.HandleResponse();
          }
      };
  })
  .AddJwtBearer(ConstantStrings.ACCESSTOKENAUTH, options =>
  {
      var accessTokenKey = jwtSettings["Keys:AccessToken"];
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings["Issuer"],
          ValidAudience = jwtSettings["Audience"],
          ValidateIssuer = Boolean.Parse(jwtSettings["ValidateIssuer"]),
          ValidateAudience = Boolean.Parse(jwtSettings["ValidateAudience"]),
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenKey)),
          ClockSkew = TimeSpan.Zero
      };
      options.Events = new JwtBearerEvents
      {
          OnChallenge = async context =>
          {
              context.Response.StatusCode = 401;
              var response = new { error_code = 401, error_msg = "Invalid Access Token or Token Expire!" };
              context.HttpContext.Response.ContentType = "application/json";
              context.HttpContext.Response.Headers.Add("error-code", "401");
              await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(response)));
              context.HandleResponse();
          }
      };
  })
  .AddPolicyScheme(ConstantStrings.REZEVE_CUSTOMER_AUTH, ConstantStrings.REZEVE_CUSTOMER_AUTH, options =>
  {
      options.ForwardDefaultSelector = context =>
      {
          var authCookie = context.Request.Cookies[ConstantStrings.AUTH_COOKIE];
          return (string.IsNullOrEmpty(authCookie)) ? ConstantStrings.ACCESSTOKENAUTH : ConstantStrings.COOKIE_AUTHENTICATION;
      };
  })
  .AddPolicyScheme(ConstantStrings.REZEVE_CUSTOMER_REFESH_AUTH, ConstantStrings.REZEVE_CUSTOMER_REFESH_AUTH, options =>
  {
      options.ForwardDefaultSelector = context =>
      {
          var refreshToken = context.Request.Cookies[ConstantStrings.COOKIE_REFRESH_AUTHENTICATION];
          if (!string.IsNullOrEmpty(refreshToken))
              context.Request.Headers.TryAdd("Authorization", "Bearer " + refreshToken);

          return ConstantStrings.REFRESHTOKENAUTH;
      };
  })
  .AddJwtBearer(ConstantStrings.REFRESHTOKENAUTH, options =>
  {
      var refreshTokenKey = jwtSettings["Keys:RefreshToken"];
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings["Issuer"],
          ValidAudience = jwtSettings["Audience"],
          ValidateIssuer = Boolean.Parse(jwtSettings["ValidateIssuer"]),
          ValidateAudience = Boolean.Parse(jwtSettings["ValidateAudience"]),
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshTokenKey)),
          ClockSkew = TimeSpan.Zero
      };
      options.Events = new JwtBearerEvents
      {
          OnChallenge = async context =>
          {
              context.Response.StatusCode = 401;
              var response = new { error_code = 498, error_msg = "Invalid Refresh Token or Refresh Token Expire!" };
              context.HttpContext.Response.ContentType = "application/json";
              context.HttpContext.Response.Headers.Add("error-code", "498");
              await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(response)));
              context.HandleResponse();
          }
      };
  })
  .AddJwtBearer("BIOMETRICTOKENAUTH", options =>
  {
      var accessTokenKey = jwtSettings["Keys:BiometricToken"];
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings["Issuer"],
          ValidAudience = jwtSettings["Audience"],
          ValidateIssuer = Boolean.Parse(jwtSettings["ValidateIssuer"]),
          ValidateAudience = Boolean.Parse(jwtSettings["ValidateAudience"]),
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenKey)),
          ClockSkew = TimeSpan.Zero
      };
      options.Events = new JwtBearerEvents
      {
          OnChallenge = async context =>
          {
              context.Response.StatusCode = 401;
              var response = new { error_code = 401, error_msg = "Invalid Access Token or Token Expire!" };
              context.HttpContext.Response.ContentType = "application/json";
              context.HttpContext.Response.Headers.Add("error-code", "401");
              await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(response)));
              context.HandleResponse();
          }
      };
  });

#endregion

// Add controllers and Swagger configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
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
});

var app = builder.Build();

if (env.IsDevelopment() || env.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    });
}

//app.UseHttpsRedirection();

app.UseHealthChecks("/");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

Console.WriteLine("About to run");

app.Run();


