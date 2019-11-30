using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiBackend
{
    class Move
    {
        public int ToFieldIndex { get; }

        public Move(int toFieldIndex)
        {
            this.ToFieldIndex = toFieldIndex;
        }
    }
}
