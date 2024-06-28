global using FilmViewer.Api;
global using FilmViewer.Models;
using FilmViewer.Html;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using System.Text.Json;




var builder = WebApplication.CreateBuilder(args);

// Добавляем контекст базе данных для взаимодействия с ней 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

var app = builder.Build();

app.UseStaticFiles();//подключение стат. файлов 

// Инициализируем ее 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

//Популярные фильмы отвечает за отображение популярныз фильмов 
app.Map("/{page:int?}", async (HttpContext context, int page = 1) =>
{
    context.Response.ContentType = "text/html";

    Movies movies = await MovieApi.GetPopularMovies(page);

    return await HtmlBuilder.BuildPopularPage(movies, "");
});

//По жанру фильмы отвечает за отображение по жанру фильмов 
app.Map("/Genre/{id:int}/{page:int?}", async (HttpContext context, int id, int page = 1) =>
{
    context.Response.ContentType = "text/html";

    Movies movies = await MovieApi.GetMoviesByGenre(id, page);

    return await HtmlBuilder.BuildPopularPage(movies, $"/Genre/{id}");
});

//Поиск фильмо по названию
app.Map("/Search/{search}/{page:int?}", async (HttpContext context, string search, int page = 1) =>
{
    context.Response.ContentType = "text/html";

    Movies movies = await MovieApi.GetMoviesByName(search, page);

    return await HtmlBuilder.BuildPopularPage(movies, $"/Search/{search}");
});

//Просмотр информации про фильм 
app.Map("/Film/{id:int?}", async (HttpContext context, int id = 1) =>
{
    context.Response.ContentType = "text/html";

    Movie movie = await MovieApi.GetMovieById(id);

    return await HtmlBuilder.BuildFilmPage(movie);
});

//Страница регистарции 
app.MapGet("/register", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(HtmlBuilder.BuildRegisterPage());
});

//Обработка данных регистрации 
app.MapPost("/register", async (HttpContext context, ApplicationDbContext db) =>
{
    string email = context.Request.Form["email"];
    string password = context.Request.Form["password"];

    var user = new User { Email = email, Password = password };
    db.Users.Add(user);
    await db.SaveChangesAsync();

    context.Response.Redirect("/");
});

//Страница авторизации 
app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(HtmlBuilder.BuildLoginPage());
});

//Обработка данных авторизации 
app.MapPost("/login", async (HttpContext context, ApplicationDbContext db) =>
{
    string email = context.Request.Form["email"];
    string password = context.Request.Form["password"];

    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

    if (user != null)
    {
        context.Response.Redirect("/");
    }
    else
    {
        context.Response.Redirect("/login?error=Invalid credentials");
    }
});

app.Run();//ЗАПУСК
