using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPLibros.Models
{
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagId { get; set; }

        [Required]
        public string TagName { get; set; } = string.Empty;

        public ICollection<TitleTag> TitleTags { get; set; } = new List<TitleTag>();
    }
}