using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FilmViewer.Models
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Обработка данных регистрации, например, сохранение в базу данных

            // Перенаправление на главную страницу после успешной регистрации
            return RedirectToPage("/Index");
        }
    }
}
