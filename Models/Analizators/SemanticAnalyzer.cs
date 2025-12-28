using System;
using System.Collections.Generic;
using System.Linq;
using static compiler_prog.CompilerData;

namespace compiler_prog
{
    public class SemanticAnalyzer
    {
        private CompilerData parentObj;
        public List<Lexem> TID_temp = new List<Lexem>();
        public Stack<string> stackCheckContVir = new Stack<string>();
        private Stack<int> stackCheckDeclare = new Stack<int>();

        public SemanticAnalyzer(ref CompilerData obj)
        {
            parentObj = obj;
            TID_temp = parentObj.TID.ToList();
        }

        public void Initialize()
        {
            stackCheckContVir.Clear();
            stackCheckDeclare.Clear();
            TID_temp = parentObj.TID.ToList();
        }

        // Методы для работы с идентификаторами
        public bool DeclareIdentifier(int number, string type)
        {
            if (number < 0 || number >= TID_temp.Count)
                return false;

            Lexem temp;
            if (TID_temp[number].numInTable == number)
            {
                if (TID_temp[number].isDeclared)
                {
                    return false; // Уже объявлен
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

        public bool CheckIdentifierDeclared(int identifierIndex)
        {
            if (identifierIndex >= 0 && identifierIndex < TID_temp.Count && TID_temp[identifierIndex].isDeclared)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetIdentifierType(int identifierIndex)
        {
            if (identifierIndex >= 0 && identifierIndex < TID_temp.Count)
            {
                return TID_temp[identifierIndex].type;
            }
            return string.Empty;
        }

        public string GetIdentifierValue(int identifierIndex)
        {
            if (identifierIndex >= 0 && identifierIndex < TID_temp.Count)
            {
                return TID_temp[identifierIndex].value;
            }
            return string.Empty;
        }

        // Методы для работы со стеком объявлений
        public void PushDeclarationToStack(int identifierIndex)
        {
            stackCheckDeclare.Push(identifierIndex);
        }

        public int PopDeclarationFromStack()
        {
            if (stackCheckDeclare.Count > 0)
            {
                return stackCheckDeclare.Pop();
            }
            return -1;
        }

        public void ClearDeclarationStack()
        {
            stackCheckDeclare.Clear();
        }

        public bool HasDeclarationsInStack()
        {
            return stackCheckDeclare.Count > 0;
        }

        // Метод для объявления всех идентификаторов из стека
        public bool DeclareAllIdentifiersInStack(string type)
        {
            bool success = true;

            while (stackCheckDeclare.Count > 0)
            {
                int identifierIndex = stackCheckDeclare.Pop();
                if (!DeclareIdentifier(identifierIndex, type))
                {
                    success = false;
                }
            }

            return success;
        }

        // Методы для работы с константами
        public int CheckNumberType(string numberValue)
        {
            if (IsBinary(numberValue) || IsOctal(numberValue) || IsHexadecimal(numberValue))
            {
                return 1; // Целое
            }
            else if (IsDecimal(numberValue))
            {
                return 1; // Целое
            }
            else if (IsReal(numberValue))
            {
                return 0; // Вещественное
            }
            else
            {
                return 21; // Ошибка
            }
        }

        public bool IsReal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            // Проверяем, содержит ли число точку или экспоненту
            bool hasDot = value.Contains('.');
            bool hasExp = value.ToLower().Contains('e');

            // Если есть точка или экспонента, это вещественное число
            if (hasDot || hasExp)
                return true;

            return false;
        }

        public bool IsBinary(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.ToUpper();
            if (value.EndsWith("B"))
            {
                string numPart = value.Substring(0, value.Length - 1);
                if (string.IsNullOrEmpty(numPart))
                    return false;

                foreach (char c in numPart)
                {
                    if (c != '0' && c != '1')
                        return false;
                }
                return true;
            }
            return false;
        }

        public bool IsOctal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.ToUpper();
            if (value.EndsWith("O"))
            {
                string numPart = value.Substring(0, value.Length - 1);
                if (string.IsNullOrEmpty(numPart))
                    return false;

                foreach (char c in numPart)
                {
                    if (c < '0' || c > '7')
                        return false;
                }
                return true;
            }
            return false;
        }

        public bool IsDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.ToUpper();
            if (value.EndsWith("D"))
            {
                string numPart = value.Substring(0, value.Length - 1);
                return double.TryParse(numPart, out _);
            }
            else
            {
                return double.TryParse(value, out _);
            }
        }

        public bool IsHexadecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.ToUpper();
            if (value.EndsWith("H"))
            {
                string numPart = value.Substring(0, value.Length - 1);
                if (string.IsNullOrEmpty(numPart))
                    return false;

                foreach (char c in numPart)
                {
                    if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')))
                        return false;
                }
                return true;
            }
            return false;
        }

        // Методы для работы с типами операций
        public string GetOperationResultType(string operation, string type1, string type2)
        {
            if (operation == "plus" || operation == "min")
            {
                if (type1 == "%" && type2 == "%")
                {
                    return "%";
                }
                else if ((type1 == "%" && type2 == "!") || (type1 == "!" && type2 == "%"))
                {
                    return "!";
                }
                else if (type1 == "!" && type2 == "!")
                {
                    return "!";
                }
                else
                {
                    return string.Empty;
                }
            }
            else if (operation == "mult")
            {
                if (type1 == "%" && type2 == "%")
                {
                    return "%";
                }
                else if ((type1 == "%" && type2 == "!") || (type1 == "!" && type2 == "%"))
                {
                    return "!";
                }
                else if (type1 == "!" && type2 == "!")
                {
                    return "!";
                }
                else
                {
                    return string.Empty;
                }
            }
            else if (operation == "div")
            {
                if (type1 == "%" && type2 == "%")
                {
                    return "!";
                }
                else if ((type1 == "%" && type2 == "!") || (type1 == "!" && type2 == "%"))
                {
                    return "!";
                }
                else if (type1 == "!" && type2 == "!")
                {
                    return "!";
                }
                else
                {
                    return string.Empty;
                }
            }
            else if (operation == "and" || operation == "or")
            {
                if (type1 == "$" && type2 == "$")
                {
                    return "$";
                }
                else
                {
                    return string.Empty;
                }
            }
            else if (operation == "NE" || operation == "EQ" || operation == "LT" ||
                     operation == "LE" || operation == "GT" || operation == "GE")
            {
                if (type1 == "$" || type2 == "$")
                {
                    return string.Empty;
                }
                else
                {
                    return "$";
                }
            }
            return string.Empty;
        }

        // Методы для работы со стеком типов
        public void PushTypeToStack(string type)
        {
            stackCheckContVir.Push(type);
        }

        public string PopTypeFromStack()
        {
            if (stackCheckContVir.Count > 0)
            {
                return stackCheckContVir.Pop();
            }
            return string.Empty;
        }

        public bool CheckAssignmentCompatibility()
        {
            if (stackCheckContVir.Count < 2)
            {
                return false;
            }

            string t2 = stackCheckContVir.Pop();
            string t1 = stackCheckContVir.Pop();

            // Простая проверка на равенство типов
            return t1 == t2;
        }

        public bool CheckBooleanExpression()
        {
            if (stackCheckContVir.Count == 0)
            {
                return false;
            }

            string type = stackCheckContVir.Pop();
            return type == "$";
        }

        // Метод для проверки идентификаторов в выражении
        public int CheckIdentifierInExpression(Lexem identifier)
        {
            if (!CheckIdentifierDeclared(identifier.numInTable))
            {
                return 113; // Идентификатор не объявлен
            }
            return 0;
        }

        // Метод для проверки числовой константы
        public int CheckNumericConstant(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 23;

            // Если это true/false
            if (value == "true" || value == "false")
                return 0;

            // Проверяем все возможные форматы чисел
            if (IsReal(value) || IsBinary(value) || IsOctal(value) ||
                IsDecimal(value) || IsHexadecimal(value))
            {
                return 0;
            }

            // Пробуем распарсить как double
            if (double.TryParse(value, out _))
            {
                return 0;
            }

            return 23; // Ошибка
        }

        // Метод для очистки всех данных
        public void ClearAll()
        {
            stackCheckContVir.Clear();
            stackCheckDeclare.Clear();
            TID_temp.Clear();
        }

        // Метод для получения типа числовой константы
        public string GetConstantType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value == "true" || value == "false")
                return "$";

            if (IsReal(value))
                return "!";

            return "%"; // По умолчанию целое
        }
    }
}