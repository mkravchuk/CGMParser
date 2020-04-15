using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CGMParser
{
    public partial class Form1 : Form
    {
        CGM cgm;

        public Form1()
        {
            InitializeComponent();

            // read command line arguments - to make easier debugging
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                string filename = args[1];                
                if (!String.IsNullOrEmpty(filename) && File.Exists(filename) && Path.GetExtension(filename).ToLower() == ".cgm")
                {
                    Read(filename);
                }
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                //InitialDirectory = @"D:\",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "*.cgm",
                FileName = "",
                Filter = "CGM files (*.cgm)|*.cgm",
                Title = "Open CGM files",

                ReadOnlyChecked = true,
                ShowReadOnly = true,
            };  
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Read(textBoxFilename.Text);
            }

        }

        /// <summary>
        /// Read CGM file and parse it.
        /// Later parsed file will be drawn in form.
        /// </summary>
        /// <param name="filename"></param>
        private void Read(string filename)
        {
            textBoxFilename.Text = filename;
            cgm = new CGM();
            cgm.Read(filename);
        }

        /// <summary>
        /// CGM file is drawn every time form repaints
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (cgm == null) return;
            cgm.Draw(e, 30, 50, (float) numericUpDownScalePercent.Value / 100.0f);
        }

        private void numericUpDownScalePercent_ValueChanged(object sender, EventArgs e)
        {
            // force form to draw and thus redraw CGM file content
            Invalidate();
        }
    }
}
