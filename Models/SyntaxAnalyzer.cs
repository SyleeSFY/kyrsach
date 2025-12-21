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

        // Для построения правил
        private List<string> rulePath = new List<string>();
        private string lastWrittenRule = "";

        private Stack<int> stackCheckDeclare = new Stack<int>();
        public Stack<string> stackCheckContVir = new Stack<string>();

        public PolizStruct[] OutputPoliz = new PolizStruct[1000];
        public int free = 0;

        public SyntaxAnalyzer(ref CompilerData obj)
        {
            parentObj = obj;
            counterSLexem = 0;
            _fileWork = new FileWork();
        }

        private void BeginRule(string ruleName)
        {
            rulePath.Clear();
            rulePath.Add("PROG");
            if (!string.IsNullOrEmpty(ruleName) && ruleName != "PROG")
            {
                rulePath.Add(ruleName);
            }
        }

        private void AddToRulePath(string element)
        {
            rulePath.Add(element);
        }

        private void WriteRuleIfComplete()
        {
            if (rulePath.Count == 0) return;

            var rule = string.Join(" -> ", rulePath);

            // Пишем правило только если оно изменилось
            if (rule != lastWrittenRule)
            {
                _fileWork.WriteFile(rule);
                lastWrittenRule = rule;
            }
        }

        private void CompleteRule()
        {
            WriteRuleIfComplete();
            // Начинаем новое правило с PROG, но не сбрасываем полностью
            // Просто удаляем все после PROG
            while (rulePath.Count > 1)
            {
                rulePath.RemoveAt(rulePath.Count - 1);
            }
        }

        private int StartProgram()
        {
            int code;
            GetCurrentLexem();

            BeginRule("");
            WriteRuleIfComplete();

            if (currentLexValue != "program")
                return 1;

            AddToRulePath("program");
            WriteRuleIfComplete();
            GetCurrentLexem();

            code = DESC();
            if (code != 0) return code;

            if (currentLexValue != "begin")
            {
                return 2;
            }

            CompleteRule(); // Завершаем предыдущее правило

            BeginRule("");
            AddToRulePath("begin");
            WriteRuleIfComplete();
            GetCurrentLexem();

            while (currentLexValue != "end" && currentLexValue != "⟂")
            {
                code = OPER();
                if (code != 0) return code;

                if (currentLexValue == ";")
                {
                    AddToRulePath(";");
                    WriteRuleIfComplete();
                    GetCurrentLexem();

                    if (currentLexValue == "else")
                    {
                        continue;
                    }

                    if (currentLexValue == "⟂")
                    {
                        return 3;
                    }

                    if (currentLexValue == "end")
                    {
                        break;
                    }
                }
                else if (currentLexValue == "else")
                {
                    continue;
                }
                else if (IsID() ||
                        currentLexValue == "if" ||
                        currentLexValue == "for" ||
                        currentLexValue == "while" ||
                        currentLexValue == "read" ||
                        currentLexValue == "write" ||
                        currentLexValue == "@")
                {
                    continue;
                }
                else if (currentLexValue == "end")
                {
                    break;
                }
                else
                {
                    return 26;
                }
            }

            if (currentLexValue != "end")
            {
                return 3;
            }

            CompleteRule(); // Завершаем предыдущее правило

            BeginRule("");
            AddToRulePath("end");
            WriteRuleIfComplete();
            GetCurrentLexem();

            if (currentLexValue != "." && currentLexValue != "⟂")
            {
                return 24;
            }

            if (currentLexValue == ".")
            {
                AddToRulePath(".");
                WriteRuleIfComplete();
                GetCurrentLexem();
            }

            if (currentLexValue != "⟂")
            {
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
            }
            else
            {
                currentLex.value = "⟂";
                currentLexValue = "⟂";
            }
        }

        public int SyntaxStart()
        {
            int answer;
            counterSLexem = 0;
            TID_temp = parentObj.TID.ToList();
            Array.Clear(OutputPoliz, 0, OutputPoliz.Length);
            free = 0;
            labelCounter = 1;
            rulePath.Clear();
            lastWrittenRule = "";
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
            if (IsBin() || IsOct() || IsHex())
            {
                return 1;
            }
            else if (IsDec())
            {
                return 1;
            }
            else if (IsReal())
            {
                return 0;
            }
            else
            {
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
                return false;
            }

            string t2 = stackCheckContVir.Pop();
            string t1 = stackCheckContVir.Pop();

            return t1 == t2;
        }

        private bool BoolEquale()
        {
            if (stackCheckContVir.Count == 0)
            {
                return false;
            }

            string t1 = stackCheckContVir.Pop();
            bool result = t1 == "$";

            return result;
        }

        void putPolizeLex(PolizStruct lex)
        {
            OutputPoliz[free] = lex;
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

            return temp;
        }

        PolizStruct makePolizeLabelWithNumber(int labelNumber)
        {
            PolizStruct temp = new PolizStruct();
            temp.classValue = 0;
            temp.value = labelNumber;
            temp.type = $"m{labelNumber}:";

            return temp;
        }

        private int DESC()
        {
            int code;
            int stackNum;

            while (IsID())
            {
                CompleteRule(); // Завершаем предыдущее правило
                BeginRule("DESC");
                AddToRulePath("I1");
                code = I1();
                if (code != 0) return code;

                AddToRulePath("TYPE");
                code = TYPE();
                if (code != 0) return code;

                string variableType = currentLexValue;

                GetCurrentLexem();

                while (stackCheckDeclare.Count > 0)
                {
                    stackNum = stackCheckDeclare.Pop();
                    if (!DecID(stackNum, variableType))
                    {
                        return 101;
                    }
                }

                if (currentLexValue != ";")
                {
                    return 4;
                }

                AddToRulePath(";");
                WriteRuleIfComplete();
                GetCurrentLexem();
            }

            return 0;
        }

        private int I1()
        {
            int code;

            AddToRulePath("ID");
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

            AddToRulePath(currentLexValue);
            WriteRuleIfComplete();
            return 0;
        }

        private int I2()
        {
            if (currentLexValue == ",")
            {
                // Удаляем последний элемент (значение ID) и добавляем запятую
                if (rulePath.Count > 0 && rulePath[rulePath.Count - 1] != "ID")
                    rulePath.RemoveAt(rulePath.Count - 1);

                AddToRulePath(",");
                WriteRuleIfComplete();
                GetCurrentLexem();
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
                AddToRulePath(":");
                WriteRuleIfComplete();
                GetCurrentLexem();
            }
            else
            {
                return 4;
            }

            if (currentLexValue == "%" || currentLexValue == "!" || currentLexValue == "$")
            {
                AddToRulePath(currentLexValue);
                WriteRuleIfComplete();
                return 0;
            }
            else
            {
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
                CompleteRule();
                BeginRule("SOST");
                int result = SOST();
                return result;
            }
            else if (IsID())
            {
                CompleteRule();
                BeginRule("LET");
                int result = LET();
                return result;
            }
            else if (currentLexValue == "if")
            {
                CompleteRule();
                BeginRule("IFEL");
                int result = IFEL();
                return result;
            }
            else if (currentLexValue == "for")
            {
                CompleteRule();
                BeginRule("FIXLO");
                int result = FIXLO();
                return result;
            }
            else if (currentLexValue == "while")
            {
                CompleteRule();
                BeginRule("IFLO");
                int result = IFLO();
                return result;
            }
            else if (currentLexValue == "read")
            {
                CompleteRule();
                BeginRule("INPU");
                int result = INPU();
                return result;
            }
            else if (currentLexValue == "write")
            {
                CompleteRule();
                BeginRule("OUPU");
                int result = OUPU();
                return result;
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
                AddToRulePath("@");
                WriteRuleIfComplete();
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

            string currentID = currentLexValue;
            int currentIDIndex = currentLex.numInTable;

            AddToRulePath("ID");
            code = ID();
            if (code != 0) return code;

            if (currentIDIndex >= TID_temp.Count || !TID_temp[currentIDIndex].isDeclared)
            {
                return 102;
            }

            string idType = TID_temp[currentIDIndex].type;

            stackCheckContVir.Clear();
            stackCheckContVir.Push(idType);

            temp.classValue = currentLex.numTable;
            temp.value = currentIDIndex;
            temp.type = currentID;
            PolizStruct idPolizEntry = temp;

            GetCurrentLexem();

            if (currentLexValue != "ass")
            {
                return 10;
            }

            AddToRulePath("ass");
            WriteRuleIfComplete();
            GetCurrentLexem();

            AddToRulePath("VIR");
            code = VIR();
            if (code != 0) return code;

            if (!LetEquale())
            {
                return 103;
            }

            // Добавляем операцию присваивания в ПОЛИЗ
            putPolizeLex(idPolizEntry);
            putPolizeLex(putOperationLex("="));

            if (currentLexValue == ";")
            {
                AddToRulePath(";");
                WriteRuleIfComplete();
                GetCurrentLexem();
            }

            return 0;
        }

        private int VIR()
        {
            int code;
            AddToRulePath("OPRD");
            code = OPRD();
            if (code != 0) return code;

            code = VIR1();
            if (code != 0) return code;

            if (stackCheckContVir.Count == 0)
            {
                return 25;
            }

            return 0;
        }

        private int VIR1()
        {
            int code;
            if (currentLexValue == "NE" || currentLexValue == "EQ" || currentLexValue == "LT" ||
                currentLexValue == "LE" || currentLexValue == "GT" || currentLexValue == "GE")
            {
                AddToRulePath(currentLexValue);
                WriteRuleIfComplete();
                string operation = currentLexValue;

                if (stackCheckContVir.Count == 0)
                {
                    return 110;
                }

                GetCurrentLexem();

                code = VIR();
                if (code != 0) return code;

                if (stackCheckContVir.Count < 2)
                {
                    return 110;
                }

                string t2 = stackCheckContVir.Pop();
                string t1 = stackCheckContVir.Pop();
                string res = GetType(operation, t1, t2);

                if (res != string.Empty)
                {
                    stackCheckContVir.Push(res);
                    putPolizeLex(putOperationLex(operation));
                    return 0;
                }
                else
                {
                    return 110;
                }
            }
            return 0;
        }

        private int OPRD()
        {
            int code;
            AddToRulePath("SLAG");
            code = SLAG();
            if (code != 0) return code;

            while (currentLexValue == "plus" || currentLexValue == "min" || currentLexValue == "or")
            {
                AddToRulePath(currentLexValue);
                WriteRuleIfComplete();
                string operation = currentLexValue;

                GetCurrentLexem();

                AddToRulePath("SLAG");
                code = SLAG();
                if (code != 0) return code;

                if (stackCheckContVir.Count < 2)
                {
                    return 111;
                }

                string t2 = stackCheckContVir.Pop();
                string t1 = stackCheckContVir.Pop();
                string res = GetType(operation, t1, t2);

                if (res != string.Empty)
                {
                    stackCheckContVir.Push(res);
                    putPolizeLex(putOperationLex(operation));
                }
                else
                {
                    return 111;
                }
            }

            return 0;
        }

        private int SLAG()
        {
            int code;
            AddToRulePath("MNOJ");
            code = MNOJ();
            if (code != 0) return code;

            while (currentLexValue == "mult" || currentLexValue == "div" || currentLexValue == "and")
            {
                AddToRulePath(currentLexValue);
                WriteRuleIfComplete();
                string operation = currentLexValue;

                GetCurrentLexem();

                AddToRulePath("MNOJ");
                code = MNOJ();
                if (code != 0) return code;

                if (stackCheckContVir.Count < 2)
                {
                    return 112;
                }

                string t2 = stackCheckContVir.Pop();
                string t1 = stackCheckContVir.Pop();
                string res = GetType(operation, t1, t2);

                if (res != string.Empty)
                {
                    stackCheckContVir.Push(res);
                    putPolizeLex(putOperationLex(operation));
                }
                else
                {
                    return 112;
                }
            }

            return 0;
        }

        private int MNOJ()
        {
            PolizStruct temp;
            int code;

            if (IsID())
            {
                AddToRulePath("ID");
                code = ID();
                if (code != 0) return code;

                if (!CheckID())
                {
                    return 113;
                }

                string idType = TID_temp[currentLex.numInTable].type;
                stackCheckContVir.Push(idType);

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;

                // Добавляем идентификатор в ПОЛИЗ
                putPolizeLex(temp);

                GetCurrentLexem();
                return 0;
            }
            else if (IsNumConst())
            {
                AddToRulePath("NUM");
                // Проверяем тип числа
                int numType = CheckNum();
                string constType = (numType == 0) ? "!" : "%";
                stackCheckContVir.Push(constType);

                // Создаем запись для константы
                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;

                // Добавляем константу в ПОЛИЗ
                putPolizeLex(temp);

                // Добавляем значение константы
                AddToRulePath(currentLexValue);
                WriteRuleIfComplete();
                GetCurrentLexem();
                return 0;
            }
            else if (currentLexValue == "false" || currentLexValue == "true")
            {
                AddToRulePath(currentLexValue);
                WriteRuleIfComplete();
                stackCheckContVir.Push("$");

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;
                putPolizeLex(temp);

                GetCurrentLexem();
                return 0;
            }
            else if (currentLexValue == "(")
            {
                AddToRulePath("(");
                WriteRuleIfComplete();
                GetCurrentLexem();

                AddToRulePath("VIR");
                code = VIR();
                if (code != 0) return code;

                if (currentLexValue != ")")
                {
                    return 21;
                }
                AddToRulePath(")");
                WriteRuleIfComplete();
                GetCurrentLexem();
                return 0;
            }
            else
            {
                return 22;
            }
        }

        private int NUM()
        {
            int checkResult = CheckNum();

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

            AddToRulePath("if");
            WriteRuleIfComplete();
            GetCurrentLexem();

            AddToRulePath("VIR");
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

            AddToRulePath("then");
            WriteRuleIfComplete();
            GetCurrentLexem();

            AddToRulePath("OPER");
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

                AddToRulePath("OPER");
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

            AddToRulePath("for");
            WriteRuleIfComplete();
            GetCurrentLexem();

            AddToRulePath("LET");
            code = LET();
            if (code != 0) return code;

            if (currentLexValue != "to")
            {
                return 14;
            }

            AddToRulePath("to");
            WriteRuleIfComplete();
            GetCurrentLexem();

            int loopStartLabelNum = labelCounter;
            PolizStruct startLabel = makePolizeLabel();
            putPolizeLex(startLabel);

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

            AddToRulePath("VIR");
            code = VIR();
            if (code != 0) return code;

            if (stackCheckContVir.Count == 0) return 105;
            string exprType = stackCheckContVir.Pop();
            if (exprType != "%" && exprType != "!") return 105;

            putPolizeLex(putOperationLex("LE"));

            falseJumpPos = free;
            int exitLabelNum = labelCounter;

            PolizStruct tempJump = new PolizStruct();
            tempJump.type = "УПЛ";
            tempJump.classValue = 2;
            tempJump.value = exitLabelNum;

            putPolizeLex(tempJump);

            if (currentLexValue != "do")
            {
                return 15;
            }

            AddToRulePath("do");
            WriteRuleIfComplete();
            GetCurrentLexem();

            AddToRulePath("OPER");
            code = OPER();
            if (code != 0) return code;

            while (currentLexValue == "@")
            {
                GetCurrentLexem();

                code = OPER();
                if (code != 0) return code;

                if (currentLexValue == ";")
                {
                    GetCurrentLexem();
                }
            }

            putPolizeLex(tempVarI);

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

            putPolizeLex(putOperationLex("plus"));

            putPolizeLex(tempVarI);

            putPolizeLex(putOperationLex("="));

            PolizStruct tempBackJump = new PolizStruct();
            tempBackJump.type = "БП";
            tempBackJump.classValue = 2;
            tempBackJump.value = loopStartLabelNum;

            putPolizeLex(tempBackJump);

            PolizStruct exitLabel = makePolizeLabelWithNumber(exitLabelNum);
            putPolizeLex(exitLabel);

            return 0;
        }

        private int IFLO()
        {
            int jumpFalsePos;
            int code;

            AddToRulePath("while");
            WriteRuleIfComplete();
            GetCurrentLexem();

            int loopStartLabelNum = labelCounter;
            putPolizeLex(makePolizeLabel());

            AddToRulePath("VIR");
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
                code = OPER();
                if (code != 0) return code;

                if (currentLexValue == ";")
                {
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

            AddToRulePath("read");
            WriteRuleIfComplete();
            GetCurrentLexem();

            if (currentLexValue != "(")
            {
                return 17;
            }

            AddToRulePath("(");
            WriteRuleIfComplete();
            GetCurrentLexem();

            AddToRulePath("ID");
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
                AddToRulePath(",");
                WriteRuleIfComplete();
                GetCurrentLexem();

                if (IsID())
                {
                    AddToRulePath("ID");
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

            AddToRulePath(")");
            WriteRuleIfComplete();

            putPolizeLex(putOperationLex("R"));
            GetCurrentLexem();

            return 0;
        }

        private int OUPU()
        {
            int code;

            AddToRulePath("write");
            WriteRuleIfComplete();
            GetCurrentLexem();

            if (currentLexValue != "(")
            {
                return 19;
            }

            AddToRulePath("(");
            WriteRuleIfComplete();
            GetCurrentLexem();

            AddToRulePath("VIR");
            code = VIR();
            if (code != 0) return code;

            // Очищаем стек типов после обработки выражения
            if (stackCheckContVir.Count > 0)
            {
                stackCheckContVir.Pop();
            }

            while (currentLexValue == ",")
            {
                AddToRulePath(",");
                WriteRuleIfComplete();
                GetCurrentLexem();

                AddToRulePath("VIR");
                code = VIR();
                if (code != 0) return code;

                // Очищаем стек типов после каждого выражения
                if (stackCheckContVir.Count > 0)
                {
                    stackCheckContVir.Pop();
                }
            }

            if (currentLexValue != ")")
            {
                return 20;
            }

            AddToRulePath(")");
            WriteRuleIfComplete();

            // Добавляем операцию W (write) в ПОЛИЗ
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

            return 114;
        }

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
            }
        }
    }
}