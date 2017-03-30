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
        static void Main(string[] args)
        {
            File.Delete("../../../CrawlerResult.txt");
            var queue = new ConcurrentQueue<Link>(args.Select(s => new Link{Target = s}));
            var visited = new HashSet<string>();

            for (int i = 0; i < 1000; i++)
            {
                if (queue.IsEmpty)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        Thread.Sleep(100);
                        if (!queue.IsEmpty)
                            break;
                    }
                }

                if (!queue.TryDequeue(out Link curLink))
                    continue;

                Console.WriteLine($"Visitng: {curLink} (in queue: {queue.Count})");
                if (visited.Contains(curLink.Target))
                    continue;
                
                File.AppendAllText("../../../CrawlerResult.txt", $"{curLink}\n");

                visited.Add(curLink.Target);

                try
                {
                    GetAllLinkFromSite(curLink).ContinueWith(task =>
                    {
                        try
                        {
                            var linksFromPage = task.Result.ToArray();
                            queue.EnqueueRange(linksFromPage);
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine($"Unable to Requenst from for link: {curLink}");
                        }
                    });
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Unable to Requenst from for link: {curLink}");
                }
            }
        }

        public static async Task<IEnumerable<Link>> GetAllLinkFromSite(Link link)
        {
            var client = new HttpClient();
            var pageContent = await client.GetStringAsync(link.Target);

            var document = new HtmlDocument();
            document.LoadHtml(pageContent);

            return document.DocumentNode.Descendants("a")
                .Select(x => x?.Attributes["href"]?.Value)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => CleanLink(new Link {Source = link.Target, Target = x}))
                .Where(x => x != null);
        }

        public static Link CleanLink(Link link)
        {
            if (Uri.TryCreate(new Uri(link.Source), link.Target, out Uri newTargetUri))
            {
                newTargetUri = new UriBuilder(newTargetUri) {Query = ""}.Uri;
                Console.WriteLine($"\tNormalized Url for link '{link}': {newTargetUri.AbsoluteUri}");
                return new Link {Source = link.Source, Target = newTargetUri.AbsoluteUri};
            }
            else
            {
                Console.Error.WriteLine($"Failed to normalize Uri for link '{link}'");
                return null;
            }
        }
    }

    public class Link
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public override string ToString()
        {
            return $"{{{Source}}} => {{{Target}}}";
        }
    }
}
