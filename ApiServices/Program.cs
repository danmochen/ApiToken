using ApiServices;
using Common;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

app.UseMiddleware<TokenMiddleware>();

app.UseAuthorization();//身份验证必须放在routing和终结点之间

app.MapControllers();

app.UseEndpoints(_ => { });//终结点

app.Run();
