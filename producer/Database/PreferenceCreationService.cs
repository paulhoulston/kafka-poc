using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using kafka_poc.Models;
using Newtonsoft.Json;

namespace kafka_poc.Database
{
    public class PreferenceCreationService : PreferenceCreationService.IOrchestratePreferenceCreation
    {
        readonly DatabaseWrapper.IAbstractAwayTheDatabase _dbAbstractor;
        public interface IOrchestratePreferenceCreation
        {
            Task CreatePreferenceAsync(PreferenceCreationModel preference, Action<int> onPreferenceCreated);
        }

        public PreferenceCreationService(DatabaseWrapper.IAbstractAwayTheDatabase dbAbstractor) => _dbAbstractor = dbAbstractor;

        public async Task CreatePreferenceAsync(
            PreferenceCreationModel preference,
            Action<int> onPreferenceCreated)
        {
            await _dbAbstractor.ExecuteAsync(db => CreatePreferenceAsync(db, preference, onPreferenceCreated));
        }

        async Task CreatePreferenceAsync(IDbConnection db, PreferenceCreationModel preference, Action<int> onPreferenceCreated)
        {
            var preferenceId = await new PreferenceCreator().CreatePreferenceAsync(db, preference);
            await new OutboxWriter<Preference>().QueueMessageAsync(db, "Preferences", new Preference(preferenceId, preference));
            onPreferenceCreated(preferenceId);
        }

        class PreferenceCreator
        {
            const string INSERT_SQL = "Insert Into Preferences([Type]) Values (@Type);";
            const string GET_MAX_ID_SQL = "select Max(Id) From Preferences";

            public async Task<int> CreatePreferenceAsync(IDbConnection db, PreferenceCreationModel preferenceModel)
            {
                await db.ExecuteAsync(INSERT_SQL, new { preferenceModel.Type });
                return await db.ExecuteScalarAsync<int>(GET_MAX_ID_SQL);
            }
        }

        class OutboxWriter<T>
        {
            const string INSERT_SQL = "Insert Into Outbox(TopicName, Data) Values (@TopicName, @Data);";

            public async Task QueueMessageAsync(IDbConnection db, string topicName, T data) =>
                await db.ExecuteAsync(INSERT_SQL, new
                {
                    TopicName = topicName,
                    Data = SerializeData(data)
                });

            string SerializeData(T data) => JsonConvert.SerializeObject(data);
        }
    }
}