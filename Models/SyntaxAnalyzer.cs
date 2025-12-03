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
                return 28; // Добавьте эту ошибку в ErrorHandler
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
            _fileWork.CreateFile();
            answer = StartProgram();
            _fileWork.Close();
            return answer;
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
            // Просто проверяем, объявлен ли идентификатор
            // НЕ добавляем тип в стек здесь - это сделает LET()
            if (TID_temp[currentLex.numInTable].isDeclared)
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

            // Сначала проверяем специальные форматы чисел
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
                    return "%";
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

        private bool CheckOperation()
        {
            // ФИКС: Проверяем достаточно ли элементов в стеке
            if (stackCheckContVir.Count < 3)
            {
                _fileWork.WriteFile($"CheckOperation ERROR: Stack has {stackCheckContVir.Count} elements, need 3");
                return false;
            }

            string t2 = stackCheckContVir.Pop();
            string op = stackCheckContVir.Pop();
            string t1 = stackCheckContVir.Pop();

            string res = GetType(op, t1, t2);

            _fileWork.WriteFile($"CheckOperation: t1={t1}, op={op}, t2={t2}, result={res}");

            if (res != string.Empty)
            {
                stackCheckContVir.Push(res);
                putPolizeLex(putOperationLex(op));
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckUnarOperation()
        {
            // ФИКС: Проверяем что стек не пуст
            if (stackCheckContVir.Count == 0)
            {
                _fileWork.WriteFile($"CheckUnarOperation ERROR: Stack is empty");
                return false;
            }

            string type = stackCheckContVir.Pop();
            bool result = type == "$";

            if (result)
            {
                stackCheckContVir.Push("$");
            }

            _fileWork.WriteFile($"CheckUnarOperation: type={type}, result={result}");

            return result;
        }
        private bool LetEquale()
        {
            // ФИКС: Проверяем достаточно ли элементов в стеке
            if (stackCheckContVir.Count < 2)
            {
                _fileWork.WriteFile($"LetEquale ERROR: Stack has {stackCheckContVir.Count} elements, need 2");
                return false;
            }

            string t1 = stackCheckContVir.Pop();
            string t2 = stackCheckContVir.Pop();

            _fileWork.WriteFile($"LetEquale: t1={t1}, t2={t2}, equal={t1 == t2}");

            return t1 == t2;
        }

        private bool BoolEquale()
        {
            // ФИКС: Проверяем что стек не пуст
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
            OutputPoliz[free++] = lex;
        }

        PolizStruct putOperationLex(string oper)
        {
            PolizStruct temp;
            int counter = 0;
            int dopCount = parentObj.Separators.Count - 1;
            if (oper == "!E")
            {
                counter = 20;
            }
            else if (oper == "!")
            {
                counter = 21;
            }
            else if (oper == "R")
            {
                counter = 22;
            }
            else if (oper == "W")
            {
                counter = 23;
            }
            else
            {
                foreach (string s in parentObj.Separators)
                {
                    if (s == oper)
                    {
                        break;
                    }
                    counter++;
                }
            }
            temp.type = oper;
            temp.classValue = 2;
            temp.value = counter;
            return temp;
        }

        void putPolizeLex5(PolizStruct lex)
        {
            lex.classValue = 5;
            OutputPoliz[free++] = lex;
            OutputPoliz[free - 1].type = lex.type;
        }

        PolizStruct makePolizeLabel(int k)
        {
            PolizStruct temp;
            temp.classValue = 0;
            temp.value = k;
            temp.type = k.ToString();
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

                // СОХРАНИТЬ ТИП ДО GetCurrentLexem()
                string variableType = currentLexValue; // Сохраняем тип ("%", "!", "$")
                _fileWork.WriteFile($"DESC: Saving type '{variableType}' for variables");

                GetCurrentLexem(); // Переходим к следующей лексеме после типа

                while (stackCheckDeclare.Count > 0)
                {
                    stackNum = stackCheckDeclare.Pop();
                    // Используем СОХРАНЕННЫЙ тип, а не currentLexValue
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

                // ФИКС: Проверяем точку с запятой после объявления типа
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
            int code;
            if (currentLexValue == ",")
            {
                _fileWork.WriteFile(TempPath + "-> ,");
                GetCurrentLexem();
                _fileWork.WriteFile(TempPath);
                code = I1();
                return code;
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

            // ФИКС: Проверяем что текущая лексема является допустимым типом
            if (currentLexValue == "%" || currentLexValue == "!" || currentLexValue == "$")
            {
                _fileWork.WriteFile("TYPE -> " + currentLexValue + " ");
                // НЕ вызываем GetCurrentLexem() здесь - это сделает вызывающий код
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
            int code;

            if (currentLexValue == "end")
            {
                return 0;
            }
            else if (currentLexValue == "@")
            {
                _fileWork.WriteFile("OPER -> SOST");
                code = SOST();
                return code;
            }
            else if (IsID())
            {
                TempPath += "OPER -> LET ";
                code = LET();
                return code;
            }
            else if (currentLexValue == "if")
            {
                TempPath += "OPER -> IFEL ";
                code = IFEL();
                return code;
            }
            else if (currentLexValue == "for")
            {
                TempPath += "OPER -> FIXLO ";
                code = FIXLO();
                return code;
            }
            else if (currentLexValue == "while")
            {
                TempPath += "OPER -> IFLO ";
                code = IFLO();
                return code;
            }
            else if (currentLexValue == "read")
            {
                TempPath += "OPER -> INPU ";
                code = INPU();
                return code;
            }
            else if (currentLexValue == "write")
            {
                TempPath += "OPER -> OUPU ";
                code = OUPU();
                return code;
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

            // 1. СОХРАНИТЕ идентификатор ДО ЛЮБЫХ операций
            string currentID = currentLexValue;
            int currentIDIndex = currentLex.numInTable;
            _fileWork.WriteFile($"LET: Saving ID '{currentID}' with index {currentIDIndex}");

            // 2. Обрабатываем ID (это может изменить currentLex)
            _fileWork.WriteFile(TempPath + "LET -> ID ");
            code = ID();
            if (code != 0) return code;

            // 3. Проверяем объявлен ли идентификатор используя СОХРАНЕННЫЙ индекс
            if (currentIDIndex >= TID_temp.Count || !TID_temp[currentIDIndex].isDeclared)
            {
                _fileWork.WriteFile($"LET ERROR: ID '{currentID}' not declared (index: {currentIDIndex}, TID count: {TID_temp.Count})");
                return 102;
            }

            // 4. Получаем тип используя СОХРАНЕННЫЙ индекс
            string idType = TID_temp[currentIDIndex].type;

            // 5. ЯВНО очищаем стек и добавляем правильные типы
            stackCheckContVir.Clear(); // Очищаем весь мусор
            stackCheckContVir.Push(idType); // Добавляем тип идентификатора
            _fileWork.WriteFile($"LET: ID '{currentID}' type = {idType}, stack cleared and type pushed");

            // 6. Создаем POLIZ запись для идентификатора
            temp.classValue = 4; // Таблица идентификаторов
            temp.value = currentIDIndex;
            temp.type = currentID;
            putPolizeLex5(temp);

            GetCurrentLexem();

            // 7. Проверяем 'ass'
            _fileWork.WriteFile(TempPath + "LET -> ass, currentLexValue = '" + currentLexValue + "'");
            if (currentLexValue != "ass")
            {
                return 10;
            }

            GetCurrentLexem();

            // 8. Обрабатываем выражение (10)
            _fileWork.WriteFile($"LET: Before VIR, currentLexValue = '{currentLexValue}'");
            code = VIR();
            if (code != 0) return code;

            // 9. Проверяем стек перед сравнением
            _fileWork.WriteFile($"LET: Stack before LetEquale = [{string.Join(", ", stackCheckContVir)}]");

            if (stackCheckContVir.Count < 2)
            {
                _fileWork.WriteFile($"LET ERROR: Stack has {stackCheckContVir.Count} elements, need 2");
                return 103;
            }

            // 10. Сравниваем типы
            if (!LetEquale())
            {
                string t2 = stackCheckContVir.Pop();
                string t1 = stackCheckContVir.Pop();
                _fileWork.WriteFile($"LET: LetEquale FAILED - {t1} != {t2}");
                return 103;
            }

            putPolizeLex(putOperationLex("="));

            // 11. ФИКС: Гибкая проверка разделителей после выражения
            _fileWork.WriteFile($"LET: After VIR, currentLexValue = '{currentLexValue}'");

            // Допустимые разделители после оператора присваивания
            bool isValidSeparator = currentLexValue == ";" ||
                                   currentLexValue == "@" ||
                                   currentLexValue == "to" ||  // для цикла for
                                   currentLexValue == "do" ||  // для циклов
                                   currentLexValue == "then" || // для if
                                   currentLexValue == "else" || // для if
                                   currentLexValue == "end";    // конец программы

            if (!isValidSeparator && currentLexValue != "⟂")
            {
                _fileWork.WriteFile($"LET ERROR: Expected ';', '@', 'to', 'do', 'then', 'else' or 'end', but got: '{currentLexValue}'");
                return 4;
            }

            _fileWork.WriteFile($"LET: Valid separator '{currentLexValue}' after expression");

            // Переходим к следующей лексеме только для ';'
            // Для других разделителей остаемся на текущей лексеме
            if (currentLexValue == ";")
            {
                GetCurrentLexem();
            }
            _fileWork.WriteFile($"=== LET SUCCESS ===");
            return 0;
        }

        private int IFEL()
        {
            int code;
            int m1, m2;

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

            m1 = free++;
            putPolizeLex(putOperationLex("!F"));

            if (currentLexValue != "then")
            {
                return 11;
            }
            _fileWork.WriteFile(temp_if + "IFEL -> then ");
            GetCurrentLexem();

            TempPath = temp_if + "IFEL -> OPER ";
            code = OPER();
            if (code != 0) return code;

            m2 = free++;
            putPolizeLex(putOperationLex("!"));
            OutputPoliz[m1] = makePolizeLabel(free);

            // ФИКС: После then-ветки проверяем наличие else
            _fileWork.WriteFile($"IFEL: After then-branch, currentLexValue = '{currentLexValue}'");

            // Если после then-ветки есть ;, пропускаем его
            if (currentLexValue == ";")
            {
                _fileWork.WriteFile("IFEL: Skipping ';' after then-branch");
                GetCurrentLexem();
            }

            // ФИКС: Теперь проверяем else сразу после then-ветки
            if (currentLexValue == "else")
            {
                _fileWork.WriteFile(temp_if + "IFEL -> else ");
                GetCurrentLexem();

                TempPath = temp_if + "IFEL -> OPER ";
                code = OPER();
                if (code != 0) return code;

                OutputPoliz[m2] = makePolizeLabel(free);
            }
            else
            {
                // Если else нет, просто устанавливаем метку
                OutputPoliz[m2] = makePolizeLabel(free);
            }

            return 0;
        }

        private int FIXLO()
        {
            int m5, m6;
            int code;

            _fileWork.WriteFile(TempPath + "FIXLO -> for ");
            temp_for = TempPath;
            GetCurrentLexem();

            // ФИКС: В цикле for первое присваивание не должно заканчиваться на ;
            // Сохраняем текущее состояние для обработки присваивания без ;
            string savedTempPath = TempPath;
            TempPath = temp_for + "FIXLO -> LET ";

            code = LET();
            if (code != 0) return code;

            // ФИКС: После LET в for может быть 'to', а не ';'
            _fileWork.WriteFile($"FIXLO: After first LET, currentLexValue = '{currentLexValue}'");

            // Восстанавливаем TempPath
            TempPath = temp_for;

            if (currentLexValue != "to")
            {
                _fileWork.WriteFile($"FIXLO ERROR: Expected 'to', but got: '{currentLexValue}'");
                return 14;
            }
            _fileWork.WriteFile(TempPath + "FIXLO -> to ");
            GetCurrentLexem();

            m5 = free++;
            code = VIR();
            if (code != 0) return code;

            if (!BoolEquale())
            {
                return 105;
            }

            m6 = free++;
            putPolizeLex(putOperationLex("!F"));

            if (currentLexValue != "do")
            {
                return 15;
            }
            _fileWork.WriteFile(TempPath + "FIXLO -> do");
            GetCurrentLexem();

            code = OPER();
            if (code != 0) return code;

            OutputPoliz[free++] = makePolizeLabel(m5);
            putPolizeLex(putOperationLex("!"));
            OutputPoliz[m6] = makePolizeLabel(free++);
            return 0;
        }

        private int IFLO()
        {
            int m3, m4;
            int code;

            _fileWork.WriteFile(TempPath + "IFLO -> while ");
            temp_while = TempPath;
            GetCurrentLexem();

            m3 = free++;
            code = VIR();
            if (code != 0) return code;

            if (!BoolEquale())
            {
                return 106;
            }

            m4 = free++;
            TempPath = temp_while;
            _fileWork.WriteFile(TempPath + "IFLO -> do");
            putPolizeLex(putOperationLex("!F"));

            if (currentLexValue != "do")
            {
                return 16;
            }

            // ФИКС: Добавляем отладочную информацию перед вызовом OPER
            _fileWork.WriteFile($"IFLO: Before OPER call, currentLexValue = '{currentLexValue}'");
            GetCurrentLexem(); // Переходим после 'do'
            _fileWork.WriteFile($"IFLO: After 'do', currentLexValue = '{currentLexValue}'");

            code = OPER();
            if (code != 0) return code;

            OutputPoliz[free++] = makePolizeLabel(m3);
            putPolizeLex(putOperationLex("!"));
            OutputPoliz[m4] = makePolizeLabel(free);
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

        private int VIR()
        {
            int code;
            code = OPRD();
            TempPath += "VIR -> OPRD ";
            if (code != 0) return code;

            code = VIR1();
            if (code != 0) return code;

            // ВАЖНО: проверяем что выражение вернуло тип в стек
            if (stackCheckContVir.Count == 0)
            {
                _fileWork.WriteFile("VIR ERROR: Expression didn't return type to stack");
                return 25; // Добавьте эту ошибку в ErrorHandler
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

                // ФИКС: Сохраняем операцию в стек
                stackCheckContVir.Push(currentLexValue);
                _fileWork.WriteFile($"VIR1: Operation '{currentLexValue}' pushed to stack");

                GetCurrentLexem();

                // ФИКС: Добавляем отладочную информацию о состоянии стека
                _fileWork.WriteFile($"VIR1: Stack before VIR = [{string.Join(", ", stackCheckContVir)}]");

                code = VIR();
                if (code != 0) return code;

                // ФИКС: Добавляем отладочную информацию перед проверкой операции
                _fileWork.WriteFile($"VIR1: Stack before CheckOperation = [{string.Join(", ", stackCheckContVir)}]");

                if (!CheckOperation())
                {
                    _fileWork.WriteFile($"VIR1 ERROR: CheckOperation failed for '{currentLexValue}'");
                    return 110;
                }
                return 0;
            }
            return 0;
        }

        private int OPRD()
        {
            int code;
            TempPath += "OPRD -> SLAG ";
            code = SLAG();
            if (code != 0) return code;

            if (currentLexValue == "plus" || currentLexValue == "min" || currentLexValue == "or")
            {
                _fileWork.WriteFile(TempPath + "OPRD -> " + currentLexValue + " ");
                stackCheckContVir.Push(currentLexValue);
                GetCurrentLexem();
                code = OPRD();
                if (code != 0) return code;
                if (!CheckOperation()) return 111;
                return 0;
            }
            return 0;
        }

        private int SLAG()
        {
            int code;
            TempPath += "SLAG -> MNOJ ";
            code = MNOJ();
            if (code != 0) return code;

            GetCurrentLexem();
            if (currentLexValue == "mult" || currentLexValue == "div" || currentLexValue == "and")
            {
                _fileWork.WriteFile(TempPath + "SLAG -> " + currentLexValue + " ");
                stackCheckContVir.Push(currentLexValue);
                GetCurrentLexem();
                code = SLAG();
                if (code != 0) return code;
                if (!CheckOperation()) return 112;
                return 0;
            }
            return 0;
        }

        private int MNOJ()
        {
            PolizStruct temp;
            int code;

            if (IsID())
            {
                _fileWork.WriteFile(TempPath + "MNOJ -> ID ");
                code = ID();
                if (code != 0) return code;

                if (!CheckID())
                {
                    return 113;
                }

                // ФИКС: Добавляем тип идентификатора в стек
                string idType = TID_temp[currentLex.numInTable].type;
                stackCheckContVir.Push(idType);
                _fileWork.WriteFile($"MNOJ: ID '{currentLexValue}' type = {idType} pushed to stack");

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;
                putPolizeLex(temp);
                return 0;
            }
            else if (IsNumConst())
            {
                code = NUM();
                _fileWork.WriteFile(TempPath + "MNOJ -> NUM ");
                if (code != 0) return code;

                _fileWork.WriteFile("-> " + currentLexValue + " ");
                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;
                putPolizeLex(temp);
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
                return 0;
            }
            else if (currentLexValue == "~")
            {
                _fileWork.WriteFile(TempPath + "MNOJ -> UNARM -> ~ ");
                code = UNARM();
                if (code != 0) return code;

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;
                putPolizeLex(putOperationLex("~"));
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
                return 0;
            }
            else
            {
                return 22;
            }
        }

        private int UNARM()
        {
            int code;
            GetCurrentLexem();
            code = MNOJ();
            if (code != 0) return code;

            if (!CheckUnarOperation())
            {
                return 114;
            }
            return 0;
        }

        private int NUM()
        {
            if (CheckNum() == 0)
            {
                stackCheckContVir.Push("!");
                return 0;
            }
            else if (CheckNum() == 1)
            {
                stackCheckContVir.Push("%");
                return 0;
            }
            else
            {
                return 23;
            }
        }
    }
}