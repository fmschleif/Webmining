using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
        private ConcurrentQueue<Link> _linksToFollow;
        private HashSet<string> _visited;
        private ConcurrentDictionary<Link, Task> _workingTasks;
        private HttpClient _client;

        private int _crawlLimit;

        public Crawler(IEnumerable<string> seedUrls)
        {
            _linksToFollow = new ConcurrentQueue<Link>(seedUrls.Select(s => new Link { Target = s }));
            _visited = new HashSet<string>();
            _workingTasks = new ConcurrentDictionary<Link, Task>();

            _client = new HttpClient(new HttpClientHandler
            {
                MaxConnectionsPerServer = int.MaxValue,
                AllowAutoRedirect = true,
            });

            _crawlLimit = 5000;
        }

        public void StartCrawling()
        {
            File.Delete("CrawlerResult.txt");

            for (int i = 0; i < _crawlLimit; i++)
            {
                Console.Write($"\rProgress: {i}/{_crawlLimit} ({(double)i/_crawlLimit:F2}%) [OpenTasks: {_workingTasks.Count}, LinkInQueue: {_linksToFollow.Count}]");
                while (_linksToFollow.IsEmpty && _workingTasks.Count > 0)
                {
                    Task.WhenAny(_workingTasks.Values).Wait();
                }

                if (_linksToFollow.IsEmpty && _workingTasks.Count == 0)
                    break;

                if (!_linksToFollow.TryDequeue(out Link curLink) || _visited.Contains(curLink.Target))
                    continue;

                //Console.WriteLine($"Visitng: {curLink.Target} (InQueue: {_linksToFollow.Count}, Visited: {_visited.Count})");

                File.AppendAllText("CrawlerResult.txt", $"{curLink}\n");

                _workingTasks.TryAdd(curLink, CrawlTargetSite(curLink).ContinueWith(task => _workingTasks.TryRemove(curLink, out Task _)));
            }
            while (!_workingTasks.IsEmpty)
            {
                Task.WhenAll(_workingTasks.Values).Wait(200);
                Console.Write($"\rProgress: {_crawlLimit}/{_crawlLimit} ({(double)_crawlLimit / _crawlLimit:F2}%) [OpenTasks: {_workingTasks.Count}, LinkInQueue: {_linksToFollow.Count}]");
            }
        }

        private async Task CrawlTargetSite(Link link)
        {
            try
            {
                _visited.Add(link.Target);
                _linksToFollow.EnqueueRange(await GetAllLinkFromSite(link));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to crawl site: {link.Target} ({e.Message})");
            }
        }

        private async Task<IEnumerable<Link>> GetAllLinkFromSite(Link link)
        {
            using (var response = await _client.GetAsync(link.Target))
            {

                if (response.Headers.Location != null)
                {
                    return new[] {CleanLink(new Link {Source = link.Target, Target = response.Headers.Location.AbsoluteUri})};
                }

                if (response.IsSuccessStatusCode)
                {
                    var document = new HtmlDocument();
                    document.LoadHtml(await response.Content.ReadAsStringAsync());

                    return document.DocumentNode.Descendants("a")
                        .Select(x => x?.Attributes["href"]?.Value)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => CleanLink(new Link {Source = link.Target, Target = x}))
                        .Where(x => x != null);
                }
                else
                {
                    throw new HttpRequestException($"{response.StatusCode} ({response.ReasonPhrase})");
                }
            }
        }

        private static Link CleanLink(Link link)
        {
            if (Uri.TryCreate(new Uri(link.Source), link.Target, out Uri newTargetUri))
            {
                newTargetUri = new UriBuilder(newTargetUri) { Query = "" }.Uri;
                return new Link { Source = link.Source, Target = newTargetUri.AbsoluteUri };
            }
            else
            {
                Debug.WriteLine($"Failed to normalize Uri for link '{link}'");
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
