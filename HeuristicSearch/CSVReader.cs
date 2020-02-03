using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HeuristicSearch
{
    class CSVReader
    {
        char seperator;
        string path;
        StringBuilder stringBuilder = new StringBuilder();
        string[][] output;

        public string[][] Output
        {
            get { return output; }
        }

        public void Set(string path, char seperator)
        {
            this.path = path;
            this.seperator = seperator;
        }

   
        
        public CSVReader(string path, char seperator)
        {
            Set(path, seperator);
        }

        public void Read(bool skipFirstLine=false)
        {
            List<string[]> output = new List<string[]>();

            using (StreamReader reader = new StreamReader(path))
            {
                string line;

                bool firstLineProcessed = false;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] outLine = line.Split(seperator);
             
                    if (skipFirstLine == false || firstLineProcessed == true)
                    {
                        output.Add(outLine);
                    }

                    firstLineProcessed = true;

                }
            }

            this.output = output.ToArray();
        }
    }
}
