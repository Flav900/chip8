using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8
{
	internal class Chip8Core
	{
		//ushort = unsigned short


		ushort opCode;
		ushort index;
		ushort programCounter = 0;

		byte[] memory = new byte[4096];

		byte[] cpuRegisters = new byte[16];//V

		ushort[] stack = new ushort[16];
		byte stackPosition;


		//	char[] display = new char[64 * 32]; //or 128 * 64

		ushort[,] display = new ushort[64,32]; //1 or 0, black or white



		public void init()
		{
			readRom("G:/Projects/Chip8/IBM_Logo.ch8");

		}

		public void doCycle()
		{
			//Fetch
			byte[] chunk = { memory[programCounter], memory[programCounter+1] };


			// Convert it to Big Endian if its in little endian, should fix read bug
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(chunk);
			}

			int value = BitConverter.ToUInt16(chunk, 0);
			string hexString = value.ToString("X4"); // "X4" for 4 digits

			Console.WriteLine(hexString);
		

			//Decode

			switch(hexString)
			{
				case "00E0":
					Console.WriteLine("Clearing Screen");

					//Execute

					break;
			}


			Console.ReadLine();


		}


		void readRom(string filePath)
		{

			memory = File.ReadAllBytes(filePath);

		}

	}
}
