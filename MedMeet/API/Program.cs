using Microsoft.EntityFrameworkCore;
using Database;
using Business_logic.Services.Implementation;
using Business_logic.Services.Interfaces;
using System.ComponentModel.Design;
using Database.Generic_Repository.Interfaces;
using Database.Generic_Repository;
using FluentValidation;
using FluentValidation.AspNetCore;
using Business_logic.Rules;
using Database.Models;
using Microsoft.AspNetCore.Identity;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MedMeetDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 41)), 
        b => b.MigrationsAssembly("CinemaBookingSystemDAL")), ServiceLifetime.Scoped);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(SpecialtyCreateDtoValidator).Assembly);

builder.Services.AddScoped<ICabinetService, CabinetService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IRecordService, RecordService>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();
builder.Services.AddScoped<IUserService, UserService>();

// Реєстрація репозиторіїв
builder.Services.AddScoped<ICabinetRepository, CabinetRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IRecordRepository, RecordRepository>();
builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<MedMeetDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();