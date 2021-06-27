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
            records.NullSafeForEach(r => resultsModel.Add(ConvertPreferenceToResult(r.Id, r)));
            return new { links = new { self = "/preferences" }, data = resultsModel };
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            IActionResult result = null;

            await _preferenceGetter.GetPreference(
                id,
                preference => result = Ok(ConvertPreferenceToResult(id, preference)),
                () => result = NotFound(new Error
                {
                    ErrorMessage = $"Preference not found [id: {id}]"
                }));

            return result;
        }

        [HttpPost]
        public async Task<dynamic> Post([FromBody] PreferenceCreationModel preference)
        {
            dynamic response = null;
            await _preferenceCreationSvc.CreatePreferenceAsync(
                preference,
                preferenceId => response = new { links = new { preference = preferenceUri(preferenceId) } });
            return response;
        }

        static dynamic ConvertPreferenceToResult(int preferenceId, PreferenceWithoutId preference) => new
        {
            links = new { self = preferenceUri(preferenceId) },
            preference = preference
        };

        static string preferenceUri(int id) => $"/preferences/{id}";
    }
}
