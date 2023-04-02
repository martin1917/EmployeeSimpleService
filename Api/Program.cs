using Application.Service;
using Application.Service.Base;
using Core.Repository;
using Data.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DbConfig>(opt =>
{
    return new DbConfig(
        User: builder.Configuration["DataBase:User"], 
        Password: builder.Configuration["DataBase:Password"],
        Host: builder.Configuration["DataBase:Host"],
        Port: builder.Configuration["DataBase:Port"],
        DataBase: builder.Configuration["DataBase:DataBase"]);
});
builder.Services.AddSingleton<ConnectionFactory>();
builder.Services.AddTransient<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
