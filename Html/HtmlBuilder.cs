using System;
using System.Threading.Tasks;
using FilmViewer.Models;

namespace FilmViewer.Html
{
    public static class HtmlBuilder
    {
        //Главный по построению 
        private static string BuildHtml(string Body, string StylePath, string ScriptPath)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>FilmViewer</title>
                    <link rel=""stylesheet"" href=""{StylePath}"">
                </head>
                <body>
                    {Body}
                    <script src=""{ScriptPath}""></script>
                </body>
                </html>
                ";
        }
        //так выглядит тело страницы 
        private static string BuildBody(string header, string main, string footer)
        {
            return $@"
                <header>{header}</header>
                <main>{main}</main>
                <footer>{footer}</footer>
                ";
        }
        //логотип 
        private static string BuildLogo()
        {
            return $@"
                <a class='logo' href='/'>
                    <span class='logoWord1'>Film</span>
                    <span class='logoWord2'>Viewer</span>
                </a>
                ";
        }
        //Кнопки регистрации и авторизации
        private static string BuildRegistrationButton()
        {
            return $@"
                <div class='registrationBlock'>
                    <a class='registerBtn' href='/register'>Register</a>
                </div>
                ";
        }

        private static string BuildLoginButton()
        {
            return $@"
                <div class='loginBlock'>
                    <a class='loginBtn' href='/login'>Login</a>
                </div>
                ";
        }
        //Поиск 
        private static string BuildSearch()
        {
            return $@"
                <div class='searchBlock'>
                    <input id='searchInput'>
                    <button id='searchBtn'>Search</button>
                </div>
                ";
        }
        //кнопки выбора жанра 
        private static async Task<string> BuildGenres()
        {
            var genres = await MovieApi.GetGenreList();
            string HtmlGenre = "<div class='genresList'>";

            foreach (var genre in genres.genres)
            {
                HtmlGenre += $"<a class='genreBtn' href='/Genre/{genre.id}/1'>{genre.name.ToUpperInvariant()}</a>";
            }

            HtmlGenre += "</div>";

            return HtmlGenre;
        }
        //Список фильмов 
        private static string BuildFilmsList(Movies Films)
        {
            string HtmlFilms = "<div class='filmsList'>";

            foreach (var film in Films.Results)
            {
                HtmlFilms += $@"
                    <a class='filmBtn' href='/Film/{film.id}'>
                        <img class='filmImg' src='https://image.tmdb.org/t/p/w185/{film.poster_path}' alt='Film poster'></img>
                        <span class='filmTitle'>{film.title}</span>  
                        <div class='filmVotes'>{Math.Round(film.vote_average, 1)}</div>
                    </a>
                    ";
            }

            HtmlFilms += "</div>";

            return HtmlFilms;
        }

        //пагинация 
        private static string BuildPagesList(int page, string path)
        {
            const int PreviousPages = 5;
            const int NextPages = 5;
            const int PagesToShow = 501;

            string HtmlPages = "<div class='pagesList'>";

            for (int i = Math.Max(page - PreviousPages, 1); i < Math.Min(page + NextPages + 1, PagesToShow); i++)
            {
                HtmlPages += $"<a class='pageBtn' href='{path}/{i}'>{i}</a>";
            }

            HtmlPages += "</div>";

            return HtmlPages;
        }
        //Заголовок страниц 
        private static async Task<string> BuildHeader()
        {
            return $@"
            <div class='topHeader'>
                {BuildLogo()}
                {BuildSearch()}
                <div class='authButtons'>
                    {BuildRegistrationButton()}
                    <div style='margin-left: 10px'></div> 
                    {BuildLoginButton()}
                </div>
                {await BuildGenres()}
            </div>";
        }
        //Страница лучших фильмов 
        public static async Task<string> BuildPopularPage(Movies movies, string PagesPath)
        {
            string HtmlFilms = BuildFilmsList(movies);
            string HtmlPages = BuildPagesList(movies.Page, PagesPath);

            return BuildHtml(
                BuildBody(
                    await BuildHeader(),
                    HtmlFilms + HtmlPages,
                    ""),
                "/styles/style.css",
                "/scripts/script.js");
        }
        //Страница с описанием выбраного фильма, оценкой и прочим 
        public static async Task<string> BuildFilmPage(Movie movie)
        {
            return BuildHtml(
                BuildBody(
                    await BuildHeader(),
                    $@"
                        <img class='movieBackground' src='https://image.tmdb.org/t/p/original/{movie.backdrop_path}' alt='Film poster'></img>
                        <div class='moviePresent'>
                            <span class='movieTitle'>{movie.title}</span>
                            <div class='filmVotes'>{Math.Round(movie.vote_average, 1)}</div>
                        </div>
                        <div class='movieOverview'>{movie.overview}</div>
                    ",
                    ""),
                "/styles/style.css",
                "/scripts/script.js");
        }
        //Страница регистрации 
        public static string BuildRegisterPage(string errorMessage = "")
        {
            return BuildHtml(
                $@"
                    <div class='authPage'>
                        <h2 class='authTitle'>Регистрация</h2>
                        <div class='registerForm'>
                            {(string.IsNullOrEmpty(errorMessage) ? "" : $"<p class='error'>{errorMessage}</p>")}
                            <form method='post' action='/register'>
                                <label for='email'>Email:</label>
                                <input type='email' id='email' name='email' required>
                                <br><br>
                                <label for='password'>Password:</label>
                                <input type='password' id='password' name='password' required>
                                <br><br>
                                <button type='submit' class='submitButton'>Register</button>
                            </form>
                            <a class='homeBtn' href='/'>Домой</a>
                        </div>
                    </div>
                ",
                "/styles/style.css",
                "/scripts/script.js");
        }
        //страница авторизации
        public static string BuildLoginPage(string errorMessage = "")
        {
            return BuildHtml(
                $@"
                    <div class='authPage'>
                        <h2 class='authTitle'>Авторизация</h2>
                        <div class='loginForm'>
                            {(string.IsNullOrEmpty(errorMessage) ? "" : $"<p class='error'>{errorMessage}</p>")}
                            <form method='post' action='/login'>
                                <label for='email'>Email:</label>
                                <input type='email' id='email' name='email' required>
                                <br><br>
                                <label for='password'>Password:</label>
                                <input type='password' id='password' name='password' required>
                                <br><br>
                                <button type='submit' class='submitButton'>Login</button>
                            </form>
                            <a class='homeBtn' href='/'>Домой</a>
                        </div>
                    </div>
                ",
                "/styles/style.css",
                "/scripts/script.js");
        }
    }
}
