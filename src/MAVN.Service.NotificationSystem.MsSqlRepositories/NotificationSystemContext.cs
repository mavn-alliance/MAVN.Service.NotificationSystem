using System.Data.Common;
using JetBrains.Annotations;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.NotificationSystem.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.NotificationSystem.MsSqlRepositories
{
    public class NotificationSystemContext : PostgreSQLContext
    {
        private const string Schema = "notification_system";

        public DbSet<TemplateEntity> Templates { get; set; }

        public NotificationSystemContext(string connectionString, bool isTraceEnabled)
            : base(Schema, connectionString, isTraceEnabled)
        {
        }

        // empty constructor needed for EF migrations
        [UsedImplicitly]
        public NotificationSystemContext() : base(Schema)
        {
        }

        //Needed constructor for using InMemoryDatabase for tests
        public NotificationSystemContext(DbContextOptions options)
            : base(Schema, options)
        {
        }

        public NotificationSystemContext(DbConnection dbConnection)
            : base(Schema, dbConnection)
        {
        }

        protected override void OnMAVNModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TemplateEntity>().HasIndex(x => x.Name).IsUnique();
        }
    }
}
