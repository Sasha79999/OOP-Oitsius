using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Document(int id, string title, string content)
        {
            Id = id;
            Title = title;
            Content = content;
        }

        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        public static IEnumerable<string> Tokenize(string text)
        {
            return Regex.Matches(text.ToLowerInvariant(), @"\w+").Select(m => m.Value);
        }

        public static string NormalizeWord(string word)
        {
            return string.Join("", Tokenize(word));
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            var stopWords = new HashSet<string> { "a", "an", "the", "is", "are", "was", "were", "in", "on", "at", "to", "for", "of", "and", "or" };
            return words.Where(w => !stopWords.Contains(w));
        }
    }

    public class InvertedIndex
    {
        private Dictionary<string, HashSet<int>> _index;
        private Dictionary<int, Document> _documents;

        public InvertedIndex()
        {
            _index = new Dictionary<string, HashSet<int>>();
            _documents = new Dictionary<int, Document>();
        }

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;

            foreach (string word in TextProcessor.Tokenize(doc.Content))
            {
                if (!_index.ContainsKey(word))
                {
                    _index[word] = new HashSet<int>();
                }

                _index[word].Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            return ToDocuments(GetIds(word));
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            HashSet<int> common = new HashSet<int>(_documents.Keys);

            foreach (string word in words)
            {
                common.IntersectWith(GetIds(word));
            }

            return ToDocuments(common);
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            HashSet<int> all = new HashSet<int>();

            foreach (string word in words)
            {
                all.UnionWith(GetIds(word));
            }

            return ToDocuments(all);
        }

        private HashSet<int> GetIds(string word)
        {
            string key = TextProcessor.NormalizeWord(word);
            return _index.ContainsKey(key) ? _index[key] : new HashSet<int>();
        }

        private List<Document> ToDocuments(HashSet<int> ids)
        {
            return ids.OrderBy(id => id).Select(id => _documents[id]).ToList();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;
    }

    public class PerformanceTest
    {
        public static void CompareSearchMethods(List<Document> documents, string searchWord)
        {
            Stopwatch sw = Stopwatch.StartNew();
            InvertedIndex index = new InvertedIndex();
            foreach (Document doc in documents)
            {
                index.AddDocument(doc);
            }
            long buildTicks = sw.ElapsedTicks;

            sw.Restart();
            string normalizedWord = TextProcessor.NormalizeWord(searchWord);
            List<Document> linearResults = documents.Where(d => TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).ToList();
            long linearTicks = sw.ElapsedTicks;

            sw.Restart();
            List<Document> indexResults = index.SearchSingleWord(searchWord);
            long indexTicks = sw.ElapsedTicks;

            Console.WriteLine("Index build: " + buildTicks + " | Linear search: " + linearTicks + " | Inverted Index: " + indexTicks + " ticks");
            Console.WriteLine("Linear found: " + linearResults.Count + ", Index found: " + indexResults.Count);
        }
    }

    class Program
    {
        static List<Document> CreateTestDocuments()
        {
            return new List<Document>
            {
                new Document(1, "Introduction to C#", "C# is a modern object-oriented programming language developed by Microsoft."),
                new Document(2, "Java Basics", "Java is a popular programming language known for its portability and performance."),
                new Document(3, "Python Overview", "Python is a high-level programming language with simple syntax."),
                new Document(4, "C# Advanced Features", "C# supports LINQ, async/await, and many modern programming paradigms."),
                new Document(5, "Comparing Languages", "C#, Java, and Python are among the most popular programming languages today.")
            };
        }

        static List<Document> GenerateDocuments(int count)
        {
            List<Document> baseDocuments = CreateTestDocuments();
            List<Document> result = new List<Document>();

            for (int i = 1; i <= count; i++)
            {
                Document source = baseDocuments[(i - 1) % baseDocuments.Count];
                result.Add(new Document(i, source.Title + " " + i, source.Content + " topic" + (i % 10)));
            }

            return result;
        }

        static void PrintResults(List<Document> results)
        {
            foreach (Document doc in results)
            {
                Console.WriteLine(doc);
                Console.WriteLine("  " + doc.Content.Substring(0, Math.Min(100, doc.Content.Length)) + "...");
            }

            Console.WriteLine("Found " + results.Count + " documents");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            List<Document> documents = CreateTestDocuments();
            InvertedIndex index = new InvertedIndex();
            foreach (Document doc in documents)
            {
                index.AddDocument(doc);
            }

            Console.WriteLine("Loaded " + index.DocumentCount + " documents");
            Console.WriteLine("Index contains " + index.UniqueWords + " unique words");
            Console.WriteLine();

            Console.WriteLine("Query: programming language (AND)");
            PrintResults(index.SearchMultipleWords("programming", "language"));
            Console.WriteLine("Query: c# async (AND)");
            PrintResults(index.SearchMultipleWords("c#", "async"));
            Console.WriteLine("Query: java python (OR)");
            PrintResults(index.SearchAnyWord("java", "python"));

            Console.WriteLine("Stopwatch: " + Stopwatch.Frequency + " ticks per second");
            Console.WriteLine("Warm-up (5 documents)");
            PerformanceTest.CompareSearchMethods(documents, "java");
            Console.WriteLine();

            foreach (int size in new int[] { 10, 100, 1000, 10000 })
            {
                Console.WriteLine("Test: " + size + " documents, word: java");
                PerformanceTest.CompareSearchMethods(GenerateDocuments(size), "java");
                Console.WriteLine();
            }
        }
    }
}