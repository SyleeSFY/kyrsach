using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler_prog.Models
{
    public class FileWork
    {
        public FileStream fs;

        public void CreateFile()
        {
            string file_path = @"syntax_tree.txt";
            fs = File.Open(file_path, FileMode.Create);
        }

        public void WriteFile(string text_write)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(text_write);
            fs.Write(info, 0, info.Length);
        }

        public void Write(byte[] array, int offset, int count)
            => fs.Write(array, 0, count);

        public void Close()
            => fs.Close();
        
    }
}
