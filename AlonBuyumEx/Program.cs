using AlonBuyumEx.DAL;
using AlonBuyumEx.Domains;
using AlonBuyumEx.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

 builder.Services.AddDbContext<DataContext>(options => options.UseSqlite("Data Source=Data.db"));

// Add the services we created in the app
builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseCors("AllowedPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Map the Endpoints we created in the Domains Layer
app.MapEndPoints();



app.Run();
