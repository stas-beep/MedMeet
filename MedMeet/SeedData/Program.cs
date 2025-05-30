﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Bogus;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        var connectionString = "server=localhost;port=3306;database=medmeetdb;user=root;password=root";

        var options = new DbContextOptionsBuilder<MedMeetDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;

        using var context = new MedMeetDbContext(options);


        Console.WriteLine("Starting seeding database ....");

        List<Cabinet> cabinets = SeedCabinets(context);
        List<Specialty> specialties = SeedSpecialties(context);
        List<User> users = SeedUsers(context, cabinets, specialties);
        List<Record> records = SeedRecords(context, users);
        List<Prescription> prescriptions = SeedPrescriptions(context, records);

        Console.WriteLine("Seeding finished.");
    }

    static List<Cabinet> SeedCabinets(MedMeetDbContext context)
    {
        if (context.Cabinets.Any())
        {
            Console.WriteLine("Cabinets already seeded. Loading from DB.");
            return context.Cabinets.ToList();
        }

        Console.WriteLine("Seeding Cabinets...");
        List<Cabinet> cabinet = new List<Cabinet>();
        for (int i = 1; i <= 20; i++)
        {
            cabinet.Add(new Cabinet
            {
                Name = $"Cabinet Room {i:000}"
            });
        }

        context.Cabinets.AddRange(cabinet);
        context.SaveChanges();

        return cabinet;
    }

    static List<Specialty> SeedSpecialties(MedMeetDbContext context)
    {
        if (context.Specialties.Any())
        {
            Console.WriteLine("Specialties already seeded. Loading from DB.");
            return context.Specialties.ToList();
        }

        Console.WriteLine("Seeding Specialties...");
        
        List<String> specialtyNames = new List<String>() {
            "Cardiology", "Dermatology", "Neurology", "Oncology", "Pediatrics",
            "Psychiatry", "Radiology", "Surgery", "Urology", "Gynecology",
            "Endocrinology", "Gastroenterology", "Nephrology", "Ophthalmology",
            "Orthopedics", "Otolaryngology", "Rheumatology", "Allergy", "Immunology", "Pathology"
        };

        List<Specialty> speciality = new List<Specialty>();
        foreach (var name in specialtyNames)
        {
            speciality.Add(new Specialty { Name = name });
        }

        context.Specialties.AddRange(speciality);
        context.SaveChanges();

        return speciality;
    }

    static List<User> SeedUsers(MedMeetDbContext context, List<Cabinet> cabinets, List<Specialty> specialties)
    {
        if (context.Users.Any())
        {
            Console.WriteLine("Users already seeded. Loading from DB.");
            return context.Users.Include(u => u.Cabinet).Include(u => u.Specialty).ToList();
        }

        Console.WriteLine("Seeding Users...");

        List<String> roles = new List<string>() { "Doctor", "Patient", "Admin"};
        var faker = new Faker();

        List<User> list = new List<User>();
        for (int i = 1; i <= 40; i++)
        {
            User user = new User
            {
                FullName = faker.Name.FullName(),
                Email = faker.Internet.Email(),
                Password = faker.Internet.Password(),
            };

            if (i % 2 == 0)
            {
                user.Role = "Doctor";
                user.CabinetId = cabinets[faker.Random.Int(0, cabinets.Count - 1)].Id;
                user.SpecialtyId = specialties[faker.Random.Int(0, specialties.Count - 1)].Id;
            }

            else
            {
                user.Role = "Patient";
                user.CabinetId = null;
                user.SpecialtyId = null;
            }
            
            list.Add(user);
        }

        context.Users.AddRange(list);
        context.SaveChanges();

        return list;
    }

    static List<Record> SeedRecords(MedMeetDbContext context, List<User> users)
    {
        if (context.Records.Any())
        {
            Console.WriteLine("Records already seeded. Loading from DB.");
            return context.Records.ToList();
        }

        Console.WriteLine("Seeding Records...");
        List<Record> list = new List<Record>();
        var patients = users.Where(u => u.Role == "Patient").ToList();
        var doctors = users.Where(u => u.Role == "Doctor").ToList();
        var faker = new Faker();

        for (int i = 0; i < 50; i++)
        {
            Record record = new Record
            {
                PatientId = patients[faker.Random.Int(0, patients.Count - 1)].Id,
                DoctorId = doctors[faker.Random.Int(0, doctors.Count - 1)].Id,
                AppointmentDate = faker.Date.Future(),
                Status = faker.PickRandom(new[] { "Scheduled", "Completed", "Cancelled" }),
                Notes = faker.Lorem.Sentence()
            };
            list.Add(record);
        }

        context.Records.AddRange(list);
        context.SaveChanges();

        return list;
    }

    static List<Prescription> SeedPrescriptions(MedMeetDbContext context, List<Record> records)
    {
        if (context.Prescriptions.Any())
        {
            Console.WriteLine("Prescriptions already seeded. Loading from DB.");
            return context.Prescriptions.ToList();
        }

        Console.WriteLine("Seeding Prescriptions...");
        List<Prescription> list = new List<Prescription>();
        var faker = new Faker();

        for (int i = 0; i < 50; i++)
        {
            var prescription = new Prescription
            {
                RecordId = records[faker.Random.Int(0, records.Count - 1)].Id,
                Medication = faker.Commerce.ProductName(),
                Dosage = $"{faker.Random.Int(1, 3)} times per day",
                Instructions = faker.Lorem.Sentence()
            };
            list.Add(prescription);
        }

        context.Prescriptions.AddRange(list);
        context.SaveChanges();

        return list;
    }
}