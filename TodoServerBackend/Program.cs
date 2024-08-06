var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization(); 
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost") // allow any origin 
    //.AllowAnyOrigin().WithOrigins("http://localhost")
    .AllowCredentials());
app.MapControllers();

app.Run();
