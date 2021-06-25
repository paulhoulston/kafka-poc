using System.Threading.Tasks;
using Dapper;
using kafka_poc.Models;
using Microsoft.Data.Sqlite;

namespace kafka_poc.Database
{
    public class PreferenceRetriever : PreferenceRetriever.IGetPreferencesById
    {
        readonly DatabaseConfig _databaseConfig;
        public PreferenceRetriever(DatabaseConfig databaseConfig) => _databaseConfig = databaseConfig;

        public interface IGetPreferencesById
        {
            Task<Preference> GetPreference(int id);
        }

        public async Task<Preference> GetPreference(int id)
        {
            using var conn = new SqliteConnection(_databaseConfig.Name);
            return await conn.QuerySingleOrDefaultAsync<Preference>(
                "Select Id, [Type] From Preferences Where Id = @id;",
                 new { id });
        }
    }
}