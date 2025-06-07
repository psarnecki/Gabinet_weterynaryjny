using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetClinicManager.Data;
using VetClinicManager.Models.Enums;

public class SeedData
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedData(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeAsync()
    {
        await _context.Database.MigrateAsync();
        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedAnimalsAsync();
        await SeedHealthRecordsAsync();
        await SeedMedicationsAsync();
        await SeedVisitsAsync();
        await SeedVisitUpdatesAsync();
        await SeedAnimalMedicationsAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roleNames = new[] { "Admin", "Vet", "Receptionist", "Client" };

        foreach (var roleName in roleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        
        await _context.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        var users = new List<User>
        {
            new User
            {
                UserName = "admin@vet.com",
                Email = "admin@vet.com",
                FirstName = "Jan",
                LastName = "Kowalski",
                EmailConfirmed = true
            },
            new User
            {
                UserName = "vet@vet.com",
                Email = "vet@vet.com",
                FirstName = "Anna",
                LastName = "Nowak",
                Specialization = "Lekarz weterynarii",
                EmailConfirmed = true
            },
            new User
            {
                    UserName = "receptionist@vet.com",
                Email = "receptionist@vet.com",
                FirstName = "Agata",
                LastName = "Poloczek",
                EmailConfirmed = true
            },
            new User
            {
                UserName = "client@vet.com",
                Email = "client@vet.com",
                FirstName = "Michał",
                LastName = "Wiśniewski",
                EmailConfirmed = true
            }
        };

        var passwords = new[] { "Admin123!", "Vet123!", "Rec123!", "Client123!" };
        var roles = new[] { "Admin", "Vet", "Receptionist", "Client" };

        for (int i = 0; i < users.Count; i++)
        {
            var existingUser = await _userManager.FindByEmailAsync(users[i].Email);
            if (existingUser == null)
            {
                var result = await _userManager.CreateAsync(users[i], passwords[i]);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(users[i], roles[i]);
                }
            }
        }
    }

    private async Task SeedAnimalsAsync()
    {
        if (await _context.Animals.AnyAsync()) return;

        var users = await _userManager.Users.ToListAsync();

        var animals = new List<Animal>
        {
            new Animal
            {
                Name = "Burek",
                Species = "Pies",
                Breed = "Mieszaniec",
                DateOfBirth = new DateTime(2018, 5, 10),
                BodyWeight = 18.5f, 
                Gender = Gender.Male,
                ImageUrl = "/uploads/default-dog.jpg",
                UserId = users[3].Id,
                MicrochipId = "123456789012345",
                LastVisitDate = DateTime.Now.AddDays(-10) 
            },
            new Animal
            {
                Name = "Mruczek",
                Species = "Kot",
                Breed = "Europejski",
                DateOfBirth = new DateTime(2020, 2, 15),
                BodyWeight = 4.2f,
                Gender = Gender.Male,
                ImageUrl = "/uploads/default-cat.jpg",
                UserId = users[3].Id
            },
            new Animal
            {
                Name = "Luna",
                Species = "Kot",
                Breed = "Syjamski",
                DateOfBirth = new DateTime(2019, 8, 22),
                BodyWeight = 3.1f,
                Gender = Gender.Female,
                ImageUrl = "/uploads/default-cat.jpg",
                UserId = users[1].Id,
                MicrochipId = "123456789012345",
                LastVisitDate = DateTime.Now.AddDays(-14) 
            }
        };

        await _context.Animals.AddRangeAsync(animals);
        await _context.SaveChangesAsync();
    }
    
    private async Task SeedAnimalMedicationsAsync()
    {
        if (await _context.AnimalMedications.AnyAsync()) return;
        
        var animals = await _context.Animals.ToListAsync(); 
        var medications = await _context.Medications.ToListAsync();
        var visits = await _context.Visits.ToListAsync();
        var updates = await _context.VisitUpdates.ToListAsync(); 

        var animalMedications = new List<AnimalMedication>
        {
            new AnimalMedication
            {
                AnimalId = animals[0].Id,
                MedicationId = medications[0].Id,
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-3),
                VisitUpdateId = updates.FirstOrDefault(u => u.VisitId == visits[0].Id).Id
            },
            new AnimalMedication
            {
                AnimalId = animals[2].Id,
                MedicationId = medications[2].Id,
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(5),
                VisitUpdateId = updates.FirstOrDefault(u => u.VisitId == visits[2].Id).Id
            }
        };

        await _context.AnimalMedications.AddRangeAsync(animalMedications);
        await _context.SaveChangesAsync();
    }
    
    private async Task SeedHealthRecordsAsync()
    {
        if (await _context.HealthRecords.AnyAsync()) return;

        var animals = await _context.Animals.ToListAsync();

        var healthRecords = new List<HealthRecord>
        {
            new HealthRecord
            {
                AnimalId = animals[0].Id,
                IsSterilized = true,
                ChronicDiseases = "Brak",
                Allergies = "Brak",
                Vaccinations = "Wścieklizna, Parwowiroza",
                LastVaccinationDate = DateTime.Now.AddMonths(-3)
            },
            new HealthRecord
            {
                AnimalId = animals[1].Id,
                IsSterilized = false,
                ChronicDiseases = "Choroba nerek",
                Allergies = "Pyłki",
                Vaccinations = "Wścieklizna, Katar koci",
                LastVaccinationDate = DateTime.Now.AddMonths(-6)
            },
            new HealthRecord
            {
                AnimalId = animals[2].Id,
                IsSterilized = true,
                ChronicDiseases = "Brak",
                Allergies = "Brak",
                Vaccinations = "Wścieklizna, Panleukopenia",
                LastVaccinationDate = DateTime.Now.AddMonths(-1)
            }
        };

        await _context.HealthRecords.AddRangeAsync(healthRecords);
        await _context.SaveChangesAsync();
    }

    private async Task SeedMedicationsAsync()
    {
        if (await _context.Medications.AnyAsync()) return;

        var medications = new List<Medication>
        {
            new Medication { Name = "Antybiotyk XYZ" },
            new Medication { Name = "Środek przeciwbólowy ABC" },
            new Medication { Name = "Krople do uszu DEF" },
            new Medication { Name = "Szampon przeciwgrzybiczy GHI" },
            new Medication { Name = "Revolution Plus" }
        };

        await _context.Medications.AddRangeAsync(medications);
        await _context.SaveChangesAsync();
    }
    
    private async Task SeedVisitsAsync()
    {
        if (await _context.Visits.AnyAsync()) return;

        var animals = await _context.Animals.ToListAsync();
        var users = await _userManager.Users.ToListAsync();

        var vetUser = users.FirstOrDefault(u => _userManager.GetRolesAsync(u).Result.Contains("Vet"));
        var vetUserId = vetUser?.Id;
        
        var visits = new List<Visit>
        {
            new Visit
            {
                Title = "Szczepienie przeciwko wściekliźnie",
                Description = "Routine vaccination",
                CreatedDate = DateTime.Now.AddDays(-10),
                Status = VisitStatus.Completed,
                Priority = VisitPriority.Normal,
                AnimalId = animals[0].Id,
                AssignedVetId = vetUserId
            },
            new Visit
            {
                Title = "Kontrola stanu zdrowia",
                Description = "Routine checkup",
                CreatedDate = DateTime.Now.AddDays(-5),
                Status = VisitStatus.Completed,
                Priority = VisitPriority.Normal, 
                AnimalId = animals[1].Id,
                AssignedVetId = vetUser.Id
            },
            new Visit
            {
                Title = "Leczenie infekcji ucha",
                Description = "Ear infection treatment",
                CreatedDate = DateTime.Now.AddDays(-2),
                Status = VisitStatus.InProgress,
                Priority = VisitPriority.Critical, 
                AnimalId = animals[2].Id,
                AssignedVetId = vetUserId
            }
        };

        await _context.Visits.AddRangeAsync(visits);
        await _context.SaveChangesAsync();
    }

    private async Task SeedVisitUpdatesAsync()
    {
        if (await _context.VisitUpdates.AnyAsync()) return;

        var visits = await _context.Visits.ToListAsync();
        var users = await _userManager.Users.ToListAsync();
        
        var vetUser = users.FirstOrDefault(u => _userManager.GetRolesAsync(u).Result.Contains("Vet"));
        var vetUserId = vetUser?.Id;

        var updates = new List<VisitUpdate>
        {
            new VisitUpdate
            {
                Notes = "Szczepienie wykonane, zwierzę w dobrym stanie",
                UpdateDate = DateTime.Now.AddDays(-9),
                ImageUrl = "/uploads/vaccine.jpg",
                PrescribedMedications = "Brak",
                VisitId = visits[0].Id,
                UpdatedByVetId = vetUserId
            },
            new VisitUpdate
            {
                Notes = "Kontrola wykazała dobry stan zdrowia",
                UpdateDate = DateTime.Now.AddDays(-4),
                ImageUrl = "/uploads/checkup.jpg",
                PrescribedMedications = "Brak",
                VisitId = visits[1].Id,
                UpdatedByVetId = vetUserId
            },
            new VisitUpdate
            {
                Notes = "Rozpoczęto leczenie antybiotykami",
                UpdateDate = DateTime.Now.AddDays(-1),
                ImageUrl = "/uploads/ear-infection.jpg",
                PrescribedMedications = "Antybiotyk XYZ, 1 tabletka dziennie przez 7 dni",
                VisitId = visits[2].Id,
                UpdatedByVetId = vetUserId
            }
        };

        await _context.VisitUpdates.AddRangeAsync(updates);
        await _context.SaveChangesAsync();
    }
}