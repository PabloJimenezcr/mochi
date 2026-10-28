using MyCategoria = Ventas.Persistence.Model.Category;
using Ventas.Persistence.Model;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");

try
{
    var connectionString = "Server=.;Database=Ventas;Trusted_Connection=True;TrustServerCertificate=True;";

    var options = new DbContextOptionsBuilder<VentasContext>()
        .UseSqlServer(connectionString)
        .Options;

    var venContext = new VentasContext(options);
    var categoria = new MyCategoria();

    //categoria.Name = "Construccion";
    //venContext.Categories.Add(categoria);
    //venContext.SaveChanges();

    //Consultar y Actualizar datos
    var categories =  venContext.Categories.Where(c => c.Name.StartsWith("Co")).ToList();

    foreach (var category in categories)
    {
        category.Name = category.Name + "©";

    }
   

    venContext.SaveChanges();


    var categoryForDeleting = venContext.Categories.FirstOrDefault(y => y.Name == "Oficina");

    venContext.Categories.Remove(categoryForDeleting);
    venContext.SaveChanges();

}
catch(Exception ex)
{
    Console.WriteLine("La Aplicacion tubo un error: " + ex.Message);
    
}