using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    class CSVWriter
    {
        const string NEWLINE = "\n";
        char seperator;
        string path;
        StringBuilder stringBuilder = new StringBuilder();

        public void Set(string path, char seperator)
        {
            this.path = path;
            this.seperator = seperator;
        }

        public CSVWriter()
        {
        }

        public CSVWriter(string path, char seperator)
        {
            Set(path, seperator);
        }

        public void Clear()
        {
            stringBuilder.Clear();
        }

        public void Add(string item)
        {
            stringBuilder.Append(item + seperator);
        }

        public void EndLine()
        {
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(NEWLINE);
        }

        public string Get()
        {
            return stringBuilder.ToString();
        }

        public void Write()
        {
            File.AppendAllText(path, Get());
        }
    }
}
