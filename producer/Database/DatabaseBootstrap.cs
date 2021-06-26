using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;

namespace kafka_poc.Database
{
    public class DatabaseBootstrap : DatabaseBootstrap.IDatabaseBootstrap
    {
        static readonly Dictionary<string, string> tableList = new Dictionary<string, string>
            {
                {
                    "Preferences",
                    @"Create Table Preferences (
                        Id Integer Primary Key Not Null
                        ,[Type] VARCHAR(100) Not Null
                    );"
                },
                {
                    "Outbox",
                     @"Create Table Outbox (
                        Id Integer Primary Key Not Null
                        ,[TopicName] Varchar(255) Not Null
                        ,[Data] Varchar Not Null
                        ,[Created] TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );"
                },
                {
                    "OutboxArchive",
                     @"Create Table OutboxArchive (
                        Id Integer Primary Key Not Null
                        ,[TopicName] Varchar(255) Not Null
                        ,[Data] Varchar Not Null
                        ,[Created] TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );"
                }
            };

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
            using var db = new SqliteConnection(databaseConfig.Name);
            db.Open();

            var sqlLiteTables = GetSqliteTables(db);

            foreach (var tableName in tableList.Keys)
            {
                if (!sqlLiteTables.Contains(tableName))
                    db.Execute(tableList[tableName]);
            }
        }

        static IEnumerable<string> GetSqliteTables(SqliteConnection db) => db.Query<string>("SELECT name FROM sqlite_master WHERE type='table';");
    }
}