
using HeuristicSearch.Heursitics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.BlocksWorld.Heuristics
{
    class PDBHeuristic
    {


        byte[] pdbBlocks;
        Dictionary<long, bool> visited;
        Dictionary<long, byte> h_table;
        Queue<Node> openSet1;
        Queue<Node> openSet2;
        byte h_max;
        HashSet<byte> values;

        public byte[] PDBBlocks
        {
            get { return pdbBlocks; }
        }

        public HashSet<byte> Values
        {
            get { return values; }
        }

        public struct Node
        {
            public byte h_value;
            public List<List<byte>> stacks;
            public Operation op;

            public Node(List<List<byte>> stacks, Operation op, byte h_value = 0)
            {
                this.h_value = h_value;
                this.op = op;

                this.stacks = new List<List<byte>>();

                foreach (var stack in stacks)
                {
                    List<byte> s = new List<byte>(stack);
                    this.stacks.Add(s);
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
                Console.WriteLine("hmax:" + h_max);
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

        public PDBHeuristic(string fileName, int? capacity)
        {
            values = new HashSet<byte>();

            string path = string.Format(Global.PDBPATH, fileName);

            string[] parts = fileName.Split('_');

            int[] pdbBlocks = new int[parts.Length];


            for (int i = 0; i < parts.Length; i++)
            {
                pdbBlocks[i] = int.Parse(parts[i]);
            }

            Set(pdbBlocks);

            if (capacity == null)
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

        private int[] PDBBlocksSameStacks(List<byte[]> pdbBlocksData)
        {
            int[] sameStacks = null;

            if (pdbBlocks.Length == 1)
            {
                return sameStacks;
            }
            else if (pdbBlocks.Length == 2)
            {
                sameStacks = new int[1];
                sameStacks[0] = pdbBlocksData[0][0] == pdbBlocksData[1][0] ? 1 : 0;
                return sameStacks;
            }
            else if(pdbBlocks.Length==3)
            {
                sameStacks = new int[4];
                sameStacks[0] = pdbBlocksData[0][0] == pdbBlocksData[1][0] ? 1 : 0;
                sameStacks[1] = pdbBlocksData[0][0] == pdbBlocksData[2][0] ? 1 : 0;
                sameStacks[2] = pdbBlocksData[1][0] == pdbBlocksData[2][0] ? 1 : 0;
                sameStacks[3] = pdbBlocksData[0][0] == pdbBlocksData[1][0] ? pdbBlocksData[1][0] == pdbBlocksData[2][0] ? 1 : 0 : 0;
                return sameStacks;
            }
            else if (pdbBlocks.Length == 4)
            {
                sameStacks = new int[11];
                sameStacks[0] = pdbBlocksData[0][0] == pdbBlocksData[1][0] ? 1 : 0;
                sameStacks[1] = pdbBlocksData[0][0] == pdbBlocksData[2][0] ? 1 : 0;
                sameStacks[2] = pdbBlocksData[1][0] == pdbBlocksData[2][0] ? 1 : 0;
                sameStacks[3] = pdbBlocksData[0][0] == pdbBlocksData[3][0] ? 1 : 0;
                sameStacks[4] = pdbBlocksData[1][0] == pdbBlocksData[3][0] ? 1 : 0;
                sameStacks[5] = pdbBlocksData[2][0] == pdbBlocksData[3][0] ? 1 : 0;

                sameStacks[6] = pdbBlocksData[0][0] == pdbBlocksData[1][0] ? pdbBlocksData[1][0] == pdbBlocksData[2][0] ? 1 : 0 : 0;
                sameStacks[7] = pdbBlocksData[0][0] == pdbBlocksData[2][0] ? pdbBlocksData[2][0] == pdbBlocksData[3][0] ? 1 : 0 : 0;
                sameStacks[8] = pdbBlocksData[1][0] == pdbBlocksData[2][0] ? pdbBlocksData[2][0] == pdbBlocksData[3][0] ? 1 : 0 : 0;
                sameStacks[9] = pdbBlocksData[0][0] == pdbBlocksData[1][0] ? pdbBlocksData[1][0] == pdbBlocksData[3][0] ? 1 : 0 : 0;

                sameStacks[10] = pdbBlocksData[0][0] == pdbBlocksData[1][0] ? pdbBlocksData[1][0] == pdbBlocksData[2][0] ? pdbBlocksData[2][0] == pdbBlocksData[3][0] ? 1 : 0 : 0 : 0;
                return sameStacks;
            }

            throw new ArgumentOutOfRangeException();
        }

        private long VisitedHash(Node n)
        {

            List<List<byte>> stacks = n.stacks;

            List<byte[]> pdbBlocksData = new List<byte[]>();


            for (int i = 0; i < pdbBlocks.Length; i++)
            {
                for (int j = 0; j < stacks.Count; j++)
                {
                    var stack = stacks[j];

                    int index = stack.FindIndex(x => x == pdbBlocks[i]);


                    if (index != -1)
                    {
                        byte[] arr = new byte[2];
                        arr[0] = (byte)(j + 1);
                        arr[1] = (byte)(index + 1);
                        pdbBlocksData.Add(arr);
                        break;
                    }

                }
            }

            int[] pdbBlocksSameStacks = PDBBlocksSameStacks(pdbBlocksData);

            long hash = 0;

            foreach (var stack in stacks.OrderByDescending(x=>x.Count))
            {
                hash *= Global.SIZE;
                hash += stack.Count;
            }

            for (int i = 0; i < pdbBlocksData.Count; i++)
            {
                var pdbBlockData = pdbBlocksData[i];

                hash *= Global.SIZE;
                hash += stacks[pdbBlockData[0] - 1].Count;

                hash *= Global.SIZE;
                hash += pdbBlockData[1];
            }

            if(pdbBlocksSameStacks!=null)
            {
                for (int i = 0; i < pdbBlocksSameStacks.Length; i++)
                {
                    hash *= Global.SIZE;
                    hash += pdbBlocksSameStacks[i];
                }
            }

            return hash;


        }

        private long PDBHash(Node n)
        {

            return VisitedHash(n);
        }

        protected virtual void ApplyOp(ref Node n, Operation op)
        {
            List<List<byte>> stacks = n.stacks;

            List<byte> fromStack = stacks[op.Vals[0]];

            List<byte> toStack;

            if (op.Vals[1] < stacks.Count)
            {
                toStack = stacks[op.Vals[1]];
            }
            else
            {
                toStack = new List<byte>();
                stacks.Add(toStack);
            }

            byte block = fromStack[fromStack.Count - 1];

            fromStack.Remove(block);
            toStack.Add(block);

            if (fromStack.Count == 0)
            {
                stacks.Remove(fromStack);
            }

            //if(pdbBlocks.Contains(block))
            {
                n.h_value += Global.STEPCOST;
            }
        }

        public Operation[] Operations(List<List<byte>> stacks)
        {
            List<Operation> operations = new List<Operation>();

            for (int i = 0; i < stacks.Count; i++)
            {
                for (int j = 0; j < stacks.Count; j++)
                {
                    if (i != j)
                    {
                        Operation op = new Operation((byte)i, (byte)j);
                        operations.Add(op);
                    }
                }
            }

            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].Count > 1)
                {
                    Operation op = new Operation((byte)i, (byte)stacks.Count);
                    operations.Add(op);
                }
            }

            return operations.ToArray();
        }

        private void Set(int[] pdbBlocks)
        {
            this.pdbBlocks = new byte[pdbBlocks.Length];

            for (int i = 0; i < pdbBlocks.Length; i++)
            {
                this.pdbBlocks[i] = (byte)pdbBlocks[i];
            }

        }

        public PDBHeuristic(int[] pdbBlocks)
        {

            Set(pdbBlocks);
            visited = new Dictionary<long, bool>(Global.SIZE ^ 7);
            h_table = new Dictionary<long, byte>(Global.SIZE ^ 6);
            openSet1 = new Queue<Node>();
            openSet2 = new Queue<Node>();
            h_max = 0;
            Search();
        }

        private void Search()
        {

            byte[] goalStateArr = new byte[Global.SIZE];

            for(int i=0;i<goalStateArr.Length;i++)
            {
                goalStateArr[i] = (byte) i;
            }
            
            Node initial_n = new Node(ToStacks(goalStateArr), new Operation(0), 0);
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




                Operation[] operations = Operations(popped_n.stacks);

                for (int i = 0; i < operations.Length; i++)
                {
                    Operation op = operations[i];        
                    
                    Node child = new Node(popped_n.stacks, op, popped_n.h_value);

                    ApplyOp(ref child, op);

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
            string path = string.Format(Global.PDBPATH, string.Join("_", pdbBlocks));

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

        public static List<List<byte>> ToStacks(byte[] stateArr)
        {
            List<List<byte>> stacks = new List<List<byte>>();

            HashSet<byte> blocksBelow = new HashSet<byte>();

            blocksBelow.Add(0);

            HashSet<byte> blocksAbove = new HashSet<byte>();

            while (true)
            {
                for (int i = 0; i < stateArr.Length; i++)
                {
                    byte blockBelow = stateArr[i];

                    if (blocksBelow.Contains(blockBelow))
                    {
                        byte blockAbove = (byte)(i + 1);

                        if (blockBelow == 0)
                        {
                            List<byte> stack = new List<byte>();
                            stack.Add(blockAbove);
                            stacks.Add(stack);
                        }
                        else
                        {
                            foreach (var stack in stacks)
                            {
                                if (stack.Contains(blockBelow))
                                {
                                    stack.Add(blockAbove);
                                    break;

                                }
                            }
                        }

                        blocksAbove.Add(blockAbove);

                    }
                }

                blocksBelow = blocksAbove;
                blocksAbove = new HashSet<byte>();

                if (blocksBelow.Count == 0)
                {
                    break;
                }
            }

            return stacks;
        }

        public int H(List<List<byte>> stacks)
        {
            int h = h_table[PDBHash(new Node(stacks, new Operation(0)))];    
            return h;
        }

    }
}
