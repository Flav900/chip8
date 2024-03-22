using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8
{
	internal class TextOnly
	{

	Chip8Core chip8 = new Chip8Core();

	public TextOnly()
		{
			chip8.init();

			for (; ; ) {
				chip8.doCycle();
			}


		}


	}
}
