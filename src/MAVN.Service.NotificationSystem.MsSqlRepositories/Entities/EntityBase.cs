using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.NotificationSystem.MsSqlRepositories.Entities
{
    public class EntityBase
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
    }
}
