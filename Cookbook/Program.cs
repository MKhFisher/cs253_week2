using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cookbook.cs
{
    public class GlobalVars
    {
        // C# does not support global variables in the classical sense so a static object is used to simulate a set of global variables per MSDN/Stackoverflow
        public static string data;
        public static List<string> words;
        public static Dictionary<string, Frequency> freq;
    }

    public class Frequency
    {
        public string term { get; set; }
        public int frequency { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Read file from command line parameter
            ReadFile(args[0]);

            // Tokenize
            Tokenize();

            // Frequencies
            Frequencies();

            // Remove stop words
            RemoveStopWords();

            // Sort top 25s
            Sort();

            int counter = 0;
            foreach (KeyValuePair<string, Frequency> pair in GlobalVars.freq)
            {
                if (counter != 25)
                {
                    Console.WriteLine("{0}  -  {1}", pair.Value.term, pair.Value.frequency);
                    counter++;
                }
            }
        }

        public static void RemoveStopWords()
        {
            HashSet<string> stopwords = new HashSet<string>();
            using (StreamReader sr = new StreamReader("stop_words.txt"))
            {
                string file_data = sr.ReadToEnd().Replace(",\n\n", "");
                string[] temp = Regex.Split(file_data, "\\W+");

                foreach (string stopword in temp)
                {
                    stopwords.Add(stopword);
                }
            }

            foreach (string word in stopwords)
            {
                GlobalVars.freq.Remove(word);
            }
        }

        public static void Sort()
        {
            GlobalVars.freq = GlobalVars.freq.OrderByDescending(x => x.Value.frequency).ToDictionary(x => x.Key, x => x.Value);
        }

        public static void Frequencies()
        {
            Hashtable duplicates = new Hashtable();

            foreach (string word in GlobalVars.words)
            {
                Frequency temp = new Frequency { term = word, frequency = 1 };
                if (duplicates[word] == null)
                {
                    duplicates[word] = temp;
                }
                else
                {
                    Frequency update = duplicates[word] as Frequency;
                    update.frequency += 1;
                }
            }

            GlobalVars.freq = new Dictionary<string, Frequency>();
            foreach (DictionaryEntry entry in duplicates)
            {
                GlobalVars.freq.Add(entry.Key as string, entry.Value as Frequency);
            }
        }

        static Regex regex_token = new Regex("(\\w+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static void Tokenize()
        {
            GlobalVars.data += ".";
            MatchCollection matches = regex_token.Matches(GlobalVars.data);
            List<string> result = new List<string>();

            foreach (Match m in matches)
            {
                if (m.Groups[1].Value.ToLower() != "s")
                {
                    result.Add(m.Groups[1].Value.ToLower());
                }
            }

            GlobalVars.words = result;
        }

        public static void ReadFile(string file)
        {
            var request = WebRequest.Create(file);
            using (var response = request.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var text = reader.ReadToEnd();
                GlobalVars.data = text.ToString().Replace('_', ' ');
            }
        }
    }
}
