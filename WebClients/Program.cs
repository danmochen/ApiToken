using Common;
using WebClients;

var builder = WebApplication.CreateBuilder();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<TokenManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();


app.Run();
