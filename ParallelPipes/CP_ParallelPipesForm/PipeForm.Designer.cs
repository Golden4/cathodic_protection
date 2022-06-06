namespace CP_ParallelPipesForm
{
    partial class PipeForm
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
            this.label9 = new System.Windows.Forms.Label();
            this.DetText = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Dt2Text = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ro_tText = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LtaText = new System.Windows.Forms.TextBox();
            this.HtText = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.PipeNameText = new System.Windows.Forms.TextBox();
            this.sigmaInterval = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 214);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(228, 17);
            this.label9.TabIndex = 16;
            this.label9.Text = "Сопротивление изоляции, Ом*м2";
            // 
            // DetText
            // 
            this.DetText.Location = new System.Drawing.Point(364, 177);
            this.DetText.Margin = new System.Windows.Forms.Padding(4);
            this.DetText.Name = "DetText";
            this.DetText.Size = new System.Drawing.Size(185, 22);
            this.DetText.TabIndex = 15;
            this.DetText.Text = "0.025";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 182);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(214, 17);
            this.label8.TabIndex = 14;
            this.label8.Text = "Толщина стенки, м: 0.01..0.025";
            // 
            // Dt2Text
            // 
            this.Dt2Text.Location = new System.Drawing.Point(364, 145);
            this.Dt2Text.Margin = new System.Windows.Forms.Padding(4);
            this.Dt2Text.Name = "Dt2Text";
            this.Dt2Text.Size = new System.Drawing.Size(185, 22);
            this.Dt2Text.TabIndex = 13;
            this.Dt2Text.Text = "1.22";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 149);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(201, 17);
            this.label7.TabIndex = 12;
            this.label7.Text = "Внешний диаметр, м: 0.3..1.5";
            // 
            // ro_tText
            // 
            this.ro_tText.Location = new System.Drawing.Point(364, 113);
            this.ro_tText.Margin = new System.Windows.Forms.Padding(4);
            this.ro_tText.Name = "ro_tText";
            this.ro_tText.Size = new System.Drawing.Size(185, 22);
            this.ro_tText.TabIndex = 11;
            this.ro_tText.Text = "2.45e-7";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 117);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(219, 17);
            this.label6.TabIndex = 10;
            this.label6.Text = "Удельное сопротивление, Ом*м";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 86);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(274, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "Расстояние до анода по горизонтали, м";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 50);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(294, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Глубина от уровня земли до верхней т.Т, м";
            // 
            // LtaText
            // 
            this.LtaText.Location = new System.Drawing.Point(364, 81);
            this.LtaText.Margin = new System.Windows.Forms.Padding(4);
            this.LtaText.Name = "LtaText";
            this.LtaText.Size = new System.Drawing.Size(185, 22);
            this.LtaText.TabIndex = 7;
            this.LtaText.Text = "50";
            // 
            // HtText
            // 
            this.HtText.Location = new System.Drawing.Point(364, 47);
            this.HtText.Margin = new System.Windows.Forms.Padding(4);
            this.HtText.Name = "HtText";
            this.HtText.Size = new System.Drawing.Size(185, 22);
            this.HtText.TabIndex = 5;
            this.HtText.Text = "2";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(195, 257);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(4);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(173, 48);
            this.SaveButton.TabIndex = 6;
            this.SaveButton.Text = "Сохранить";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 17);
            this.label1.TabIndex = 19;
            this.label1.Text = "Название трубы";
            // 
            // PipeNameText
            // 
            this.PipeNameText.Location = new System.Drawing.Point(364, 15);
            this.PipeNameText.Margin = new System.Windows.Forms.Padding(4);
            this.PipeNameText.Name = "PipeNameText";
            this.PipeNameText.Size = new System.Drawing.Size(185, 22);
            this.PipeNameText.TabIndex = 18;
            this.PipeNameText.Text = "Труба 1";
            // 
            // sigmaInterval
            // 
            this.sigmaInterval.Location = new System.Drawing.Point(364, 206);
            this.sigmaInterval.Name = "sigmaInterval";
            this.sigmaInterval.Size = new System.Drawing.Size(100, 42);
            this.sigmaInterval.TabIndex = 20;
            this.sigmaInterval.Text = "Задать";
            this.sigmaInterval.UseVisualStyleBackColor = true;
            this.sigmaInterval.Click += new System.EventHandler(this.sigmaInterval_Click);
            // 
            // PipeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 320);
            this.Controls.Add(this.sigmaInterval);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PipeNameText);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.DetText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.HtText);
            this.Controls.Add(this.Dt2Text);
            this.Controls.Add(this.LtaText);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ro_tText);
            this.Controls.Add(this.label6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PipeForm";
            this.Text = "Изменение параметров трубы";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button sigmaInterval;

        #endregion

        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox DetText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox Dt2Text;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ro_tText;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox LtaText;
        private System.Windows.Forms.TextBox HtText;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PipeNameText;
    }
}