using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetClinicManager.Data;

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
        await _context.Database.EnsureCreatedAsync();
        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedAnimalsAsync();
        await SeedOrdersAsync();
        await SeedOrderUpdatesAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roleNames = new[] { "Admin", "Vet", "Client" };

        foreach (var roleName in roleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new List<User>
        {
            new User
            {
                UserName = "admin@admin.com",
                Email = "admin@vetclinic.com",
                FirstName = "Jan",
                LastName = "Kowalski",
                Specialization = "Administrator",
                EmailConfirmed = true
            },
            new User
            {
                UserName = "vet@vetclinic.com",
                Email = "vet@vetclinic.com",
                FirstName = "Anna",
                LastName = "Nowak",
                Specialization = "Lekarz weterynarii",
                EmailConfirmed = true
            },
            new User
            {
                UserName = "assistant@vetclinic.com",
                Email = "assistant@vetclinic.com",
                FirstName = "Michał",
                LastName = "Wiśniewski",
                Specialization = "Asystent",
                EmailConfirmed = true
            }
        };

        var passwords = new[] { "Admin123!", "Vet123!", "Client123!" };
        var roles = new[] { "Admin", "Vet", "Client" };

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
                Gender = "Samiec",
                ImageUrl = "/uploads/default-dog.jpg",
                UserId = users[0].Id
            },
            new Animal
            {
                Name = "Mruczek",
                Species = "Kot",
                Breed = "Europejski",
                DateOfBirth = new DateTime(2020, 2, 15),
                Gender = "Samiec",
                ImageUrl = "/uploads/default-cat.jpg",
                UserId = users[0].Id
            },
            new Animal
            {
                Name = "Luna",
                Species = "Kot",
                Breed = "Syjamski",
                DateOfBirth = new DateTime(2019, 8, 22),
                Gender = "Samica",
                ImageUrl = "/uploads/default-cat.jpg",
                UserId = users[1].Id
            }
        };

        await _context.Animals.AddRangeAsync(animals);
        await _context.SaveChangesAsync();
    }

    private async Task SeedOrdersAsync()
    {
        if (await _context.Orders.AnyAsync()) return;

        var animals = await _context.Animals.ToListAsync();
        var users = await _userManager.Users.ToListAsync();

        var vetUser = users.First(u => u.Specialization == "Lekarz weterynarii");
        var assistantUser = users.First(u => u.Specialization == "Asystent");

        var orders = new List<Order>
        {
            new Order
            {
                Title = "Szczepienie przeciwko wściekliźnie",
                Description = "Routine vaccination",
                CreatedDate = DateTime.Now.AddDays(-10),
                AnimalId = animals[0].Id,
                AssignedUserId = vetUser.Id
            },
            new Order
            {
                Title = "Kontrola stanu zdrowia",
                Description = "Routine checkup",
                CreatedDate = DateTime.Now.AddDays(-5),
                AnimalId = animals[1].Id,
                AssignedUserId = vetUser.Id
            },
            new Order
            {
                Title = "Leczenie infekcji ucha",
                Description = "Ear infection treatment",
                CreatedDate = DateTime.Now.AddDays(-2),
                AnimalId = animals[2].Id,
                AssignedUserId = assistantUser.Id
            }
        };

        await _context.Orders.AddRangeAsync(orders);
        await _context.SaveChangesAsync();
    }

    private async Task SeedOrderUpdatesAsync()
    {
        if (await _context.OrderUpdates.AnyAsync()) return;

        var orders = await _context.Orders.ToListAsync();

        var updates = new List<OrderUpdate>
        {
            new OrderUpdate
            {
                Notes = "Szczepienie wykonane, zwierzę w dobrym stanie",
                UpdateDate = DateTime.Now.AddDays(-9),
                ImageUrl = "/uploads/vaccine.jpg",
                PrescribedMedications = "Brak",
                MedicalOrderId = orders[0].Id
            },
            new OrderUpdate
            {
                Notes = "Kontrola wykazała dobry stan zdrowia",
                UpdateDate = DateTime.Now.AddDays(-4),
                ImageUrl = "/uploads/checkup.jpg",
                PrescribedMedications = "Brak",
                MedicalOrderId = orders[1].Id
            },
            new OrderUpdate
            {
                Notes = "Rozpoczęto leczenie antybiotykami",
                UpdateDate = DateTime.Now.AddDays(-1),
                ImageUrl = "/uploads/ear-infection.jpg",
                PrescribedMedications = "Antybiotyk XYZ, 1 tabletka dziennie przez 7 dni",
                MedicalOrderId = orders[2].Id
            }
        };

        await _context.OrderUpdates.AddRangeAsync(updates);
        await _context.SaveChangesAsync();
    }
}