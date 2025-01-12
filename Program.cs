using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
    {
        // Cho phép API versioning theo URL, header hoặc query string
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true; // Giả sử phiên bản mặc định khi không chỉ định
        options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Hoặc sử dụng một kiểu đọc khác như header, query string
    });

var odataBuilder = new ODataConventionModelBuilder();
odataBuilder.EntityType<ma_literal>();

builder.Services.AddControllers().AddOData(opt => opt.Select().Expand().Filter().OrderBy().Count().SetMaxTop(1000));

builder.Services.AddDbContext<TaphoaEntities>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("taphoa"))
);

builder.Services.AddIdentity<ma_user, IdentityRole>()
    .AddEntityFrameworkStores<TaphoaEntities>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                builder.WithOrigins(["http://localhost:3000"])
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader();
            });
    });

builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/Authentication/Login";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        //c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI");
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
