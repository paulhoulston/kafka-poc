using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;

namespace kafka_poc.Database
{
    public class DatabaseBootstrap : DatabaseBootstrap.IDatabaseBootstrap
    {
        public interface IDatabaseBootstrap
        {
            void Setup();
        }

        private readonly DatabaseConfig databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            connection.Open();

            var tableList = new[] { "Preferences" };

            var sqlLiteTables = GetSqliteTables(connection);

            foreach (var tableName in tableList)
            {
                if (!sqlLiteTables.Contains(tableName))
                {
                    if (tableName.Equals("Preferences")) CreatePreferencesTable(connection);
                }
            }
        }

        static void CreatePreferencesTable(SqliteConnection connection) => connection.Execute(
            @"Create Table Preferences (
                 Id INTEGER PRIMARY KEY
                ,[Type] VARCHAR(100) NOT NULL
            );");
        static IEnumerable<string> GetSqliteTables(SqliteConnection connection) => connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table';");
    }
}