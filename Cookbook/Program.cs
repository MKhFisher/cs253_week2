using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cookbook.cs
{
    public class GlobalVars
    {
        public List<string> stopwords;
        public List<string> data;
        public List<Frequency> result;
    }

    public class Frequency
    {
        public string term;
        public int frequency;
    }

    class Program
    {
        static void Main(string[] args)
        {
            GlobalVars vars = new GlobalVars();
            GetFile("https://raw.githubusercontent.com/crista/exercises-in-programming-style/master/stop_words.txt", vars, "stopwords");

        }

        public static void GetFile(string file, GlobalVars vars, string file_type)
        {
            var request = WebRequest.Create(file);
            using (var response = request.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var words = reader.ReadToEnd();
                string[] temp = words.Split(',');

                List<string> result = new List<string>();
                for (int i = 0; i < temp.Length; i++)
                {
                    result.Add(temp[i].Trim());
                }

                if (file_type == "stopwords")
                {
                    vars.stopwords = result;
                }
                else if (file_type == "data")
                {
                    vars.data = result;
                }
            }
        }
    }
}
