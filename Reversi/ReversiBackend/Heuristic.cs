using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiBackend
{
    public interface Heuristic
    {
        double Evaluate(GameState gameState);
    }
}
