using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace kafka_poc.Database
{
    public class DatabaseWrapper : DatabaseWrapper.IAbstractAwayTheDatabase
    {
        static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        readonly DatabaseConfig _databaseConfig;

        public DatabaseWrapper(DatabaseConfig databaseConfig) => _databaseConfig = databaseConfig;

        public interface IAbstractAwayTheDatabase
        {
            Task ExecuteAsync(Func<IDbConnection, Task> sqlFunc, bool useTransaction = true);
        }

        public async Task ExecuteAsync(Func<IDbConnection, Task> sqlFunc, bool useTransaction = true)
        {
            DbTransaction transaction = null;
            await _semaphore.WaitAsync();
            using var db = new SqliteConnection(_databaseConfig.Name);

            if (useTransaction)
            {
                await db.OpenAsync();
                transaction = await db.BeginTransactionAsync();
            }
            await sqlFunc(db);

            if (useTransaction)
                await transaction.CommitAsync();

            _semaphore.Release();
        }
    }
}