// <copyright file="Program.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using MyNunitWeb.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<MyNUnitDbContext>(options =>
    options.UseSqlite("Data Source=mynunit.db"));

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();