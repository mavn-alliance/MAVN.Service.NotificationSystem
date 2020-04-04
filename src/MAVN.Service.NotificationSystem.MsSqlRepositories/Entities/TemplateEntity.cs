using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.NotificationSystem.MsSqlRepositories.Entities
{
    [Table("templates")]
    public class TemplateEntity : EntityBase
    {
        [Column("name")]
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [Column("list_of_localization")]
        [MaxLength(300)]
        [Required]
        public string ListOfLocalization { get; set; }
    }
}
