using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8
{
	internal class FileRead
	{

		public FileRead() {


			Console.WriteLine("Test Read");
			Console.ReadLine();


			// big endian is left to right, which is what chip8 uses
			// little endian is right to left 
			//chip 8 reads 2 bytes in at a time

			string filePath = "G:/Projects/Chip8/IBM_Logo.ch8";

			byte[] ram = File.ReadAllBytes(filePath);


			for (int i = 0; i < ram.Length; i += 2)
			{
				
				if (i + 1 < ram.Length)
				{
					byte[] chunk = { ram[i], ram[i + 1] };

					// Convert it to Big Endian if its in little endian, should fix read bug
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(chunk);
					}

					int value = BitConverter.ToUInt16(chunk, 0);
					string hexString = value.ToString("X4"); // "X4" for 4 digits
					Console.WriteLine(hexString);
					Console.ReadLine();
				}
			}



			/*using (
				
				FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				byte[] buffer = new byte[2];

				while (fs.Read(buffer, 0, buffer.Length) == buffer.Length)
				{
				
					int value = BitConverter.ToUInt16(buffer, 0);
					string hexString = value.ToString("X4"); // "X4" for 4 digits

					Console.WriteLine(hexString);
					Console.ReadLine();
				}
			}
			*/
		}

	}
}
