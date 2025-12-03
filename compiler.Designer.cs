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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.selectFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startLexicalAnalyzerButton = new System.Windows.Forms.ToolStripMenuItem();
            this.startSyntax = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.menuStrip1.SuspendLayout();
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
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectFileToolStripMenuItem,
            this.startLexicalAnalyzerButton,
            this.startSyntax,
            this.выходToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(914, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // selectFileToolStripMenuItem
            // 
            this.selectFileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.selectFileToolStripMenuItem.Name = "selectFileToolStripMenuItem";
            this.selectFileToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.selectFileToolStripMenuItem.Text = "Открыть файл";
            this.selectFileToolStripMenuItem.Click += new System.EventHandler(this.SelectFileToolStripMenuItem_Click);
            // 
            // startLexicalAnalyzerButton
            // 
            this.startLexicalAnalyzerButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.startLexicalAnalyzerButton.Name = "startLexicalAnalyzerButton";
            this.startLexicalAnalyzerButton.Size = new System.Drawing.Size(35, 20);
            this.startLexicalAnalyzerButton.Text = "ЛА";
            this.startLexicalAnalyzerButton.Click += new System.EventHandler(this.StartLexAnalyzer);
            // 
            // startSyntax
            // 
            this.startSyntax.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.startSyntax.Name = "startSyntax";
            this.startSyntax.Size = new System.Drawing.Size(61, 20);
            this.startSyntax.Text = "ПОЛИЗ";
            this.startSyntax.Click += new System.EventHandler(this.StartSyntaxClick);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.exitApp);
            // 
            // groupBoxInputFile
            // 
            this.groupBoxInputFile.Controls.Add(this.textBox1);
            this.groupBoxInputFile.Location = new System.Drawing.Point(324, 300);
            this.groupBoxInputFile.Name = "groupBoxInputFile";
            this.groupBoxInputFile.Size = new System.Drawing.Size(369, 270);
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
            this.textBox1.Size = new System.Drawing.Size(357, 248);
            this.textBox1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Location = new System.Drawing.Point(324, 27);
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
            this.groupBox2.Location = new System.Drawing.Point(12, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(150, 270);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Служебные слова";
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
            this.serviceGrid.Location = new System.Drawing.Point(3, 16);
            this.serviceGrid.MultiSelect = false;
            this.serviceGrid.Name = "serviceGrid";
            this.serviceGrid.ReadOnly = true;
            this.serviceGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.serviceGrid.RowHeadersVisible = false;
            this.serviceGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.serviceGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.serviceGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.serviceGrid.Size = new System.Drawing.Size(144, 251);
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
            this.col1.Name = "col1";
            this.col1.ReadOnly = true;
            this.col1.Width = 91;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.separatorsGrid);
            this.groupBox3.Location = new System.Drawing.Point(168, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(150, 270);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Разделители";
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
            this.separatorsGrid.Location = new System.Drawing.Point(3, 16);
            this.separatorsGrid.MultiSelect = false;
            this.separatorsGrid.Name = "separatorsGrid";
            this.separatorsGrid.ReadOnly = true;
            this.separatorsGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.separatorsGrid.RowHeadersVisible = false;
            this.separatorsGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.separatorsGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.separatorsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.separatorsGrid.Size = new System.Drawing.Size(144, 251);
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
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 91;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.constantsGrid);
            this.groupBox4.Location = new System.Drawing.Point(12, 300);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(150, 270);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Константы";
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
            this.constantsGrid.Location = new System.Drawing.Point(3, 16);
            this.constantsGrid.MultiSelect = false;
            this.constantsGrid.Name = "constantsGrid";
            this.constantsGrid.ReadOnly = true;
            this.constantsGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.constantsGrid.RowHeadersVisible = false;
            this.constantsGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.constantsGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.constantsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.constantsGrid.Size = new System.Drawing.Size(144, 251);
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
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 91;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.indentGrid);
            this.groupBox5.Location = new System.Drawing.Point(168, 300);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(150, 270);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Идентификаторы";
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
            this.indentGrid.Location = new System.Drawing.Point(3, 16);
            this.indentGrid.MultiSelect = false;
            this.indentGrid.Name = "indentGrid";
            this.indentGrid.ReadOnly = true;
            this.indentGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.indentGrid.RowHeadersVisible = false;
            this.indentGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.indentGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.indentGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.indentGrid.Size = new System.Drawing.Size(144, 251);
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
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 91;
            // 
            // statusBar
            // 
            this.statusBar.Controls.Add(this.statusLabel);
            this.statusBar.Location = new System.Drawing.Point(12, 576);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(884, 73);
            this.statusBar.TabIndex = 8;
            this.statusBar.TabStop = false;
            this.statusBar.Text = "Вывод программы";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statusLabel.Location = new System.Drawing.Point(6, 33);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.polizTextBox);
            this.groupBox6.Location = new System.Drawing.Point(699, 27);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(197, 543);
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
            this.polizTextBox.Size = new System.Drawing.Size(185, 521);
            this.polizTextBox.TabIndex = 1;
            // 
            // compiler
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(914, 661);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxInputFile);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1280, 700);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(930, 700);
            this.Name = "compiler";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Транслятор программных языков рекурсивным спуском";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.lexicalAnalyzerForm_Closed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectFileToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxInputFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ToolStripMenuItem startLexicalAnalyzerButton;
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
        private System.Windows.Forms.ToolStripMenuItem startSyntax;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox polizTextBox;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
    }
}