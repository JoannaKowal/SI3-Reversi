using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiBackend
{
    public abstract class AIPlayer
    {
        public int visitedNodes = 0;
        public abstract void MakeMove();
    }
}
