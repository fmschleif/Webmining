using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler
{
    class Program
    {
        private static Dictionary<Link, string> ContentDict = new Dictionary<Link, string>();

        static void Main(string[] args)
        {
            var crawler = new Crawler(args, 1000);

            crawler.SiteVisited += async (sender, eventArgs) =>
            {
                ContentDict[eventArgs.Link] = await eventArgs.Response.Content.ReadAsStringAsync();
            };

            crawler.StartCrawling();
        }
    }
}
