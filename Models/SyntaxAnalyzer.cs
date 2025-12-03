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

            string t2 = stackCheckContVir.Pop(); // тип выражения (правой части)
            string t1 = stackCheckContVir.Pop(); // тип идентификатора (левой части)

            _fileWork.WriteFile($"LetEquale: t1(ID)={t1}, t2(expression)={t2}, equal={t1 == t2}");

            return t1 == t2; // Типы должны совпадать
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
            OutputPoliz[free] = lex;

            // Логируем что добавляем
            string logMsg = $"POLIZ[{free}]: ";
            if (lex.classValue == 0) // метка
                logMsg += $"Label L{lex.value}";
            else if (lex.classValue == 2) // операция
                logMsg += $"Operation '{lex.type}'";
            else if (lex.classValue == 3) // константа
                logMsg += $"Constant '{lex.type}'";
            else if (lex.classValue == 4) // идентификатор
                logMsg += $"Variable '{lex.type}'";
            else if (lex.classValue == 5) // особый случай
                logMsg += $"Special '{lex.type}'";

            _fileWork.WriteFile(logMsg);
            free++;
        }

        PolizStruct putOperationLex(string oper)
        {
            PolizStruct temp = new PolizStruct();

            if (oper == "!F")
            {
                temp.type = "!F";
                temp.classValue = 2;
                temp.value = 20;
            }
            else if (oper == "!")
            {
                temp.type = "!";
                temp.classValue = 2;
                temp.value = 21;
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
                foreach (string s in parentObj.Separators)
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

        PolizStruct makePolizeLabel(int k)
        {
            PolizStruct temp;
            temp.classValue = 0; // 0 означает метку
            temp.value = k;
            temp.type = "L" + k.ToString();
            _fileWork.WriteFile($"Created label L{k} for POLIZ position {k}");
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

            // 1. СОХРАНИТЕ идентификатор
            string currentID = currentLexValue;
            int currentIDIndex = currentLex.numInTable;
            _fileWork.WriteFile($"LET: Saving ID '{currentID}' with index {currentIDIndex}");

            // 2. Обрабатываем ID
            _fileWork.WriteFile(TempPath + "LET -> ID ");
            code = ID();
            if (code != 0) return code;

            // 3. Проверяем объявление
            if (currentIDIndex >= TID_temp.Count || !TID_temp[currentIDIndex].isDeclared)
            {
                _fileWork.WriteFile($"LET ERROR: ID '{currentID}' not declared or out of range");
                return 102;
            }

            string idType = TID_temp[currentIDIndex].type;

            // 4. ОЧИЩАЕМ стек и ДОБАВЛЯЕМ тип ID
            stackCheckContVir.Clear();
            stackCheckContVir.Push(idType);
            _fileWork.WriteFile($"LET: Pushed ID type '{idType}' to stack");

            // 5. СОЗДАЕМ POLIZ запись для идентификатора (но НЕ добавляем сейчас!)
            temp.classValue = currentLex.numTable;
            temp.value = currentIDIndex;
            temp.type = currentID;
            // НЕ ВЫЗЫВАЕМ putPolizeLex(temp) здесь!

            GetCurrentLexem();

            // 6. Проверяем 'ass'
            if (currentLexValue != "ass")
            {
                _fileWork.WriteFile($"LET ERROR: Expected 'ass', but got: '{currentLexValue}'");
                return 10;
            }
            _fileWork.WriteFile("LET -> ass");
            GetCurrentLexem();

            // 7. Обрабатываем выражение (оно добавится в ПОЛИЗ первым)
            code = VIR();
            if (code != 0) return code;

            // 8. Проверяем типы - теперь в стеке должно быть: тип_выражения, тип_ID
            if (!LetEquale())
            {
                _fileWork.WriteFile($"LET ERROR: Type mismatch in assignment");
                return 103;
            }

            // 9. ТЕПЕРЬ добавляем идентификатор в ПОЛИЗ (после выражения)
            putPolizeLex(temp);
            _fileWork.WriteFile($"LET: Added ID '{currentID}' to POLIZ after expression");

            // 10. Добавляем операцию присваивания
            putPolizeLex(putOperationLex("="));
            _fileWork.WriteFile($"LET: Added assignment operation");

            // 11. Проверяем разделитель
            if (currentLexValue == ";")
            {
                GetCurrentLexem();
                _fileWork.WriteFile("LET: Skipping ';'");
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
            GetCurrentLexem(); // пропускаем "if"

            TempPath += "IFEL -> VIR ";
            code = VIR();
            if (code != 0) return code;

            if (!BoolEquale())
            {
                return 104;
            }

            m1 = free++; // сохраняем адрес для !F (условный переход)
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

            m2 = free++; // сохраняем адрес для ! (безусловный переход)
            putPolizeLex(putOperationLex("!"));

            // ОЧЕНЬ ВАЖНО: Сначала устанавливаем метку для !F
            // !F должен перейти НА else-ветку (или после then, если else нет)
            // В данный момент free указывает на следующую команду ПОСЛЕ !
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

                // Устанавливаем метку для ! (безусловного перехода)
                // ! должен перейти ПОСЛЕ else-ветки
                OutputPoliz[m2] = makePolizeLabel(free);
            }
            else
            {
                // Если else нет, ! должен перейти на текущую позицию (после then)
                OutputPoliz[m2] = makePolizeLabel(free);
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

            // Сохраняем позицию начала проверки условия
            int loopStartLabel = free; // Это позиция, куда нужно вернуться
            _fileWork.WriteFile($"FIXLO: Loop start label at position {loopStartLabel}");

            // Сначала добавляем переменную i для сравнения
            int iIndex = -1;
            for (int i = 0; i < TID_temp.Count; i++)
            {
                if (TID_temp[i].value == "i")
                {
                    iIndex = i;
                    break;
                }
            }

            if (iIndex == -1) return 115;

            PolizStruct tempVarI;
            tempVarI.classValue = 4; // TID
            tempVarI.value = iIndex;
            tempVarI.type = "i";
            putPolizeLex(tempVarI);
            _fileWork.WriteFile($"FIXLO: Added variable i at position {free - 1}");

            // Обрабатываем верхнюю границу
            code = VIR();
            if (code != 0) return code;

            if (stackCheckContVir.Count == 0) return 105;
            string exprType = stackCheckContVir.Pop();
            if (exprType != "%" && exprType != "!") return 105;

            putPolizeLex(putOperationLex("LE"));
            _fileWork.WriteFile($"FIXLO: Added LE operation at position {free - 1}");

            // Условный переход - ВАЖНО: нужно добавить операцию !F
            falseJumpPos = free;

            // Создаем операцию !F
            PolizStruct tempJump = new PolizStruct();
            tempJump.type = "!F";
            tempJump.classValue = 2;
            tempJump.value = 0; // Временное значение

            // Добавляем в ПОЛИЗ
            putPolizeLex(tempJump);
            _fileWork.WriteFile($"FIXLO: Added !F operation at position {falseJumpPos}");

            if (currentLexValue != "do")
            {
                _fileWork.WriteFile($"FIXLO ERROR: Expected 'do', but got: '{currentLexValue}'");
                return 15;
            }
            _fileWork.WriteFile(TempPath + "FIXLO -> do");
            GetCurrentLexem();

            code = OPER();
            if (code != 0) return code;

            // Инкремент i := i + 1
            putPolizeLex(tempVarI);
            _fileWork.WriteFile($"FIXLO: Added variable i for increment at position {free - 1}");

            PolizStruct tempOne;
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

            // Безусловный переход к началу цикла
            int backJumpPos = free;
            PolizStruct tempBackJump = new PolizStruct();
            tempBackJump.type = "!";
            tempBackJump.classValue = 2;
            tempBackJump.value = loopStartLabel; // Прыжок к началу проверки

            putPolizeLex(tempBackJump);
            _fileWork.WriteFile($"FIXLO: Added ! operation at position {backJumpPos} to jump to L{loopStartLabel}");

            // Устанавливаем цель перехода для !F (выход из цикла)
            OutputPoliz[falseJumpPos].value = free;
            _fileWork.WriteFile($"FIXLO: Set !F at position {falseJumpPos} to jump to L{free}");

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

            // ФИКС: Используем BoolEquale() - для while выражение ДОЛЖНО быть логическим
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

            GetCurrentLexem();
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

                // Сохраняем операцию
                string operation = currentLexValue;

                GetCurrentLexem();

                // Обрабатываем правый операнд
                code = VIR();
                if (code != 0) return code;

                // Проверяем операцию сравнения
                if (stackCheckContVir.Count < 2)
                {
                    _fileWork.WriteFile($"VIR1 ERROR: Not enough operands for '{operation}'");
                    return 110;
                }

                string t2 = stackCheckContVir.Pop();  // правый операнд
                string t1 = stackCheckContVir.Pop();  // левый операнд
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
                    _fileWork.WriteFile($"VIR1 ERROR: CheckOperation failed for '{operation}'");
                    return 110;
                }
            }
            return 0;
        }

        private bool CheckOperation()
        {
            if (stackCheckContVir.Count < 3)
            {
                _fileWork.WriteFile($"CheckOperation ERROR: Stack has {stackCheckContVir.Count} elements, need 3");
                return false;
            }

            // ВАЖНО: Для правильной работы нужен порядок: t2, t1, op
            // Потому что операции добавляются в стек ПОСЛЕ операндов

            string t2 = stackCheckContVir.Pop();  // правый операнд
            string t1 = stackCheckContVir.Pop();  // левый операнд
            string op = stackCheckContVir.Pop();  // операция

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
        private int OPRD()
        {
            int code;
            _fileWork.WriteFile($"OPRD START: currentLexValue = '{currentLexValue}'");

            TempPath += "OPRD -> SLAG ";
            code = SLAG();
            if (code != 0) return code;

            // Проверяем операции сложения/вычитания
            while (currentLexValue == "plus" || currentLexValue == "min" || currentLexValue == "or")
            {
                _fileWork.WriteFile(TempPath + "OPRD -> " + currentLexValue + " ");
                string operation = currentLexValue; // сохраняем операцию

                GetCurrentLexem(); // переходим к следующему слагаемому

                // Обрабатываем правый операнд
                code = SLAG();
                if (code != 0) return code;

                // **ВАЖНО: Операция добавляется ПОСЛЕ обработки правого операнда**
                // Но тип операции нужно проверить СЕЙЧАС
                if (stackCheckContVir.Count < 2)
                {
                    _fileWork.WriteFile($"OPRD ERROR: Not enough operands for '{operation}'");
                    return 111;
                }

                string t2 = stackCheckContVir.Pop();  // правый операнд
                string t1 = stackCheckContVir.Pop();  // левый операнд
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

            // Проверяем операции умножения/деления
            while (currentLexValue == "mult" || currentLexValue == "div" || currentLexValue == "and")
            {
                _fileWork.WriteFile(TempPath + "SLAG -> " + currentLexValue + " ");
                string operation = currentLexValue; // сохраняем операцию

                GetCurrentLexem(); // переходим к следующему множителю

                // Обрабатываем правый операнд
                code = MNOJ();
                if (code != 0) return code;

                // **ВАЖНО: Операция добавляется ПОСЛЕ обработки правого операнда**
                // Но тип операции нужно проверить СЕЙЧАС
                if (stackCheckContVir.Count < 2)
                {
                    _fileWork.WriteFile($"SLAG ERROR: Not enough operands for '{operation}'");
                    return 112;
                }

                string t2 = stackCheckContVir.Pop();  // правый операнд
                string t1 = stackCheckContVir.Pop();  // левый операнд
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

                // ФИКС: Добавляем тип идентификатора в стек
                string idType = TID_temp[currentLex.numInTable].type;
                stackCheckContVir.Push(idType);
                _fileWork.WriteFile($"MNOJ: ID '{currentLexValue}' type = {idType} pushed to stack");

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;
                putPolizeLex(temp);

                // ВАЖНО: Вызываем GetCurrentLexem() здесь, после обработки ID
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
                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;
                putPolizeLex(temp);

                // ВАЖНО: Вызываем GetCurrentLexem() здесь, после обработки константы
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

                // ВАЖНО: Вызываем GetCurrentLexem() здесь
                GetCurrentLexem();
                _fileWork.WriteFile($"MNOJ END (bool): currentLexValue = '{currentLexValue}'");
                return 0;
            }
            else if (currentLexValue == "~")
            {
                _fileWork.WriteFile(TempPath + "MNOJ -> UNARM -> ~ ");

                // Сохраняем операцию "~" в стек для унарной операции
                stackCheckContVir.Push("~");

                GetCurrentLexem(); // Пропускаем "~"

                code = UNARM();
                if (code != 0) return code;

                // Унарная операция уже обработана в UNARM()
                _fileWork.WriteFile($"MNOJ END (unary): currentLexValue = '{currentLexValue}'");
                return 0;
            }
            else if (currentLexValue == "(")
            {
                _fileWork.WriteFile(TempPath + "MNOJ -> ( ");
                temp_str = TempPath;
                GetCurrentLexem(); // Пропускаем "("

                TempPath += "MNOJ -> VIR ";
                code = VIR();
                if (code != 0) return code;

                _fileWork.WriteFile(temp_str + "MNOJ -> ) ");
                if (currentLexValue != ")")
                {
                    return 21;
                }

                GetCurrentLexem(); // Пропускаем ")"
                _fileWork.WriteFile($"MNOJ END (parentheses): currentLexValue = '{currentLexValue}'");
                return 0;
            }
            else
            {
                _fileWork.WriteFile($"MNOJ ERROR: Unexpected token '{currentLexValue}'");
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