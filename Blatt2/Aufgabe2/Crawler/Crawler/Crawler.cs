using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler
{
    public class Crawler
    {
        private IConcurentLinkStorage _linksToFollow;
        private MultiValueDictionary<string, Link> _visited;
        private ConcurrentDictionary<Link, Task> _workingTasks;
        private HttpClient _client;

        private int _crawlLimit;

        public IReadOnlyCollection<Link> LinksToFollow => _linksToFollow;
        public IReadOnlyCollection<Link> CurrentlyProcessedLinks => (IReadOnlyCollection<Link>) _workingTasks.Keys;
        public IReadOnlyCollection<string> VisitedSites => (IReadOnlyCollection<string>) _visited.Keys;
        public IEnumerable<Link> LinksFromVisitedSites => _visited.Values.SelectMany(links => links);
        public IReadOnlyCollection<Link> LinksFromSite(string url) => _visited[url];
        public int CrawlLimit => _crawlLimit;

        public event EventHandler<Link> SiteBeforeVisit;
        public event EventHandler<SiteVisitedEventArgs> SiteAfterVisit;

        public Crawler(IEnumerable<string> seedUrls, int crawlLimit = 5000, SearchType searchType = SearchType.BreadthFirstSearch)
        {
            if (searchType == SearchType.BreadthFirstSearch)
                _linksToFollow = new ConcurrentLinkQueue();
            else if (searchType == SearchType.DepthFirstSearch)
                _linksToFollow = new ConcurrentLinkStack();

            _linksToFollow.AddRange(seedUrls.Select(seedUrl => new Link {Target = seedUrl}));

            _visited = new MultiValueDictionary<string, Link>();
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

                if (_linksToFollow.TryGet(out Link curLink))
                {
                    if (!_visited.ContainsKey(curLink.Target))
                        _workingTasks.TryAdd(curLink, CrawlTargetSite(curLink).ContinueWith(task => _workingTasks.TryRemove(curLink, out Task _)));
                    else
                        _visited.Add(curLink.Target, curLink);
                }
            }
            Task.WhenAll(_workingTasks.Values).Wait();
        }

        private async Task CrawlTargetSite(Link link)
        {
            try
            {
                OnSiteBeforeVisit(link);
                _visited.Add(link.Target, link);
                _linksToFollow.AddRange(await GetAllLinkFromSite(link));
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

        public enum SearchType
        {
            BreadthFirstSearch, DepthFirstSearch
        }

        private interface IConcurentLinkStorage : IReadOnlyCollection<Link>
        {
            bool IsEmpty { get; }
            void Add(Link link);
            void AddRange(IEnumerable<Link> links);
            bool TryGet(out Link link);
        }

        private class ConcurrentLinkQueue : ConcurrentQueue<Link>, IConcurentLinkStorage
        {
            public new bool IsEmpty => base.IsEmpty;
            public void Add(Link link) => Enqueue(link);
            public void AddRange(IEnumerable<Link> links) => links.ForEach(Enqueue);
            public bool TryGet(out Link link) => TryDequeue(out link);
        }

        private class ConcurrentLinkStack : ConcurrentStack<Link>, IConcurentLinkStorage
        {
            public new bool IsEmpty => base.IsEmpty;
            public void Add(Link link) => Push(link);
            public void AddRange(IEnumerable<Link> links) => links.ForEach(Push);
            public bool TryGet(out Link link) => TryPop(out link);
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
