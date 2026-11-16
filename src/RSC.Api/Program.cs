using RSC.Classes;

var builder = WebApplication.CreateBuilder(args);

// Tell the middle layer where the database is (appsettings.json -> ConnectionStrings:RSC)
DbSettings.ConnectionString = builder.Configuration.GetConnectionString("RSC") ?? DbSettings.ConnectionString;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow the React dev server (Vite, port 5173) to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
