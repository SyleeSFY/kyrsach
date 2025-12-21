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

        // ОСНОВНЫЕ КЛЮЧЕВЫЕ СЛОВА (без операторов)
        public List<string> Keywords = new List<string>()  {
            "program", "begin", "end",             // программа
            "dim",                                 // объявление
            "if", "then", "else",                  // условный оператор
            "for", "to", "do",                     // цикл for
            "while",                               // цикл while
            "read", "write",                       // ввод/вывод
            "%", "!", "$",                         // типы данных
            "true", "false"                        // логические константы
        };

        // ОПЕРАТОРЫ (вынесены в отдельный список)
        public readonly List<string> OPERATORS = new List<string>()
        {
            // Операции отношения
            "NE", "EQ", "LT", "LE", "GT", "GE",
            
            // Арифметические операции
            "plus", "min", "mult", "div",
            
            // Логические операции
            "or", "and", "not",
            
            // Присваивание
            "ass"
        };

        // СУЩЕСТВУЮЩИЙ СПИСОК РАЗДЕЛИТЕЛЕЙ
        public List<string> Delimiters = new List<string>()  {
            ":", ",", ".", "@", "\\", "#", "(", ")", ";", "~"
        };
    }
}