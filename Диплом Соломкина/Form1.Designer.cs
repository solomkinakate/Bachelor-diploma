namespace RestrictedGraph
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonDoTask = new System.Windows.Forms.Button();
            this.inputFileButton = new System.Windows.Forms.Button();
            this.outputFileButton = new System.Windows.Forms.Button();
            this.groupBoxRestriction = new System.Windows.Forms.GroupBox();
            this.radioButtonVentil = new System.Windows.Forms.RadioButton();
            this.radioButtonBarrier = new System.Windows.Forms.RadioButton();
            this.groupBoxTask = new System.Windows.Forms.GroupBox();
            this.RadioButtonRandomWalk = new System.Windows.Forms.RadioButton();
            this.radioButtonShortestPath = new System.Windows.Forms.RadioButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.radioButtonShluz = new System.Windows.Forms.RadioButton();
            this.groupBoxRestriction.SuspendLayout();
            this.groupBoxTask.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonDoTask
            // 
            this.buttonDoTask.Location = new System.Drawing.Point(347, 125);
            this.buttonDoTask.Name = "buttonDoTask";
            this.buttonDoTask.Size = new System.Drawing.Size(102, 23);
            this.buttonDoTask.TabIndex = 4;
            this.buttonDoTask.Text = "Go";
            this.buttonDoTask.UseVisualStyleBackColor = true;
            this.buttonDoTask.Click += new System.EventHandler(this.Go);
            // 
            // inputFileButton
            // 
            this.inputFileButton.Location = new System.Drawing.Point(12, 125);
            this.inputFileButton.Name = "inputFileButton";
            this.inputFileButton.Size = new System.Drawing.Size(150, 23);
            this.inputFileButton.TabIndex = 11;
            this.inputFileButton.Text = "Выбрать файл ввода";
            this.inputFileButton.UseVisualStyleBackColor = true;
            this.inputFileButton.Click += new System.EventHandler(this.inputFileButton_Click);
            // 
            // outputFileButton
            // 
            this.outputFileButton.Location = new System.Drawing.Point(168, 125);
            this.outputFileButton.Name = "outputFileButton";
            this.outputFileButton.Size = new System.Drawing.Size(150, 23);
            this.outputFileButton.TabIndex = 12;
            this.outputFileButton.Text = "Выбрать файл вывода";
            this.outputFileButton.UseVisualStyleBackColor = true;
            this.outputFileButton.Click += new System.EventHandler(this.outputFileButton_Click);
            // 
            // groupBoxRestriction
            // 
            this.groupBoxRestriction.Controls.Add(this.radioButtonShluz);
            this.groupBoxRestriction.Controls.Add(this.radioButtonVentil);
            this.groupBoxRestriction.Controls.Add(this.radioButtonBarrier);
            this.groupBoxRestriction.Location = new System.Drawing.Point(29, 13);
            this.groupBoxRestriction.Name = "groupBoxRestriction";
            this.groupBoxRestriction.Size = new System.Drawing.Size(200, 100);
            this.groupBoxRestriction.TabIndex = 13;
            this.groupBoxRestriction.TabStop = false;
            this.groupBoxRestriction.Text = "Ограничение достижимости";
            // 
            // radioButtonVentil
            // 
            this.radioButtonVentil.AutoSize = true;
            this.radioButtonVentil.Location = new System.Drawing.Point(3, 39);
            this.radioButtonVentil.Name = "radioButtonVentil";
            this.radioButtonVentil.Size = new System.Drawing.Size(85, 17);
            this.radioButtonVentil.TabIndex = 1;
            this.radioButtonVentil.TabStop = true;
            this.radioButtonVentil.Text = "Вентильная";
            this.radioButtonVentil.UseVisualStyleBackColor = true;
            // 
            // radioButtonBarrier
            // 
            this.radioButtonBarrier.AutoSize = true;
            this.radioButtonBarrier.Location = new System.Drawing.Point(3, 16);
            this.radioButtonBarrier.Name = "radioButtonBarrier";
            this.radioButtonBarrier.Size = new System.Drawing.Size(80, 17);
            this.radioButtonBarrier.TabIndex = 0;
            this.radioButtonBarrier.TabStop = true;
            this.radioButtonBarrier.Text = "Барьерная";
            this.radioButtonBarrier.UseVisualStyleBackColor = true;
            // 
            // groupBoxTask
            // 
            this.groupBoxTask.Controls.Add(this.RadioButtonRandomWalk);
            this.groupBoxTask.Controls.Add(this.radioButtonShortestPath);
            this.groupBoxTask.Location = new System.Drawing.Point(249, 17);
            this.groupBoxTask.Name = "groupBoxTask";
            this.groupBoxTask.Size = new System.Drawing.Size(200, 100);
            this.groupBoxTask.TabIndex = 14;
            this.groupBoxTask.TabStop = false;
            this.groupBoxTask.Text = "Задача";
            // 
            // RadioButtonRandomWalk
            // 
            this.RadioButtonRandomWalk.AutoSize = true;
            this.RadioButtonRandomWalk.Location = new System.Drawing.Point(3, 39);
            this.RadioButtonRandomWalk.Name = "RadioButtonRandomWalk";
            this.RadioButtonRandomWalk.Size = new System.Drawing.Size(138, 17);
            this.RadioButtonRandomWalk.TabIndex = 1;
            this.RadioButtonRandomWalk.TabStop = true;
            this.RadioButtonRandomWalk.Text = "Случайные блуждания";
            this.RadioButtonRandomWalk.UseVisualStyleBackColor = true;
            // 
            // radioButtonShortestPath
            // 
            this.radioButtonShortestPath.AutoSize = true;
            this.radioButtonShortestPath.Location = new System.Drawing.Point(3, 16);
            this.radioButtonShortestPath.Name = "radioButtonShortestPath";
            this.radioButtonShortestPath.Size = new System.Drawing.Size(111, 17);
            this.radioButtonShortestPath.TabIndex = 0;
            this.radioButtonShortestPath.TabStop = true;
            this.radioButtonShortestPath.Text = "Кратчайший путь";
            this.radioButtonShortestPath.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialogIn";
            // 
            // radioButtonShluz
            // 
            this.radioButtonShluz.AutoSize = true;
            this.radioButtonShluz.Location = new System.Drawing.Point(3, 62);
            this.radioButtonShluz.Name = "radioButtonShluz";
            this.radioButtonShluz.Size = new System.Drawing.Size(78, 17);
            this.radioButtonShluz.TabIndex = 2;
            this.radioButtonShluz.TabStop = true;
            this.radioButtonShluz.Text = "Шлюзовая";
            this.radioButtonShluz.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 165);
            this.Controls.Add(this.groupBoxTask);
            this.Controls.Add(this.groupBoxRestriction);
            this.Controls.Add(this.outputFileButton);
            this.Controls.Add(this.inputFileButton);
            this.Controls.Add(this.buttonDoTask);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Графы с нестандартной достижимостью";
            this.groupBoxRestriction.ResumeLayout(false);
            this.groupBoxRestriction.PerformLayout();
            this.groupBoxTask.ResumeLayout(false);
            this.groupBoxTask.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonDoTask;
        private System.Windows.Forms.Button inputFileButton;
        private System.Windows.Forms.Button outputFileButton;
        private System.Windows.Forms.GroupBox groupBoxRestriction;
        private System.Windows.Forms.GroupBox groupBoxTask;
        private System.Windows.Forms.RadioButton radioButtonVentil;
        private System.Windows.Forms.RadioButton radioButtonBarrier;
        private System.Windows.Forms.RadioButton radioButtonShortestPath;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.RadioButton RadioButtonRandomWalk;
        private System.Windows.Forms.RadioButton radioButtonShluz;
    }
}

