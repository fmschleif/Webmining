using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Lucene
{
    class LuceneService
    {
        private Analyzer analyzer = new WhitespaceAnalyzer();
        private Directory luceneIndexDirectory;
        private IndexWriter writer;
        private string indexPath = @"c:\temp\LuceneIndex";

        public LuceneService()
        {
            InitialiseLucene();
        }

        private void InitialiseLucene()
        {
            if (System.IO.Directory.Exists(indexPath))
            {
                System.IO.Directory.Delete(indexPath, true);
            }

            luceneIndexDirectory = FSDirectory.Open(indexPath);
            writer = new IndexWriter(luceneIndexDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        public void BuildIndex(IEnumerable<NewsgroupFile> dataToIndex)
        {
            using (writer)
            {
                foreach (var newsgroupFile in dataToIndex)
                {
                    Document doc = new Document();
                    doc.Add(new Field("FileName",
                        newsgroupFile.FileName,
                        Field.Store.YES,
                        Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("Content",
                        newsgroupFile.Content,
                        Field.Store.YES,
                        Field.Index.ANALYZED));
                    writer.AddDocument(doc);
                }
                writer.Optimize();
            }
        }

        public IEnumerable<NewsgroupFile> Search(string searchTerm)
        {
            IndexSearcher searcher = new IndexSearcher(luceneIndexDirectory);
            QueryParser parser = new QueryParser(Net.Util.Version.LUCENE_30, "Content", analyzer);
            
            Query query = parser.Parse(searchTerm);
            var hitsFound = searcher.Search(query, 10);

            return hitsFound.ScoreDocs.Select(doc => new NewsgroupFile
            {
                Score = doc.Score,
                FileName = searcher.Doc(doc.Doc).Get("FileName"),
                Content = searcher.Doc(doc.Doc).Get("Content")
            }).OrderByDescending(x => x.Score);
        }
    }
}
