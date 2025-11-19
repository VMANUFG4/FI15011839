using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPLibros.Models
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; } = string.Empty;

        public ICollection<Title> Titles { get; set; } = new List<Title>();
    }
}