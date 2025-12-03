using compiler_prog;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CompilerModelLang
{
    public partial class compiler : Form
    {
        private bool _fileOpen = false;

        public CompilerData data;
        public LexicalAnalyzer analysisObj;
        public SyntaxAnalyzer syntaxObj;

        public compiler()
        {
            InitializeComponent();
            data = new CompilerData();
            analysisObj = new LexicalAnalyzer(ref data);
            syntaxObj = new SyntaxAnalyzer(ref data);
            data.Service
                .Select((service, index) => new { Service = service, Index = index })
                .ToList()
                .ForEach(item =>
                {
                    serviceGrid.Rows.Add(item.Index, item.Service);
                });
            data.Separators
                .Select((service, index) => new { Service = service, Index = index })
                .ToList()
                .ForEach(item =>
                {
                    separatorsGrid.Rows.Add(item.Index, item.Service);
                });
        }

        private void lexicalAnalyzerForm_Closed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
        }

        private void SelectFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                var fileName = openFileDialog1.FileName;
                var fileText = System.IO.File.ReadAllText(fileName);
                textBox1.Text = fileText;
                _fileOpen = true;
                statusLabel.Text = "Файл успешно открыт";
            }
        }

        private void StartLexAnalyzer(object sender, EventArgs e)
        {
            string resultString = null;
            int counterCell = 0;
            if (_fileOpen)
            {
                textBox2.Text = null;
                indentGrid.Rows.Clear();
                constantsGrid.Rows.Clear();
                data.LexicalValid = analysisObj.LexicalAnalyzerScan(textBox1.Text);
                if (data.LexicalValid)
                {
                    data.LexOut.ForEach(x => resultString += "(" + x.numTable + "," + x.numInTable + ")");

                    // Генерация ячеек таблицы индентификаторов
                    foreach (LexemStruct temp in data.TID)
                    {
                        indentGrid.Rows.Add(counterCell, temp.value);
                        counterCell++;
                    }
                    counterCell = 0;
                    // Генерация ячеек таблицы констант
                    foreach (string currentSymbol in data.Constants)
                    {
                        if (currentSymbol == ".")
                        {
                            continue;
                        }
                        constantsGrid.Rows.Add(counterCell, currentSymbol);
                        counterCell++;

                    }
                }
                else
                    resultString = null;

                textBox2.Text = resultString;
                statusLabel.Text = data.LexicalStatus;
            }
            else
            {
                statusLabel.Text = "Файл для анализа не открыт!";
            }
        }

        public void StartSyntaxClick(object sender, EventArgs e)
        {
            string answer;
            int answerInt = 0;
            string resultString = String.Empty;

            if (data.LexicalValid)
            {
                answerInt = syntaxObj.SyntaxStart();
                if (answerInt == 0)
                {
                    // Формирование окна ПОЛИЗ - ИСПРАВЛЕННАЯ ЧАСТЬ
                    for (int i = 0; i < syntaxObj.free; i++)  // Используем free вместо Count()
                    {
                        if (syntaxObj.OutputPoliz[i].classValue == 0) // метка
                        {
                            resultString += $"L{syntaxObj.OutputPoliz[i].value} ";
                        }
                        else if (syntaxObj.OutputPoliz[i].classValue == 2) // операция
                        {
                            resultString += $"{syntaxObj.OutputPoliz[i].type} ";
                        }
                        else if (syntaxObj.OutputPoliz[i].classValue == 5) // присваивание
                        {
                            resultString += $"{syntaxObj.OutputPoliz[i].type} ";
                        }
                        else // идентификатор или константа
                        {
                            resultString += $"{syntaxObj.OutputPoliz[i].type} ";
                        }
                    }
                    // Вывод в окно ПОЛИЗ
                    polizTextBox.Text = resultString;
                }
                statusLabel.Text = ErrorHandler(answerInt);
                MessageBox.Show($"Syntax result: {answerInt}, POLIZ elements: {syntaxObj.free}", "Debug Info");
            }
            else
            {
                statusLabel.Text = "Синтаксический анализ: невозможно произвести анализ, лексический анализатор завершен с ошибкой.";
            }
        }

        private string ErrorHandler(int answer)
        {
            string predycat = null;
            string errorCode = null;

            if (answer == 0)
            {
                predycat = "Синтаксически управляемый перевод: ";
                errorCode = "успешно";
            }
            else
            {
                if (answer < 100)
                    predycat = "Синтаксическая ошибка! ";
                else
                    predycat = "Семантическая ошибка! ";

                switch (answer)
                {
                    case 1:
                        errorCode = "Ожидается 'program' в начале программы";
                        break;
                    case 2:
                        errorCode = "Ожидается 'begin' после описания";
                        break;
                    case 3:
                        errorCode = "Ожидается 'end.' в конце программы";
                        break;
                    case 4:
                        errorCode = "Ожидается ';' после объявления типа";
                        break;
                    case 5:
                        errorCode = "Неизвестный тип данных (ожидается %, ! или $)";
                        break;
                    case 7:
                        errorCode = "Идентификатор содержит недопустимые символы";
                        break;
                    case 8:
                        errorCode = "Неизвестный или необъявленный оператор";
                        break;
                    case 10:
                        errorCode = "Пропущен знак 'ass' в операторе присваивания";
                        break;
                    case 11:
                        errorCode = "Ожидается 'then' в конструкции if";
                        break;
                    case 14:
                        errorCode = "Ожидается 'to' в цикле for";
                        break;
                    case 15:
                        errorCode = "Ожидается 'do' в цикле for";
                        break;
                    case 16:
                        errorCode = "Ожидается 'do' в цикле while";
                        break;
                    case 17:
                        errorCode = "Ожидается '(' в конструкции read";
                        break;
                    case 18:
                        errorCode = "Ожидается ')' в конструкции read";
                        break;
                    case 19:
                        errorCode = "Ожидается '(' в конструкции write";
                        break;
                    case 20:
                        errorCode = "Ожидается ')' в конструкции write";
                        break;
                    case 21:
                        errorCode = "Ожидается ')' в выражении";
                        break;
                    case 22:
                        errorCode = "Символ не является множителем";
                        break;
                    case 23:
                        errorCode = "Нераспознанная числовая константа";
                        break;
                    case 24:
                        errorCode = "Ожидается '.' после end";
                        break;
                    case 101:
                        errorCode = "Повторное определение идентификатора";
                        break;
                    case 102:
                        errorCode = "Идентификатор в операторе присваивания не описан";
                        break;
                    case 103:
                        errorCode = "Операнды операции присваивания имеют разные типы";
                        break;
                    case 104:
                        errorCode = "Ожидался логический тип ($) в конструкции if";
                        break;
                    case 105:
                        errorCode = "Ожидался логический тип ($) в цикле for";
                        break;
                    case 106:
                        errorCode = "Ожидался логический тип ($) в цикле while";
                        break;
                    case 107:
                        errorCode = "Первый идентификатор в операторе read не описан";
                        break;
                    case 108:
                        errorCode = "Последующие идентификаторы в операторе read не описаны";
                        break;
                    case 110:
                        errorCode = "Операнды операции отношения имеют разные типы";
                        break;
                    case 111:
                        errorCode = "Операнды операции сложения имеют разные типы";
                        break;
                    case 112:
                        errorCode = "Операнды операции умножения имеют разные типы";
                        break;
                    case 113:
                        errorCode = "Идентификатор в выражении не описан";
                        break;
                    case 114:
                        errorCode = "Операнд унарной операции имеет недопустимый тип";
                        break;
                    default:
                        errorCode = "Неизвестная ошибка: " + answer;
                        break;
                }
            }
            return $"{predycat} {errorCode}";
        }

        private void StartInterpeter(object sender, EventArgs e)
        {

        }

        private void exitApp(object sender, EventArgs e)
            =>this.Close();


        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
