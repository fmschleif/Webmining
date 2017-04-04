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
            File.Delete("DeinzerGraph.txt");

            var crawler = new Crawler(args, 10);

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

            ExportAsDeinzerGraph(crawler, "DeinzerGraph.txt");
        }

        public static void ExportAsDeinzerGraph(Crawler crawler, string filePath)
        {
            File.WriteAllText(filePath, "# Websites in the Graph\n");
            File.AppendAllText(filePath, string.Join("\n", crawler.VisitedSites.Select(s => $"knoten {s}")));

            File.AppendAllText(filePath, "\n\n# Links between Websites\n");
            File.AppendAllText(filePath, string.Join("\n", crawler.LinksFromVisitedSites.Where(link => link.Source != null).Select(link => $"kante {link.Source} {link.Target}")));
        }
    }
}
