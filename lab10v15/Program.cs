using System;
using System.Collections.Generic;

namespace Lab10V15
{
    public interface IStreamReader
    {
        string ReadLine();
        string ReadAll();
    }

    public class FileStreamReader : IStreamReader
    {
        private string[] _lines;
        private int _position;

        public FileStreamReader(string path)
        {
            _lines = new string[] { "Файл: " + path, "appName=MyApp", "version=1.0" };
            _position = 0;
        }

        public string ReadLine()
        {
            if (_position < _lines.Length)
            {
                string line = _lines[_position];
                _position++;
                return line;
            }
            return "";
        }

        public string ReadAll()
        {
            string result = "";
            for (int i = 0; i < _lines.Length; i++)
            {
                if (i > 0)
                {
                    result = result + ", ";
                }
                result = result + _lines[i];
            }
            return result;
        }
    }

    public class NetworkStreamReader : IStreamReader
    {
        private string[] _lines;
        private int _position;

        public NetworkStreamReader(string address)
        {
            _lines = new string[] { "Підключено до " + address, "Status: 200 OK", "Hello" };
            _position = 0;
        }

        public string ReadLine()
        {
            if (_position < _lines.Length)
            {
                string line = _lines[_position];
                _position++;
                return line;
            }
            return "";
        }

        public string ReadAll()
        {
            string result = "";
            for (int i = 0; i < _lines.Length; i++)
            {
                if (i > 0)
                {
                    result = result + ", ";
                }
                result = result + _lines[i];
            }
            return result;
        }
    }

    public abstract class ConfigurationLoader
    {
        protected Dictionary<string, string> _settings = new Dictionary<string, string>();

        public abstract void LoadSettings(string source);
        public abstract string GetSetting(string key);

        public void DisplayAllSettings()
        {
            Console.WriteLine("Усі налаштування:");
            foreach (string key in _settings.Keys)
            {
                Console.WriteLine(key + " = " + _settings[key]);
            }
        }
    }

    public class JsonConfigLoader : ConfigurationLoader
    {
        public override void LoadSettings(string source)
        {
            Console.WriteLine("Завантаження налаштувань із JSON: " + source);
            _settings["appName"] = "MyApp";
            _settings["version"] = "1.0";
        }

        public override string GetSetting(string key)
        {
            if (_settings.ContainsKey(key))
            {
                return _settings[key];
            }
            return "не знайдено";
        }
    }

    public class XmlConfigLoader : ConfigurationLoader
    {
        public override void LoadSettings(string source)
        {
            Console.WriteLine("Завантаження налаштувань із XML: " + source);
            _settings["appName"] = "MyXmlApp";
            _settings["timeout"] = "30";
        }

        public override string GetSetting(string key)
        {
            if (_settings.ContainsKey(key))
            {
                return "<" + key + ">" + _settings[key] + "</" + key + ">";
            }
            return "не знайдено";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Колекція типу IStreamReader");

            List<IStreamReader> readers = new List<IStreamReader>();
            readers.Add(new FileStreamReader("config.txt"));
            readers.Add(new NetworkStreamReader("192.168.0.1"));

            foreach (IStreamReader reader in readers)
            {
                Console.WriteLine("Перший рядок: " + reader.ReadLine());
                Console.WriteLine("Другий рядок: " + reader.ReadLine());
                Console.WriteLine("Усе разом: " + reader.ReadAll());
                Console.WriteLine();
            }

            Console.WriteLine("Колекція типу ConfigurationLoader");

            List<ConfigurationLoader> loaders = new List<ConfigurationLoader>();
            loaders.Add(new JsonConfigLoader());
            loaders.Add(new XmlConfigLoader());

            foreach (ConfigurationLoader loader in loaders)
            {
                loader.LoadSettings("settings");
                loader.DisplayAllSettings();
                Console.WriteLine("appName: " + loader.GetSetting("appName"));
                Console.WriteLine("version: " + loader.GetSetting("version"));
                Console.WriteLine();
            }
        }
    }
}