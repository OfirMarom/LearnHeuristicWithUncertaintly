using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    interface IUndoToken
    {
        int H { get; set; }
        Operation Op { get; set; }
    }
}
