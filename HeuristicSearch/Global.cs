using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    

    class Global
    {
        public static readonly Type DOMAINTYPE = typeof(SlidingPuzzle.SlidingPuzzle);

        public const int SIZE = 16;

        public const int NITER = 50;

        public const double UNCERTTHRESH = 1;

        public const int TFMAXBATCHSIZE = 100;

        public const int UNCERTMINSAMPLE = 100;

        public const int STEPCOST = 1;

        public const float PHIMAX = 1;

        public const float PHIMIN = -5;

        public const int HCAHCEMAXRECORDS = 50000000;

        public const int MAXSTEPS = 1000;


        public const string ROOTPATH = @"dir\{0}";

        public const string TRAINFN = "runstats.csv";

        public const string TESTADMISFN = "out{0}.csv";

        public const string NNFN = "nn{0}";

        public const string TESTEFFICFN = "oute.csv";

        public const string TESTEFFICFNSUMM = "outesumm.csv";

        public const string TESTTASKFN = "test.csv";

        public const string LOGFN = "log.csv";

        public static readonly string PDBPATH;

        public static readonly string TESTTASKPATH;

        public static readonly string RUNPATH;




        static Global()
        {
            string domainName = string.Empty;

            if (DOMAINTYPE == typeof(SlidingPuzzle.SlidingPuzzle))
            {
                domainName = (Global.SIZE - 1).ToString() + "-puzzle"; 
            }
            else if (DOMAINTYPE == typeof(PancakePuzzle.PancakePuzzle))
            {
                domainName = Global.SIZE.ToString() + "-pancake";
            }
            else if (DOMAINTYPE == typeof(BlocksWorld.BlocksWorld))
            {
                domainName = Global.SIZE.ToString() + "-blocksworld";
            }
            else
            {
                throw new ArgumentException();
            }

            PDBPATH = string.Format(ROOTPATH, @"Domains\" + domainName + @"\PDBs\{0}");
            TESTTASKPATH = string.Format(ROOTPATH, @"Domains\" + domainName + @"\test.csv");
            RUNPATH = string.Format(ROOTPATH, @"Domains\" + domainName + @"\r{0}\{1}\{2}");
        }

        public static void SerializeObject<T>(T serializableObject, string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, serializableObject);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T DeserializeObject<T>(string path)
        {
            try
            {

                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    T obj = (T)formatter.Deserialize(stream);
                    stream.Close();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool IsDictionary(object o)
        {
            Type t = o.GetType();
            bool isDict = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
            return isDict;
        }

        public static double InvLaplace(double mu, double sigma, double p)
        {
            double sign = p > 0.5 ? 1 : -1;
            double val = mu - sigma * sign * Math.Log(1 - 2 * Math.Abs(p - 0.5));
            return val;
           
        }

        private static Random random = new Random();

      
        public static Random Random
        {
            get { return random; }
        }

        public static int MaxLengthJaggedArrays(byte[][] arr)
        {
            
            int maxLength = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Length >maxLength)
                {
                    maxLength = arr[i].Length;
                }
            }

            return maxLength;
        }

        public static double TrimPhi(double phi)
        {
            phi = Math.Min(phi, Global.PHIMAX);

            phi = Math.Max(phi, Global.PHIMIN);

            return phi;
        }

        public static double ToSigma(double phi)
        {
            return Math.Log(1 + Math.Exp(phi));
        }

        public static int Sample(double[] weights, double norm = 1)
        {

            double u = Global.Random.NextDouble();

            double prev = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                double curr = prev + weights[i] / norm;

                if (u >= prev && u < curr)
                {
                    return i;
                }

                prev = curr;
            }

            return weights.Length - 1;

        }

        public static double CostOfPath(IState[] path)
        {
            double pathCost = 0;

            for (int i = 0; i < path.Length - 1; i++)
            {
                pathCost += Global.STEPCOST ;
            }

            return pathCost;
        }

    }
}
