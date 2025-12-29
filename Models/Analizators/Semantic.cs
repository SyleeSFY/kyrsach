using System;
using System.Collections.Generic;
using System.Linq;
using static compiler_prog.Data;

namespace compiler_prog
{
    public class Semantic
    {
        private Data parentObj;
        public List<Lexem> TID_temp = new List<Lexem>();
        public Stack<string> stackCheckContVir = new Stack<string>();
        private Stack<int> stackCheckDeclare = new Stack<int>();

        public Semantic(ref Data obj)
        {
            parentObj = obj;
            TID_temp = new List<Lexem>(parentObj.TID);
        }

        public void Initialize()
        {
            stackCheckContVir.Clear();
            stackCheckDeclare.Clear();
            TID_temp = new List<Lexem>(parentObj.TID);
        }

        public bool DeclareIdentifier(int number, string type)
        {
            if (number < 0 || number >= TID_temp.Count)
                return false;

            if (TID_temp[number].isDeclared)
            {
                return false; // Уже объявлен
            }

            // Создаем новую структуру с обновленными значениями
            Lexem updatedLexem = new Lexem
            {
                numTable = TID_temp[number].numTable,
                numInTable = TID_temp[number].numInTable,
                value = TID_temp[number].value,
                isDeclared = true,
                type = type
            };

            // Заменяем в списке
            TID_temp[number] = updatedLexem;
            return true;
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

        public int CheckNumberType(string numberValue)
        {
            if (IsBinary(numberValue) || IsOctal(numberValue) || IsHexadecimal(numberValue))
            {
                return 1;
            }
            else if (IsDecimal(numberValue))
            {
                return 1;
            }
            else if (IsReal(numberValue))
            {
                return 0;
            }
            else
            {
                return 21;
            }
        }

        public bool IsReal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            bool hasDot = value.Contains('.');
            bool hasExp = value.ToLower().Contains('e');

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

        public string GetOperationResultType(string operation, string type1, string type2)
        {
            // Для ass (присваивание)
            if (operation == "ass" || operation == "=")
            {
                return type2;
            }

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

        public int CheckIdentifierInExpression(Lexem identifier)
        {
            if (!CheckIdentifierDeclared(identifier.numInTable))
            {
                return 113;
            }
            return 0;
        }

        public int CheckNumericConstant(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 23;

            if (value == "true" || value == "false")
                return 0;

            if (IsReal(value) || IsBinary(value) || IsOctal(value) ||
                IsDecimal(value) || IsHexadecimal(value))
            {
                return 0;
            }

            if (double.TryParse(value, out _))
            {
                return 0;
            }

            return 23;
        }

        public void ClearAll()
        {
            stackCheckContVir.Clear();
            stackCheckDeclare.Clear();
            TID_temp.Clear();
        }

        public string GetConstantType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value == "true" || value == "false")
                return "$";

            if (IsReal(value))
                return "!";

            return "%";
        }
    }
}