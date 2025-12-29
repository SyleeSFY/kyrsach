using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static compiler_prog.Data;

namespace compiler_prog
{
    public class Lexical
    {
        private readonly List<string> service = new List<string>();
        private readonly List<string> separators = new List<string>();
        private readonly List<string> operators = new List<string>();

        private List<string> indentificators = new List<string>();
        private List<string> constants = new List<string>();

        private Data parentObj;

        private char currentSymbol;
        private char previousSymbol;

        private int currentElemPosition = 0;

        private List<char> bufferElem = new List<char>();

        private string pipeString;
        private string bufferString;

        private string lexStatus = "Лексический анализатор не запускался";

        public List<Lexem> TID = new List<Lexem>();

        private StringBuilder lexOutput = new StringBuilder();

        public Lexical(ref Data obj)
        {
            parentObj = obj;
            this.service = parentObj.Keywords;
            this.separators = parentObj.Delimiters;
            this.operators = new List<string>(parentObj.OPERATORS);
        }

        private bool IsOperator(string lexem)
        {
            return operators.Contains(lexem);
        }

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

        private string GetTableName(int tableNum)
        {
            switch (tableNum)
            {
                case 1: return "Keywords";
                case 2: return "Delimiters";
                case 3: return "Constants";
                case 4: return "Identifiers";
                case 5: return "Operators";
                default: return tableNum.ToString();
            }
        }

        private void AddLexem(int tableNum, int numInTable, bool isDeclared, string isValue)
        {
            Lexem tempLexem;
            tempLexem.numTable = tableNum;
            tempLexem.numInTable = numInTable;
            tempLexem.isDeclared = isDeclared;
            tempLexem.value = isValue;
            tempLexem.type = String.Empty;
            parentObj.LexOut.Add(tempLexem);

            string tableName = GetTableName(tableNum);

            string currentOutput = lexOutput.ToString();

            bool addNewLineBefore = false;

            // Проверяем, нужно ли добавлять перенос строки ПЕРЕД лексемой
            if (tableNum == 1) // Если это ключевое слово
            {
                // program, begin, end - с новой строки
                if (isValue == "program" || isValue == "begin" || isValue == "end")
                {
                    addNewLineBefore = true;
                }
            }

            // Добавляем перенос строки если нужно
            if (addNewLineBefore && lexOutput.Length > 0)
            {
                // Проверяем, не является ли последний символ уже переносом строки
                if (!currentOutput.EndsWith(Environment.NewLine))
                {
                    lexOutput.Append(Environment.NewLine);
                }
            }

            // Добавляем саму лексему
            if (lexOutput.Length > 0 && !currentOutput.EndsWith(Environment.NewLine))
            {
                lexOutput.Append(" ");
            }

            lexOutput.Append($"({tableName},{numInTable})");

            bool addNewLineAfter = false;

            if (tableNum == 1) // Если это ключевое слово
            {
                if (isValue == "program" || isValue == "begin")
                {
                    addNewLineAfter = true;
                }
                // end не имеет пустой строки после
            }

            // Добавляем перенос строки после лексемы если нужно
            if (addNewLineAfter)
            {
                lexOutput.Append(Environment.NewLine);
            }
        }

        public string GetLexOutput()
        {
            return lexOutput.ToString();
        }

        public List<string> GetOperators()
        {
            return new List<string>(operators);
        }

        private void AddLexemTID(int tableNum, int numInTable, bool isDeclared, string isValue)
        {
            Lexem tempLexem;
            tempLexem.numTable = tableNum;
            tempLexem.numInTable = numInTable;
            tempLexem.isDeclared = isDeclared;
            tempLexem.value = isValue;
            tempLexem.type = String.Empty;
            parentObj.TID.Add(tempLexem);
        }

        private int AddToTable(List<string> table, string value = null)
        {
            int newElemIndex = table.Count();
            table.Add(value ?? bufferString);
            return newElemIndex;
        }

        private void GetSymbol()
        {
            if (currentElemPosition + 1 < pipeString.Length)
            {
                currentElemPosition++;
                currentSymbol = pipeString[currentElemPosition];
            }
            else
            {
                currentSymbol = '⟂';
            }
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
            for (int i = 0; i < table.Count; i++)
            {
                if (unknownWord == table[i])
                {
                    return i;
                }
            }
            return -1;
        }

        private bool IsSeparator()
        {
            string symbol = currentSymbol.ToString();
            foreach (string symbString in separators)
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
            return char.IsDigit(currentSymbol) ||
                   currentSymbol == 'A' || currentSymbol == 'a' ||
                   currentSymbol == 'B' || currentSymbol == 'b' ||
                   currentSymbol == 'C' || currentSymbol == 'c' ||
                   currentSymbol == 'D' || currentSymbol == 'd' ||
                   currentSymbol == 'E' || currentSymbol == 'e' ||
                   currentSymbol == 'F' || currentSymbol == 'f' ||
                   currentSymbol == 'O' || currentSymbol == 'o' ||
                   currentSymbol == 'H' || currentSymbol == 'h';
        }

        private void ViewPreviousSymbol()
        {
            if (currentElemPosition > 0)
            {
                previousSymbol = pipeString[currentElemPosition - 1];
            }
        }

        private string ConcatChar(char symbol1, char symbol2)
        {
            return symbol1.ToString() + symbol2;
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
            lexOutput.Clear();
        }

        private void SetLexStatus()
        {
            parentObj.LexicalStatus = lexStatus;
        }

        private bool IsLetterAlphabet(char symbol)
        {
            // Проверяем латинские буквы
            symbol = char.ToLower(symbol);
            bool isLatin = symbol >= 'a' && symbol <= 'z';

            // Проверяем кириллицу
            bool isCyrillic = (symbol >= 'а' && symbol <= 'я') || symbol == 'ё' || symbol == 'Ё';

            return isLatin || isCyrillic;
        }

        public bool LexicalAnalyzerScan(string inputProgrammText)
        {
            ClearAll();
            SetPipeString(inputProgrammText);
            string currentState = "H";
            int lookPosition;
            bool inStringLiteral = false;

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
                        else if (currentSymbol == '"')
                        {
                            // Начало строкового литерала
                            AddSymbol(); // Добавляем открывающую кавычку
                            GetSymbol();
                            currentState = "STRING";
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
                            // Сохраняем точку в буфер
                            AddSymbol();
                            GetSymbol();
                            // Проверяем, что следует после точки
                            if (char.IsDigit(currentSymbol))
                            {
                                // Если после точки цифра - это дробное число
                                currentState = "RD";
                            }
                            else if (currentSymbol == 'E' || currentSymbol == 'e')
                            {
                                // Если после точки E/e - это тоже часть числа
                                currentState = "RD";
                            }
                            else
                            {
                                // Иначе это просто точка (разделитель)
                                currentState = "SO";
                            }
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

                    case "STRING":
                        // Обработка строкового литерала
                        if (currentSymbol == '⟂')
                        {
                            lexStatus = "Лексическая ошибка: незакрытая строковая константа";
                            currentState = "ERR";
                        }
                        else if (currentSymbol == '"')
                        {
                            // Закрывающая кавычка
                            AddSymbol(); // Добавляем закрывающую кавычку
                            GetSymbol();

                            // Сохраняем строку как константу
                            BufferToString();
                            lookPosition = AddToTable(constants);
                            AddLexem(3, lookPosition, false, bufferString);

                            currentState = "H";
                        }
                        else
                        {
                            // Добавляем символ в строку
                            AddSymbol();
                            GetSymbol();
                            currentState = "STRING";
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
                        else
                        {
                            lexStatus = "Лексическая ошибка: неизвестный тип данных";
                            currentState = "ERR";
                        }
                        break;

                    case "I":
                        if (IsLetterAlphabet(currentSymbol) || char.IsDigit(currentSymbol) || currentSymbol == '_')
                        {
                            AddSymbol();
                            GetSymbol();
                            currentState = "I";
                        }
                        else
                        {
                            BufferToString();

                            // Сначала проверяем, является ли это ключевым словом
                            lookPosition = LookTable(bufferString, service);
                            if (lookPosition != -1)
                            {
                                AddLexem(1, lookPosition, false, bufferString);
                            }
                            // Затем проверяем операторы
                            else if (IsOperator(bufferString))
                            {
                                lookPosition = LookTable(bufferString, operators);
                                if (lookPosition == -1)
                                {
                                    lookPosition = AddToTable(operators);
                                }
                                AddLexem(5, lookPosition, false, bufferString);
                            }
                            // Иначе это идентификатор
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
                            if (char.IsDigit(currentSymbol))
                            {
                                currentState = "RD";
                            }
                            else
                            {
                                // Если после точки не цифра, обрабатываем как целое число
                                currentState = "DN";
                            }
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

                        // Проверяем, является ли буфер просто точкой
                        if (bufferString == ".")
                        {
                            lookPosition = LookTable(bufferString, separators);
                            if (lookPosition != -1)
                            {
                                AddLexem(2, lookPosition, false, bufferString);
                            }
                            else
                            {
                                lookPosition = AddToTable(separators, bufferString);
                                AddLexem(2, lookPosition, false, bufferString);
                            }
                        }
                        else
                        {
                            lookPosition = AddToTable(constants);
                            AddLexem(3, lookPosition, false, bufferString);
                        }
                        currentState = "H";
                        break;

                    case "C":
                        if (currentSymbol == '⟂')
                        {
                            lexStatus = "Лексическая ошибка: не найден закрывающий символ для комментария.";
                            currentState = "ERR";
                        }
                        else if (IsCommentEnd())
                        {
                            currentState = "H";
                        }
                        else
                        {
                            GetSymbol();
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
            return currentState == "OUT";
        }
    }
}