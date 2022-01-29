namespace CP_ParallelPipesForm
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.PipesGropup = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ZaText = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.I0Text = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.ro_gText = new System.Windows.Forms.TextBox();
            this.CalcButton = new System.Windows.Forms.Button();
            this.NameVar = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.AddPipeButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.NfiText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.LText = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.CalcMessage = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PipesGropup
            // 
            this.PipesGropup.Location = new System.Drawing.Point(452, 54);
            this.PipesGropup.Name = "PipesGropup";
            this.PipesGropup.Size = new System.Drawing.Size(434, 292);
            this.PipesGropup.TabIndex = 1;
            this.PipesGropup.TabStop = false;
            this.PipesGropup.Text = "Трубы";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ZaText);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.I0Text);
            this.groupBox1.Location = new System.Drawing.Point(12, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(434, 95);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Анод";
            // 
            // ZaText
            // 
            this.ZaText.Location = new System.Drawing.Point(279, 53);
            this.ZaText.Name = "ZaText";
            this.ZaText.Size = new System.Drawing.Size(140, 20);
            this.ZaText.TabIndex = 8;
            this.ZaText.Text = "2.5";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 56);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(230, 13);
            this.label11.TabIndex = 7;
            this.label11.Text = "Глубина от уровня земли до центр. точки, м";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Сила тока, А";
            // 
            // I0Text
            // 
            this.I0Text.Location = new System.Drawing.Point(279, 27);
            this.I0Text.Name = "I0Text";
            this.I0Text.Size = new System.Drawing.Size(140, 20);
            this.I0Text.TabIndex = 6;
            this.I0Text.Text = "0,3";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.ro_gText);
            this.groupBox2.Location = new System.Drawing.Point(12, 156);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(434, 66);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Грунт";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 30);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(166, 13);
            this.label13.TabIndex = 5;
            this.label13.Text = "Уд сопр грунта, Ом*м: 5..10000";
            // 
            // ro_gText
            // 
            this.ro_gText.Location = new System.Drawing.Point(279, 27);
            this.ro_gText.Name = "ro_gText";
            this.ro_gText.Size = new System.Drawing.Size(140, 20);
            this.ro_gText.TabIndex = 6;
            this.ro_gText.Text = "10000";
            // 
            // CalcButton
            // 
            this.CalcButton.Location = new System.Drawing.Point(379, 386);
            this.CalcButton.Name = "CalcButton";
            this.CalcButton.Size = new System.Drawing.Size(143, 54);
            this.CalcButton.TabIndex = 10;
            this.CalcButton.Text = "Вычислить";
            this.CalcButton.UseVisualStyleBackColor = true;
            this.CalcButton.Click += new System.EventHandler(this.CalcButton_Click);
            // 
            // NameVar
            // 
            this.NameVar.Location = new System.Drawing.Point(200, 34);
            this.NameVar.Name = "NameVar";
            this.NameVar.Size = new System.Drawing.Size(246, 20);
            this.NameVar.TabIndex = 7;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 37);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(132, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Название исследования";
            // 
            // AddPipeButton
            // 
            this.AddPipeButton.Location = new System.Drawing.Point(640, 352);
            this.AddPipeButton.Name = "AddPipeButton";
            this.AddPipeButton.Size = new System.Drawing.Size(111, 34);
            this.AddPipeButton.TabIndex = 15;
            this.AddPipeButton.Text = "Добавить трубу";
            this.AddPipeButton.UseVisualStyleBackColor = true;
            this.AddPipeButton.Click += new System.EventHandler(this.AddPipeButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.NfiText);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.LText);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(12, 237);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(434, 109);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Параметры труб";
            // 
            // NfiText
            // 
            this.NfiText.Location = new System.Drawing.Point(276, 60);
            this.NfiText.Name = "NfiText";
            this.NfiText.Size = new System.Drawing.Size(140, 20);
            this.NfiText.TabIndex = 3;
            this.NfiText.Text = "50";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(159, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Число фиктивных источников";
            // 
            // LText
            // 
            this.LText.Location = new System.Drawing.Point(276, 27);
            this.LText.Name = "LText";
            this.LText.Size = new System.Drawing.Size(140, 20);
            this.LText.TabIndex = 1;
            this.LText.Text = "1000";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Длина труб, м";
            // 
            // CalcMessage
            // 
            this.CalcMessage.AutoSize = true;
            this.CalcMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CalcMessage.Location = new System.Drawing.Point(319, 361);
            this.CalcMessage.Name = "CalcMessage";
            this.CalcMessage.Size = new System.Drawing.Size(280, 16);
            this.CalcMessage.TabIndex = 16;
            this.CalcMessage.Text = "Вычисляю, пожалуйста, подождите...";
            this.CalcMessage.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.WorkerProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.RunWorkerCompleted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(9, 396);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Руководитель: Болотнов А.М.";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.AboutMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(901, 24);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // AboutMenuItem
            // 
            this.AboutMenuItem.Name = "AboutMenuItem";
            this.AboutMenuItem.Size = new System.Drawing.Size(94, 20);
            this.AboutMenuItem.Text = "О программе";
            this.AboutMenuItem.Click += new System.EventHandler(this.AboutMenuItemClick);
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadResultsToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // LoadResultsToolStripMenuItem
            // 
            this.LoadResultsToolStripMenuItem.Name = "LoadResultsToolStripMenuItem";
            this.LoadResultsToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.LoadResultsToolStripMenuItem.Text = "Загрузить результаты вычислений...";
            this.LoadResultsToolStripMenuItem.Click += new System.EventHandler(this.LoadResultsMenuItemClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(9, 409);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(213, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Создатель программы: Алсынбаев Ф.С.";
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.VersionLabel.Location = new System.Drawing.Point(9, 422);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(127, 13);
            this.VersionLabel.TabIndex = 20;
            this.VersionLabel.Text = "Версия программы: 0.2";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 446);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CalcMessage);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.AddPipeButton);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.NameVar);
            this.Controls.Add(this.CalcButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.PipesGropup);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "ВКР Алсынбаев Ф. - Расчет электрических полей катодной защиты для параллельных тр" +
    "уб";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox PipesGropup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox I0Text;
        private System.Windows.Forms.TextBox ZaText;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox ro_gText;
        private System.Windows.Forms.Button CalcButton;
        private System.Windows.Forms.TextBox NameVar;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button AddPipeButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox NfiText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox LText;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label CalcMessage;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadResultsToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label VersionLabel;
    }
}

