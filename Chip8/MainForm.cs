using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8
{
	public partial class MainForm : Form
	{
		string filename;
		
		Chip8 chip;

		public MainForm()
		{
			
			InitializeComponent();

			chip = new Chip8();


		}

		private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{

			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				 InitialDirectory = "C:\\Projects\\Chip8", //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
				//InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				Title = "Select a file",
				Filter = "All files (*.*)|*.*",
				RestoreDirectory = true
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{

				chip.Stop();

				filename = openFileDialog.FileName;
				//Console.WriteLine("Loading " + filename + "...");
				chip.Start(filename);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			chip.Stop();
			Application.Exit();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void MainForm_Load(object sender, EventArgs e)
		{

		}

		private void ShowVersionInfo()
		{
			// Get the assembly version
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			string versionInfo = $"Version: {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
			MessageBox.Show(versionInfo, "Version Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			ShowVersionInfo();
		}
	}
}
