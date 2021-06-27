using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using kafka_poc.Models;

namespace kafka_poc.Database
{
    public class PreferenceLister : PreferenceLister.IListPreferences
    {
        const string LIST_SQL = "Select Id, [Type], Created, LastModified From Preferences";
        readonly DatabaseWrapper.IAbstractAwayTheDatabase _dbWrapper;
        public PreferenceLister(DatabaseWrapper.IAbstractAwayTheDatabase dbWrapper) => _dbWrapper = dbWrapper;

        public interface IListPreferences
        {
            Task<IEnumerable<Preference>> ListPreferences();
        }

        public async Task<IEnumerable<Preference>> ListPreferences()
        {
            IEnumerable<Preference> preferences = null;
            await _dbWrapper.ExecuteAsync(async db => preferences = await db.QueryAsync<Preference>(LIST_SQL));
            return preferences;
        }
    }
}