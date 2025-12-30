using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler_prog
{
    public struct Lexem
    {
        public int numTable;
        public int numInTable;
        public bool isDeclared;
        public string value;
        public string type;
    }
}
