using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using kafka_poc.Models;

namespace kafka_poc.Database
{
    public class OutboxLister : OutboxLister.IGetAllOutboxItems
    {
        const string GET_SQL = "Select Id, TopicName, Data From Outbox;";
        public interface IGetAllOutboxItems
        {
            Task<IEnumerable<OutboxModel>> GetAllAsync(IDbConnection db);
        }

        public async Task<IEnumerable<OutboxModel>> GetAllAsync(IDbConnection db) => await db.QueryAsync<OutboxModel>(GET_SQL);
    }
}