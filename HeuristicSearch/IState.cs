using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace HeuristicSearch
{
    interface IState
    {
        int H { get; set; }
        byte[] Arr { get; set; }
        Operation Op { get; set; }
        IState Clone();
        long GetHashCodeLong(); 
    }
}
