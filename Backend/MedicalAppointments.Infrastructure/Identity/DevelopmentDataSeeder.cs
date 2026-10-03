using MedicalAppointments.Domain.Entities;

using MedicalAppointments.Domain.Enums;

using MedicalAppointments.Infrastructure.Persistence;

using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Logging;



namespace MedicalAppointments.Infrastructure.Identity;



/// <summary>

/// Development-only seed for repeatable manual testing. Never runs in Production or IntegrationTesting.

/// Credentials come from configuration (User Secrets / environment). Passwords are never logged.

/// </summary>

public static class DevelopmentDataSeeder

{

    private static readonly (string Name, string Description)[] ReferenceSpecialties =

    [

        ("Cardiología", "Diagnóstico y seguimiento de enfermedades del corazón y sistema circulatorio."),

        ("Dermatología", "Atención de piel, cabello y uñas."),

        ("Pediatría", "Atención médica para bebés, niños y adolescentes."),

        ("Medicina Interna", "Evaluación integral y manejo de enfermedades en adultos."),

        ("Neurología", "Trastornos del sistema nervioso y cerebro.")

    ];



    private static readonly DevelopmentDoctorProfile[] DevelopmentDoctors =

    [

        new("Cardiología", "cardiologia@dev.local", "Dr. Carlos Mendoza", "DEV-CARD-001"),

        new("Dermatología", "dermatologia@dev.local", "Dra. Laura Martínez", "DEV-DERM-001"),

        new("Pediatría", "pediatria@dev.local", "Dra. Andrea Gómez", "DEV-PED-001"),

        new("Medicina Interna", "medicinainterna@dev.local", "Dr. Daniel Rodríguez", "DEV-MI-001"),

        new("Neurología", "neurologia@dev.local", "Dr. Felipe Torres", "DEV-NEURO-001")

    ];



    private static readonly (TimeOnly Start, TimeOnly End)[] DailyAvailabilitySlots =

    [

        (new TimeOnly(9, 0), new TimeOnly(9, 30)),

        (new TimeOnly(10, 0), new TimeOnly(10, 30)),

        (new TimeOnly(14, 0), new TimeOnly(14, 30)),

        (new TimeOnly(15, 0), new TimeOnly(15, 30))

    ];



    public static async Task SeedAsync(

        IServiceProvider serviceProvider,

        IConfiguration configuration,

        IWebHostEnvironment environment,

        CancellationToken cancellationToken = default)

    {

        if (environment.IsProduction() || environment.IsEnvironment("IntegrationTesting"))

        {

            return;

        }



        if (!configuration.GetValue("DevSeed:Enabled", false))

        {

            return;

        }



        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DevelopmentDataSeeder");

        logger.LogInformation("Development seed enabled. Applying reference data and configured test users.");



        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var now = DateTimeOffset.UtcNow;



        await SeedSpecialtiesAsync(dbContext, now, cancellationToken);



        await SeedAdminAsync(userManager, configuration, logger, cancellationToken);

        await SeedDevelopmentDoctorsAsync(dbContext, userManager, configuration, logger, now, cancellationToken);

        await SeedPatientAsync(dbContext, userManager, configuration, logger, now, cancellationToken);

    }



    private static async Task SeedSpecialtiesAsync(

        ApplicationDbContext dbContext,

        DateTimeOffset now,

        CancellationToken cancellationToken)

    {

        foreach (var (name, description) in ReferenceSpecialties)

        {

            var exists = await dbContext.Specialties.AnyAsync(s => s.Name == name, cancellationToken);

            if (exists)

            {

                continue;

            }



            await dbContext.Specialties.AddAsync(Specialty.Create(name, description, now), cancellationToken);

        }



        await dbContext.SaveChangesAsync(cancellationToken);

    }



    private static async Task SeedAdminAsync(

        UserManager<ApplicationUser> userManager,

        IConfiguration configuration,

        ILogger logger,

        CancellationToken cancellationToken)

    {

        var email = configuration["DevSeed:Admin:Email"] ?? configuration["DevAdmin:Email"];

        var password = configuration["DevSeed:Admin:Password"] ?? configuration["DevAdmin:Password"];

        var fullName = configuration["DevSeed:Admin:FullName"]

            ?? configuration["DevAdmin:FullName"]

            ?? "Development Administrator";



        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))

        {

            logger.LogInformation("Development admin seed skipped: email or password not configured.");

            return;

        }



        if (await userManager.FindByEmailAsync(email.Trim()) is not null)

        {

            return;

        }



        var user = new ApplicationUser

        {

            UserName = email.Trim(),

            Email = email.Trim(),

            FullName = fullName.Trim(),

            IsActive = true,

            CreatedAt = DateTimeOffset.UtcNow

        };



        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)

        {

            logger.LogWarning("Development admin seed failed to create user.");

            return;

        }



        await userManager.AddToRoleAsync(user, AppRoles.Administrator);

        logger.LogInformation("Development administrator account ensured for {Email}.", email.Trim());

    }



    private static async Task SeedDevelopmentDoctorsAsync(

        ApplicationDbContext dbContext,

        UserManager<ApplicationUser> userManager,

        IConfiguration configuration,

        ILogger logger,

        DateTimeOffset now,

        CancellationToken cancellationToken)

    {

        var defaultPassword = configuration["DevSeed:Doctors:DefaultPassword"];

        if (string.IsNullOrWhiteSpace(defaultPassword))

        {

            logger.LogInformation(

                "Development doctors password reset skipped: DevSeed:Doctors:DefaultPassword is not configured.");

        }



        var canonicalEmails = DevelopmentDoctors

            .Select(d => d.Email)

            .ToHashSet(StringComparer.OrdinalIgnoreCase);



        foreach (var profile in DevelopmentDoctors)

        {

            await EnsureDevelopmentDoctorAsync(

                dbContext,

                userManager,

                profile,

                defaultPassword,

                logger,

                now,

                cancellationToken);

        }



        await DeactivateLegacyDevelopmentDoctorsAsync(

            dbContext,

            userManager,

            canonicalEmails,

            configuration,

            logger,

            now,

            cancellationToken);



        await dbContext.SaveChangesAsync(cancellationToken);

    }



    private static async Task EnsureDevelopmentDoctorAsync(

        ApplicationDbContext dbContext,

        UserManager<ApplicationUser> userManager,

        DevelopmentDoctorProfile profile,

        string? defaultPassword,

        ILogger logger,

        DateTimeOffset now,

        CancellationToken cancellationToken)

    {

        var specialty = await dbContext.Specialties

            .FirstOrDefaultAsync(s => s.Name == profile.SpecialtyName && s.IsActive, cancellationToken);



        if (specialty is null)

        {

            logger.LogWarning(

                "Development doctor seed skipped for {Email}: specialty {SpecialtyName} not found.",

                profile.Email,

                profile.SpecialtyName);

            return;

        }



        var normalizedEmail = profile.Email.Trim();

        var user = await userManager.FindByEmailAsync(normalizedEmail);

        Doctor doctor;



        if (user is null)

        {

            if (string.IsNullOrWhiteSpace(defaultPassword))

            {

                logger.LogInformation(

                    "Development doctor seed skipped for {Email}: user does not exist and default password is not configured.",

                    normalizedEmail);

                return;

            }



            user = new ApplicationUser

            {

                UserName = normalizedEmail,

                Email = normalizedEmail,

                FullName = profile.FullName.Trim(),

                EmailConfirmed = true,

                IsActive = true,

                CreatedAt = now

            };



            var createResult = await userManager.CreateAsync(user, defaultPassword);

            if (!createResult.Succeeded)

            {

                logger.LogWarning("Development doctor seed failed to create identity user for {Email}.", normalizedEmail);

                return;

            }



            await userManager.AddToRoleAsync(user, AppRoles.Doctor);



            doctor = Doctor.Create(

                Guid.NewGuid(),

                user.Id,

                specialty.Id,

                profile.FullName.Trim(),

                profile.License.Trim(),

                now);



            await dbContext.Doctors.AddAsync(doctor, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

        }

        else

        {

            user.FullName = profile.FullName.Trim();

            user.EmailConfirmed = true;

            user.IsActive = true;

            await userManager.UpdateAsync(user);



            if (!await userManager.IsInRoleAsync(user, AppRoles.Doctor))

            {

                await userManager.AddToRoleAsync(user, AppRoles.Doctor);

            }



            var existingDoctor = await dbContext.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id, cancellationToken);

            if (existingDoctor is null)

            {

                doctor = Doctor.Create(

                    Guid.NewGuid(),

                    user.Id,

                    specialty.Id,

                    profile.FullName.Trim(),

                    profile.License.Trim(),

                    now);

                await dbContext.Doctors.AddAsync(doctor, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);

            }

            else

            {

                existingDoctor.UpdateProfile(profile.FullName.Trim(), profile.License.Trim(), specialty.Id, now);

                existingDoctor.Activate(now);

                doctor = existingDoctor;

            }

        }



        await dbContext.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(defaultPassword))

        {

            await ResetDevelopmentUserPasswordAsync(userManager, user, defaultPassword, logger, cancellationToken);

        }



        await EnsureFutureAvailabilityAsync(dbContext, doctor, now, cancellationToken);

        logger.LogInformation("Development doctor ensured: {Email}", normalizedEmail);

    }



    private static async Task ResetDevelopmentUserPasswordAsync(

        UserManager<ApplicationUser> userManager,

        ApplicationUser user,

        string newPassword,

        ILogger logger,

        CancellationToken cancellationToken)

    {

        _ = cancellationToken;

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        var resetResult = await userManager.ResetPasswordAsync(user, resetToken, newPassword);

        if (!resetResult.Succeeded)

        {

            logger.LogWarning("Development doctor password reset failed for {Email}.", user.Email);

        }

    }



    private static async Task DeactivateLegacyDevelopmentDoctorsAsync(

        ApplicationDbContext dbContext,

        UserManager<ApplicationUser> userManager,

        HashSet<string> canonicalEmails,

        IConfiguration configuration,

        ILogger logger,

        DateTimeOffset now,

        CancellationToken cancellationToken)

    {

        var legacyEmail = configuration["DevSeed:Doctor:Email"]?.Trim();

        var doctors = await dbContext.Doctors.ToListAsync(cancellationToken);



        foreach (var doctor in doctors)

        {

            var user = await userManager.FindByIdAsync(doctor.UserId);

            if (user?.Email is null)

            {

                continue;

            }



            var email = user.Email.Trim();

            var isCanonical = canonicalEmails.Contains(email);

            var isLegacySingleDoctor = !string.IsNullOrWhiteSpace(legacyEmail)

                                       && string.Equals(email, legacyEmail, StringComparison.OrdinalIgnoreCase)

                                       && !isCanonical;



            if (isCanonical)

            {

                continue;

            }



            var isDevDoctorEmail = email.EndsWith("@dev.local", StringComparison.OrdinalIgnoreCase)

                                   && await userManager.IsInRoleAsync(user, AppRoles.Doctor);



            if (!isDevDoctorEmail && !isLegacySingleDoctor)

            {

                continue;

            }



            if (doctor.IsActive)

            {

                doctor.Deactivate(now);

                logger.LogInformation(

                    "Legacy development doctor deactivated: {Email} (superseded by per-specialty dev doctors).",

                    email);

            }

        }



        foreach (var profile in DevelopmentDoctors)

        {

            var specialty = await dbContext.Specialties

                .AsNoTracking()

                .FirstOrDefaultAsync(s => s.Name == profile.SpecialtyName, cancellationToken);

            if (specialty is null)

            {

                continue;

            }



            var canonicalUser = await userManager.FindByEmailAsync(profile.Email);

            if (canonicalUser is null)

            {

                continue;

            }



            var canonicalDoctor = doctors.FirstOrDefault(d => d.UserId == canonicalUser.Id);

            if (canonicalDoctor is null)

            {

                continue;

            }



            foreach (var duplicate in doctors.Where(

                         d => d.SpecialtyId == specialty.Id && d.IsActive && d.Id != canonicalDoctor.Id))

            {

                duplicate.Deactivate(now);

                logger.LogInformation(

                    "Duplicate active doctor deactivated for specialty {SpecialtyName}.",

                    profile.SpecialtyName);

            }

        }

    }



    private static async Task EnsureFutureAvailabilityAsync(

        ApplicationDbContext dbContext,

        Doctor doctor,

        DateTimeOffset now,

        CancellationToken cancellationToken)

    {

        var today = DateOnly.FromDateTime(now.UtcDateTime);

        var businessDaysAdded = 0;

        var checkDate = today;



        while (businessDaysAdded < 5)

        {

            checkDate = checkDate.AddDays(1);

            if (checkDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)

            {

                continue;

            }



            foreach (var (start, end) in DailyAvailabilitySlots)

            {

                var exists = await dbContext.DoctorAvailabilities.AnyAsync(

                    a => a.DoctorId == doctor.Id

                         && a.Date == checkDate

                         && a.StartTime == start

                         && a.EndTime == end,

                    cancellationToken);



                if (exists)

                {

                    continue;

                }



                await dbContext.DoctorAvailabilities.AddAsync(

                    DoctorAvailability.Create(Guid.NewGuid(), doctor.Id, checkDate, start, end, now),

                    cancellationToken);

            }



            businessDaysAdded++;

        }



        await dbContext.SaveChangesAsync(cancellationToken);

    }



    private static async Task SeedPatientAsync(

        ApplicationDbContext dbContext,

        UserManager<ApplicationUser> userManager,

        IConfiguration configuration,

        ILogger logger,

        DateTimeOffset now,

        CancellationToken cancellationToken)

    {

        var email = configuration["DevSeed:Patient:Email"];

        var password = configuration["DevSeed:Patient:Password"];

        var fullName = configuration["DevSeed:Patient:FullName"] ?? "Development Patient";



        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))

        {

            logger.LogInformation("Development patient seed skipped: email or password not configured.");

            return;

        }



        var normalizedEmail = email.Trim();

        if (await userManager.FindByEmailAsync(normalizedEmail) is not null)

        {

            return;

        }



        var user = new ApplicationUser

        {

            UserName = normalizedEmail,

            Email = normalizedEmail,

            FullName = fullName.Trim(),

            IsActive = true,

            CreatedAt = now

        };



        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)

        {

            logger.LogWarning("Development patient seed failed to create identity user.");

            return;

        }



        await userManager.AddToRoleAsync(user, AppRoles.Patient);



        var patient = Patient.Create(

            Guid.NewGuid(),

            user.Id,

            fullName.Trim(),

            new DateOnly(1992, 6, 15),

            "Calle Demo 100",

            "555-0100",

            Sex.Female,

            disability: null,

            MaritalStatus.Single,

            now);



        await dbContext.Patients.AddAsync(patient, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Development patient account ensured for {Email}.", normalizedEmail);

    }



    private sealed record DevelopmentDoctorProfile(

        string SpecialtyName,

        string Email,

        string FullName,

        string License);

}


