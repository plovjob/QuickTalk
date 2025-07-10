using QuickTalk.Messages.WebApi.Extensions;
using QuickTalk.Messages.WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationDependencies();
builder.Services.AddPersistenceDependencies(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(builder =>
{
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
    builder.AllowAnyOrigin();
});

app.UseAuthorization();
app.MapHub<MessageHub>("/chathub");
app.MapControllers();
app.Run();
