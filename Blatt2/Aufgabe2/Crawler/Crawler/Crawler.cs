using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler
{
    public class Crawler
    {
        ConcurrentQueue<Link> _linksToFollow;

        public Crawler(IEnumerable<string> seedUrls)
        {
            _linksToFollow = new ConcurrentQueue<Link>(seedUrls.Select(s => new Link { Target = s }));
        }

        public void StartCrawling()
        {
            File.Delete("CrawlerResult.txt");
            var visited = new HashSet<string>();

            for (int i = 0; i < 1000; i++)
            {
                if (_linksToFollow.IsEmpty)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        Thread.Sleep(100);
                        if (!_linksToFollow.IsEmpty)
                            break;
                    }
                }

                if (!_linksToFollow.TryDequeue(out Link curLink))
                    continue;

                Console.WriteLine($"Visitng: {curLink.Target} (in queue: {_linksToFollow.Count})");
                if (visited.Contains(curLink.Target))
                    continue;

                File.AppendAllText("CrawlerResult.txt", $"{curLink}\n");

                visited.Add(curLink.Target);

                try
                {
                    GetAllLinkFromSite(curLink).ContinueWith(task =>
                    {
                        try
                        {
                            var linksFromPage = task.Result.ToArray();
                            _linksToFollow.EnqueueRange(linksFromPage);
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
                .Select(x => CleanLink(new Link { Source = link.Target, Target = x }))
                .Where(x => x != null);
        }

        public static Link CleanLink(Link link)
        {
            if (Uri.TryCreate(new Uri(link.Source), link.Target, out Uri newTargetUri))
            {
                newTargetUri = new UriBuilder(newTargetUri) { Query = "" }.Uri;
                return new Link { Source = link.Source, Target = newTargetUri.AbsoluteUri };
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
