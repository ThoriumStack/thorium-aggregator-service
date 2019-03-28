using System;
using System.Threading.Tasks;

namespace Thorium.Aggregator.Infrastructure
{
    public class DistributedCache
    {
        public Task<T> GetOrRun<T>(string key, Func<Task<T>> func)
        {
//            return Task.Factory.StartNew(() =>
//            {
//                try
//                {
//                    return Policy.Handle<RedisException>().Retry().Execute(() =>
//                    {
//                        //todo: finish cache stuff when server updates
//                        /*var serializer = new JilSerializer();
//                        var cacheClient = new StackExchangeRedisCacheClient(serializer, "localhost");
//
//
//                        var objectFromCache = cacheClient.Get<T>(key);
//                        ;
//
//                        if (objectFromCache != null)
//                        {
//                            return objectFromCache;
//                        }*/
//
//                        var task = func();
//                        task.Wait();
//                        T result = task.Result;
//                        //cacheClient.Add(key, result);
//                        return result;
//                    });
//                }
//                catch (RedisException)
//                {
//                    var task = func();
//                    task.Wait();
//                    return task.Result;
//                }
//            });
            throw new NotImplementedException();
        }
    }
}