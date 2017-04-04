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

        public IReadOnlyCollection<Link> LinksToFollow => _linksToFollow;
        public IReadOnlyCollection<Link> CurrentlyProcessedLinks => (IReadOnlyCollection<Link>) _workingTasks.Keys;
        public IReadOnlyCollection<string> VisitedSites => _visited;
        public int CrawlLimit => _crawlLimit;

        public event EventHandler<Link> SiteBeforeVisit;
        public event EventHandler<SiteVisitedEventArgs> SiteAfterVisit;

        public Crawler(IEnumerable<string> seedUrls, int crawlLimit = 5000)
        {
            _linksToFollow = new ConcurrentQueue<Link>(seedUrls.Select(s => new Link { Target = s }));
            _visited = new HashSet<string>();
            _workingTasks = new ConcurrentDictionary<Link, Task>();

            _client = new HttpClient(new HttpClientHandler
            {
                MaxConnectionsPerServer = int.MaxValue,
                AllowAutoRedirect = true,
            });

            _crawlLimit = crawlLimit;
        }

        public void StartCrawling()
        {
            while (_visited.Count < _crawlLimit)
            {
                while (_linksToFollow.IsEmpty && _workingTasks.Count > 0)
                {
                    Task.WhenAny(_workingTasks.Values).Wait();
                }

                if (_linksToFollow.IsEmpty && _workingTasks.Count == 0)
                    break;

                if (!_linksToFollow.TryDequeue(out Link curLink) || _visited.Contains(curLink.Target))
                    continue;

                _workingTasks.TryAdd(curLink, CrawlTargetSite(curLink).ContinueWith(task => _workingTasks.TryRemove(curLink, out Task _)));
            }
            Task.WhenAll(_workingTasks.Values).Wait();
        }

        private async Task CrawlTargetSite(Link link)
        {
            try
            {
                OnSiteBeforeVisit(link);
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
                OnSiteAfterVisit(link, response);

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

        public class SiteVisitedEventArgs
        {
            public Link Link { get; }
            public HttpResponseMessage Response { get; }

            public SiteVisitedEventArgs(Link link, HttpResponseMessage response)
            {
                Link = link;
                Response = response;
            }
        }

        protected virtual void OnSiteBeforeVisit(Link link)
        {
            SiteBeforeVisit?.Invoke(this, link);
        }

        protected virtual void OnSiteAfterVisit(Link link, HttpResponseMessage response)
        {
            SiteAfterVisit?.Invoke(this, new SiteVisitedEventArgs(link, response));
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
