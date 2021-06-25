using System.Threading.Tasks;
using Dapper;
using kafka_poc.Models;
using Microsoft.Data.Sqlite;

namespace kafka_poc.Database
{
    public class PreferenceCreator : PreferenceCreator.ICreatePreferences
    {
        readonly DatabaseConfig _databaseConfig;
        public PreferenceCreator(DatabaseConfig databaseConfig) => _databaseConfig = databaseConfig;

        public async Task<Preference> CreatePreference(PreferenceWithoutInternals preferenceModel)
        {
            using var conn = new SqliteConnection(_databaseConfig.Name);
            await conn.ExecuteAsync("Insert Into Preferences([Type]) Values (@Type);", new { preferenceModel.Type });
            var prefId = await conn.ExecuteScalarAsync<int>("select Max(Id) From Preferences");

            return new Preference(prefId, preferenceModel);
        }

        public interface ICreatePreferences
        {
            Task<Preference> CreatePreference(PreferenceWithoutInternals preference);
        }
    }
}