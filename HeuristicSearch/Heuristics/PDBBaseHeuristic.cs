using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.Heursitics
{
    //https://stackoverflow.com/questions/3118003/serialising-and-deserialising-v-large-dictionary-in-c-sharp
    abstract class PDBBaseHeuristic
    {
        protected byte[] pdbArr;
        Dictionary<long, bool> visited;
        Dictionary<long, byte> h_table;
        Queue<Node> openSet1;
        Queue<Node> openSet2;
        byte h_max;
        HashSet<byte> values; 
        
        public byte[] PDBArr
        {
            get { return pdbArr; }
        }

        public HashSet<byte> Values
        {
            get { return values; }
        }


        public struct ValueArr
        {
            public byte v0;
            public byte v1;
            public byte v2;
            public byte v3;
            public byte v4;
            public byte v5;
            public byte v6;
            public int size;

            public ValueArr(int size)
            {
                this.size = size;
                v0 = 0;
                v1 = 0;
                v2 = 0;
                v3 = 0;
                v4 = 0;
                v5 = 0;
                v6 = 0;
            }



            public byte Get(int index)
            {
                if(index>=size)
                {
                    throw new IndexOutOfRangeException();
                }

                switch (index)
                {
                    case 0:
                        return v0;
                    case 1:
                        return v1;
                    case 2:
                        return v2;
                    case 3:
                        return v3;
                    case 4:
                        return v4;
                    case 5:
                        return v5;
                    case 6:
                        return v6;
                    default:
                        throw new IndexOutOfRangeException();

                }

            }

            public void Set(int index, byte val)
            {
                if (index >= size)
                {
                    throw new IndexOutOfRangeException();
                }


                switch (index)
                {
                    case 0:
                        v0 = val;
                        break;
                    case 1:
                        v1 = val;
                        break;
                    case 2:
                        v2 = val;
                        break;
                    case 3:
                        v3 = val;
                        break;
                    case 4:
                        v4 = val;
                        break;
                    case 5:
                        v5 = val;
                        break;
                    case 6:
                        v6 = val;
                        break;
                    default:
                        throw new IndexOutOfRangeException();

                }
            }
        }

        public struct Node
        {
            public byte h_value;
            public ValueArr pdb;
            public byte op;

            public Node(byte[] pdbArr,byte op, byte h_value = 0)
            {
                this.h_value = h_value;
                this.op = op;
                pdb = new ValueArr(pdbArr.Length);

                for (int i = 0; i < pdb.size; ++i)
                {
                    pdb.Set(i, pdbArr[i]);
                }
            }


            public Node(ValueArr pdb, byte op, byte h_value = 0)
            {
                this.h_value = h_value;
                this.op = op;
                this.pdb = new ValueArr(pdb.size);

                for (int i = 0; i < pdb.size; ++i)
                {
                    byte val = pdb.Get(i);
                    this.pdb.Set(i, val);
                }
            }
        }

        private Node Pop()
        {
            if (openSet1.Count > 0)
            {
                Node n = openSet1.Dequeue();
                return n;
            }
            else
            {
                h_max += Global.STEPCOST;
                openSet1 = openSet2;
                openSet2 = new Queue<Node>();
                Node n = openSet1.Dequeue();
                return n;
            }

        }

        private void Push(Node n)
        {
            if (n.h_value == h_max)
            {
                openSet1.Enqueue(n);
            }
            else
            {
                openSet2.Enqueue(n);
            }
        }

        private int CountOpenSets()
        {
            return openSet1.Count + openSet2.Count;
        }

        public PDBBaseHeuristic(string fileName, int? capacity)
        {
            values = new HashSet<byte>();

            string path = string.Format(Global.PDBPATH, fileName);

            string[] parts = fileName.Split('_');

            pdbArr = new byte[parts.Length];

            for (int i = 0; i < pdbArr.Length; i++)
            {
                pdbArr[i] = byte.Parse(parts[i]);
            }

            if(capacity==null)
            {
                h_table = new Dictionary<long, byte>();
            }
            else
            {
                h_table = new Dictionary<long, byte>((int)capacity);
            }
            
            using (var stream = File.OpenRead(path))
            {
                using (var reader = new BinaryReader(stream))
                {
                    while (stream.Position < stream.Length)
                    {
                        long key = reader.ReadInt64();
                        byte value = reader.ReadByte();
                        values.Add(value);
                        h_table.Add(key, value);
                    }
                }
            }
        }

        protected virtual long VisitedHash(Node n)
        {
            return 0;
        }

        protected virtual long PDBHash(Node n)
        {
            return 0;
        }

        protected virtual void ApplyOp(ref Node n, byte op)
        {

        }

        protected virtual byte NTHOP(byte op, int index)
        {
            return 0;
        }

        protected virtual int NOPS (byte op)
        {
            return 0;
        }



        public PDBBaseHeuristic(int[] pdbArr)
        {
            this.pdbArr = pdbArr.Select(x => (byte)x).ToArray();
            visited = new Dictionary<long, bool>(Global.SIZE ^ 7);
            h_table = new Dictionary<long, byte>(Global.SIZE ^ 6);
            openSet1 = new Queue<Node>();
            openSet2 = new Queue<Node>();
            h_max = 0;
            Search();
        }

        private void Search()
        {
            Node initial_n = new Node(pdbArr,0);
            Push(initial_n);
            visited[VisitedHash(initial_n)] = true;
            h_table[PDBHash(initial_n)] = 0;
            
            while (CountOpenSets() > 0)
            {
                Node popped_n = Pop();

                long pdbHash = PDBHash(popped_n);

                byte h;

                bool containsKey = h_table.TryGetValue(pdbHash, out h);

                if (!containsKey)
                {
                    h_table[pdbHash] = popped_n.h_value;
                }
                else
                {
                    if (popped_n.h_value < h)
                    {
                        h_table[pdbHash] = popped_n.h_value;
                    }
                }

               
                int nops = NOPS(popped_n.op);

                for (int i = 0; i < nops; i++)
                {
                    byte newOp = NTHOP(popped_n.op, i) ;

                    Node child = new Node(popped_n.pdb,popped_n.op, popped_n.h_value);

                    ApplyOp(ref child, newOp);

                    long visitedHash = VisitedHash(child);

                    bool isVisited;

                    containsKey = visited.TryGetValue(visitedHash, out isVisited);

                    if (!containsKey)
                    {
                        visited[visitedHash] = true;

                        if (visited.Count % 10000000 == 0)
                        {
                            Console.WriteLine(visited.Count);
                            GC.Collect();
                        }

                        Push(child);
                    }
                }
            }

            Console.WriteLine(visited.Count);
            Console.WriteLine(h_table.Count);
            visited.Clear();

        }


        public void Save()
        {
            string path = string.Format( Global.PDBPATH ,  string.Join("_", pdbArr));

            using (var stream = File.OpenWrite(path))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    foreach (var key in h_table.Keys)
                    {
                        writer.Write(key);
                        writer.Write(h_table[key]);
                    }

                    writer.Close();
                }

                stream.Close();
            }


        }

        public int H(byte[] pdb)
        {
            Node n = new Node(pdb,0);
            int h = h_table[PDBHash(n)];
            return h;
        }

    }
}
