


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost") // Replace with your Angular app's URL
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
    });


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Config.readConfig();
Console.WriteLine(Config.Development);

app.Run();
