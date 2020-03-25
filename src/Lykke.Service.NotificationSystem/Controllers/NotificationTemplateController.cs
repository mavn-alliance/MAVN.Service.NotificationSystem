using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.NotificationSystem.Client;
using Lykke.Service.NotificationSystem.Client.Models.NotificationTemplate;
using Lykke.Service.NotificationSystem.Domain.Models;
using Lykke.Service.NotificationSystem.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NotificationSystem.Controllers
{
    /// <summary>
    /// NotificationSystem client API interface for work with templates. 
    /// </summary>
    [ApiVersion("1.0")]
    [Route("/api/templates")]
    [ApiController]
    [Produces("application/json")]
    public class NotificationTemplateController : Controller, INotificationTemplateApi
    {
        private readonly ITemplateService _templateService;

        public NotificationTemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        /// <summary>
        /// Create ot update template.
        /// </summary>
        /// <param name="template">info about template</param>
        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task CreateTemplateAsync(NewTemplateRequest template)
        {
            var local = Localization.From(template.LocalizationCode);

            var existingTemplate = await _templateService.GetTemplateInfoAsync(template.TemplateName);
            if (existingTemplate != null && existingTemplate.HasLocalization(local))
                throw new ValidationApiException(HttpStatusCode.BadRequest, "Template with this localization already exist");
            
            await _templateService.CreateOrUpdateTemplateAsync(template.TemplateName, template.TemplateBody, local);

            Response.StatusCode = (int) HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Update template
        /// </summary>
        /// <param name="template">info about template</param>
        [HttpPut("")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task UpdateTemplateAsync([FromBody] NewTemplateRequest template)
        {
            var local = Localization.From(template.LocalizationCode);

            var existingTemplate = await _templateService.GetTemplateInfoAsync(template.TemplateName);
            if (existingTemplate == null || !existingTemplate.HasLocalization(local))
                throw new ValidationApiException(HttpStatusCode.BadRequest, "Template with this localization not found, please use POST method for add");

            await _templateService.CreateOrUpdateTemplateAsync(template.TemplateName, template.TemplateBody, local);

            Response.StatusCode = (int)HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Get info about all templates.
        /// Info not include template body.
        /// </summary>
        /// <returns>List of template descriptions</returns>
        [HttpGet()]
        [ProducesResponseType(typeof(List<TemplateInfoDto>), (int)HttpStatusCode.OK)]
        public async Task<List<TemplateInfoDto>> GetTemplateteRegistryAsync()
        {
            var list = await _templateService.GetTemplateInfoListAsync();

            var result = list
                .Select(e => new TemplateInfoDto
                {
                    TemplateName = e.Name,
                    AvailableLocalizations = e.AvailableLocalizations.Select(l => LanguageDto.From(l.ToString())).ToList()
                })
                .ToList();

            return result;
        }

        /// <summary>
        /// Get full info template by name and language code.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="templateLanguage">Language of template</param>
        /// <returns>Template with content(body)</returns>
        [HttpGet("{templateName}/{templateLanguage}")]
        [ProducesResponseType(typeof(TemplateResponce), (int)HttpStatusCode.OK)]
        public async Task<TemplateResponce> FindTemplateAsync([FromRoute] string templateName, [FromRoute] string templateLanguage)
        {
            var template = await _templateService.FindTemplateContentAsync(templateName, Localization.From(templateLanguage));

            if (template == null)
            {
                return new TemplateResponce();
            }

            var response = new TemplateResponce
            {
                Template = new TemplateDto
                {
                    TemplateName = template.Name,
                    LocalizationCode = LanguageDto.From(template.Localization.ToString()),
                    TemplateBody = template.Content
                }
            };

            return response;
        }

        /// <summary>
        /// Delete template by name, with all language
        /// </summary>
        /// <param name="templateName">Name of template</param>
        [HttpDelete("{templateName}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task DeleteTemplateAsync([FromRoute] string templateName)
        {
            await _templateService.DeleteIfExistAsync(templateName);

            Response.StatusCode = (int)HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Delete template by name and language
        /// </summary>
        /// <param name="templateName">>Name of template</param>
        /// <param name="templateLanguage">Template language</param>
        /// <returns></returns>
        [HttpDelete("{templateName}/{templateLanguage}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task DeleteTemplateAsync([FromRoute] string templateName, [FromRoute] string templateLanguage)
        {
            await _templateService.DeleteIfExistAsync(templateName, Localization.From(templateLanguage));

            Response.StatusCode = (int)HttpStatusCode.NoContent;
        }
    }
}
