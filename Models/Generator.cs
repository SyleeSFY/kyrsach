using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler_prog.Models
{
    public enum TargetLanguage : byte
    {
        CSharp = 0
    }

    public class Generator
    {
        public string Code { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Errors { get; set; }

        public Generator()
        {
            Code = "";
            Warnings = new List<string>();
            Errors = new List<string>();
        }
    }

}
