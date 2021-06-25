using System.Collections.Generic;
using System.Threading.Tasks;
using kafka_poc.Database;
using kafka_poc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace kafka_poc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PreferencesController : ControllerBase
    {
        readonly ILogger<PreferencesController> _logger;
        readonly PreferenceRetriever.IGetPreferencesById _preferenceGetter;
        readonly PreferenceLister.IListPreferences _preferenceLister;
        readonly PreferenceCreationService.IOrchestratePreferenceCreation _preferenceCreationSvc;

        public PreferencesController(
            ILogger<PreferencesController> logger,
            PreferenceRetriever.IGetPreferencesById preferenceGetter,
            PreferenceLister.IListPreferences preferenceLister,
            PreferenceCreationService.IOrchestratePreferenceCreation preferenceCreationSvc)
        {
            _logger = logger;
            _preferenceGetter = preferenceGetter;
            _preferenceLister = preferenceLister;
            _preferenceCreationSvc = preferenceCreationSvc;
        }

        [HttpGet]
        public async Task<dynamic> Get()
        {
            var records = await _preferenceLister.ListPreferences();

            var resultsModel = new List<dynamic>();
            records.ForEach(r => resultsModel.Add(ConvertPreferenceToResult(r)));
            return new { links = new { self = "/preferences" }, data = resultsModel };
        }

        [HttpGet("{id:int}")]
        public async Task<dynamic> Get(int id) => PreferencesController.ConvertPreferenceToResult((Preference)await _preferenceGetter.GetPreference(id));

        [HttpPost]
        public async Task<dynamic> Post([FromBody] PreferenceWithoutInternals preference)
        {
            dynamic response = null;
            await _preferenceCreationSvc.CreatePreferenceAsync(
                preference,
                preferenceId => response = new { links = new { preference = preferenceUri(preferenceId) } });
            return response;
        }

        static dynamic ConvertPreferenceToResult(Preference record) => new
        {
            links = new { self = preferenceUri(record.Id) },
            preference = record
        };

        static string preferenceUri(int id) => $"/preferences/{id}";
    }
}
