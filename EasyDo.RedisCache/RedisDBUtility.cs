using EasyDo.Configuration;
using EasyDo.Dependency;
using ServiceStack.Redis;

namespace EasyDo.RedisCache
{
    public class RedisDbUtility : ISingletonDependency
    {
        private readonly PooledRedisClientManager _prcm;
        public RedisDbUtility(EasyDoConfiguration easyDoConfiguration)
        {
            _prcm = CreateManager(easyDoConfiguration.RedisConnectionStrings(), easyDoConfiguration.RedisConnectionStrings());
           // ServiceStack.Licensing.RegisterLicense(@"2238-e1JlZjoyMjM4LE5hbWU6MzFIdWlZaSxUeXBlOlJlZGlzQnVzaW5lc3MsSGFzaDp1N01oOGgwdmx0RlJ1UU9LOUZMWEszalNJWVB4UUZtN0tmK3FtdG9sWE9iTGRkdjIrbEtUSTlKYnZkTE90Q2g5QnNDOWFEQTZJTSswdjhucDdjMzVyZUQyVExTYWRFdHNWSzFRV3cxTkhSWjhpWDJFSGhDdit2NHZmM1NYaVIxMDVwTG1iK0ZOZFBKYlpWSzYzK3BOR0tLbldSV3prc2FJd0ZEMmJTbGltdjg9LEV4cGlyeToyMDE2LTAyLTExfQ==");
        }

        private PooledRedisClientManager CreateManager(string[] readWriteHosts, string[] readOnlyHosts)
        {
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts,
                new RedisClientManagerConfig
                {
                    MaxWritePoolSize = readWriteHosts.Length * 80,
                    MaxReadPoolSize = readOnlyHosts.Length * 80,
                    AutoStart = true,
                });
        }

        public IRedisClient GetRedisClient()
        {
            return _prcm.GetClient();
        }
    }
}
