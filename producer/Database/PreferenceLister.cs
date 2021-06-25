using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using kafka_poc.Models;
using Microsoft.Data.Sqlite;

namespace kafka_poc.Database
{
    public class PreferenceLister : PreferenceLister.IListPreferences
    {
        readonly DatabaseConfig _databaseConfig;
        public PreferenceLister(DatabaseConfig databaseConfig) => _databaseConfig = databaseConfig;


        public interface IListPreferences
        {
            Task<IEnumerable<Preference>> ListPreferences();
        }

        public async Task<IEnumerable<Preference>> ListPreferences()
        {
            using var conn = new SqliteConnection(_databaseConfig.Name);
            return await conn.QueryAsync<Preference>(
                "Select Id, [Type] From Preferences");
        }
    }
}