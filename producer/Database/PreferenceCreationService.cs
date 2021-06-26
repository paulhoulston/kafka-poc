using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using kafka_poc.Models;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace kafka_poc.Database
{
    public class PreferenceCreationService : PreferenceCreationService.IOrchestratePreferenceCreation
    {
        readonly DatabaseConfig _databaseConfig;

        public interface IOrchestratePreferenceCreation
        {
            Task CreatePreferenceAsync(PreferenceWithoutInternals preference, Action<int> onPreferenceCreated);
        }

        public PreferenceCreationService(DatabaseConfig databaseConfig) => _databaseConfig = databaseConfig;

        public async Task CreatePreferenceAsync(
            PreferenceWithoutInternals preference,
            Action<int> onPreferenceCreated)
        {
            await SemaphoreManager.Lock(SemaphoreManager.Keys.Preferences);
            using var db = new SqliteConnection(_databaseConfig.Name);
            await db.OpenAsync();

            using var trans = await db.BeginTransactionAsync();

            var preferenceId = await new PreferenceCreator().CreatePreference(db, preference);
            await new OutboxWriter<Preference>().QueueMessage(db, "Preferences", new Preference(preferenceId, preference));

            await trans.CommitAsync();
            SemaphoreManager.Release(SemaphoreManager.Keys.Preferences);

            onPreferenceCreated(preferenceId);
        }

        class PreferenceCreator
        {
            const string INSERT_SQL = "Insert Into Preferences([Type]) Values (@Type);";
            const string GET_MAX_ID_SQL = "select Max(Id) From Preferences";

            public async Task<int> CreatePreference(IDbConnection db, PreferenceWithoutInternals preferenceModel)
            {
                await db.ExecuteAsync(INSERT_SQL, new { preferenceModel.Type });
                return await db.ExecuteScalarAsync<int>(GET_MAX_ID_SQL);
            }
        }

        class OutboxWriter<T>
        {
            const string INSERT_SQL = "Insert Into Outbox(TopicName, Data) Values (@TopicName, @Data);";

            public async Task QueueMessage(IDbConnection db, string topicName, T data) =>
                await db.ExecuteAsync(INSERT_SQL, new
                {
                    TopicName = topicName,
                    Data = SerializeData(data)
                });

            string SerializeData(T data) => JsonConvert.SerializeObject(data);
        }
    }
}