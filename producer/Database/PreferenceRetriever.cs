using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using kafka_poc.Models;

namespace kafka_poc.Database
{
    public class PreferenceRetriever : PreferenceRetriever.IGetPreferencesById
    {
        const string GET_PREFERENCE_BY_ID_SQL = @"Select
                                                    Id
                                                    ,[Type]
                                                    ,Created
                                                    ,LastModified
                                                From
                                                    Preferences
                                                Where
                                                    Id = @id;";
        readonly DatabaseWrapper.IAbstractAwayTheDatabase _dbWrapper;
        public PreferenceRetriever(DatabaseWrapper.IAbstractAwayTheDatabase dbWrapper) => _dbWrapper = dbWrapper;

        public interface IGetPreferencesById
        {
            Task GetPreferenceAsync(
                int preferenceId,
                Action<PreferenceWithoutId> onPreferenceFound,
                Action onPreferenceNotFound);
        }

        public async Task GetPreferenceAsync(
            int preferenceId,
            Action<PreferenceWithoutId> onPreferenceFound,
            Action onPreferenceNotFound) =>
                await _dbWrapper.ExecuteAsync(db => GetPreference(db, preferenceId, onPreferenceFound, onPreferenceNotFound), false);

        async Task GetPreference(
            IDbConnection db,
            int preferenceId,
            Action<PreferenceWithoutId> onPreferenceFound,
            Action onPreferenceNotFound)
        {
            var preference = await db.QuerySingleOrDefaultAsync<Preference>(GET_PREFERENCE_BY_ID_SQL, new { id = preferenceId });

            if (preference == null)
                onPreferenceNotFound();
            else
                onPreferenceFound(preference);

        }
    }
}