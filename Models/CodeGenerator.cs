using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using compiler_prog.Models;

namespace compiler_prog
{
    public class CodeGenerator
    {
        private SyntaxAnalyzer _syntaxAnalyzer;
        private List<LexemStruct> _tid;
        private List<string> _constants;
        private PolizStruct[] _poliz;
        private int _polizLength;

        public GeneratedCode GeneratedCode { get; private set; }
        public TargetLanguage TargetLanguage { get; set; }

        public CodeGenerator(SyntaxAnalyzer syntaxAnalyzer, List<LexemStruct> tid,
                           List<string> constants, PolizStruct[] poliz, int polizLength)
        {
            _syntaxAnalyzer = syntaxAnalyzer;
            _tid = tid;
            _constants = constants;
            _poliz = poliz;
            _polizLength = polizLength;
            GeneratedCode = new GeneratedCode();
            TargetLanguage = TargetLanguage.CSharp;
        }

        /// <summary>
        /// Генерация кода на C#
        /// </summary>
        public GeneratedCode GenerateCSharpCode()
        {
            GeneratedCode = new GeneratedCode();

            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine();
                sb.AppendLine("namespace GeneratedProgram");
                sb.AppendLine("{");
                sb.AppendLine("    class Program");
                sb.AppendLine("    {");
                sb.AppendLine("        static void Main(string[] args)");
                sb.AppendLine("        {");

                sb.AppendLine("            // Объявление переменных");
                foreach (var variable in _tid.Where(v => v.isDeclared))
                {
                    string csharpType = ConvertToCSharpType(variable.type);
                    string defaultValue = GetDefaultValue(csharpType);
                    sb.AppendLine($"            {csharpType} {variable.value} = {defaultValue};");
                }
                sb.AppendLine();

                sb.AppendLine("            // Исполняемый код");
                var generatedInstructions = InterpretPoliz();
                foreach (var instruction in generatedInstructions)
                {
                    sb.AppendLine("            " + instruction);
                }

                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                GeneratedCode.Code = sb.ToString();

                return GeneratedCode;
            }
            catch (Exception ex)
            {
                GeneratedCode.Errors.Add($"Ошибка генерации кода: {ex.Message}");
                return GeneratedCode;
            }
        }

        /// <summary>
        /// Алгоритм интерпретации ПОЛИЗ для генерации C# кода
        /// </summary>
        private List<string> InterpretPoliz()
        {
            var instructions = new List<string>();
            var stack = new Stack<string>();
            var labels = new Dictionary<int, string>();

            // Первый проход: находим все метки
            for (int i = 0; i < _polizLength; i++)
            {
                if (_poliz[i].classValue == 0) // Метка
                {
                    labels[_poliz[i].value] = $"label{_poliz[i].value}";
                }
            }

            // Второй проход: генерация кода
            for (int i = 0; i < _polizLength; i++)
            {
                var polizItem = _poliz[i];

                switch (polizItem.classValue)
                {
                    case 0: // Метка
                        instructions.Add($"{labels[polizItem.value]}:");
                        break;

                    case 2: // Операция
                        GenerateOperationCode(polizItem, instructions, ref stack, labels);
                        break;

                    case 3: // Константа
                        string constantValue = _constants[polizItem.value];
                        stack.Push(constantValue);
                        break;

                    case 4: // Идентификатор
                        stack.Push(polizItem.type);
                        break;

                    case 5: // Специальный (для read/write)
                        stack.Push(polizItem.type);
                        break;
                }
            }

            return instructions;
        }

        /// <summary>
        /// Генерация кода для операций
        /// </summary>
        private void GenerateOperationCode(PolizStruct operation, List<string> instructions,
                                         ref Stack<string> stack, Dictionary<int, string> labels)
        {
            switch (operation.type)
            {
                case "=":
                    string right = stack.Pop();
                    string left = stack.Pop();
                    instructions.Add($"{left} = {right};");
                    break;

                case "plus":
                case "min":
                case "mult":
                case "div":
                    string op2 = stack.Pop();
                    string op1 = stack.Pop();
                    string csharpOp = ConvertToCSharpOperator(operation.type);
                    stack.Push($"({op1} {csharpOp} {op2})");
                    break;

                case "and":
                    string and2 = stack.Pop();
                    string and1 = stack.Pop();
                    stack.Push($"({and1} && {and2})");
                    break;

                case "or":
                    string or2 = stack.Pop();
                    string or1 = stack.Pop();
                    stack.Push($"({or1} || {or2})");
                    break;

                case "EQ":
                case "NE":
                case "LT":
                case "LE":
                case "GT":
                case "GE":
                    string cmp2 = stack.Pop();
                    string cmp1 = stack.Pop();
                    string cmpOp = ConvertToCSharpComparison(operation.type);
                    stack.Push($"({cmp1} {cmpOp} {cmp2})");
                    break;

                case "!":
                    // Безусловный переход
                    int gotoLabel = operation.value;
                    instructions.Add($"goto {labels[gotoLabel]};");
                    break;

                case "!F":
                    // Условный переход, если false
                    int condLabel = operation.value;
                    string condition = stack.Pop();
                    instructions.Add($"if (!({condition})) goto {labels[condLabel]};");
                    break;

                case "R":
                    // Операция чтения
                    string readVar = stack.Pop();
                    instructions.Add($"Console.Write(\"Введите значение для {readVar}: \");");
                    instructions.Add($"{readVar} = Convert.ToDouble(Console.ReadLine());");
                    break;

                case "W":
                    // Операция записи
                    string writeValue = stack.Pop();
                    instructions.Add($"Console.WriteLine({writeValue});");
                    break;

                case "~":
                    string unaryOperand = stack.Pop();
                    stack.Push($"!({unaryOperand})");
                    break;
            }
        }

        /// <summary>
        /// Преобразование типов исходного языка в C# типы
        /// </summary>
        private string ConvertToCSharpType(string sourceType)
        {
            switch (sourceType)
            {
                case "%":
                    return "int";     // целые числа
                case "!":
                    return "double";  // вещественные числа
                case "$":
                    return "bool";    // логические значения
                default:
                    return "object";  // по умолчанию
            }
        }

        /// <summary>
        /// Получение значения по умолчанию для типа
        /// </summary>
        private string GetDefaultValue(string csharpType)
        {
            switch (csharpType)
            {
                case "int":
                    return "0";
                case "double":
                    return "0.0";
                case "bool":
                    return "false";
                default:
                    return "null";
            }
        }

        /// <summary>
        /// Преобразование операторов в C# операторы
        /// </summary>
        private string ConvertToCSharpOperator(string sourceOperator)
        {
            switch (sourceOperator)
            {
                case "plus":
                    return "+";
                case "min":
                    return "-";
                case "mult":
                    return "*";
                case "div":
                    return "/";
                default:
                    return sourceOperator;
            }
        }

        /// <summary>
        /// Преобразование операторов сравнения
        /// </summary>
        private string ConvertToCSharpComparison(string sourceComparison)
        {
            switch (sourceComparison)
            {
                case "EQ":
                    return "==";
                case "NE":
                    return "!=";
                case "LT":
                    return "<";
                case "LE":
                    return "<=";
                case "GT":
                    return ">";
                case "GE":
                    return ">=";
                default:
                    return sourceComparison;
            }
        }

        /// <summary>
        /// Основной метод генерации кода
        /// </summary>
        public GeneratedCode GenerateCode()
        {
            switch (TargetLanguage)
            {
                case TargetLanguage.CSharp:
                    return GenerateCSharpCode();
                default:
                    return GenerateCSharpCode();
            }
        }

        /// <summary>
        /// Сохранение сгенерированного кода в файл
        /// </summary>
        public void SaveToFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, GeneratedCode.Code);
            }
            catch (Exception ex)
            {
                GeneratedCode.Errors.Add($"Ошибка сохранения файла: {ex.Message}");
            }
        }
    }
}