namespace CGMParser
{
    partial class Form1
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
            System.Windows.Forms.OpenFileDialog openFileDialog1;
            this.buttonOpen = new System.Windows.Forms.Button();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.numericUpDownScalePercent = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScalePercent)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(6, 2);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(152, 27);
            this.buttonOpen.TabIndex = 0;
            this.buttonOpen.Text = "Open CGM file";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "*.cgm";
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "CGM files (*.cgm)|*.cgm";
            openFileDialog1.Title = "Open CGM files";
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.Location = new System.Drawing.Point(165, 2);
            this.textBoxFilename.Name = "textBoxFilename";
            this.textBoxFilename.Size = new System.Drawing.Size(431, 26);
            this.textBoxFilename.TabIndex = 1;
            // 
            // numericUpDownScalePercent
            // 
            this.numericUpDownScalePercent.Location = new System.Drawing.Point(693, 3);
            this.numericUpDownScalePercent.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownScalePercent.Name = "numericUpDownScalePercent";
            this.numericUpDownScalePercent.Size = new System.Drawing.Size(74, 26);
            this.numericUpDownScalePercent.TabIndex = 2;
            this.numericUpDownScalePercent.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownScalePercent.ValueChanged += new System.EventHandler(this.numericUpDownScalePercent_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(638, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Scale";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(773, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "%";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1185, 808);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownScalePercent);
            this.Controls.Add(this.textBoxFilename);
            this.Controls.Add(this.buttonOpen);
            this.Name = "Form1";
            this.Text = "CGM parser - made by Myroslav Kravchuk as test work at 15.04.2020 - reads only Po" +
    "lyline";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScalePercent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.TextBox textBoxFilename;
        private System.Windows.Forms.NumericUpDown numericUpDownScalePercent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

