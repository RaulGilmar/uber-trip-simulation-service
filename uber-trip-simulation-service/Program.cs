using uber_trip_simulation_service.Models;

var builder = WebApplication.CreateBuilder(args);

// Repositorio singleton
var repository = new AppRepository();
repository.Customers.AddRange(new[]
{
    new Customer { Id = 1, FirstName = "Jose", LastName = "Gomez", Email = "jose@mail.com" },
    new Customer { Id = 2, FirstName = "Ana", LastName = "Lopez", Email = "ana@mail.com" },
    new Customer { Id = 3, FirstName = "Luis", LastName = "Perez", Email = "luis@mail.com" }
});
repository.Drivers.AddRange(new[]
{
    new Driver { Id = 1, FirstName = "Carlos", LastName = "Diaz", CarBrand = "Toyota", CarModel = "Corolla", LicensePlate = "AAA111", Rating = 4.5f, IsAvailable = false },
    new Driver { Id = 2, FirstName = "Maria", LastName = "Suarez", CarBrand = "Ford", CarModel = "Fiesta", LicensePlate = "BBB222", Rating = 4.8f, IsAvailable = false },
    new Driver { Id = 3, FirstName = "Pedro", LastName = "Martinez", CarBrand = "Chevrolet", CarModel = "Onix", LicensePlate = "CCC333", Rating = 5.0f, IsAvailable = true }
});
builder.Services.AddSingleton(repository);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Trip}/{action=Welcome}/{id?}");

app.Run();