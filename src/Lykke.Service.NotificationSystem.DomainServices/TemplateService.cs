using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.NotificationSystem.Domain.Models;
using Lykke.Service.NotificationSystem.Domain.Repositories;
using Lykke.Service.NotificationSystem.Domain.Services;

namespace Lykke.Service.NotificationSystem.DomainServices
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly ITemplateContentRepository _templateContentRepository;
        private readonly ILog _log;

        public TemplateService(ITemplateRepository templateRepository, ITemplateContentRepository templateContentRepository, ILogFactory logFactory)
        {
            _templateRepository = templateRepository;
            _templateContentRepository = templateContentRepository;
            _log = logFactory.CreateLog(this);
        }

        public async Task CreateOrUpdateTemplateAsync(string templateName, string templateBody, Localization localization)
        {
            await _templateContentRepository.SaveContentAsync(templateName, localization, templateBody);

            var info = await _templateRepository.GetTemplateInfoAsync(templateName);

            if (info == null)
            {
                info = new NotificationTemplateInfo(templateName, new List<Localization> {localization});
                await _templateRepository.InsertOrUpdateAsync(info);
            }
            else if (!info.HasLocalization(localization))
            {
                info.AvailableLocalizations.Add(localization);
                await _templateRepository.InsertOrUpdateAsync(info);
            }

            _log.Info("Template updated", new {Name = templateName, Localization = localization.ToString()});
        }

        public async Task<NotificationTemplateInfo> GetTemplateInfoAsync(string templateName)
        {
            var info = await _templateRepository.GetTemplateInfoAsync(templateName);
            return info;
        }

        public async Task<IReadOnlyList<NotificationTemplateInfo>> GetTemplateInfoListAsync()
        {
            var infoList = await _templateRepository.GetListTemlateInfoAsync();
            return infoList;
        }

        public async Task<NotificationTemplateContent> FindTemplateContentAsync(string templateName, Localization local)
        {
            var info = await _templateRepository.GetTemplateInfoAsync(templateName);

            if (info == null || !info.HasLocalization(local))
                return null;

            var content = await _templateContentRepository.GetContentAsync(info.Name, local);

            if (content == null)
                return null;

            var result = new NotificationTemplateContent(info.Name, local, content);
            return result;
        }

        public async Task DeleteIfExistAsync(string templateName, Localization local)
        {
            await _templateRepository.DeleteTemplateLocalizationAsync(templateName, local);
            await _templateContentRepository.DeleteContentAsync(templateName, local);

            _log.Info("Template Localization deleted", new { Name = templateName, Localization = local.ToString() });
        }

        public async Task DeleteIfExistAsync(string templateName)
        {
            await _templateRepository.DeleteTemplateAsync(templateName);
            _log.Info("Template deleted", new { Name = templateName });
        }
    }
}
