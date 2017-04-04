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
        private static ConcurrentDictionary<Link, string> ContentDict = new ConcurrentDictionary<Link, string>();

        static void Main(string[] args)
        {
            File.Delete("FollowedLinks.txt");
            File.Delete("NotFollowedLinks.txt");

            var crawler = new Crawler(args, 1000);

            crawler.SiteBeforeVisit += (sender, link) =>
            {
                File.AppendAllText("FollowedLinks.txt", $"{link}\n");
                Console.Write($"\rProgress: {crawler.VisitedSites.Count}/{crawler.CrawlLimit} ({100.0 * crawler.VisitedSites.Count / crawler.CrawlLimit:F2}%) [OpenTasks: {crawler.CurrentlyProcessedLinks.Count}, LinkInQueue: {crawler.LinksToFollow.Count}]        ");
            };

            crawler.SiteAfterVisit += async (sender, eventArgs) =>
            {
                Console.Write($"\rProgress: {crawler.VisitedSites.Count}/{crawler.CrawlLimit} ({100.0 * crawler.VisitedSites.Count / crawler.CrawlLimit:F2}%) [OpenTasks: {crawler.CurrentlyProcessedLinks.Count}, LinkInQueue: {crawler.LinksToFollow.Count}]        ");
                ContentDict.TryAdd(eventArgs.Link, await eventArgs.Response.Content.ReadAsStringAsync());
            };

            crawler.StartCrawling();

            File.WriteAllText("NotFollowedLinks.txt", String.Join("\n", crawler.LinksToFollow));
        }
    }
}
