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

        // ОБНОВЛЕННЫЙ СПИСОК СЕРВИСНЫХ СЛОВ
        public List<string> Service = new List<string>()  {
            "program", "begin", "end",             // программа
            "dim", "ass",                          // объявление и присваивание
            "if", "then", "else",                  // условный оператор
            "for", "to", "do",                     // цикл for
            "while", "do",                         // цикл while (do уже есть)
            "read", "write",                       // ввод/вывод
            "%", "!", "$",                         // типы данных
            "NE", "EQ", "LT", "LE", "GT", "GE",    // операции отношения
            "plus", "min", "mult", "div",          // арифметические операции
            "or", "and", "not",                    // логические операции
            "true", "false"                        // логические константы
        };

        // ОБНОВЛЕННЫЙ СПИСОК РАЗДЕЛИТЕЛЕЙ
        public List<string> Separators = new List<string>()  {
            ":", ",", ".", "@", "\\", "#", "(", ")", ";", "~"
        };
    }
}