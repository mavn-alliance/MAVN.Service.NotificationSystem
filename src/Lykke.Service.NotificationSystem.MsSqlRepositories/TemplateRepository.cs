using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Service.NotificationSystem.Domain.Models;
using Lykke.Service.NotificationSystem.Domain.Repositories;
using Lykke.Service.NotificationSystem.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.NotificationSystem.MsSqlRepositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly MsSqlContextFactory<NotificationSystemContext> _msSqlContextFactory;

        public TemplateRepository(MsSqlContextFactory<NotificationSystemContext> msSqlContextFactory)
        {
            _msSqlContextFactory = msSqlContextFactory;
        }

        public async Task<NotificationTemplateInfo> GetTemplateInfoAsync(string templateName)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = await context.Templates.FirstOrDefaultAsync(x => x.Name == templateName);

                if (entity == null)
                    return null;

                var info = new NotificationTemplateInfo(
                    entity.Name,
                    SplitLocalizations(entity));

                return info;
            }
        }

        public async Task InsertOrUpdateAsync(NotificationTemplateInfo templateInfo)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = await context.Templates.FirstOrDefaultAsync(x => x.Name == templateInfo.Name);

                if (entity == null)
                {
                    entity = new TemplateEntity
                    {
                        Name = templateInfo.Name,
                        ListOfLocalization = ConvertListLocalizationToString(templateInfo.AvailableLocalizations)
                    };

                    await context.AddAsync(entity);
                }
                else
                {
                    entity.ListOfLocalization = ConvertListLocalizationToString(templateInfo.AvailableLocalizations);

                    context.Update(entity);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<NotificationTemplateInfo>> GetListTemlateInfoAsync()
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entityList = await context.Templates.ToListAsync();
                var list = entityList.Select(e =>
                        new NotificationTemplateInfo(
                            e.Name,
                            SplitLocalizations(e))
                    )
                    .OrderBy(e => e.Name)
                    .ToList();

                return list;
            }
        }

        public async Task DeleteTemplateLocalizationAsync(string templateName, Localization local)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = await context.Templates.FirstOrDefaultAsync(x => x.Name == templateName);

                if (entity == null)
                    return;

                var locals = SplitLocalizations(entity);
                locals.RemoveAll(e => e.Equals(local));

                if (!locals.Any())
                {
                    context.Templates.Remove(entity);

                    await context.SaveChangesAsync();

                    return;
                }

                entity.ListOfLocalization = ConvertListLocalizationToString(locals);

                context.Templates.Update(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteTemplateAsync(string templateName)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = await context.Templates.FirstOrDefaultAsync(x => x.Name == templateName);

                if (entity == null)
                    return;

                context.Templates.Remove(entity);

                await context.SaveChangesAsync();
            }
        }

        private string ConvertListLocalizationToString(IEnumerable<Localization> locals)
        {
            return locals.Aggregate("", (c, i) => (string.IsNullOrEmpty(c) ? "" : (c + ";")) + i);
        }

        private static List<Localization> SplitLocalizations(TemplateEntity entity)
        {
            return entity.ListOfLocalization.Split(';').Select(Localization.From).ToList();
        }
    }
}
