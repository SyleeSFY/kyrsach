namespace CompilerModelLang
{
    partial class compiler
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBoxInputFile = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.serviceGrid = new System.Windows.Forms.DataGridView();
            this.col0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.separatorsGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.constantsGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.indentGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusBar = new System.Windows.Forms.GroupBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.polizTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dataOperators = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CsharpCode = new System.Windows.Forms.GroupBox();
            this.codeGenerators = new System.Windows.Forms.TextBox();
            this.groupBoxInputFile.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serviceGrid)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.separatorsGrid)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.constantsGrid)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indentGrid)).BeginInit();
            this.statusBar.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataOperators)).BeginInit();
            this.CsharpCode.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBoxInputFile
            // 
            this.groupBoxInputFile.Controls.Add(this.textBox1);
            this.groupBoxInputFile.Location = new System.Drawing.Point(325, 285);
            this.groupBoxInputFile.Name = "groupBoxInputFile";
            this.groupBoxInputFile.Size = new System.Drawing.Size(540, 270);
            this.groupBoxInputFile.TabIndex = 1;
            this.groupBoxInputFile.TabStop = false;
            this.groupBoxInputFile.Text = "Исходный файл";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.HighlightText;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(6, 16);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(528, 248);
            this.textBox1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Location = new System.Drawing.Point(502, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(369, 270);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Лексический анализатор";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.HighlightText;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(6, 15);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(357, 249);
            this.textBox2.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.serviceGrid);
            this.groupBox2.Location = new System.Drawing.Point(13, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(150, 270);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Keywords";
            // 
            // serviceGrid
            // 
            this.serviceGrid.AllowUserToAddRows = false;
            this.serviceGrid.AllowUserToDeleteRows = false;
            this.serviceGrid.AllowUserToResizeColumns = false;
            this.serviceGrid.AllowUserToResizeRows = false;
            this.serviceGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.serviceGrid.CausesValidation = false;
            this.serviceGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.serviceGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.serviceGrid.ColumnHeadersVisible = false;
            this.serviceGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col0,
            this.col1});
            this.serviceGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serviceGrid.Location = new System.Drawing.Point(3, 22);
            this.serviceGrid.MultiSelect = false;
            this.serviceGrid.Name = "serviceGrid";
            this.serviceGrid.ReadOnly = true;
            this.serviceGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.serviceGrid.RowHeadersVisible = false;
            this.serviceGrid.RowHeadersWidth = 62;
            this.serviceGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.serviceGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.serviceGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.serviceGrid.Size = new System.Drawing.Size(144, 245);
            this.serviceGrid.TabIndex = 0;
            this.serviceGrid.TabStop = false;
            // 
            // col0
            // 
            this.col0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col0.HeaderText = "№";
            this.col0.MinimumWidth = 50;
            this.col0.Name = "col0";
            this.col0.ReadOnly = true;
            this.col0.Width = 50;
            // 
            // col1
            // 
            this.col1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.col1.HeaderText = "Символ";
            this.col1.MinimumWidth = 8;
            this.col1.Name = "col1";
            this.col1.ReadOnly = true;
            this.col1.Width = 91;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.separatorsGrid);
            this.groupBox3.Location = new System.Drawing.Point(169, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(150, 270);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Delimiters";
            // 
            // separatorsGrid
            // 
            this.separatorsGrid.AllowUserToAddRows = false;
            this.separatorsGrid.AllowUserToDeleteRows = false;
            this.separatorsGrid.AllowUserToResizeColumns = false;
            this.separatorsGrid.AllowUserToResizeRows = false;
            this.separatorsGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.separatorsGrid.CausesValidation = false;
            this.separatorsGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.separatorsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.separatorsGrid.ColumnHeadersVisible = false;
            this.separatorsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.separatorsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.separatorsGrid.Location = new System.Drawing.Point(3, 22);
            this.separatorsGrid.MultiSelect = false;
            this.separatorsGrid.Name = "separatorsGrid";
            this.separatorsGrid.ReadOnly = true;
            this.separatorsGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.separatorsGrid.RowHeadersVisible = false;
            this.separatorsGrid.RowHeadersWidth = 62;
            this.separatorsGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.separatorsGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.separatorsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.separatorsGrid.Size = new System.Drawing.Size(144, 245);
            this.separatorsGrid.TabIndex = 1;
            this.separatorsGrid.TabStop = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn1.HeaderText = "№";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 50;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 50;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn2.HeaderText = "Символ";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 91;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.constantsGrid);
            this.groupBox4.Location = new System.Drawing.Point(13, 285);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(150, 270);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Constants";
            // 
            // constantsGrid
            // 
            this.constantsGrid.AllowUserToAddRows = false;
            this.constantsGrid.AllowUserToDeleteRows = false;
            this.constantsGrid.AllowUserToResizeColumns = false;
            this.constantsGrid.AllowUserToResizeRows = false;
            this.constantsGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.constantsGrid.CausesValidation = false;
            this.constantsGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.constantsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.constantsGrid.ColumnHeadersVisible = false;
            this.constantsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.constantsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.constantsGrid.Location = new System.Drawing.Point(3, 22);
            this.constantsGrid.MultiSelect = false;
            this.constantsGrid.Name = "constantsGrid";
            this.constantsGrid.ReadOnly = true;
            this.constantsGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.constantsGrid.RowHeadersVisible = false;
            this.constantsGrid.RowHeadersWidth = 62;
            this.constantsGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.constantsGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.constantsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.constantsGrid.Size = new System.Drawing.Size(144, 245);
            this.constantsGrid.TabIndex = 1;
            this.constantsGrid.TabStop = false;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn3.HeaderText = "№";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 50;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 50;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn4.HeaderText = "Символ";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 91;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.indentGrid);
            this.groupBox5.Location = new System.Drawing.Point(169, 285);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(150, 270);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Identifiers";
            // 
            // indentGrid
            // 
            this.indentGrid.AllowUserToAddRows = false;
            this.indentGrid.AllowUserToDeleteRows = false;
            this.indentGrid.AllowUserToResizeColumns = false;
            this.indentGrid.AllowUserToResizeRows = false;
            this.indentGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.indentGrid.CausesValidation = false;
            this.indentGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.indentGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.indentGrid.ColumnHeadersVisible = false;
            this.indentGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.indentGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indentGrid.Location = new System.Drawing.Point(3, 22);
            this.indentGrid.MultiSelect = false;
            this.indentGrid.Name = "indentGrid";
            this.indentGrid.ReadOnly = true;
            this.indentGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.indentGrid.RowHeadersVisible = false;
            this.indentGrid.RowHeadersWidth = 62;
            this.indentGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.indentGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.indentGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.indentGrid.Size = new System.Drawing.Size(144, 245);
            this.indentGrid.TabIndex = 1;
            this.indentGrid.TabStop = false;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn5.HeaderText = "№";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 50;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 50;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn6.HeaderText = "Символ";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 91;
            // 
            // statusBar
            // 
            this.statusBar.Controls.Add(this.statusLabel);
            this.statusBar.Location = new System.Drawing.Point(13, 561);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1233, 73);
            this.statusBar.TabIndex = 8;
            this.statusBar.TabStop = false;
            this.statusBar.Text = "Статус программы";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statusLabel.Location = new System.Drawing.Point(6, 33);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 20);
            this.statusLabel.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.polizTextBox);
            this.groupBox6.Location = new System.Drawing.Point(877, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(369, 270);
            this.groupBox6.TabIndex = 9;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "ПОЛИЗ";
            // 
            // polizTextBox
            // 
            this.polizTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
            this.polizTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.polizTextBox.Location = new System.Drawing.Point(6, 16);
            this.polizTextBox.Multiline = true;
            this.polizTextBox.Name = "polizTextBox";
            this.polizTextBox.Size = new System.Drawing.Size(358, 248);
            this.polizTextBox.TabIndex = 1;
            this.polizTextBox.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(18, 645);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(277, 30);
            this.button1.TabIndex = 10;
            this.button1.Text = "Открыть файл";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.SelectFileToolStripMenuItem_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(325, 645);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(259, 30);
            this.button2.TabIndex = 11;
            this.button2.Text = "ЛА";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.StartLexAnalyzer);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(632, 645);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(259, 30);
            this.button3.TabIndex = 12;
            this.button3.Text = "ПОЛИЗ";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.StartSyntaxClick);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dataOperators);
            this.groupBox7.Location = new System.Drawing.Point(331, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(150, 270);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Operators";
            // 
            // dataOperators
            // 
            this.dataOperators.AllowUserToAddRows = false;
            this.dataOperators.AllowUserToDeleteRows = false;
            this.dataOperators.AllowUserToResizeColumns = false;
            this.dataOperators.AllowUserToResizeRows = false;
            this.dataOperators.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataOperators.CausesValidation = false;
            this.dataOperators.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataOperators.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataOperators.ColumnHeadersVisible = false;
            this.dataOperators.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8});
            this.dataOperators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataOperators.Location = new System.Drawing.Point(3, 22);
            this.dataOperators.MultiSelect = false;
            this.dataOperators.Name = "dataOperators";
            this.dataOperators.ReadOnly = true;
            this.dataOperators.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataOperators.RowHeadersVisible = false;
            this.dataOperators.RowHeadersWidth = 62;
            this.dataOperators.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataOperators.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataOperators.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataOperators.Size = new System.Drawing.Size(144, 245);
            this.dataOperators.TabIndex = 1;
            this.dataOperators.TabStop = false;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn7.HeaderText = "№";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 50;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 50;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn8.HeaderText = "Символ";
            this.dataGridViewTextBoxColumn8.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            this.dataGridViewTextBoxColumn8.Width = 91;
            // 
            // CsharpCode
            // 
            this.CsharpCode.Controls.Add(this.codeGenerators);
            this.CsharpCode.Location = new System.Drawing.Point(877, 285);
            this.CsharpCode.Name = "CsharpCode";
            this.CsharpCode.Size = new System.Drawing.Size(369, 270);
            this.CsharpCode.TabIndex = 10;
            this.CsharpCode.TabStop = false;
            this.CsharpCode.Text = "Сгенерированный C#";
            // 
            // codeGenerators
            // 
            this.codeGenerators.BackColor = System.Drawing.SystemColors.HighlightText;
            this.codeGenerators.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.codeGenerators.Location = new System.Drawing.Point(6, 22);
            this.codeGenerators.Multiline = true;
            this.codeGenerators.Name = "codeGenerators";
            this.codeGenerators.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.codeGenerators.Size = new System.Drawing.Size(358, 239);
            this.codeGenerators.TabIndex = 1;
            // 
            // compiler
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(1258, 664);
            this.Controls.Add(this.CsharpCode);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxInputFile);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1280, 720);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(930, 720);
            this.Name = "compiler";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Транслятор программных языков рекурсивным спуском";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.lexicalAnalyzerForm_Closed);
            this.groupBoxInputFile.ResumeLayout(false);
            this.groupBoxInputFile.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.serviceGrid)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.separatorsGrid)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.constantsGrid)).EndInit();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.indentGrid)).EndInit();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataOperators)).EndInit();
            this.CsharpCode.ResumeLayout(false);
            this.CsharpCode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxInputFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox statusBar;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.DataGridView serviceGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn col0;
        private System.Windows.Forms.DataGridViewTextBoxColumn col1;
        private System.Windows.Forms.DataGridView separatorsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridView constantsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridView indentGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox polizTextBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.DataGridView dataOperators;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.GroupBox CsharpCode;
        private System.Windows.Forms.TextBox codeGenerators;
    }
}