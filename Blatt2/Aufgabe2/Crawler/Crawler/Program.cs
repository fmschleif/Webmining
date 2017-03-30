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
            var crawler = new Crawler(args);
            crawler.StartCrawling();
        }
    }
}
