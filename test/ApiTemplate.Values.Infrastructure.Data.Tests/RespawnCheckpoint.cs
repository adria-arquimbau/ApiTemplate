using Respawn;

namespace ApiTemplate.Values.Infrastructure.Data.Tests
{
    public class RespawnCheckpoint
    {
        public static Checkpoint Checkpoint()
        {
            return new Checkpoint
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] {"public"},
                TablesToIgnore = new[] {"__EFMigrationsHistory"}
            };
        }
    }
}