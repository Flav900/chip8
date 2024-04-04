using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8
{
	internal class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Console.WriteLine("Chip 8 Interpreter by Flav900.");
			Console.WriteLine("Follows the customary left side of the QWERTY controls");
			Console.WriteLine("1,2,3,4,Q,W,E,R,A,S,D,F,Z,X,C");

			//new Chip8();

			// new MainForm().ShowDialog();
			//  new FileRead();

			//  new TextOnly();


			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

	}
}
