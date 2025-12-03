using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static compiler_prog.CompilerData;

namespace compiler_prog
{
    public class LexicalAnalyzer
    {
        private readonly List<string> service = new List<string>();
        private readonly List<string> separators = new List<string>();

        private List<string> indentificators = new List<string>();
        private List<string> constants = new List<string>();

        private CompilerData parentObj;

        private char currentSymbol;
        private char previousSymbol;

        private int currentElemPosition = 0;

        private List<char> bufferElem = new List<char>();

        private string pipeString;
        private string bufferString;

        private string lexStatus = "Лексический анализатор не запускался";

        public List<LexemStruct> TID = new List<LexemStruct>();

        public LexicalAnalyzer(ref CompilerData obj)
        {
            parentObj = obj;
            this.service = parentObj.Service;
            this.separators = parentObj.Separators;
        }

        // ДОБАВЛЕННЫЕ МЕТОДЫ ДЛЯ ПРОВЕРКИ ОПЕРАЦИЙ
        private bool IsRelationOperation(string lexem)
        {
            return lexem == "NE" || lexem == "EQ" || lexem == "LT" ||
                   lexem == "LE" || lexem == "GT" || lexem == "GE";
        }

        private bool IsAdditionOperation(string lexem)
        {
            return lexem == "plus" || lexem == "min" || lexem == "or";
        }

        private bool IsMultiplicationOperation(string lexem)
        {
            return lexem == "mult" || lexem == "div" || lexem == "and";
        }

        private bool IsUnaryOperation(string lexem)
        {
            return lexem == "~" || lexem == "not";
        }

        // ОСТАЛЬНЫЕ СУЩЕСТВУЮЩИЕ МЕТОДЫ...

        private void AddLexem(int tableNum, int numInTable, bool isDeclared, string isValue)
        {
            LexemStruct tempLexem;
            tempLexem.numTable = tableNum;
            tempLexem.numInTable = numInTable;
            tempLexem.isDeclared = isDeclared;
            tempLexem.value = isValue;
            tempLexem.type = String.Empty;
            parentObj.LexOut.Add(tempLexem);
        }

        private void AddLexemTID(int tableNum, int numInTable, bool isDeclared, string isValue)
        {
            LexemStruct tempLexem;
            tempLexem.numTable = tableNum;
            tempLexem.numInTable = numInTable;
            tempLexem.isDeclared = isDeclared;
            tempLexem.value = isValue;
            tempLexem.type = String.Empty;
            parentObj.TID.Add(tempLexem);
        }

        private int AddToTable(List<string> table)
        {
            int newElemIndex = table.Count();
            table.Add(bufferString);
            return newElemIndex;
        }

        private void GetSymbol()
        {
            currentElemPosition++;
            currentSymbol = pipeString[currentElemPosition];
        }

        private void SetPipeString(string newPipeString)
        {
            pipeString = newPipeString + '⟂';
            currentSymbol = pipeString[0];
            currentElemPosition = 0;
        }

        private void AddSymbol()
        {
            bufferElem.Add(currentSymbol);
        }

        private void ClearBuffer()
        {
            bufferElem.Clear();
            bufferString = String.Empty;
        }

        private void BufferToString()
        {
            bufferString = string.Join("", bufferElem);
        }

        private bool IsType()
        {
            return (currentSymbol == '$' || currentSymbol == '!' || currentSymbol == '%');
        }

        private int LookTable(string unknownWord, List<string> table)
        {
            int counter = 0;
            foreach (string s in table)
            {
                if (unknownWord == s)
                {
                    return counter;
                }
                counter++;
            }
            return -1;
        }

        private bool IsSeparator()
        {
            string symbol = String.Join("", currentSymbol);
            foreach (string symbString in parentObj.Separators)
            {
                if (symbString == symbol)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsCommentEnd()
        {
            if (currentSymbol == '#')
            {
                GetSymbol();
                if (currentSymbol == '\\')
                {
                    GetSymbol();
                    return true;
                }
            }
            else
            {
                GetSymbol();
            }
            return false;
        }

        private bool IsNumericAlphabet()
        {
            if (char.IsDigit(currentSymbol) || currentSymbol == 'A' || currentSymbol == 'a' || currentSymbol == 'B' || currentSymbol == 'b' || currentSymbol == 'C' || currentSymbol == 'c'
                || currentSymbol == 'D' || currentSymbol == 'd' || currentSymbol == 'E' || currentSymbol == 'e' || currentSymbol == 'F' || currentSymbol == 'f'
                || currentSymbol == 'O' || currentSymbol == 'o' || currentSymbol == 'H' || currentSymbol == 'h')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ViewPreviousSymbol()
        {
            previousSymbol = pipeString[currentElemPosition - 1];
        }

        private string ConcatChar(char symbol1, char symbol2)
        {
            return String.Join("", symbol1, symbol2);
        }

        private void AddResultToData()
        {
            parentObj.Constants = constants;
            parentObj.Indentificators = indentificators;
        }

        private void ClearAll()
        {
            parentObj.LexOut.Clear();
            parentObj.TID.Clear();
            indentificators.Clear();
            constants.Clear();
            currentSymbol = ' ';
            previousSymbol = ' ';
            pipeString = String.Empty;
            bufferString = String.Empty;
            bufferElem.Clear();
        }

        private void SetLexStatus()
        {
            parentObj.LexicalStatus = lexStatus;
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

        public bool LexicalAnalyzerScan(string inputProgrammText)
        {
            ClearAll();
            SetPipeString(inputProgrammText);
            string currentState = "H";
            int lookPosition;

            while (currentState != "OUT" && currentState != "ERR")
            {
                switch (currentState)
                {
                    case "H":
                        ClearBuffer();
                        if (currentSymbol == '⟂')
                        {
                            lexStatus = "Лексический анализ успешно завершен.";
                            currentState = "OUT";
                        }
                        else if (char.IsWhiteSpace(currentSymbol))
                        {
                            GetSymbol();
                        }
                        else if (IsLetterAlphabet(currentSymbol))
                        {
                            AddSymbol();
                            GetSymbol();
                            currentState = "I";
                        }
                        else if (char.IsDigit(currentSymbol))
                        {
                            AddSymbol();
                            GetSymbol();
                            currentState = "Z";
                        }
                        else if (IsType())
                        {
                            AddSymbol();
                            currentState = "T";
                        }
                        else if (currentSymbol == '.')
                        {
                            currentState = "RD";
                            AddSymbol();
                            GetSymbol();
                        }
                        else if (IsSeparator())
                        {
                            AddSymbol();
                            GetSymbol();
                            if (IsSeparator())
                            {
                                currentState = "SD";
                            }
                            else
                            {
                                currentState = "SO";
                            }
                        }
                        else
                        {
                            lexStatus = "Лексическая ошибка: неизвестный символ алфавита";
                            currentState = "ERR";
                        }
                        break;
                    case "T":
                        BufferToString();
                        lookPosition = LookTable(bufferString, service);
                        if (lookPosition != -1)
                        {
                            AddLexem(1, lookPosition, false, bufferString);
                            GetSymbol();
                            currentState = "H";
                        }
                        break;

                    case "I":
                        if (IsLetterAlphabet(currentSymbol) || char.IsDigit(currentSymbol))
                        {
                            AddSymbol();
                            GetSymbol();
                            currentState = "I";
                        }
                        else
                        {
                            BufferToString();

                            // ИСПОЛЬЗОВАНИЕ ДОБАВЛЕННЫХ МЕТОДОВ ДЛЯ ПРОВЕРКИ ОПЕРАЦИЙ
                            if (IsRelationOperation(bufferString) ||
                                IsAdditionOperation(bufferString) ||
                                IsMultiplicationOperation(bufferString) ||
                                IsUnaryOperation(bufferString))
                            {
                                // Это служебное слово - операция
                                lookPosition = LookTable(bufferString, service);
                                if (lookPosition != -1)
                                {
                                    AddLexem(1, lookPosition, false, bufferString);
                                }
                            }
                            else
                            {
                                lookPosition = LookTable(bufferString, service);
                                if (lookPosition != -1)
                                {
                                    AddLexem(1, lookPosition, false, bufferString);
                                }
                                else
                                {
                                    lookPosition = LookTable(bufferString, indentificators);
                                    if (lookPosition == -1)
                                    {
                                        lookPosition = AddToTable(indentificators);
                                        AddLexemTID(4, lookPosition, false, bufferString);
                                    }
                                    AddLexem(4, lookPosition, false, bufferString);
                                }
                            }
                            currentState = "H";
                        }
                        break;

                    case "SD":
                        ViewPreviousSymbol();
                        if (LookTable(ConcatChar(previousSymbol, currentSymbol), separators) != -1)
                        {
                            AddSymbol();
                            GetSymbol();
                        }
                        currentState = "SO";
                        break;

                    case "SO":
                        BufferToString();
                        lookPosition = LookTable(bufferString, separators);
                        if (lookPosition != -1)
                        {
                            if (bufferString == "\\")
                            {
                                ClearBuffer();
                                GetSymbol();
                                currentState = "C";
                            }
                            else
                            {
                                AddLexem(2, lookPosition, false, bufferString);
                                currentState = "H";
                            }
                        }
                        else
                        {
                            lexStatus = "Лексическая ошибка: неизвестный разделитель.";
                            currentState = "ERR";
                        }
                        break;

                    case "Z":
                        if (IsNumericAlphabet())
                        {
                            AddSymbol();
                            GetSymbol();
                            currentState = "Z";
                        }
                        else if (currentSymbol == '.')
                        {
                            AddSymbol();
                            GetSymbol();
                            currentState = "RD";
                        }
                        else if (currentSymbol == '+' || currentSymbol == '-')
                        {
                            ViewPreviousSymbol();
                            if (previousSymbol == 'E' || previousSymbol == 'e')
                            {
                                AddSymbol();
                                GetSymbol();
                                currentState = "RP";
                            }
                            else
                            {
                                currentState = "DN";
                            }
                        }
                        else if (IsLetterAlphabet(currentSymbol))
                        {
                            lexStatus = "Лексическая ошибка: недопустимый алфавит числовой константы.";
                            currentState = "ERR";
                        }
                        else
                        {
                            currentState = "DN";
                        }
                        break;

                    case "RD":
                        if (char.IsDigit(currentSymbol))
                        {
                            AddSymbol();
                            GetSymbol();
                            currentState = "RD";
                        }
                        else if (currentSymbol == 'E' || currentSymbol == 'e')
                        {
                            AddSymbol();
                            GetSymbol();
                            if (currentSymbol == '+' || currentSymbol == '-' || char.IsDigit(currentSymbol))
                            {
                                AddSymbol();
                                GetSymbol();
                                currentState = "RP";
                            }
                            else
                            {
                                currentState = "DN";
                            }
                        }
                        else
                        {
                            currentState = "DN";
                        }
                        break;

                    case "RP":
                        if (char.IsDigit(currentSymbol))
                        {
                            AddSymbol();
                            GetSymbol();
                        }
                        else
                        {
                            currentState = "DN";
                        }
                        break;

                    case "DN":
                        BufferToString();
                        lookPosition = AddToTable(constants);
                        AddLexem(3, lookPosition, false, bufferString);
                        currentState = "H";
                        break;

                    case "C":
                        if (IsCommentEnd())
                        {
                            currentState = "H";
                        }
                        else
                        {
                            if (currentSymbol == '⟂')
                            {
                                lexStatus = "Лексическая ошибка: не найден закрывающий символ для комментария.";
                                currentState = "ERR";
                            }
                        }
                        break;

                    case "OUT":
                        break;

                    case "ERR":
                        break;
                }
            }
            SetLexStatus();
            AddResultToData();
            if (currentState == "OUT")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}