using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Lucene
{
    class Program
    {

        static void Main(string[] args)
        {
            var data = new DirectoryInfo(args[0])
                .GetFiles("*.*", SearchOption.AllDirectories)
                .Select(info => new NewsgroupFile { Content = File.ReadAllText(info.FullName), FileName = info.FullName });

            var service = new LuceneService();
            service.BuildIndex(data);
            var result = service.Search("love").ToList();

        }
    }
}
