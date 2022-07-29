using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proxy_Scraper
{
    internal class Scraper
    {
        public static int proxyCount { set; get; }
        public static async Task<List<string>> scrapeProxies(int threads)
        {
            List<string> proxies = new List<string>();
            //Update the apis for the scrape separated with ','
            var proxyUrlToParse =
                "";
            string[] urls = proxyUrlToParse.Split(',');
            var blockingCollection = new BlockingCollection<string>();
            var client = new HttpClient();

            IEnumerable<Task> producerTasks = urls.Select(url => Task.Run(async () =>
            {
                try
                {
                    blockingCollection.Add(await client.GetStringAsync(url));
                }
                catch (Exception e)
                {

                }
            })).ToArray();

            Task<List<string>> consumerTask = Task.Run(() => blockingCollection
                .GetConsumingEnumerable()
                .AsParallel()
                .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                .WithDegreeOfParallelism(threads)
                .SelectMany(c =>
                {
                    string[] matches = Regex.Matches(c, @"\d+\.\d+\.\d+\.\d+:\d+").Cast<Match>()
                        .Select(m => m.Value)
                        .ToArray();
                    return matches;
                }).ToList());

            await Task.WhenAll(producerTasks);
            blockingCollection.CompleteAdding();
            proxies = await consumerTask;
            proxies = proxies.Distinct().ToList();
            proxyCount = proxies.Count;
            return proxies;
        }
    }
}
