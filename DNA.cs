using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeRunnerGenericAlg
{
    public class DNA
    {

        public char[] chars;
        public int fitness = 0;

        public DNA(char[] chars)
        {
            this.chars = chars;
        }

        public override string ToString()
        {
            return new string(chars) + " Fitness: " + fitness;
        }
    }
}
