using AuthenticationAPI.Data;
using AuthenticationAPI.Services;
using AuthenticationAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDiscoveryClient(builder.Configuration);
//FOR DEVELOPMENT ONLY
builder.Services.AddDbContext<AuthenticationContext>(options =>
    options.UseInMemoryDatabase("AuthenticationDb"));
// Uncomment the following line to use PostgreSQL instead of in-memory database
// builder.Services.AddDbContext<AuthenticationContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();