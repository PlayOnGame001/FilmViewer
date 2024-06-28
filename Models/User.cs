using System.ComponentModel.DataAnnotations;

namespace FilmViewer.Models
{
    public class User
    {
        public int Id { get; set; }
        //валидация 
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
