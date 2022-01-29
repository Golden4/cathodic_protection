namespace CP_ParallelPipesForm
{
    partial class GraphicForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.SaveGraphButton = new System.Windows.Forms.Button();
            this.CompireGraphButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(783, 462);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // SaveGraphButton
            // 
            this.SaveGraphButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.SaveGraphButton.Location = new System.Drawing.Point(267, 480);
            this.SaveGraphButton.Name = "SaveGraphButton";
            this.SaveGraphButton.Size = new System.Drawing.Size(145, 41);
            this.SaveGraphButton.TabIndex = 1;
            this.SaveGraphButton.Text = "Сохранить график";
            this.SaveGraphButton.UseVisualStyleBackColor = true;
            this.SaveGraphButton.Click += new System.EventHandler(this.SaveGraphButton_Click);
            // 
            // CompireGraphButton
            // 
            this.CompireGraphButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.CompireGraphButton.Location = new System.Drawing.Point(418, 480);
            this.CompireGraphButton.Name = "CompireGraphButton";
            this.CompireGraphButton.Size = new System.Drawing.Size(145, 41);
            this.CompireGraphButton.TabIndex = 2;
            this.CompireGraphButton.Text = "Сравнить графики";
            this.CompireGraphButton.UseVisualStyleBackColor = true;
            this.CompireGraphButton.Click += new System.EventHandler(this.CompireGraphButton_Click);
            // 
            // GraphicForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 533);
            this.Controls.Add(this.CompireGraphButton);
            this.Controls.Add(this.SaveGraphButton);
            this.Controls.Add(this.chart1);
            this.Name = "GraphicForm";
            this.Text = "GraphicForm";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button SaveGraphButton;
        private System.Windows.Forms.Button CompireGraphButton;
    }
}