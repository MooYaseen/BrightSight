using Graduation.Data;
using Graduation.Entities;
using Graduation.Extensions;
using Graduation.Helpers; // تأكد من إضافة هذا الـ using
using Graduation.Services;
using Graduation.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


// builder.WebHost.UseUrls("http://0.0.0.0:5248");


// Add services to the container.
// هنا سنضيف تهيئة الـ controllers وندمج الفلتر
builder.Services.AddControllers(options =>
{
    // هذا هو السطر الذي سيضيف الـ LogUserActivity كفلتر عام
    options.Filters.Add<LogUserActivity>();
});


// builder.Services.AddControllers()
//     .AddJsonOptions(options =>
//     {
//         options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//     });




builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

// هنا يتم تسجيل الـ Action Filter كخدمة
builder.Services.AddScoped<LogUserActivity>();


// ✅ إعدادات TomTom
builder.Services.Configure<TomTomSettings>(
builder.Configuration.GetSection("TomTomSettings"));


builder.Services.AddScoped<TomTomService>();

// قراءة إعدادات JWT من ملف appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<AudioUploadService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ SEED ROLES & USERS هنا
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    // Apply any pending migrations (optional but recommended)
    await context.Database.MigrateAsync();

    // Call seeding method
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during seeding");
}
// ✅ انتهى الجزء الخاص بالـ Seeding

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:3000"));

app.UseStaticFiles(); // ✅ إضافة هذا السطر

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

app.Run();