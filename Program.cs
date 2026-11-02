using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// AUTENTICACIÓN POR COOKIE (LOGIN VÍA API DE USUARIOS)
// La sesión dura 4 horas: al expirar, el middleware redirige al login
// (/Home/Index) para que el usuario obtenga un token nuevo, y al volver a
// iniciar sesión regresa automáticamente a la página donde estaba
// (el middleware agrega ?ReturnUrl=/pagina-anterior a la redirección).

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.Name = "CookPortugal";
        option.Cookie.HttpOnly = true; // Protege la cookie de accesos por scripts
        option.LoginPath = "/Home/Index";
        //option.ExpireTimeSpan = TimeSpan.(10);// duracion de la sesion 
        option.AccessDeniedPath = "/Home/Index";
        option.ExpireTimeSpan = TimeSpan.FromHours(1); // Tiempo de vida de la sesión (4 horas) luego pide login nuevo para renovar el token
        option.SlidingExpiration = false; // No renueva la cookie automáticamente: al vencer, obliga a logearse de nuevo
    });

// Authorization: aplicar por defecto (requiere login para controladores/razor)
builder.Services.AddAuthorization();

// Si adem�s tienes Razor Pages:
builder.Services.AddRazorPages(options =>
{
    // Autorizar todo por defecto; permitir anonimo s�lo en la pagina de login
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Index");
});



builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // habilitAR
app.UseAuthorization();

// app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
