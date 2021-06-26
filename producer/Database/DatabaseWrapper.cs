using System;
using System.Data;
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
            Task ExecuteAsync(Func<IDbConnection, Task> sqlFunc);
        }

        public async Task ExecuteAsync(Func<IDbConnection, Task> sqlFunc)
        {
            await _semaphore.WaitAsync();
            using var db = new SqliteConnection(_databaseConfig.Name);
            await db.OpenAsync();

            using var trans = await db.BeginTransactionAsync();

            await sqlFunc(db);

            await trans.CommitAsync();
            _semaphore.Release();
        }
    }
}