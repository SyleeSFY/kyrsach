using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler_prog
{
    public class CompilerData
    {
        public bool LexicalValid = false;
        public string LexicalStatus;
        public string SyntaxStatus;
        public string SemanticStatus;

        public PolizStruct[] PolizOut = new PolizStruct[1000];
        public List<LexemStruct> LexOut = new List<LexemStruct>();
        public List<LexemStruct> TID = new List<LexemStruct>();

        public List<string> Indentificators = new List<string>();
        public List<string> Constants = new List<string>();

        public List<string> Keywords = new List<string>()  {
            "program", "begin", "end",             
            "dim",                                 
            "if", "then", "else",                  
            "for", "to", "do",                    
            "while",                               
            "read", "write",                       
            "%", "!", "$",                        
            "true", "false"                        
        };

        public readonly List<string> OPERATORS = new List<string>()
        {
            "NE", "EQ", "LT", "LE", "GT", "GE",
            "plus", "min", "mult", "div",
            "or", "and", "not",
            "ass"
        };

        public List<string> Delimiters = new List<string>()  {
            ":", ",", ".", "@", "\\", "#", "(", ")", ";", "~"
        };
    }
}