using BookClub.Api.JWT;
using BookClub.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtCfg = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtCfg);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = jwtCfg.AsValidationParams();
        o.SaveToken = true;              
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/token/role",
    (TokenRequestRole body) =>
        Results.Ok(new { access_token = JwtHelpers.IssueToken(jwtCfg, role: body.Role) }))
   .WithTags("Auth")
   .AllowAnonymous();           

app.MapPost("/token/perms",
    (TokenRequestPerms body) =>
        Results.Ok(new { access_token = JwtHelpers.IssueToken(jwtCfg, perms: body.Permissions) }))
   .WithTags("Auth")
   .AllowAnonymous();

app.Run();
