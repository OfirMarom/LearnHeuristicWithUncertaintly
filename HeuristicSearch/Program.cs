using HeuristicSearch.Heuristics;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using HeuristicSearch.NNs;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace HeuristicSearch
{
    class Program
    {
        /* In Global change 'dir'in ROOTPATH to the required directory
         * In Global change DOMAINTYPE to the required type
         * In Global change SIZE to the required domain size
         * In Global change NITER to the number of iterations to run
         */

        static IMultHeuristic multHeuristic;

        static IRepresentation representationSolve;

        static IRepresentation representationUncert;

        static void Main(string[] args)
        {
            Control.UseNativeMKL();

            Console.WriteLine("start1");

            //PancakePuzzle.BuildPDBs.BuildPDBs24();

            //GenTest(null, 50,Global.MAXSTEPS);

            multHeuristic = null;

            representationSolve = new SlidingPuzzle.Representations.TwoDim(new Func<double, double>(x => x / 10), new Func<double, double>(x => x * 10));

            representationUncert = new SlidingPuzzle.Representations.TwoDim(null, null);

           
            for (int r = 1; r <= 5; r++)
            {
                
                {
                    List<Tuple<int, bool, int>> paramaters = new List<Tuple<int, bool, int>>();
                    paramaters.Add(new Tuple<int, bool, int>(1, true, 1));
                    
                    foreach (var p in paramaters)
                    {
                        Train(r, Global.NITER, p.Item1, p.Item2, p.Item3, false);
                    }
                }
                
                
                
                {
                    int nnIndex = Global.NITER - 1;
                    List<Tuple<string, int, double?>> paramaters = new List<Tuple<string, int, double?>>();
                    paramaters.Add(new Tuple<string, int, double?>("1", nnIndex, null));
                   

                    foreach (var p in paramaters)
                    {
                        TestAdmissibility(r, p.Item1, p.Item2, p.Item3);
                    }

                }
                
                

                
                {
                    List<Tuple<int, bool, int>> paramaters = new List<Tuple<int, bool, int>>();

                    paramaters.Add(new Tuple<int, bool, int>(2, true, 1));

                    foreach (var p in paramaters)
                    {
                        Train(r, Global.NITER, p.Item1, p.Item2, p.Item3, false);
                    }
                }
                

                
                {
                    int nnIndex = Global.NITER - 1;
                    List<Tuple<string, int, double?>> paramaters = new List<Tuple<string, int, double?>>();
                    paramaters.Add(new Tuple<string, int, double?>("2", nnIndex, 0.5));
                    paramaters.Add(new Tuple<string, int, double?>("2", nnIndex, 0.95));
                    paramaters.Add(new Tuple<string, int, double?>("2", nnIndex, 0.9));
                    paramaters.Add(new Tuple<string, int, double?>("2", nnIndex, 0.75));              
                    paramaters.Add(new Tuple<string, int, double?>("2", nnIndex, 0.25));
                    paramaters.Add(new Tuple<string, int, double?>("2", nnIndex, 0.1));
                    paramaters.Add(new Tuple<string, int, double?>("2", nnIndex, 0.05));


                    foreach (var p in paramaters)
                    {
                        TestAdmissibility(r, p.Item1, p.Item2, p.Item3);
                    }

                }
                


                
                {
                    List<Tuple<int, bool, int>> paramaters = new List<Tuple<int, bool, int>>();
                    paramaters.Add(new Tuple<int, bool, int>(1, true, 1));
                    paramaters.Add(new Tuple<int, bool, int>(1, false, 1));
                    paramaters.Add(new Tuple<int, bool, int>(1, false, 2));
                    paramaters.Add(new Tuple<int, bool, int>(1, false, 4));
                    paramaters.Add(new Tuple<int, bool, int>(1, false, 6));
                    paramaters.Add(new Tuple<int, bool, int>(1, false, 8));
                    paramaters.Add(new Tuple<int, bool, int>(1, false, 10));

                    foreach (var p in paramaters)
                    {
                        Train(r, Global.NITER, p.Item1, p.Item2, p.Item3, true);
                    }

                }
                

                
                {

                    int numTasks = 1000;

                    int numIter = 5;

                    int nnIndex = Global.NITER - 1;

                    TestDomainData[][] testData = new TestDomainData[numIter][];

                    for (int i = 0; i < numIter; i++)
                    {
                        TestDomainData[] data = new TestDomainData[numTasks];

                        for (int j = 0; j < numTasks; j++)
                        {
                            data[j] = GenTest(null, 1, j + 1, false, false)[0];
                        }

                        testData[i] = data;
                    }

                    List<Tuple<string, int>> paramaters = new List<Tuple<string, int>>();
                    paramaters.Add(new Tuple<string, int>("1e", nnIndex));
                    paramaters.Add(new Tuple<string, int>("k1", nnIndex));
                    paramaters.Add(new Tuple<string, int>("k2", nnIndex));
                    paramaters.Add(new Tuple<string, int>("k4", nnIndex));
                    paramaters.Add(new Tuple<string, int>("k6", nnIndex));
                    paramaters.Add(new Tuple<string, int>("k8", nnIndex));
                    paramaters.Add(new Tuple<string, int>("k10", nnIndex));

                    foreach (var p in paramaters)
                    {
                        TestEfficiency(r, p.Item1, p.Item2, testData);
                    }

                }
                

            }
            
        
            Console.ReadLine();
        }


        public static void TestEfficiency(int run,  string runType, int nnIndex, TestDomainData[][] testData)
        {
            
            string csvWritePath = string.Format(Global.RUNPATH, run, runType, Global.TESTEFFICFN);

            string csvSummWritePath = string.Format(Global.RUNPATH, run, runType, Global.TESTEFFICFNSUMM);

            CSVWriter csvWriter = new CSVWriter(csvWritePath, ',');

            CSVWriter csvSummWriter = new CSVWriter(csvSummWritePath, ',');

            int timeout = 60000;

            string nnFileName = string.Format(Global.NNFN, nnIndex);

            string nnLoadPath = string.Format(Global.RUNPATH, run, runType, nnFileName);

            MyNN nnSolve = MyNN.Load(nnLoadPath);

           for (int i= 0;i<testData.GetLength(0);i++)
            {
                csvWriter.Add(i.ToString());
                csvWriter.EndLine();
                csvWriter.Write();
                csvWriter.Clear();


                IHeuristic heuristic = new NNPreTrainedHeuristic(nnSolve, representationSolve, null, true);

                int numSolved = SolveTestData(heuristic, testData[i], csvWriter, timeout);

                csvSummWriter.Add(i.ToString());
                csvSummWriter.Add(numSolved.ToString());
                csvSummWriter.EndLine();
                csvSummWriter.Write();
                csvSummWriter.Clear();
            }

      

        }


        

        public static void TestAdmissibility(int run, string runType, int nnIndex,double? confLevel)
        {
            string nnFileName = string.Format(Global.NNFN, nnIndex);

            string outputFileName = string.Format(Global.TESTADMISFN, confLevel == null ? "" : confLevel.ToString());

            string nnLoadPath = string.Format(Global.RUNPATH, run, runType, nnFileName);

            string csvWritePath = string.Format(Global.RUNPATH, run, runType, outputFileName );

            CSVWriter csvWriter = new CSVWriter(csvWritePath, ',');

            MyNN nnSolve = MyNN.Load(nnLoadPath);

            TestDomainData[] data = ReadTestDomainData(Global.TESTTASKPATH);

            IHeuristic heuristic = new NNPreTrainedHeuristic(nnSolve, representationSolve,confLevel, true);

            
            SolveTestData(heuristic, data, csvWriter,null);

            double sumExpanded = 0;
            double sumGenerated = 0;
            double sumOptimal = 0;
            double sumCost = 0;
            double sumSubOpt = 0;


            for (int i = 0; i < data.Length; i++)
            {
                TestDomainData testDomainData = data[i];

                sumExpanded += testDomainData.SolvedExpanded;
                sumGenerated += testDomainData.SolvedGenerated;
                sumOptimal += testDomainData.OptimalCost;
                sumCost += testDomainData.SolvedCost;
                sumSubOpt += testDomainData.SolvedCost / (double)testDomainData.OptimalCost;
            }


            Console.WriteLine("-----");
            Console.WriteLine(sumExpanded / data.Length);
            Console.WriteLine(sumGenerated / data.Length);
            Console.WriteLine(sumOptimal / data.Length);
            Console.WriteLine(sumCost / data.Length);
            Console.WriteLine((sumSubOpt / data.Length) - 1);

            csvWriter.Write();

        }

       

        public static void Train(int run, int iter, int numOutputSolve, bool genDomainFromUncert, int lengthInc, bool effRun)
        {
            string runType = GetRunType(numOutputSolve, genDomainFromUncert, lengthInc, effRun);

            string csvWritePath = string.Format(Global.RUNPATH, run, runType, Global.TRAINFN);
            
            int numHiddenSolve = 20;

            if(multHeuristic!=null)
            {
               numHiddenSolve = 8;
            }

            int? numHiddenUncert = numHiddenSolve;

            if (genDomainFromUncert==false)
            {
                numHiddenUncert = null;
            }

            float dropout = 0F;

            if (multHeuristic==null && numOutputSolve>1)
            {
                dropout = 0.025F;
            }

          
            NNBayesHeuristic heuristic = new NNBayesHeuristic(representationSolve, representationUncert, numHiddenSolve, numOutputSolve, numHiddenUncert, dropout, true);

            heuristic.ConfLevel = null;

            if(numOutputSolve>1)
            {
                heuristic.ConfLevel = 0.01;
            }

            double incConfLevel = 0.05;

            double percSolvedThresh = 0.6;

            int numTasks=10;
   
            int tMax;

            if(effRun==true)
            {
                tMax = 1000;
            }
            else if(multHeuristic==null)
            {
                tMax = 60000;
            }
            else
            {
                tMax = 5 * 60000;
            }

            CSVWriter csv = new CSVWriter(csvWritePath, ',');

            Stopwatch sw = new Stopwatch();

            sw.Start();

            int length = lengthInc;

            for(int n=0;n<iter;n++)
            {
                List<List<IState>> plans = new List<List<IState>>(numTasks);

                int countSolved = 0;

                for (int i = 0; i < numTasks; i++)
                {
                    heuristic.ClearCache();

                    DomainContainer domainContainer;

                    if (genDomainFromUncert == true)
                    {
                        if (heuristic.IsTrained)
                        {
                            domainContainer = new DomainContainer(heuristic, Global.UNCERTTHRESH, Global.MAXSTEPS);
                        }
                        else
                        {
                            domainContainer = new DomainContainer(heuristic, 1);
                        }
                    }
                    else
                    {
                        domainContainer =  new DomainContainer(heuristic, length);
                    }

                    ISearchDomain domain = domainContainer.Domain;

                    heuristic.ClearCache();

                    GC.Collect();

                    IDAStar<ISearchDomain> planner = new IDAStar<ISearchDomain>(domain);

                    List<IState> plan = planner.Search(domain.Initial(), tMax);

                    Console.WriteLine("expanded: " + planner.Expanded);
                    Console.WriteLine("generated: " + planner.Generated);
                    Console.WriteLine("elapsed: " + planner.SwopWatch.ElapsedMilliseconds);

                    if (plan.Count > 0)
                    {
                        countSolved++;
                        Console.WriteLine("solved solution length:" + plan.Count);
                        plans.Add(plan);
                    }

                    csv.Add(n.ToString());
                    csv.Add(planner.SwopWatch.ElapsedMilliseconds.ToString());
                    csv.Add(planner.Expanded.ToString());
                    csv.Add(planner.Generated.ToString());
                    csv.Add(heuristic.ConfLevel == null ? "NULL" : heuristic.ConfLevel.ToString());
                    csv.Add(plan.Count.ToString());
                    csv.Add(domain.NumSteps.ToString());

                    csv.EndLine();

                }

                string nnFileName = string.Format(Global.NNFN, n);

                string nnSavePath = string.Format(Global.RUNPATH, run, runType, nnFileName);

                heuristic.NNSolve.Save(nnSavePath,true);

                double percSolved = countSolved / (double)numTasks;


                if (percSolved < percSolvedThresh)
                {
                    heuristic.UpdateBeta = false;

                    if (heuristic.ConfLevel != null && heuristic.ConfLevel < 0.5)
                    {
                        heuristic.ConfLevel += incConfLevel;

                        if (heuristic.ConfLevel > 0.5)
                        {
                            heuristic.ConfLevel = 0.5;
                        }
                    }
                }
                else
                {
                    heuristic.UpdateBeta = true;
                }

                
             

                if (plans.Count > 0)
                {
                    double avg = plans.Select(x => x.Count).Average();
                    double max = plans.Select(x => x.Count).Max();
                    double min = plans.Select(x => x.Count).Min();
                    Console.WriteLine("solved: " + plans.Count + " avg: " + avg + " max: " + max + " min: " + min);
                    Console.WriteLine("current conf level: " + heuristic.ConfLevel == null ? "NULL" : heuristic.ConfLevel.ToString());
                }

                long t = sw.ElapsedMilliseconds;

                length += lengthInc;

                csv.Add(t.ToString());

                csv.EndLine();

                csv.Write();

                csv.Clear();

    

                heuristic.Update(plans.ToArray());

               
            }

            heuristic.Dispose();
        }

       
        static string GetRunType(int numOutputSolve, bool genDomainFromUncert, int lengthInc, bool effRun)
        {
            if (numOutputSolve == 1 && genDomainFromUncert == true && effRun==false)
            {
                return "1";
            }
            else if (numOutputSolve > 1 && genDomainFromUncert == true)
            {
                return "2";
            }
            else if(numOutputSolve == 1 && genDomainFromUncert == true && effRun==true)
            {
                return "1e";
            }
            else
            {
                return "k" + lengthInc;
            }
        }

        static int PostBlocksWorld(IState initState, IState goalState, int sleep = 100)
        {
            System.Threading.Thread.Sleep(sleep);

            string size = initState.Arr.Length.ToString();

            string initStateStr = string.Join("+", initState.Arr);

            string goalStateStr = string.Join("+", goalState.Arr);

            var request = (HttpWebRequest)WebRequest.Create("http://users.cecs.anu.edu.au/~jks/cgi-bin/bwstates/bwoptcgi");

            var postData = "algo=3&verb=1&size=" + size + "&initial=" + initStateStr + "&goal=" + goalStateStr + "&mode=SOLVE";

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;


            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            responseString = responseString.Replace("\n", "");

            return int.Parse(responseString);
        }


        static TestDomainData[] ReadTestDomainData(string path)
        {
            List<TestDomainData> data = new List<TestDomainData>();

            CSVReader reader = new CSVReader(path, ',');

            reader.Read(false);


            for (int i = 0; i < reader.Output.Length; i++)
            {
                string[] outLine = reader.Output[i];

                string[] splitInit = outLine[0].Split(' ');

                byte[] init = new byte[splitInit.Length];

                for (int j = 0; j < splitInit.Length; j++)
                {
                    init[j] = byte.Parse(splitInit[j]);
                }


                int optimalCost = int.Parse(outLine[1]);
                long generated = outLine[2] == string.Empty ? 0 : long.Parse(outLine[2]);

                TestDomainData testDomainData = new TestDomainData(init, optimalCost, generated);

                data.Add(testDomainData);
            }

            return data.ToArray();
        }

        static TestDomainData[] GenTest(IHeuristic heuristic, int numTasks = 100, int numSteps = Global.MAXSTEPS, bool writeCSV = true, bool computeCost=true)
        {
            TestDomainData[] data = new TestDomainData[numTasks];

            string csvWritePath = string.Format(Global.ROOTPATH, Global.TESTTASKFN);

            CSVWriter csv = new CSVWriter(csvWritePath, ',');

            for (int i = 0; i < numTasks; i++)
            {
                DomainContainer domainContainer = new DomainContainer(heuristic, numSteps);

                ISearchDomain domain = domainContainer.Domain;

                IState initState = domain.Initial();

                string initStateStr = string.Join(" ", initState.Arr);

                int cost = 0;

                long generated = 0;

                if(computeCost==true)
                {
                    if (Global.DOMAINTYPE == typeof(BlocksWorld.BlocksWorld) && heuristic == null)
                    {
                        cost = PostBlocksWorld(initState, domain.Goal());
                    }
                    else
                    {
                        IDAStar<ISearchDomain> planner = new IDAStar<ISearchDomain>(domain);
                        List<IState> plan = planner.Search(initState);
                        cost = (plan.Count - 1);
                        generated = planner.Generated;
                    }
                }

               
                data[i] = new TestDomainData(initState.Arr, cost, generated);

                csv.Add(initStateStr);

                csv.Add(cost.ToString());

                csv.Add(generated.ToString());

                csv.EndLine();

            }

            if(writeCSV==true)
            {
                csv.Write();
            }

            return data;
        }

        static int SolveTestData(IHeuristic heuristic, TestDomainData[] data, CSVWriter csvWriter, int? timeout)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int countSolved = 0;

            for (int i = 0; i < data.Length; i++)
            {

                Console.WriteLine("iter: " + i.ToString());

                if (timeout != null && sw.ElapsedMilliseconds > timeout)
                {
                    break;
                }

                heuristic.ClearCache();

                GC.Collect();

                TestDomainData testDomainData = data[i];

                DomainContainer domainContainer = new DomainContainer(heuristic, testDomainData.Init);

                ISearchDomain domain = domainContainer.Domain;

                IDAStar<ISearchDomain> planner = new IDAStar<ISearchDomain>(domain);

                List<IState> plan = planner.Search(domain.Initial(), timeout);

                testDomainData.SolvedCost = plan.Count - 1;

                testDomainData.SolvedExpanded = planner.Expanded;

                testDomainData.SolvedGenerated = planner.Generated;


                if (testDomainData.SolvedCost > 0)
                {

                    csvWriter.Add(planner.Expanded.ToString());
                    csvWriter.Add(planner.Generated.ToString());
                    csvWriter.Add(testDomainData.OptimalCost.ToString());
                    csvWriter.Add(testDomainData.SolvedCost.ToString());
                    csvWriter.Add(planner.SwopWatch.ElapsedMilliseconds.ToString());
                    csvWriter.EndLine();
                    csvWriter.Write();
                    csvWriter.Clear();
                    countSolved++;
                }



                Console.WriteLine("cost: " + (plan.Count - 1).ToString());

                Console.WriteLine("generated: " + planner.Generated);

                Console.WriteLine("elapsed: " + planner.SwopWatch.ElapsedMilliseconds);

            }

            return countSolved;
        }
    }
}
