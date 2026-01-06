using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphDailyTaskLogService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphDailyTaskLogService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        // REN GRAPH: alle daily task logs for en task
        public async Task<IEnumerable<DailyTaskLogDetailDto>> GetLogsForTaskAsync(int taskId)
        {
            var result = new List<DailyTaskLogDetailDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (t:Task {id: $taskId})<-[:FOR_TASK]-(dtl:DailyTaskLog)
                RETURN dtl.id AS logId, dtl.timeToComplete AS timeToComplete
                ORDER BY dtl.id
            ", new { taskId });

            await foreach (var record in cursor)
            {
                result.Add(new DailyTaskLogDetailDto
                {
                    LogId = record["logId"].As<int>(),
                    TimeToComplete = record["timeToComplete"].As<string>()
                });
            }

            return result;
        }

        // UPDATE
        public async Task<bool> UpdateDailyTaskLogNodeAsync(int id, string? timeToComplete)
        {
            await using var session = _driver.AsyncSession();

            if (string.IsNullOrEmpty(timeToComplete))
                return false;

            var result = await session.RunAsync(@"
                MATCH (dtl:DailyTaskLog {id: $id})
                SET dtl.timeToComplete = $timeToComplete
            ", new { id, timeToComplete });

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // DELETE
        public async Task<bool> DeleteDailyTaskLogNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (dtl:DailyTaskLog {id: $id})
                DETACH DELETE dtl
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }

        // SYNC
        public async Task MirrorDailyTaskLogFromSqlAsync(int logId)
        {
            var log = await _sqlContext.daily_task_logs
                .Include(l => l.task)
                .FirstOrDefaultAsync(l => l.id == logId);

            if (log == null)
                return;

            await using var session = _driver.AsyncSession();

            const string logCypher = @"
MERGE (dtl:DailyTaskLog {id: $id})
SET dtl.timeToComplete = $timeToComplete
WITH dtl
MATCH (t:Task {id: $taskId})
MERGE (dtl)-[:FOR_TASK]->(t);
";

            await session.RunAsync(logCypher, new
            {
                id = log.id,
                timeToComplete = log.time_to_finish,
                taskId = log.task_id
            });
        }
    }

    public class DailyTaskLogDetailDto
    {
        public int LogId { get; set; }
        public string? TimeToComplete { get; set; }
    }
}