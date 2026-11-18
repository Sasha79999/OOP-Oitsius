using System;
using System.Collections.Generic;

namespace IndependentWork14
{
    public class FileMetadata
    {
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; }

        public FileMetadata(string fileName, long size, DateTime lastModified, string hash)
        {
            FileName = fileName;
            Size = size;
            LastModified = lastModified;
            Hash = hash;
        }
    }

    public class FileMetadataCache
    {
        private Dictionary<string, FileMetadata> _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

        public int Count
        {
            get { return _cache.Count; }
        }

        public void Add(FileMetadata metadata)
        {
            _cache[metadata.FileName] = metadata;
        }

        public FileMetadata? Get(string fileName)
        {
            if (_cache.TryGetValue(fileName, out FileMetadata? metadata))
            {
                return metadata;
            }

            return null;
        }

        public bool Remove(string fileName)
        {
            return _cache.Remove(fileName);
        }

        public bool Contains(string fileName)
        {
            return _cache.ContainsKey(fileName);
        }
    }

    public class TagManager
    {
        private Dictionary<string, HashSet<string>> _articleTags = new Dictionary<string, HashSet<string>>();

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.ContainsKey(articleId))
            {
                _articleTags[articleId] = new HashSet<string>();
            }

            _articleTags[articleId].Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.ContainsKey(articleId))
            {
                _articleTags[articleId].Remove(tag);
            }
        }

        public HashSet<string> GetTags(string articleId)
        {
            if (_articleTags.ContainsKey(articleId))
            {
                return new HashSet<string>(_articleTags[articleId]);
            }

            return new HashSet<string>();
        }

        public HashSet<string> GetAllUniqueTags()
        {
            HashSet<string> all = new HashSet<string>();

            foreach (HashSet<string> tags in _articleTags.Values)
            {
                all.UnionWith(tags);
            }

            return all;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            HashSet<string> common = GetTags(articleId1);
            common.IntersectWith(GetTags(articleId2));
            return common;
        }

        public HashSet<string> FindTagsOnlyIn(string articleId, string otherArticleId)
        {
            HashSet<string> only = GetTags(articleId);
            only.ExceptWith(GetTags(otherArticleId));
            return only;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Кеш метаданих файлів (Dictionary)");
            FileMetadataCache cache = new FileMetadataCache();
            cache.Add(new FileMetadata("report.txt", 2048, new DateTime(2026, 11, 1), "a1b2c3"));
            cache.Add(new FileMetadata("data.csv", 10240, new DateTime(2026, 11, 5), "d4e5f6"));
            Console.WriteLine("Файлів у кеші: " + cache.Count);

            FileMetadata? found = cache.Get("REPORT.TXT");
            if (found != null)
            {
                Console.WriteLine("Get(\"REPORT.TXT\"): " + found.FileName + ", " + found.Size + " байт");
            }

            Console.WriteLine("Contains(\"data.csv\"): " + cache.Contains("data.csv"));
            Console.WriteLine("Remove(\"data.csv\"): " + cache.Remove("data.csv"));
            Console.WriteLine("Contains(\"data.csv\"): " + cache.Contains("data.csv"));
            Console.WriteLine("Файлів у кеші: " + cache.Count);

            Console.WriteLine();
            Console.WriteLine("Теги статей (HashSet)");
            TagManager tags = new TagManager();
            tags.AddTag("A1", "csharp");
            tags.AddTag("A1", "linq");
            tags.AddTag("A1", "collections");
            tags.AddTag("A1", "csharp");
            tags.AddTag("A2", "csharp");
            tags.AddTag("A2", "collections");
            tags.AddTag("A2", "generics");
            tags.AddTag("A3", "sql");

            Console.WriteLine("Теги A1 (дублікат csharp не додався): " + string.Join(", ", tags.GetTags("A1")));
            Console.WriteLine("Усі унікальні теги: " + string.Join(", ", tags.GetAllUniqueTags()));
            Console.WriteLine("Спільні теги A1 і A2: " + string.Join(", ", tags.FindCommonTags("A1", "A2")));
            Console.WriteLine("Теги лише в A1 (без A2): " + string.Join(", ", tags.FindTagsOnlyIn("A1", "A2")));

            tags.RemoveTag("A1", "linq");
            Console.WriteLine("Теги A1 після видалення linq: " + string.Join(", ", tags.GetTags("A1")));
        }
    }
}