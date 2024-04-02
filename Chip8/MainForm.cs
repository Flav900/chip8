using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8
{
	public partial class MainForm : Form
	{
		string filename;

		private SDLPanel sdlPanel;

		public MainForm()
		{
			

			sdlPanel = new SDLPanel
			{
				Dock = DockStyle.Fill
			};
			Controls.Add(sdlPanel);

			InitializeComponent();


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

				sdlPanel.Stop();

				filename = openFileDialog.FileName;
				//Console.WriteLine("Loading " + filename + "...");
				sdlPanel.Start(filename);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}
	}
}
