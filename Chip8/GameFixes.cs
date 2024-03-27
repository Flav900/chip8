using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8
{
	class GameFixes
	{

		public static void gameFix(Chip8Core chip8, string filename)
		{
			filename = Path.GetFileName(filename);

			switch(filename)
			{
				case "8ceattourny_d1.ch8":
				case "8ceattourny_d2.ch8":
				case "8ceattourny_d3.ch8":
					chip8.OldChip8Behaviour = true;
					break;
				default:
					//no fixes needed
					break;
			}

		}
	}
}
