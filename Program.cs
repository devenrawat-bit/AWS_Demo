using AWS_S3_Tutorial.Data;
using AWS_S3_Tutorial.Model;
using AWS_S3_Tutorial.Services.Implementation;
using AWS_S3_Tutorial.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<AwsSettings>( //after populating the value, store in di container 
    builder.Configuration.GetSection("AWS"));

//registering the services 
builder.Services.AddScoped<IFileUpload, FileUpload>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
