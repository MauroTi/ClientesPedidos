using ClientesPedidos;
using ClientesPedidos.Controllers;
using ClientesPedidos.Dao;


var builder = WebApplication.CreateBuilder(args);

// ✅ REGISTRE TODOS OS SERVIÇOS AQUI
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Conexao>();
builder.Services.AddScoped<PedidosDAO>();
builder.Services.AddScoped<ClientesDAO>();



var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();