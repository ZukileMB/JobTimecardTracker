using JobTimecardTracker.Data;
using Microsoft.OpenApi.Models;
using NHibernate;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

var sessionFactory = NHibernateHelper.CreateSessionFactory(connectionString);
builder.Services.AddSingleton<ISessionFactory>(sessionFactory);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Job & Timecard Tracker API",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Job & Timecard Tracker API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();