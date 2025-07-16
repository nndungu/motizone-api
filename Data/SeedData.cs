using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MotiZone.Models;

namespace MotiZone.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            context.Users.RemoveRange(context.Users);
            context.CartItems.RemoveRange(context.CartItems);
            context.Rentals.RemoveRange(context.Rentals);
            context.Parts.RemoveRange(context.Parts);
            context.Customizations.RemoveRange(context.Customizations);
            context.Orders.RemoveRange(context.Orders);
            context.OrderItems.RemoveRange(context.OrderItems);
            await context.SaveChangesAsync();

            Console.WriteLine("🧹 Cleared existing cart items, rentals, customizations, parts, and vehicles.");

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            Console.WriteLine("=== Seeding Started ===");

            // === 1. Seed Roles ===
            var roles = new[] { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                    Console.WriteLine($"✔ Created role: {role}");
                }
                else
                {
                    Console.WriteLine($"ℹ Role exists: {role}");
                }
            }

            // === 2. Seed Admin User ===
            var adminEmail = "admin@motizone.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");

                Console.WriteLine("✔ Created admin user.");
            }
            else
            {
                Console.WriteLine("ℹ Admin user already exists.");
            }

            // === 3. Seed Customer Users ===
            var customerData = new[]
            {
                "john.kamau@kenya.co.ke",
                "jane.nabakka@uganda.ug",
                "paul.komba@tanzania.tz"
            };

            foreach (var email in customerData)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new User
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(user, "Customer@123");
                    await userManager.AddToRoleAsync(user, "Customer");

                    Console.WriteLine($"✔ Created customer user: {email}");
                }
                else
                {
                    Console.WriteLine($"ℹ Customer user exists: {email}");
                }
            }

            // === 4. Seed Vehicles ===
            if (!context.Vehicles.Any())
            {
                context.Vehicles.AddRange(
                    new Vehicle
                    {
                        Name = "Toyota Corolla 2021",
                        Make = "Toyota",
                        Model = "Corolla",
                        Year = 2021,
                        Price = 18000,
                        DailyRate = 3000,
                        Region = "Dar esSalaam",
                        Type = "Sedan"
                    },
                    new Vehicle
                    {
                        Name = "Nissan X-Trail 2020",
                        Make = "Nissan",
                        Model = "X-Trail",
                        Year = 2020,
                        Price = 22000,
                        DailyRate = 3500,
                        Region = "Kampala",
                        Type = "Race"
                    },
                    new Vehicle
                    {
                        Name = "Suzuki Vitara 2022",
                        Make = "Suzuki",
                        Model = "Vitara",
                        Year = 2022,
                        Price = 25000,
                        DailyRate = 4000,
                        Region = "Nairobi",
                        Type = "Bus"
                    }
                );
                Console.WriteLine("✔ Seeded vehicles.");
            }
            else
            {
                Console.WriteLine("ℹ Vehicles already seeded.");
            }

            // === 5. Seed Parts ===
            if (!context.Parts.Any())
            {
                context.Parts.AddRange(
                    new Part
                    {
                        Name = "Air Filter", Description = "Universal air filter", Category = "Engine", Price = 2500
                    },
                    new Part
                    {
                        Name = "Brake Pads", Description = "Front brake pads", Category = "Brakes", Price = 4200
                    },
                    new Part
                    {
                        Name = "Oil Filter", Description = "Oil filter for Japanese vehicles", Category = "Engine",
                        Price = 1800
                    }
                );
                Console.WriteLine("✔ Seeded parts.");
            }
            else
            {
                Console.WriteLine("ℹ Parts already seeded.");
            }

            // === 6. Seed Customizations ===
            if (!context.Customizations.Any())
            {
                context.Customizations.AddRange(
                    new Customization
                    {
                        Name = "Bull Bar", Type = "Body", Description = "Heavy-duty bull bar for safaris", Price = 15000
                    },
                    new Customization
                        { Name = "Roof Rack", Type = "Body", Description = "Durable rack for luggage", Price = 10000 },
                    new Customization
                        { Name = "LED Light Bar", Type = "Body", Description = "Bright off-road light", Price = 8500 }
                );
                Console.WriteLine("✔ Seeded customizations.");
            }
            else
            {
                Console.WriteLine("ℹ Customizations already seeded.");
            }

            // === 7. Seed Rental ===
            if (!context.Rentals.Any())
            {
                var customer = await userManager.FindByEmailAsync("john.kamau@kenya.co.ke");
                var vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Make == "Toyota");

                if (customer != null && vehicle != null)
                {
                    context.Rentals.Add(new Rental
                    {
                        UserId = customer.Id,
                        VehicleId = vehicle.Id,
                        StartDate = DateTime.UtcNow.AddDays(-2),
                        EndDate = DateTime.UtcNow.AddDays(3),
                        TotalCost = vehicle.DailyRate * 5
                    });

                    Console.WriteLine("✔ Seeded rental for john.kamau@kenya.co.ke");
                }
                else
                {
                    Console.WriteLine("⚠ Rental not seeded – user or vehicle missing.");
                }
            }
            else
            {
                Console.WriteLine("ℹ Rentals already seeded.");
            }

            // === 8. Seed Cart Items ===
            if (!context.CartItems.Any())
            {
                var customer = await userManager.FindByEmailAsync("john.kamau@kenya.co.ke");
                var vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Make == "Toyota");
                var part = await context.Parts.FirstOrDefaultAsync(p => p.Name == "Air Filter");
                var customization = await context.Customizations.FirstOrDefaultAsync(c => c.Name == "Bull Bar");

                if (customer != null)
                {
                    var cartItems = new List<CartItem>();

                    if (vehicle != null)
                    {
                        cartItems.Add(new CartItem
                        {
                            UserId = customer.Id,
                            VehicleId = vehicle.Id,
                            Quantity = 1,
                            DateAdded = DateTime.UtcNow
                        });
                    }

                    if (part != null)
                    {
                        cartItems.Add(new CartItem
                        {
                            UserId = customer.Id,
                            PartId = part.Id,
                            Quantity = 1,
                            DateAdded = DateTime.UtcNow
                        });
                    }

                    if (customization != null)
                    {
                        cartItems.Add(new CartItem
                        {
                            UserId = customer.Id,
                            CustomizationId = customization.Id,
                            Quantity = 1,
                            DateAdded = DateTime.UtcNow
                        });
                    }

                    context.CartItems.AddRange(cartItems);
                    Console.WriteLine($"✔ Seeded {cartItems.Count} cart items for {customer.Email}");
                }
                else
                {
                    Console.WriteLine("⚠ Could not seed cart items – customer user missing.");
                }
            }
            else
            {
                Console.WriteLine("ℹ Cart items already seeded.");
            }

            // === 9. Seed orders
            if (!context.OrderItems.Any())
            {
                var orders = await context.Orders.Include(o => o.User).ToListAsync();
                var part = await context.Parts.FirstOrDefaultAsync(p => p.Name == "Air Filter");
                var vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Make == "Toyota");

                foreach (var order in orders)
                {
                    context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        PartId = part?.Id,
                        Quantity = 1,
                        Price = part?.Price ?? 0,
                    });
                    context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        VehicleId = vehicle?.Id,
                        Quantity = 1,
                        Price = part?.Price ?? 0,
                    });
                }

                Console.WriteLine("✔ Seeded order items.");
            }

            // === 10. Seed Admin Orders ===
            if (!context.Orders.Any())
            {
                var admin = await userManager.FindByEmailAsync("admin@motizone.com");
                var vehicle = await context.Vehicles.FirstOrDefaultAsync();

                if (admin != null && vehicle != null)
                {
                    var order = new Order
                    {
                        UserId = admin.Id,
                        OrderDate = DateTime.UtcNow,
                        TotalAmount = vehicle.Price,
                        //Status = " ",
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                VehicleId = vehicle.Id,
                                Quantity = 1,
                                Price = vehicle.Price,
                            }
                        }

                    };
                    context.Orders.Add(order);
                    Console.WriteLine("✔ Seeded admin order.");
                }
                else
                {
                    Console.WriteLine("ℹ Orders already seeded.");
                }

                // === 11. Save Changes ===
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Seeding complete.");
            }
        }
    }
}
    
