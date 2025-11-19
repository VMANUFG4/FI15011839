using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPLibros.Models
{
    public class TitleTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TitleTagId { get; set; }

        [Required]
        public int TitleId { get; set; }

        [Required]
        public int TagId { get; set; }

        [ForeignKey("TitleId")]
        public Title Title { get; set; } = null!;

        [ForeignKey("TagId")]
        public Tag Tag { get; set; } = null!;
    }
}