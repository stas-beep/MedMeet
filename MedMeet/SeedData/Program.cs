using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Bogus;
using Database;
using Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        string connectionString = "server=localhost;port=3306;database=medmeetdb;user=root;password=root";

        var options = new DbContextOptionsBuilder<MedMeetDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
        using var context = new MedMeetDbContext(options);

        var store = new UserStore<User, IdentityRole<int>, MedMeetDbContext, int>(context);
        PasswordHasher<User> hasher = new PasswordHasher<User>();
        List<IUserValidator<User>> userValidators = new List<IUserValidator<User>> { new UserValidator<User>() };
        List<IPasswordValidator<User>> passwordValidators = new List<IPasswordValidator<User>> { new PasswordValidator<User>() };
        var logger = NullLogger<UserManager<User>>.Instance;

        UserManager<User> userManager = new UserManager<User>(store, null, hasher, userValidators, passwordValidators, null, null, null, logger);


        Console.WriteLine("Починаємо заповнювати базу даних тестовими даними ....");

        var roleStore = new RoleStore<IdentityRole<int>, MedMeetDbContext, int>(context);
        var roleValidators = new List<IRoleValidator<IdentityRole<int>>>{ new RoleValidator<IdentityRole<int>>() };
        var lookupNormalizer = new UpperInvariantLookupNormalizer(); 
        var errorDescriber = new IdentityErrorDescriber(); 
        var roleLogger = NullLogger<RoleManager<IdentityRole<int>>>.Instance;
        var roleManager = new RoleManager<IdentityRole<int>>(roleStore, roleValidators, lookupNormalizer, errorDescriber, roleLogger);
        
        await EnsureRolesAsync(roleManager);
        await SeedAdminAsync(userManager);
        
        List<Cabinet> cabinets = SeedCabinets(context);
        List<Specialty> specialties = SeedSpecialties(context);
        List<User> users = await SeedUsersAsync(context, userManager, cabinets, specialties);
        List<Record> records = await SeedRecordsAsync(context, users, userManager);
        List<Prescription> prescriptions = SeedPrescriptions(context, records);

        Console.WriteLine("Заповнення завершилось.");
    }

    static List<Cabinet> SeedCabinets(MedMeetDbContext context)
    {
        if (context.Cabinets.Any())
        {
            Console.WriteLine("Кабінети уже містять певні дані.Завантажуємо...");
            return context.Cabinets.ToList();
        }

        Console.WriteLine("Заповнюємо кабінети...");
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
            Console.WriteLine("Спеціальності вже існують в базі даних. Завантажуємо...");
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

    static async Task<User> SeedAdminAsync(UserManager<User> userManager)
    {
        string adminEmail = "admin@medmeet.com";
        string password = "Admin123!";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin != null)
        {
            Console.WriteLine("Адміністратор вже існує.");
            return existingAdmin;
        }

        User admin = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Administrator"
        };

        var result = await userManager.CreateAsync(admin, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            Console.WriteLine("Адміністратор створений успішно.");
            return admin;
        }
        else
        {
            Console.WriteLine($"Помилка при створенні адміністратора: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return null;
        }
    }

    static async Task EnsureRolesAsync(RoleManager<IdentityRole<int>> roleManager)
    {
        string[] roles = { "Doctor", "Patient", "Admin" };

        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
                Console.WriteLine($"Роль {role} була створена успішно");
            }
        }
    }

    static async Task<List<User>> SeedUsersAsync(MedMeetDbContext context, UserManager<User> userManager, List<Cabinet> cabinets, List<Specialty> specialties)
    {
        if (context.Users.Count() > 1)
        {
            Console.WriteLine("Користувачі уже є в базі даних. Перевіряємо ролі...");

            var existingUsers = context.Users.Include(u => u.Cabinet).Include(u => u.Specialty).ToList();

            foreach (var user in existingUsers)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Count == 0)
                {
                    string role = (user.SpecialtyId.HasValue && user.CabinetId.HasValue) ? "Doctor" : "Patient";
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            return existingUsers;
        }

        Console.WriteLine("Заповнюємо користувачами...");

        var faker = new Faker();
        List<User> users = new List<User>();

        for (int i = 1; i <= 40; i++)
        {
            string email = faker.Internet.Email();
            string password = faker.Internet.Password(10, false, "", "!1Aa");

            User user = new User
            {
                UserName = email,
                Email = email,
                FullName = faker.Name.FullName(),
                SpecialtyId = null,
                CabinetId = null
            };

            string role;

            if (i % 2 == 0)
            {
                role = "Doctor";
                var randomCabinet = cabinets[faker.Random.Int(0, cabinets.Count - 1)];
                var randomSpecialty = specialties[faker.Random.Int(0, specialties.Count - 1)];

                user.CabinetId = randomCabinet.Id;
                user.SpecialtyId = randomSpecialty.Id;
            }
            else
            {
                role = "Patient";
            }

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                users.Add(user);
            }
            else
            {
                Console.WriteLine($"Не вдалося створити користувача: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        return users;
    }

    static async Task<List<Record>> SeedRecordsAsync(MedMeetDbContext context, List<User> users, UserManager<User> userManager)
    {
        if (context.Records.Any())
        {
            Console.WriteLine("Записи уже існують в базі даних. Завантажуємо...");
            return context.Records.ToList();
        }

        Console.WriteLine("Заповнюємо записами...");
        List<Record> list = new List<Record>();

        List<User> patients = new List<User>();
        List<User> doctors = new List<User>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains("Patient"))
            {
                patients.Add(user);
            }
            if (roles.Contains("Doctor"))
            {
                doctors.Add(user);
            }
        }

        if (patients.Count == 0 || doctors.Count == 0)
        {
            Console.WriteLine("Немає користувачів з ролями 'Patient' або 'Doctor'.");
            return list;
        }

        Faker faker = new Faker();

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
            Console.WriteLine("Призначення уже є в базі даних. Завантажуємо ...");
            return context.Prescriptions.ToList();
        }

        if (records == null || records.Count == 0)
        {
            Console.WriteLine("Неможливо згенерувати призначення, бо немає записів.");
            return new List<Prescription>();
        }

        Console.WriteLine("Заповнюємо призначеннями...");
        List<Prescription> list = new List<Prescription>();
        Faker faker = new Faker();

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