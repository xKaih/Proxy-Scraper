using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proxy_Scraper
{
    internal class Program
    {
        public static async Task Main()
        {
            var sw = Stopwatch.StartNew();
            await Scraper.scrapeProxies(100);
            sw.Stop();
            Console.WriteLine($"{Scraper.proxyCount} proxies in {sw.ElapsedMilliseconds}ms");
            Console.ReadLine();
        }
        
    }
}
