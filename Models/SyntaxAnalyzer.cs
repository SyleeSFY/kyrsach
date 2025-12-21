using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler_prog.Models;
using static compiler_prog.CompilerData;

namespace compiler_prog
{
    public enum ErrorCode
    {
        StartNotWithProgram = 0,
    }

    public class SyntaxAnalyzer
    {
        private int labelCounter = 1;

        private CodeGenerator _codeGenerator;
        private FileWork _fileWork;

        private LexemStruct currentLex;
        public List<LexemStruct> TID_temp = new List<LexemStruct>();
        private string currentLexValue;
        private int counterSLexem;
        CompilerData parentObj;
        string TempPath;

        string temp_str;
        string temp_while;
        string temp_for;
        string temp_if;

        // Семантический анализатор
        private Stack<int> stackCheckDeclare = new Stack<int>();
        public Stack<string> stackCheckContVir = new Stack<string>();

        // ПОЛИЗ
        public PolizStruct[] OutputPoliz = new PolizStruct[1000];
        public int free = 0;

        public SyntaxAnalyzer(ref CompilerData obj)
        {
            parentObj = obj;
            counterSLexem = 0;
            _fileWork = new FileWork();
        }

        private int StartProgram()
        {
            int code;
            _fileWork.WriteFile("Цепочка разбора");
            GetCurrentLexem();

            if (currentLexValue != "program")
                return 1;

            _fileWork.WriteFile("PROG -> program");
            GetCurrentLexem();

            TempPath = "PROG -> ";
            code = DESC();
            if (code != 0) return code;

            if (currentLexValue != "begin")
            {
                return 2;
            }
            _fileWork.WriteFile("PROG -> begin");
            GetCurrentLexem();

            // ФИКС: Обрабатываем все операторы до end
            while (currentLexValue != "end" && currentLexValue != "⟂")
            {
                _fileWork.WriteFile($"StartProgram: Обрабатываем оператор, текущая лексема = '{currentLexValue}'");

                code = OPER();
                if (code != 0) return code;

                // ФИКС: Улучшенная обработка разделителей после операторов
                if (currentLexValue == ";")
                {
                    _fileWork.WriteFile("PROG -> ;");
                    GetCurrentLexem();

                    // ФИКС: Проверяем специальные случаи после ;
                    if (currentLexValue == "else")
                    {
                        _fileWork.WriteFile("StartProgram: Found 'else' after ';' - this is valid for if statement");
                        // Продолжаем цикл без ошибки - else будет обработан в OPER()
                        continue;
                    }

                    // ФИКС: Проверяем не кончился ли файл после ;
                    if (currentLexValue == "⟂")
                    {
                        return 3;
                    }

                    // ФИКС: Если после ; сразу end - выходим
                    if (currentLexValue == "end")
                    {
                        break;
                    }
                }
                // ФИКС: Обработка else без точки с запятой (для then-веток без ;)
                else if (currentLexValue == "else")
                {
                    _fileWork.WriteFile("StartProgram: Found 'else' without ';' - this is valid for if statement");
                    // Продолжаем цикл без ошибки - else будет обработан в OPER()
                    continue;
                }
                // ФИКС: Если текущая лексема - начало нового оператора, продолжаем
                else if (IsID() ||
                        currentLexValue == "if" ||
                        currentLexValue == "for" ||
                        currentLexValue == "while" ||
                        currentLexValue == "read" ||
                        currentLexValue == "write" ||
                        currentLexValue == "@")
                {
                    _fileWork.WriteFile($"StartProgram: Next operator '{currentLexValue}' without ';' - continuing");
                    // Продолжаем цикл без ошибки
                    continue;
                }
                // ФИКС: Если встретили end - выходим из цикла
                else if (currentLexValue == "end")
                {
                    break;
                }
                else
                {
                    _fileWork.WriteFile($"StartProgram ERROR: Expected ';', 'else', 'end' or operator, but got: '{currentLexValue}'");
                    return 26;
                }
            }

            if (currentLexValue != "end")
            {
                _fileWork.WriteFile($"Ожидается 'end', но получено: '{currentLexValue}'");
                return 3;
            }
            _fileWork.WriteFile("PROG -> end");
            GetCurrentLexem();

            // ФИКС: Принимаем точку из любой таблицы (разделителей или констант)
            if (currentLexValue != "." && currentLexValue != "⟂")
            {
                _fileWork.WriteFile($"Ожидается '.', но получено: '{currentLexValue}'");
                return 24;
            }

            if (currentLexValue == ".")
            {
                _fileWork.WriteFile("PROG -> .");
                GetCurrentLexem();
            }

            // ФИКС: Проверяем что достигнут конец файла после точки
            if (currentLexValue != "⟂")
            {
                _fileWork.WriteFile($"Ожидается конец файла после '.', но получено: '{currentLexValue}'");
                return 28;
            }

            return 0;
        }

        private void GetCurrentLexem()
        {
            if (counterSLexem < parentObj.LexOut.Count)
            {
                currentLex = parentObj.LexOut[counterSLexem];
                currentLexValue = currentLex.value;
                counterSLexem++;
                _fileWork.WriteFile($"GetCurrentLexem: '{currentLexValue}'");
            }
            else
            {
                currentLex.value = "⟂";
                currentLexValue = "⟂";
            }
            byte[] info = new UTF8Encoding(true).GetBytes("\n");
            _fileWork.Write(info, 0, info.Length);
        }

        public int SyntaxStart()
        {
            int answer;
            counterSLexem = 0;
            TID_temp = parentObj.TID.ToList();
            Array.Clear(OutputPoliz, 0, OutputPoliz.Length);
            free = 0;
            labelCounter = 1;
            _fileWork.CreateFile();
            answer = StartProgram();

            if (answer == 0)
            {
                _codeGenerator = new CodeGenerator(this, TID_temp, parentObj.Constants, OutputPoliz, free);
                var generatedCode = _codeGenerator.GenerateCode();
                SaveGeneratedCode(generatedCode);
            }

            _fileWork.Close();
            return answer;
        }

        public GeneratedCode GetCodeGenerator()
        {
            return _codeGenerator.GenerateCode();
        }

        private bool IsID()
        {
            return currentLex.numTable == 4;
        }

        private bool IsNumConst()
        {
            return currentLex.numTable == 3;
        }

        public bool IsReal()
        {
            bool isCorrectNum;
            string currentState = "S";
            int realCounter = 0;

            foreach (char c in currentLexValue)
            {
                switch (currentState)
                {
                    case "S":
                        if (char.IsDigit(c))
                        {
                            currentState = "D";
                        }
                        else if (c == '.')
                        {
                            realCounter++;
                            currentState = "DP";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "D":
                        if (char.IsDigit(c))
                        {
                            // Собираем "буфер"
                        }
                        else if (c == '.')
                        {
                            realCounter++;
                            currentState = "DP";
                        }
                        else if (c == 'E' || c == 'e')
                        {
                            realCounter++;
                            currentState = "EXP";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "DP":
                        if (char.IsDigit(c))
                        {
                            // Собираем "буфер"
                        }
                        else if (c == 'E' || c == 'e')
                        {
                            currentState = "EXP";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "EXP":
                        if (c == '+' || c == '-' || char.IsDigit(c))
                        {
                            realCounter++;
                            currentState = "EXP1";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "EXP1":
                        if (char.IsDigit(c))
                        {

                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "ERR":
                        break;
                }
            }
            if (currentState != "ERR" && currentState != "D")
            {
                isCorrectNum = true;
            }
            else isCorrectNum = false;
            return isCorrectNum;
        }

        private bool IsBin()
        {
            bool isCorrectNum;
            string currentState = "S";

            foreach (char c in currentLexValue)
            {
                switch (currentState)
                {
                    case "S":
                        if (c == '0' || c == '1')
                        {
                            currentState = "D";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "D":
                        if (c == '0' || c == '1')
                        {
                            // Листаем
                        }
                        else if (c == 'B' || c == 'b')
                        {
                            currentState = "ST";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "ST":
                        currentState = "ERR";
                        break;
                    case "ERR":
                        break;
                }
            }
            if (currentState == "ST") isCorrectNum = true;
            else isCorrectNum = false;
            return isCorrectNum;
        }

        public bool IsOct()
        {
            bool isCorrectNum;
            string currentState = "S";

            foreach (char c in currentLexValue)
            {
                switch (currentState)
                {
                    case "S":
                        if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7')
                        {
                            currentState = "D";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "D":
                        if (char.IsDigit(c) || c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7')
                        {
                            // Листаем
                        }
                        else if (c == 'O' || c == 'o')
                        {
                            currentState = "ST";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "ST":
                        currentState = "ERR";
                        break;
                    case "ERR":
                        break;
                }
            }
            if (currentState == "ST") isCorrectNum = true;
            else isCorrectNum = false;
            return isCorrectNum;
        }

        public bool IsDec()
        {
            bool isCorrectNum;
            string currentState = "S";

            foreach (char c in currentLexValue)
            {
                switch (currentState)
                {
                    case "S":
                        if (char.IsDigit(c))
                        {
                            currentState = "D";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "D":
                        if (char.IsDigit(c))
                        {
                            // Листаем
                        }
                        else if (c == 'D' || c == 'd')
                        {
                            currentState = "ST";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "ST":
                        currentState = "ERR";
                        break;
                    case "ERR":
                        break;
                }
            }
            if (currentState != "ERR") isCorrectNum = true;
            else isCorrectNum = false;
            return isCorrectNum;
        }

        public bool IsHex()
        {
            bool isCorrectNum;
            string currentState = "S";

            foreach (char c in currentLexValue)
            {
                switch (currentState)
                {
                    case "S":
                        if (char.IsDigit(c))
                        {
                            currentState = "D";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "D":
                        if (char.IsDigit(c) || c == 'A' || c == 'a' || c == 'B' || c == 'b' || c == 'C' || c == 'c' || c == 'D' || c == 'd'
                            || c == 'E' || c == 'e' || c == 'F' || c == 'f')
                        {

                        }
                        else if (c == 'H' || c == 'h')
                        {
                            currentState = "ST";
                        }
                        else
                        {
                            currentState = "ERR";
                        }
                        break;
                    case "ST":
                        currentState = "ERR";
                        break;
                    case "ERR":
                        break;
                }
            }
            if (currentState == "ST") isCorrectNum = true;
            else isCorrectNum = false;
            return isCorrectNum;
        }

        private bool IsLetterAlphabet(char symbol)
        {
            if (symbol == 'A' || symbol == 'a' || symbol == 'B' || symbol == 'b' || symbol == 'C' || symbol == 'c' ||
                symbol == 'D' || symbol == 'd' || symbol == 'E' || symbol == 'e' || symbol == 'F' || symbol == 'f' ||
                symbol == 'G' || symbol == 'g' || symbol == 'H' || symbol == 'h' || symbol == 'I' || symbol == 'i' ||
                symbol == 'J' || symbol == 'j' || symbol == 'K' || symbol == 'k' || symbol == 'L' || symbol == 'l' ||
                symbol == 'M' || symbol == 'm' || symbol == 'N' || symbol == 'n' || symbol == 'O' || symbol == 'o' ||
                symbol == 'P' || symbol == 'p' || symbol == 'Q' || symbol == 'q' || symbol == 'R' || symbol == 'r' ||
                symbol == 'S' || symbol == 's' || symbol == 'T' || symbol == 't' || symbol == 'U' || symbol == 'u' ||
                symbol == 'V' || symbol == 'v' || symbol == 'W' || symbol == 'w' || symbol == 'X' || symbol == 'x' ||
                symbol == 'Y' || symbol == 'y' || symbol == 'Z' || symbol == 'z')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Семантические функции
        private bool DecID(int number, string type)
        {
            LexemStruct temp;
            if (TID_temp[number].numInTable == number)
            {
                if (TID_temp[number].isDeclared)
                {
                    return false;
                }
                else
                {
                    temp = TID_temp[number];
                    temp.isDeclared = true;
                    temp.type = type;
                    TID_temp[number] = temp;
                    return true;
                }
            }
            return false;
        }

        private bool CheckID()
        {
            if (currentLex.numInTable < TID_temp.Count && TID_temp[currentLex.numInTable].isDeclared)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int CheckNum()
        {
            _fileWork.WriteFile($"CheckNum: '{currentLexValue}'");

            if (IsBin() || IsOct() || IsHex())
            {
                _fileWork.WriteFile($"CheckNum: binary/octal/hex -> integer (%)");
                return 1; // целые числа
            }
            else if (IsDec())
            {
                _fileWork.WriteFile($"CheckNum: decimal -> integer (%)");
                return 1; // целые числа
            }
            else if (IsReal())
            {
                _fileWork.WriteFile($"CheckNum: real -> real (!)");
                return 0; // вещественные числа
            }
            else
            {
                _fileWork.WriteFile($"CheckNum: unrecognized format");
                return 21;
            }
        }

        private string GetType(string op, string t1, string t2)
        {
            _fileWork.WriteFile($"GetType: op='{op}', t1='{t1}', t2='{t2}'");

            if (op == "plus" || op == "min")
            {
                if (t1 == "%" && t2 == "%")
                {
                    return "%";
                }
                else if (t1 == "%" && t2 == "!" || t1 == "!" && t2 == "%")
                {
                    return "!";
                }
                else if (t1 == "!" && t2 == "!")
                {
                    return "!";
                }
                else
                {
                    return String.Empty;
                }
            }
            else if (op == "mult")
            {
                if (t1 == "%" && t2 == "%")
                {
                    return "%";
                }
                else if (t1 == "%" && t2 == "!" || t1 == "!" && t2 == "%")
                {
                    return "!";
                }
                else if (t1 == "!" && t2 == "!")
                {
                    return "!";
                }
                else
                {
                    return String.Empty;
                }
            }
            else if (op == "div")
            {
                if (t1 == "%" && t2 == "%")
                {
                    return "!";
                }
                else if (t1 == "%" && t2 == "!" || t1 == "!" && t2 == "%")
                {
                    return "!";
                }
                else if (t1 == "!" && t2 == "!")
                {
                    return "!";
                }
                else
                {
                    return String.Empty;
                }
            }
            else if (op == "and" || op == "or")
            {
                if (t1 == "$" && t2 == "$")
                {
                    return "$";
                }
                else
                {
                    return String.Empty;
                }
            }
            else if (op == "NE" || op == "EQ" || op == "LT" || op == "LE" || op == "GT" || op == "GE")
            {
                if (t1 == "$" || t2 == "$")
                {
                    return String.Empty;
                }
                else
                {
                    return "$";
                }
            }
            return String.Empty;
        }

        private bool LetEquale()
        {
            if (stackCheckContVir.Count < 2)
            {
                _fileWork.WriteFile($"LetEquale ERROR: Stack has {stackCheckContVir.Count} elements, need 2");
                return false;
            }

            string t2 = stackCheckContVir.Pop();
            string t1 = stackCheckContVir.Pop();

            _fileWork.WriteFile($"LetEquale: t1(ID)={t1}, t2(expression)={t2}, equal={t1 == t2}");

            return t1 == t2;
        }

        private bool BoolEquale()
        {
            if (stackCheckContVir.Count == 0)
            {
                _fileWork.WriteFile($"BoolEquale ERROR: Stack is empty");
                return false;
            }

            string t1 = stackCheckContVir.Pop();
            bool result = t1 == "$";

            _fileWork.WriteFile($"BoolEquale: t1={t1}, result={result}");

            return result;
        }

        // ПОЛИЗ функции
        void putPolizeLex(PolizStruct lex)
        {
            OutputPoliz[free] = lex;

            string logMsg = $"POLIZ[{free}]: ";
            if (lex.classValue == 0) // метка
                logMsg += $"Label m{lex.value}:";
            else if (lex.classValue == 2 && lex.type == "УПЛ")
                logMsg += $"УПЛ m{lex.value}";
            else if (lex.classValue == 2 && lex.type == "БП")
                logMsg += $"БП m{lex.value}";
            else if (lex.classValue == 2) // операция
                logMsg += $"Operation '{lex.type}'";
            else if (lex.classValue == 3) // константа
                logMsg += $"Constant '{lex.type}'";
            else if (lex.classValue == 4) // идентификатор
                logMsg += $"Variable '{lex.type}'";

            _fileWork.WriteFile(logMsg);
            free++;
        }

        PolizStruct putOperationLex(string oper)
        {
            PolizStruct temp = new PolizStruct();

            if (oper == "!F" || oper == "УПЛ")
            {
                temp.type = "УПЛ";
                temp.classValue = 2;
                temp.value = 0;
            }
            else if (oper == "!" || oper == "БП")
            {
                temp.type = "БП";
                temp.classValue = 2;
                temp.value = 0;
            }
            else if (oper == "R")
            {
                temp.type = "R";
                temp.classValue = 2;
                temp.value = 22;
            }
            else if (oper == "W")
            {
                temp.type = "W";
                temp.classValue = 2;
                temp.value = 23;
            }
            else
            {
                int counter = 0;
                foreach (string s in parentObj.Delimiters)
                {
                    if (s == oper) break;
                    counter++;
                }
                temp.type = oper;
                temp.classValue = 2;
                temp.value = counter;
            }
            return temp;
        }

        void putPolizeLex5(PolizStruct lex)
        {
            lex.classValue = 5;
            OutputPoliz[free++] = lex;
            OutputPoliz[free - 1].type = lex.type;
        }

        PolizStruct makePolizeLabel()
        {
            PolizStruct temp = new PolizStruct();
            temp.classValue = 0;
            temp.value = labelCounter;
            temp.type = $"m{labelCounter}:";

            int currentLabel = labelCounter;
            labelCounter++;

            _fileWork.WriteFile($"Created label m{currentLabel}:");
            return temp;
        }

        PolizStruct makePolizeLabelWithNumber(int labelNumber)
        {
            PolizStruct temp = new PolizStruct();
            temp.classValue = 0;
            temp.value = labelNumber;
            temp.type = $"m{labelNumber}:";

            _fileWork.WriteFile($"Created label m{labelNumber}: with value {labelNumber}");
            return temp;
        }

        // ГРАММАТИКА
        private int DESC()
        {
            int code;
            int stackNum;

            while (IsID())
            {
                _fileWork.WriteFile(TempPath + "DESC -> I1");
                code = I1();
                if (code != 0) return code;

                _fileWork.WriteFile(TempPath + "DESC -> TYPE");
                code = TYPE();
                if (code != 0) return code;

                string variableType = currentLexValue;
                _fileWork.WriteFile($"DESC: Saving type '{variableType}' for variables");

                GetCurrentLexem();

                while (stackCheckDeclare.Count > 0)
                {
                    stackNum = stackCheckDeclare.Pop();
                    if (!DecID(stackNum, variableType))
                    {
                        _fileWork.WriteFile($"DESC ERROR: Failed to declare ID {stackNum} with type {variableType}");
                        return 101;
                    }
                    else
                    {
                        _fileWork.WriteFile($"DESC: Successfully declared ID {stackNum} with type {variableType}");
                    }
                }

                if (currentLexValue != ";")
                {
                    _fileWork.WriteFile($"DESC ERROR: Expected ';' after type declaration, but got: '{currentLexValue}'");
                    return 4;
                }
                _fileWork.WriteFile(TempPath + "DESC -> ;");
                GetCurrentLexem();
            }

            return 0;
        }

        private int I1()
        {
            int code;
            _fileWork.WriteFile("-> ID ");
            code = ID();
            if (code != 0) return code;

            stackCheckDeclare.Push(currentLex.numInTable);
            GetCurrentLexem();

            code = I2();
            if (code != 0) return code;

            return 0;
        }

        private int ID()
        {
            int code;
            code = IsIDAlphabet();
            if (code != 0) return code;

            _fileWork.WriteFile("-> " + currentLexValue + " ");
            return 0;
        }

        private int I2()
        {
            if (currentLexValue == ",")
            {
                _fileWork.WriteFile(TempPath + "-> ,");
                GetCurrentLexem();
                _fileWork.WriteFile(TempPath);
                return I1();
            }
            else if (currentLexValue == ":")
            {
                return 0;
            }
            else if (IsID())
            {
                return 3;
            }
            return 0;
        }

        private int TYPE()
        {
            if (currentLexValue == ":")
            {
                _fileWork.WriteFile("TYPE -> :");
                GetCurrentLexem();
            }
            else
            {
                return 4;
            }

            if (currentLexValue == "%" || currentLexValue == "!" || currentLexValue == "$")
            {
                _fileWork.WriteFile("TYPE -> " + currentLexValue + " ");
                return 0;
            }
            else
            {
                _fileWork.WriteFile($"TYPE ERROR: Expected type ('%', '!', or '$'), but got: '{currentLexValue}'");
                return 5;
            }
        }

        private int IsIDAlphabet()
        {
            for (int i = 1; i < currentLexValue.Length; i++)
            {
                if (!char.IsDigit(currentLexValue[i]) && !IsLetterAlphabet(currentLexValue[i]))
                {
                    return 7;
                }
            }
            return 0;
        }

        private int OPER()
        {
            if (currentLexValue == "end")
            {
                return 0;
            }
            else if (currentLexValue == "@")
            {
                _fileWork.WriteFile("OPER -> SOST");
                return SOST();
            }
            else if (IsID())
            {
                TempPath += "OPER -> LET ";
                return LET();
            }
            else if (currentLexValue == "if")
            {
                TempPath += "OPER -> IFEL ";
                return IFEL();
            }
            else if (currentLexValue == "for")
            {
                TempPath += "OPER -> FIXLO ";
                return FIXLO();
            }
            else if (currentLexValue == "while")
            {
                TempPath += "OPER -> IFLO ";
                return IFLO();
            }
            else if (currentLexValue == "read")
            {
                TempPath += "OPER -> INPU ";
                return INPU();
            }
            else if (currentLexValue == "write")
            {
                TempPath += "OPER -> OUPU ";
                return OUPU();
            }
            else
            {
                return 8;
            }
        }

        private int SOST()
        {
            int code;
            GetCurrentLexem();
            code = OPER();
            if (code != 0) return code;

            while (currentLexValue == "@")
            {
                _fileWork.WriteFile("SOST -> @");
                GetCurrentLexem();
                code = OPER();
                if (code != 0) return code;
            }

            return 0;
        }

        private int LET()
        {
            PolizStruct temp;
            int code;

            _fileWork.WriteFile($"=== LET START: {currentLexValue} ===");

            string currentID = currentLexValue;
            int currentIDIndex = currentLex.numInTable;
            _fileWork.WriteFile($"LET: Saving ID '{currentID}' with index {currentIDIndex}");

            _fileWork.WriteFile(TempPath + "LET -> ID ");
            code = ID();
            if (code != 0) return code;

            if (currentIDIndex >= TID_temp.Count || !TID_temp[currentIDIndex].isDeclared)
            {
                _fileWork.WriteFile($"LET ERROR: ID '{currentID}' not declared or out of range");
                return 102;
            }

            string idType = TID_temp[currentIDIndex].type;

            stackCheckContVir.Clear();
            stackCheckContVir.Push(idType);
            _fileWork.WriteFile($"LET: Pushed ID type '{idType}' to stack");

            temp.classValue = currentLex.numTable;
            temp.value = currentIDIndex;
            temp.type = currentID;
            PolizStruct idPolizEntry = temp;

            GetCurrentLexem();

            if (currentLexValue != "ass")
            {
                _fileWork.WriteFile($"LET ERROR: Expected 'ass', but got: '{currentLexValue}'");
                return 10;
            }
            _fileWork.WriteFile("LET -> ass");
            GetCurrentLexem();

            code = VIR();
            if (code != 0) return code;

            if (!LetEquale())
            {
                _fileWork.WriteFile($"LET ERROR: Type mismatch in assignment");
                return 103;
            }

            putPolizeLex(idPolizEntry);
            _fileWork.WriteFile($"LET: Added ID '{currentID}' to POLIZ after expression");

            putPolizeLex(putOperationLex("="));
            _fileWork.WriteFile($"LET: Added assignment operation");

            if (currentLexValue == ";")
            {
                GetCurrentLexem();
                _fileWork.WriteFile("LET: Skipping ';'");
            }

            _fileWork.WriteFile($"=== LET SUCCESS ===");
            return 0;
        }

        private int VIR()
        {
            int code;
            code = OPRD();
            TempPath += "VIR -> OPRD ";
            if (code != 0) return code;

            code = VIR1();
            if (code != 0) return code;

            if (stackCheckContVir.Count == 0)
            {
                _fileWork.WriteFile("VIR ERROR: Expression didn't return type to stack");
                return 25;
            }

            _fileWork.WriteFile($"VIR: Expression type = {stackCheckContVir.Peek()}");
            return 0;
        }

        private int VIR1()
        {
            int code;
            if (currentLexValue == "NE" || currentLexValue == "EQ" || currentLexValue == "LT" ||
                currentLexValue == "LE" || currentLexValue == "GT" || currentLexValue == "GE")
            {
                _fileWork.WriteFile(TempPath + "VIR1 -> " + currentLexValue + " ");

                string operation = currentLexValue;

                if (stackCheckContVir.Count == 0)
                {
                    _fileWork.WriteFile($"VIR1 ERROR: No left operand for '{operation}'");
                    return 110;
                }

                GetCurrentLexem();

                code = VIR();
                if (code != 0) return code;

                if (stackCheckContVir.Count < 2)
                {
                    _fileWork.WriteFile($"VIR1 ERROR: Not enough operands for '{operation}'");
                    return 110;
                }

                string t2 = stackCheckContVir.Pop();
                string t1 = stackCheckContVir.Pop();
                string res = GetType(operation, t1, t2);

                _fileWork.WriteFile($"VIR1: CheckOperation: t1={t1}, op={operation}, t2={t2}, result={res}");

                if (res != string.Empty)
                {
                    stackCheckContVir.Push(res);
                    putPolizeLex(putOperationLex(operation));
                    return 0;
                }
                else
                {
                    _fileWork.WriteFile($"VIR1 ERROR: Type mismatch for '{operation}' between {t1} and {t2}");
                    return 110;
                }
            }
            return 0;
        }

        private int OPRD()
        {
            int code;
            _fileWork.WriteFile($"OPRD START: currentLexValue = '{currentLexValue}'");

            TempPath += "OPRD -> SLAG ";
            code = SLAG();
            if (code != 0) return code;

            while (currentLexValue == "plus" || currentLexValue == "min" || currentLexValue == "or")
            {
                _fileWork.WriteFile(TempPath + "OPRD -> " + currentLexValue + " ");
                string operation = currentLexValue;

                GetCurrentLexem();

                code = SLAG();
                if (code != 0) return code;

                if (stackCheckContVir.Count < 2)
                {
                    _fileWork.WriteFile($"OPRD ERROR: Not enough operands for '{operation}'");
                    return 111;
                }

                string t2 = stackCheckContVir.Pop();
                string t1 = stackCheckContVir.Pop();
                string res = GetType(operation, t1, t2);

                _fileWork.WriteFile($"OPRD: CheckOperation: t1={t1}, op={operation}, t2={t2}, result={res}");

                if (res != string.Empty)
                {
                    stackCheckContVir.Push(res);
                    putPolizeLex(putOperationLex(operation));
                }
                else
                {
                    _fileWork.WriteFile($"OPRD ERROR: Type mismatch for '{operation}' between {t1} and {t2}");
                    return 111;
                }

                _fileWork.WriteFile($"OPRD: Performed {operation} -> {stackCheckContVir.Peek()}");
            }

            _fileWork.WriteFile($"OPRD END: currentLexValue = '{currentLexValue}'");
            return 0;
        }

        private int SLAG()
        {
            int code;
            _fileWork.WriteFile($"SLAG START: currentLexValue = '{currentLexValue}'");

            TempPath += "SLAG -> MNOJ ";
            code = MNOJ();
            if (code != 0) return code;

            while (currentLexValue == "mult" || currentLexValue == "div" || currentLexValue == "and")
            {
                _fileWork.WriteFile(TempPath + "SLAG -> " + currentLexValue + " ");
                string operation = currentLexValue;

                GetCurrentLexem();

                code = MNOJ();
                if (code != 0) return code;

                if (stackCheckContVir.Count < 2)
                {
                    _fileWork.WriteFile($"SLAG ERROR: Not enough operands for '{operation}'");
                    return 112;
                }

                string t2 = stackCheckContVir.Pop();
                string t1 = stackCheckContVir.Pop();
                string res = GetType(operation, t1, t2);

                _fileWork.WriteFile($"SLAG: CheckOperation: t1={t1}, op={operation}, t2={t2}, result={res}");

                if (res != string.Empty)
                {
                    stackCheckContVir.Push(res);
                    putPolizeLex(putOperationLex(operation));
                }
                else
                {
                    _fileWork.WriteFile($"SLAG ERROR: Type mismatch for '{operation}' between {t1} and {t2}");
                    return 112;
                }

                _fileWork.WriteFile($"SLAG: Performed {operation} -> {stackCheckContVir.Peek()}");
            }

            _fileWork.WriteFile($"SLAG END: currentLexValue = '{currentLexValue}'");
            return 0;
        }

        private int MNOJ()
        {
            PolizStruct temp;
            int code;

            _fileWork.WriteFile($"MNOJ START: currentLexValue = '{currentLexValue}'");

            if (IsID())
            {
                _fileWork.WriteFile(TempPath + "MNOJ -> ID ");
                code = ID();
                if (code != 0) return code;

                if (!CheckID())
                {
                    return 113;
                }

                string idType = TID_temp[currentLex.numInTable].type;
                stackCheckContVir.Push(idType);
                _fileWork.WriteFile($"MNOJ: ID '{currentLexValue}' type = {idType} pushed to stack");

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;

                putPolizeLex(temp);

                GetCurrentLexem();
                _fileWork.WriteFile($"MNOJ END (ID): currentLexValue = '{currentLexValue}'");
                return 0;
            }
            else if (IsNumConst())
            {
                code = NUM();
                _fileWork.WriteFile(TempPath + "MNOJ -> NUM ");
                if (code != 0) return code;

                _fileWork.WriteFile("-> " + currentLexValue + " ");

                string constType = IsReal() ? "!" : "%";
                stackCheckContVir.Push(constType);

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;

                putPolizeLex(temp);

                GetCurrentLexem();
                _fileWork.WriteFile($"MNOJ END (NUM): currentLexValue = '{currentLexValue}'");
                return 0;
            }
            else if (currentLexValue == "false" || currentLexValue == "true")
            {
                _fileWork.WriteFile(TempPath + "MNOJ -> " + currentLexValue + " ");
                stackCheckContVir.Push("$");

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;
                putPolizeLex(temp);

                GetCurrentLexem();
                _fileWork.WriteFile($"MNOJ END (bool): currentLexValue = '{currentLexValue}'");
                return 0;
            }
            else if (currentLexValue == "(")
            {
                _fileWork.WriteFile(TempPath + "MNOJ -> ( ");
                temp_str = TempPath;
                GetCurrentLexem();

                TempPath += "MNOJ -> VIR ";
                code = VIR();
                if (code != 0) return code;

                _fileWork.WriteFile(temp_str + "MNOJ -> ) ");
                if (currentLexValue != ")")
                {
                    return 21;
                }

                GetCurrentLexem();
                _fileWork.WriteFile($"MNOJ END (parentheses): currentLexValue = '{currentLexValue}'");
                return 0;
            }
            else
            {
                _fileWork.WriteFile($"MNOJ ERROR: Unexpected token '{currentLexValue}'");
                return 22;
            }
        }

        private int NUM()
        {
            int checkResult = CheckNum();

            _fileWork.WriteFile($"NUM: Checking '{currentLexValue}', result={checkResult}");

            if (checkResult == 0)
            {
                stackCheckContVir.Push("!");
                return 0;
            }
            else if (checkResult == 1)
            {
                stackCheckContVir.Push("%");
                return 0;
            }
            else
            {
                return 23;
            }
        }

        private int IFEL()
        {
            int code;
            int jumpFalsePos, jumpUncondPos;

            _fileWork.WriteFile(TempPath + "IFEL -> if ");
            temp_if = TempPath;
            GetCurrentLexem();

            TempPath += "IFEL -> VIR ";
            code = VIR();
            if (code != 0) return code;

            if (!BoolEquale())
            {
                return 104;
            }

            jumpFalsePos = free;
            int elseLabelNum = labelCounter;

            PolizStruct tempJumpFalse = new PolizStruct();
            tempJumpFalse.type = "УПЛ";
            tempJumpFalse.classValue = 2;
            tempJumpFalse.value = elseLabelNum;
            putPolizeLex(tempJumpFalse);

            if (currentLexValue != "then")
            {
                return 11;
            }
            _fileWork.WriteFile(temp_if + "IFEL -> then ");
            GetCurrentLexem();

            TempPath = temp_if + "IFEL -> OPER ";
            code = OPER();
            if (code != 0) return code;

            jumpUncondPos = free;
            int afterIfLabelNum = labelCounter + 1;

            PolizStruct tempJumpUncond = new PolizStruct();
            tempJumpUncond.type = "БП";
            tempJumpUncond.classValue = 2;
            tempJumpUncond.value = afterIfLabelNum;
            putPolizeLex(tempJumpUncond);

            if (currentLexValue == "else")
            {
                putPolizeLex(makePolizeLabel());

                GetCurrentLexem();

                TempPath = temp_if + "IFEL -> OPER ";
                code = OPER();
                if (code != 0) return code;

                putPolizeLex(makePolizeLabel());
            }
            else
            {
                putPolizeLex(makePolizeLabel());
            }

            return 0;
        }

        private int FIXLO()
        {
            int falseJumpPos;
            int code;

            _fileWork.WriteFile(TempPath + "FIXLO -> for ");
            temp_for = TempPath;
            GetCurrentLexem();

            string savedTempPath = TempPath;
            TempPath = temp_for + "FIXLO -> LET ";

            code = LET();
            if (code != 0) return code;

            _fileWork.WriteFile($"FIXLO: After first LET, currentLexValue = '{currentLexValue}'");

            TempPath = temp_for;

            if (currentLexValue != "to")
            {
                _fileWork.WriteFile($"FIXLO ERROR: Expected 'to', but got: '{currentLexValue}'");
                return 14;
            }
            _fileWork.WriteFile(TempPath + "FIXLO -> to ");
            GetCurrentLexem();

            int loopStartLabelNum = labelCounter;
            PolizStruct startLabel = makePolizeLabel();
            putPolizeLex(startLabel);

            int loopStartPos = free - 1;
            _fileWork.WriteFile($"FIXLO: Loop start label m{loopStartLabelNum} at position {loopStartPos}");

            PolizStruct tempVarI;
            tempVarI.classValue = 4;
            int iIndex = -1;
            for (int j = 0; j < TID_temp.Count; j++)
            {
                if (TID_temp[j].value == "i" || TID_temp[j].value.Contains("i"))
                {
                    iIndex = j;
                    break;
                }
            }
            if (iIndex == -1) iIndex = 0;
            tempVarI.value = iIndex;
            tempVarI.type = "i";
            putPolizeLex(tempVarI);
            _fileWork.WriteFile($"FIXLO: Added variable i at position {free - 1}");

            code = VIR();
            if (code != 0) return code;

            if (stackCheckContVir.Count == 0) return 105;
            string exprType = stackCheckContVir.Pop();
            if (exprType != "%" && exprType != "!") return 105;

            putPolizeLex(putOperationLex("LE"));
            _fileWork.WriteFile($"FIXLO: Added LE operation at position {free - 1}");

            falseJumpPos = free;
            int exitLabelNum = labelCounter;

            PolizStruct tempJump = new PolizStruct();
            tempJump.type = "УПЛ";
            tempJump.classValue = 2;
            tempJump.value = exitLabelNum;

            putPolizeLex(tempJump);
            _fileWork.WriteFile($"FIXLO: Added УПЛ operation at position {falseJumpPos} to jump to m{exitLabelNum}");

            if (currentLexValue != "do")
            {
                _fileWork.WriteFile($"FIXLO ERROR: Expected 'do', but got: '{currentLexValue}'");
                return 15;
            }
            _fileWork.WriteFile(TempPath + "FIXLO -> do");
            GetCurrentLexem();

            int beforeLoopBodyPos = free;

            code = OPER();
            if (code != 0) return code;

            while (currentLexValue == "@")
            {
                _fileWork.WriteFile("FIXLO: Found @ for compound statement");
                GetCurrentLexem();

                code = OPER();
                if (code != 0) return code;

                if (currentLexValue == ";")
                {
                    _fileWork.WriteFile("FIXLO: Skipping ; after @ operator");
                    GetCurrentLexem();
                }
            }

            putPolizeLex(tempVarI);
            _fileWork.WriteFile($"FIXLO: Added variable i for increment at position {free - 1}");

            PolizStruct tempOne = new PolizStruct();
            tempOne.classValue = 3;
            int constOneIndex = parentObj.Constants.IndexOf("1");
            if (constOneIndex == -1)
            {
                parentObj.Constants.Add("1");
                constOneIndex = parentObj.Constants.Count - 1;
            }
            tempOne.value = constOneIndex;
            tempOne.type = "1";
            putPolizeLex(tempOne);
            _fileWork.WriteFile($"FIXLO: Added constant 1 at position {free - 1}");

            putPolizeLex(putOperationLex("plus"));
            _fileWork.WriteFile($"FIXLO: Added plus operation at position {free - 1}");

            putPolizeLex(tempVarI);
            _fileWork.WriteFile($"FIXLO: Added variable i for assignment at position {free - 1}");

            putPolizeLex(putOperationLex("="));
            _fileWork.WriteFile($"FIXLO: Added = operation at position {free - 1}");

            PolizStruct tempBackJump = new PolizStruct();
            tempBackJump.type = "БП";
            tempBackJump.classValue = 2;
            tempBackJump.value = loopStartLabelNum;

            putPolizeLex(tempBackJump);
            _fileWork.WriteFile($"FIXLO: Added БП operation at position {free - 1} to jump to m{loopStartLabelNum}");

            PolizStruct exitLabel = makePolizeLabelWithNumber(exitLabelNum);
            putPolizeLex(exitLabel);
            _fileWork.WriteFile($"FIXLO: Added exit label m{exitLabelNum}");

            return 0;
        }

        private int IFLO()
        {
            int jumpFalsePos;
            int code;

            _fileWork.WriteFile(TempPath + "IFLO -> while ");
            temp_while = TempPath;
            GetCurrentLexem();

            int loopStartLabelNum = labelCounter;
            putPolizeLex(makePolizeLabel());

            code = VIR();
            if (code != 0) return code;

            if (!BoolEquale()) return 106;

            jumpFalsePos = free;
            int exitLabelNum = labelCounter;

            PolizStruct tempJumpFalse = new PolizStruct();
            tempJumpFalse.type = "УПЛ";
            tempJumpFalse.classValue = 2;
            tempJumpFalse.value = exitLabelNum;
            putPolizeLex(tempJumpFalse);

            if (currentLexValue != "do") return 16;

            GetCurrentLexem();

            while (currentLexValue != "end" && currentLexValue != "⟂" &&
                   currentLexValue != "while" && currentLexValue != "for" &&
                   currentLexValue != "if" && currentLexValue != "read" &&
                   currentLexValue != "write")
            {
                _fileWork.WriteFile($"IFLO: Обрабатываем оператор внутри цикла: '{currentLexValue}'");

                code = OPER();
                if (code != 0) return code;

                if (currentLexValue == ";")
                {
                    _fileWork.WriteFile("IFLO: Пропускаем ';'");
                    GetCurrentLexem();
                }
            }

            PolizStruct tempJumpBack = new PolizStruct();
            tempJumpBack.type = "БП";
            tempJumpBack.classValue = 2;
            tempJumpBack.value = loopStartLabelNum;
            putPolizeLex(tempJumpBack);

            putPolizeLex(makePolizeLabel());

            return 0;
        }

        private int INPU()
        {
            PolizStruct temp;
            int code;

            _fileWork.WriteFile(TempPath + "INPU -> read ");
            GetCurrentLexem();

            if (currentLexValue != "(")
            {
                return 17;
            }
            _fileWork.WriteFile(TempPath + "INPU -> ( ");
            GetCurrentLexem();

            _fileWork.WriteFile(TempPath + "INPU -> ID ");
            code = ID();
            if (code != 0) return code;

            if (!CheckID())
            {
                return 107;
            }

            temp.classValue = currentLex.numTable;
            temp.value = currentLex.numInTable;
            temp.type = currentLexValue;
            putPolizeLex5(temp);
            GetCurrentLexem();

            while (currentLexValue == ",")
            {
                _fileWork.WriteFile(TempPath + "INPU -> , ");
                GetCurrentLexem();

                if (IsID())
                {
                    _fileWork.WriteFile(TempPath + "INPU -> ID ");
                    code = ID();
                    if (code != 0) return code;

                    if (!CheckID())
                    {
                        return 108;
                    }

                    temp.classValue = currentLex.numTable;
                    temp.value = currentLex.numInTable;
                    temp.type = currentLexValue;
                    putPolizeLex5(temp);
                }
                GetCurrentLexem();
            }

            if (currentLexValue != ")")
            {
                return 18;
            }
            _fileWork.WriteFile(TempPath + "INPU -> ) ");

            putPolizeLex(putOperationLex("R"));
            GetCurrentLexem();
            return 0;
        }

        private int OUPU()
        {
            int code;

            _fileWork.WriteFile(TempPath + "OUPU -> write ");
            GetCurrentLexem();

            if (currentLexValue != "(")
            {
                return 19;
            }
            _fileWork.WriteFile(TempPath + "OUPU -> ( ");
            GetCurrentLexem();

            code = VIR();
            if (code != 0) return code;

            while (currentLexValue == ",")
            {
                GetCurrentLexem();
                code = VIR();
                if (code != 0) return code;
            }

            if (currentLexValue != ")")
            {
                return 20;
            }
            _fileWork.WriteFile(TempPath + "OUPU -> ) ");

            putPolizeLex(putOperationLex("W"));
            GetCurrentLexem();
            return 0;
        }

        private int UNARM()
        {
            int code;
            GetCurrentLexem();
            code = MNOJ();
            if (code != 0) return code;

            // Унарная операция не реализована полностью
            return 114;
        }

        #region Генерация кода
        private void SaveGeneratedCode(GeneratedCode generatedCode)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $"generated_code_{timestamp}.txt";
                _codeGenerator.SaveToFile(outputPath);
            }
            catch (Exception ex)
            {
                _fileWork.WriteFile($"Ошибка сохранения кода: {ex.Message}");
            }
        }
        #endregion
    }
}