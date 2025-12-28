using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using compiler_prog.Models;
using static compiler_prog.Data;

namespace compiler_prog
{
    public enum ErrorCode
    {
        StartNotWithProgram = 0,
    }

    public class Syntax

    {
        private int labelCounter = 1;
        private CodeGenerator _codeGenerator;
        private FileWork _fileWork;
        private Lexem currentLex;
        private string currentLexValue;
        private int counterSLexem;
        private Data parentObj;
        private Semantic semanticAnalyzer;

        // Для построения правил
        private List<string> rulePath = new List<string>();
        private string lastWrittenRule = "";

        public Poliz[] OutputPoliz = new Poliz[1000];
        public int free = 0;

        public Syntax(ref Data obj)
        {
            parentObj = obj;
            counterSLexem = 0;
            _fileWork = new FileWork();
            semanticAnalyzer = new Semantic(ref obj);
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

            if (rule != lastWrittenRule)
            {
                _fileWork.WriteFile(rule);
                lastWrittenRule = rule;
            }
        }

        private void CompleteRule()
        {
            WriteRuleIfComplete();
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

            CompleteRule();
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

            CompleteRule();
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
            semanticAnalyzer.Initialize();
            Array.Clear(OutputPoliz, 0, OutputPoliz.Length);
            free = 0;
            labelCounter = 1;
            rulePath.Clear();
            lastWrittenRule = "";
            _fileWork.CreateFile();
            answer = StartProgram();

            if (answer == 0)
            {
                _codeGenerator = new CodeGenerator(this, semanticAnalyzer.TID_temp,
                    parentObj.Constants, OutputPoliz, free);
                var generatedCode = _codeGenerator.GenerateCode();
                SaveGeneratedCode(generatedCode);
            }

            _fileWork.Close();
            return answer;
        }

        public Generator GetCodeGenerator()
        {
            if (_codeGenerator == null)
                return null;
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

        private bool IsLetterAlphabet(char symbol)
        {
            symbol = char.ToLower(symbol);
            return symbol >= 'a' && symbol <= 'z';
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

        private int DESC()
        {
            int code;

            while (IsID())
            {
                CompleteRule();
                BeginRule("DESC");
                AddToRulePath("I1");
                code = I1();
                if (code != 0) return code;

                AddToRulePath("TYPE");
                code = TYPE();
                if (code != 0) return code;

                string variableType = currentLexValue;
                GetCurrentLexem();

                // Объявляем все идентификаторы из стека с указанным типом
                if (!semanticAnalyzer.DeclareAllIdentifiersInStack(variableType))
                {
                    return 101; // Ошибка повторного объявления
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

        /// <summary>
        /// Обработка индефикатора
        /// </summary>
        /// <returns></returns>
        private int I1()
        {
            int code;

            AddToRulePath("ID");
            code = ID();
            if (code != 0) return code;

            semanticAnalyzer.PushDeclarationToStack(currentLex.numInTable);
            GetCurrentLexem();

            code = I2();
            if (code != 0) return code;

            return 0;
        }

        private int I2()
        {
            if (currentLexValue == ",")
            {
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

        /// <summary>
        /// Проверка индефикатора (названия)
        /// </summary>
        /// <returns></returns>
        private int ID()
        {
            int code;
            code = IsIDAlphabet();
            if (code != 0) return code;

            AddToRulePath(currentLexValue);
            WriteRuleIfComplete();
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
                int result = ProcessAssignment();
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

        /// <summary>
        /// Обработка присваивания
        /// </summary>
        /// <returns></returns>
        private int ProcessAssignment()
        {
            Poliz temp;
            int code;

            string currentID = currentLexValue;
            int currentIDIndex = currentLex.numInTable;

            AddToRulePath("ID");
            code = ID();
            if (code != 0) return code;

            if (!semanticAnalyzer.CheckIdentifierDeclared(currentIDIndex))
            {
                return 102;
            }

            string idType = semanticAnalyzer.GetIdentifierType(currentIDIndex);
            semanticAnalyzer.PushTypeToStack(idType);

            temp.classValue = currentLex.numTable;
            temp.value = currentIDIndex;
            temp.type = currentID;
            Poliz idPolizEntry = temp;

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

            if (!semanticAnalyzer.CheckAssignmentCompatibility())
            {
                return 103;
            }

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

        /// <summary>
        /// Обработка выражений
        /// </summary>
        /// <returns></returns>
       
        //<выражение> → <операнд> <выражение'>
        private int VIR()
        {
            int code;
            AddToRulePath("OPRD");
            code = OPRD();
            if (code != 0) return code;

            code = VIR1();
            if (code != 0) return code;

            if (semanticAnalyzer.stackCheckContVir.Count == 0)
            {
                return 25;
            }

            return 0;
        }

        //<выражение'> → <операции_отношения> <операнд> <выражение'> | ε
        private int VIR1()
        {
            int code;
            if (currentLexValue == "NE" || currentLexValue == "EQ" || currentLexValue == "LT" ||
                currentLexValue == "LE" || currentLexValue == "GT" || currentLexValue == "GE")
            {
                AddToRulePath(currentLexValue);
                WriteRuleIfComplete();
                string operation = currentLexValue;

                if (semanticAnalyzer.stackCheckContVir.Count == 0)
                {
                    return 110;
                }

                GetCurrentLexem();

                code = VIR();
                if (code != 0) return code;

                if (semanticAnalyzer.stackCheckContVir.Count < 2)
                {
                    return 110;
                }

                string t2 = semanticAnalyzer.PopTypeFromStack();
                string t1 = semanticAnalyzer.PopTypeFromStack();
                string res = semanticAnalyzer.GetOperationResultType(operation, t1, t2);

                if (!string.IsNullOrEmpty(res))
                {
                    semanticAnalyzer.PushTypeToStack(res);
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

        //<операнд> → <слагаемое> <операнд'>
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

                if (semanticAnalyzer.stackCheckContVir.Count < 2)
                {
                    return 111;
                }

                string t2 = semanticAnalyzer.PopTypeFromStack();
                string t1 = semanticAnalyzer.PopTypeFromStack();
                string res = semanticAnalyzer.GetOperationResultType(operation, t1, t2);

                if (!string.IsNullOrEmpty(res))
                {
                    semanticAnalyzer.PushTypeToStack(res);
                    putPolizeLex(putOperationLex(operation));
                }
                else
                {
                    return 111;
                }
            }

            return 0;
        }

        //<операнд'> → <операции_группы_сложения> <слагаемое> <операнд'> | ε
        private int OPRD1()
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

                if (semanticAnalyzer.stackCheckContVir.Count < 2)
                {
                    return 111;
                }

                string t2 = semanticAnalyzer.PopTypeFromStack();
                string t1 = semanticAnalyzer.PopTypeFromStack();
                string res = semanticAnalyzer.GetOperationResultType(operation, t1, t2);

                if (!string.IsNullOrEmpty(res))
                {
                    semanticAnalyzer.PushTypeToStack(res);
                    putPolizeLex(putOperationLex(operation));
                }
                else
                {
                    return 111;
                }
            }

            return 0;
        }

        /// <summary>
        /// *, /, and
        /// </summary>
        /// <returns></returns>
        //<слагаемое> → <множитель> <слагаемое'>
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

                if (semanticAnalyzer.stackCheckContVir.Count < 2)
                {
                    return 112;
                }

                string t2 = semanticAnalyzer.PopTypeFromStack();
                string t1 = semanticAnalyzer.PopTypeFromStack();
                string res = semanticAnalyzer.GetOperationResultType(operation, t1, t2);

                if (!string.IsNullOrEmpty(res))
                {
                    semanticAnalyzer.PushTypeToStack(res);
                    putPolizeLex(putOperationLex(operation));
                }
                else
                {
                    return 112;
                }
            }

            return 0;
        }

        //<слагаемое'> → <операции_умножения> <множитель> <слагаемое'> | ε
        private int SLAG1()
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

                if (semanticAnalyzer.stackCheckContVir.Count < 2)
                {
                    return 112;
                }

                string t2 = semanticAnalyzer.PopTypeFromStack();
                string t1 = semanticAnalyzer.PopTypeFromStack();
                string res = semanticAnalyzer.GetOperationResultType(operation, t1, t2);

                if (!string.IsNullOrEmpty(res))
                {
                    semanticAnalyzer.PushTypeToStack(res);
                    putPolizeLex(putOperationLex(operation));
                }
                else
                {
                    return 112;
                }
            }

            return 0;
        }

        //<множитель> → ID | NUM | true | false | ( <выражение> )
        private int MNOJ()
        {
            Poliz temp;
            int code;

            if (currentLexValue == "~") 
            {
                AddToRulePath("not");
                WriteRuleIfComplete();
                GetCurrentLexem();

                code = MNOJ();  // ← not M
                if (code != 0) return code;

                if (semanticAnalyzer.stackCheckContVir.Count > 0)
                {
                    string type = semanticAnalyzer.PopTypeFromStack();
                    if (type != "$")
                    {
                        return 115; // Ошибка: not применим только к bool
                    }
                    semanticAnalyzer.PushTypeToStack("$");
                }

                putPolizeLex(putOperationLex("not"));
                return 0;
            }

            if (IsID())
            {
                AddToRulePath("ID");
                code = ID();
                if (code != 0) return code;

                code = semanticAnalyzer.CheckIdentifierInExpression(currentLex);
                if (code != 0) return code;

                string idType = semanticAnalyzer.GetIdentifierType(currentLex.numInTable);
                semanticAnalyzer.PushTypeToStack(idType);

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;
                putPolizeLex(temp);

                GetCurrentLexem();
                return 0;
            }
            else if (IsNumConst())
            {
                AddToRulePath("NUM");

                int checkResult = semanticAnalyzer.CheckNumericConstant(currentLexValue);
                if (checkResult != 0) return checkResult;

                // Определяем тип константы
                string constType = semanticAnalyzer.GetConstantType(currentLexValue);
                semanticAnalyzer.PushTypeToStack(constType);

                temp.classValue = currentLex.numTable;
                temp.value = currentLex.numInTable;
                temp.type = currentLexValue;
                putPolizeLex(temp);

                AddToRulePath(currentLexValue);
                WriteRuleIfComplete();
                GetCurrentLexem();
                return 0;
            }
            else if (currentLexValue == "false" || currentLexValue == "true")
            {
                AddToRulePath(currentLexValue);
                WriteRuleIfComplete();
                semanticAnalyzer.PushTypeToStack("$");

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

        private int IFEL()
        {
            int code;
            int jumpFalsePos;

            AddToRulePath("if");
            WriteRuleIfComplete();
            GetCurrentLexem();

            AddToRulePath("VIR");
            code = VIR();
            if (code != 0) return code;

            if (!semanticAnalyzer.CheckBooleanExpression())
            {
                return 104;
            }

            jumpFalsePos = free;
            int elseLabelNum = labelCounter;

            Poliz tempJumpFalse = new Poliz();
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

            int afterIfLabelNum = labelCounter + 1;

            Poliz tempJumpUncond = new Poliz();
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
            code = ProcessAssignment();
            if (code != 0) return code;

            if (currentLexValue != "to")
            {
                return 14;
            }

            AddToRulePath("to");
            WriteRuleIfComplete();
            GetCurrentLexem();

            int loopStartLabelNum = labelCounter;
            Poliz startLabel = makePolizeLabel();
            putPolizeLex(startLabel);

            Poliz tempVarI;
            tempVarI.classValue = 4;
            int iIndex = -1;

            for (int j = 0; j < semanticAnalyzer.TID_temp.Count; j++)
            {
                if (semanticAnalyzer.GetIdentifierValue(j) == "i" ||
                    semanticAnalyzer.GetIdentifierValue(j).Contains("i"))
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

            if (semanticAnalyzer.stackCheckContVir.Count == 0) return 105;
            string exprType = semanticAnalyzer.PopTypeFromStack();
            if (exprType != "%" && exprType != "!") return 105;

            putPolizeLex(putOperationLex("LE"));

            falseJumpPos = free;
            int exitLabelNum = labelCounter;

            Poliz tempJump = new Poliz();
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

            Poliz tempOne = new Poliz();
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

            Poliz tempBackJump = new Poliz();
            tempBackJump.type = "БП";
            tempBackJump.classValue = 2;
            tempBackJump.value = loopStartLabelNum;

            putPolizeLex(tempBackJump);

            Poliz exitLabel = makePolizeLabelWithNumber(exitLabelNum);
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

            if (!semanticAnalyzer.CheckBooleanExpression()) return 106;

            jumpFalsePos = free;
            int exitLabelNum = labelCounter;

            Poliz tempJumpFalse = new Poliz();
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

            Poliz tempJumpBack = new Poliz();
            tempJumpBack.type = "БП";
            tempJumpBack.classValue = 2;
            tempJumpBack.value = loopStartLabelNum;
            putPolizeLex(tempJumpBack);

            putPolizeLex(makePolizeLabel());

            return 0;
        }

        private int INPU()
        {
            Poliz temp;
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

            if (!semanticAnalyzer.CheckIdentifierDeclared(currentLex.numInTable))
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

                    if (!semanticAnalyzer.CheckIdentifierDeclared(currentLex.numInTable))
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

            if (semanticAnalyzer.stackCheckContVir.Count > 0)
            {
                semanticAnalyzer.PopTypeFromStack();
            }

            while (currentLexValue == ",")
            {
                AddToRulePath(",");
                WriteRuleIfComplete();
                GetCurrentLexem();

                AddToRulePath("VIR");
                code = VIR();
                if (code != 0) return code;

                if (semanticAnalyzer.stackCheckContVir.Count > 0)
                {
                    semanticAnalyzer.PopTypeFromStack();
                }
            }

            if (currentLexValue != ")")
            {
                return 20;
            }

            AddToRulePath(")");
            WriteRuleIfComplete();

            putPolizeLex(putOperationLex("W"));
            GetCurrentLexem();

            return 0;
        }

        void putPolizeLex(Poliz lex)
        {
            OutputPoliz[free] = lex;
            free++;
        }

        Poliz putOperationLex(string oper)
        {
            Poliz temp = new Poliz();

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

        void putPolizeLex5(Poliz lex)
        {
            lex.classValue = 5;
            OutputPoliz[free++] = lex;
            OutputPoliz[free - 1].type = lex.type;
        }

        Poliz makePolizeLabel()
        {
            Poliz temp = new Poliz();
            temp.classValue = 0;
            temp.value = labelCounter;
            temp.type = $"m{labelCounter}:";

            int currentLabel = labelCounter;
            labelCounter++;

            return temp;
        }

        Poliz makePolizeLabelWithNumber(int labelNumber)
        {
            Poliz temp = new Poliz();
            temp.classValue = 0;
            temp.value = labelNumber;
            temp.type = $"m{labelNumber}:";

            return temp;
        }

        private void SaveGeneratedCode(Generator generatedCode)
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