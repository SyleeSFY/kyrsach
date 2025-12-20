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

        private string GetVariableName(int tidIndex)
        {
            if (tidIndex >= 0 && tidIndex < _tid.Count)
                return _tid[tidIndex].value;
            return $"var_{tidIndex}";
        }

        private string GetOperandValue(PolizStruct item)
        {
            if (item.classValue == 4) return GetVariableName(item.value);
            if (item.classValue == 3)
            {
                if (item.value >= 0 && item.value < _constants.Count)
                    return _constants[item.value];
                return "0";
            }
            return "null";
        }

        private string ConvertToCSharpType(string sourceType)
        {
            switch (sourceType)
            {
                case "%": return "int";
                case "!": return "double";
                case "$": return "bool";
                default: return "object";
            }
        }

        private string GetDefaultValue(string type)
        {
            switch (type)
            {
                case "int": return "0";
                case "double": return "0.0";
                case "bool": return "false";
                default: return "null";
            }
        }

        private string ConvertToCSharpArithmetic(string op)
        {
            switch (op)
            {
                case "plus": return "+";
                case "minus": return "-";
                case "mul": return "*";
                case "div": return "/";
                default: return op;
            }
        }

        private string ConvertToCSharpComparison(string op)
        {
            switch (op)
            {
                case "EQ": return "==";
                case "NE": return "!=";
                case "LT": return "<";
                case "LE": return "<=";
                case "GT": return ">";
                case "GE": return ">=";
                default: return op;
            }
        }

        private void ProcessPolizItem(PolizStruct item, List<string> instructions, Stack<string> stack)
        {
            if (item.classValue == 3 || item.classValue == 4)
            {
                stack.Push(GetOperandValue(item));
            }
            else if (item.classValue == 2)
            {
                if (item.type == "=")
                {
                    if (stack.Count >= 2)
                    {
                        string varName = stack.Pop();
                        string value = stack.Pop();
                        instructions.Add(varName + " = " + value + ";");
                    }
                }
                else if (item.type == "W")
                {
                    if (stack.Count > 0)
                    {
                        string value = stack.Pop();
                        instructions.Add("Console.WriteLine(" + value + ");");
                    }
                }
                else if (item.type == "plus" || item.type == "minus" ||
                         item.type == "mul" || item.type == "div")
                {
                    if (stack.Count >= 2)
                    {
                        string right = stack.Pop();
                        string left = stack.Pop();
                        string expr = "(" + left + " " + ConvertToCSharpArithmetic(item.type) + " " + right + ")";
                        stack.Push(expr);
                    }
                }
                else if (item.type == "GT" || item.type == "LT" || item.type == "EQ" ||
                         item.type == "NE" || item.type == "LE" || item.type == "GE")
                {
                    if (stack.Count >= 2)
                    {
                        string right = stack.Pop();
                        string left = stack.Pop();
                        string expr = "(" + left + " " + ConvertToCSharpComparison(item.type) + " " + right + ")";
                        stack.Push(expr);
                    }
                }
            }
        }

        private List<string> GenerateFromPoliz()
        {
            var instructions = new List<string>();
            var stack = new Stack<string>();

            for (int i = 0; i < _polizLength; i++)
            {
                var current = _poliz[i];

                // --- if-else ---
                if (current.classValue == 2 && current.type == "УПЛ" && stack.Count > 0)
                {
                    string condition = stack.Pop();
                    instructions.Add("if (" + condition + ")");
                    instructions.Add("{");

                    int elseLabel = current.value;
                    i++;
                    while (i < _polizLength && !(_poliz[i].classValue == 2 && _poliz[i].type == "БП"))
                    {
                        ProcessPolizItem(_poliz[i], instructions, stack);
                        i++;
                    }
                    instructions.Add("}"); // then

                    int j = i;
                    while (j < _polizLength)
                    {
                        if (_poliz[j].classValue == 0 && _poliz[j].value == elseLabel)
                        {
                            instructions.Add("else");
                            instructions.Add("{");
                            j++;
                            while (j < _polizLength && !(_poliz[j].classValue == 0 && _poliz[j].value == _poliz[i].value))
                            {
                                ProcessPolizItem(_poliz[j], instructions, stack);
                                j++;
                            }
                            instructions.Add("}"); // else
                            i = j - 1;
                            break;
                        }
                        j++;
                    }
                    continue;
                }

                // --- Цикл ---
                if (current.classValue == 0 && i + 3 < _polizLength &&
                    _poliz[i + 1].classValue == 4 &&
                    _poliz[i + 2].classValue == 3 &&
                    _poliz[i + 3].classValue == 2 && _poliz[i + 3].type == "LE")
                {
                    string loopVar = GetOperandValue(_poliz[i + 1]);
                    string endValue = GetOperandValue(_poliz[i + 2]);

                    // Проверяем, есть ли инкремент внутри тела
                    bool hasIncrementInside = false;
                    for (int k = i + 4; k < _polizLength; k++)
                    {
                        if (_poliz[k].classValue == 2 && _poliz[k].type == "=" &&
                            _poliz[k - 1].classValue == 4 && GetOperandValue(_poliz[k - 1]) == loopVar)
                        {
                            hasIncrementInside = true;
                            break;
                        }
                        if (_poliz[k].classValue == 2 && _poliz[k].type == "БП") break;
                    }

                    // Генерируем только цикл с условием
                    if (hasIncrementInside)
                        instructions.Add("while (" + loopVar + " <= " + endValue + ")");
                    else
                        instructions.Add("for (" + loopVar + " <= " + endValue + "; " + loopVar + "++)");

                    instructions.Add("{");

                    i += 4; // пропускаем условие
                    while (i < _polizLength)
                    {
                        if (_poliz[i].classValue == 2 && _poliz[i].type == "БП")
                        {
                            i++;
                            break;
                        }
                        ProcessPolizItem(_poliz[i], instructions, stack);
                        i++;
                    }

                    instructions.Add("}");
                    continue;
                }

                // --- Остальные инструкции ---
                ProcessPolizItem(current, instructions, stack);
            }

            return instructions;
        }

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

                // Объявление переменных
                foreach (var v in _tid.Where(x => x.isDeclared))
                {
                    string type = ConvertToCSharpType(v.type);
                    sb.AppendLine("            " + type + " " + v.value + " = " + GetDefaultValue(type) + ";");
                }

                sb.AppendLine();
                sb.AppendLine("            // Исполняемый код");
                var instructions = GenerateFromPoliz();
                foreach (var instr in instructions)
                    sb.AppendLine("            " + instr);

                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");
                GeneratedCode.Code = sb.ToString();
                return GeneratedCode;
            }
            catch (Exception ex)
            {
                GeneratedCode.Errors.Add("Ошибка генерации кода: " + ex.Message);
                return GeneratedCode;
            }
        }

        public GeneratedCode GenerateCode()
        {
            return TargetLanguage == TargetLanguage.CSharp ? GenerateCSharpCode() : GenerateCSharpCode();
        }

        public void SaveToFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, GeneratedCode.Code);
            }
            catch (Exception ex)
            {
                GeneratedCode.Errors.Add("Ошибка сохранения файла: " + ex.Message);
            }
        }
    }
}
