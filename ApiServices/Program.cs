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

app.UseAuthorization();//�����֤�������routing���ս��֮��

app.MapControllers();

app.UseEndpoints(_ => { });//�ս��

app.Run();
