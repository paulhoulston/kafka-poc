using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using kafka_poc.Models;

namespace kafka_poc.Database
{
    public class OutboxArchiver : OutboxArchiver.IArchiveOutboxItems
    {
        public interface IArchiveOutboxItems
        {
            Task ArchiveOutboxEntry(IDbConnection db, OutboxModel item);
        }

        const string INSERT_INTO_OUTBOX_ARCHIVE_SQL = @"Insert Into OutboxArchive(
                                                            TopicName
                                                            ,Data)
                                                        Select
                                                            TopicName
                                                            ,Data
                                                        From
                                                            Outbox
                                                        Where
                                                            Id = @Id";
        const string DELETE_FROM_OUTBOX_SQL = @"Delete From
                                                    Outbox
                                                Where
                                                    Id = @Id";

        public async Task ArchiveOutboxEntry(IDbConnection db, OutboxModel item)
        {
            await db.ExecuteAsync(
                INSERT_INTO_OUTBOX_ARCHIVE_SQL, new
                {
                    Id = item.Id,
                    Created = DateTime.UtcNow
                });

            await db.ExecuteAsync(DELETE_FROM_OUTBOX_SQL, new { item.Id });
        }
    }
}