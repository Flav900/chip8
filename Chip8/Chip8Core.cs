using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chip8
{
	internal class Chip8Core
	{
		//ushort = unsigned short


	//	ushort opCode;
		ushort index;
		ushort programCounter = 512;

		byte[] memory = new byte[4096];

		byte[] cpuRegisters = new byte[16];//V

		ushort[] stack = new ushort[16];

		byte stackPosition = 0;


		//ushort[] display = new ushort[64 * 32]; //or 128 * 64

	    char[,] display = new char[64,32]; //1 or 0, black or white

		private bool canDraw = false;

		public bool CanDraw { get; set; }



		// Font data
		byte[] fontData = new byte[] {
			0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
			0x20, 0x60, 0x20, 0x20, 0x70, // 1
			0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
			0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
			0x90, 0x90, 0xF0, 0x10, 0x10, // 4
			0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
			0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
			0xF0, 0x10, 0x20, 0x40, 0x40, // 7
			0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
			0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
			0xF0, 0x90, 0xF0, 0x90, 0x90, // A
			0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
			0xF0, 0x80, 0x80, 0x80, 0xF0, // C
			0xE0, 0x90, 0x90, 0x90, 0xE0, // D
			0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
			0xF0, 0x80, 0xF0, 0x80, 0x80  // F
		};



		public void init()
		{
			// I need to add fonts and other functions in the first 512
			//512 onwards where I'm supposed to load rom

			//index 80 (hex 50) 0x050

			Array.Copy(fontData, 0, memory, 80, fontData.Length);


			readRom("C:/Projects/Chip8/IBM_Logo.ch8");

		}


		//Regex is nice but I think its too complex for this
		//Regex setIndexRegisterRegex = new Regex(@"^A[0-9A-F]{3}$", RegexOptions.IgnoreCase);
		//	else if (setIndexRegisterRegex.IsMatch(hexString)) {

		public void doCycle()
		{

			//Console.WriteLine("PC: " + programCounter);
			try
			{
				//Fetch
				byte[] chunk = { memory[programCounter], memory[programCounter + 1] };

				programCounter += 2; //less error prone to put it here at the top


				// Convert it to Big Endian if its in little endian, should fix read bug
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(chunk);
				}

				string opCode = BitConverter.ToUInt16(chunk, 0).ToString("X4"); // "X4" for 4 digits

				Console.WriteLine(opCode);


				//Decode

				if (opCode.Equals("00E0"))
				{
					Console.WriteLine("Clearing Screen");
					clearScreen();
				}
				else if (opCode.StartsWith("A"))
				{


					string nnn = opCode.Substring(1);

					Console.WriteLine("Set Index to " + nnn);

					index = (ushort)Convert.ToInt32(nnn, 16);

				}
				else if (opCode.StartsWith("1"))
				{
					//Set the PC to nnn
					string nnn = opCode.Substring(1);

					Console.WriteLine("Set the Program Counter to " + nnn);

					programCounter = (ushort)Convert.ToInt32(nnn, 16);
				

				}
				else if (opCode.StartsWith("6"))
				{
					int x = Int32.Parse(opCode.Substring(1, 1));

					string nn = opCode.Substring(2);

					Console.WriteLine("Set Cpu Register " + x + " to " + nn);

					//set register VX
					cpuRegisters[x] = Convert.ToByte(nn, 16);


				}
				else if (opCode.StartsWith("7"))
				{
					//add value to register VX
					int x = Int32.Parse(opCode.Substring(1, 1));

					string nn = opCode.Substring(2);


					Console.WriteLine("Add " + nn + " to Cpu Register " + x);

					int sum = cpuRegisters[x] + Convert.ToByte(nn, 16);

					Console.WriteLine("sum " + sum);

					//need to check what happens when overflow is detected
					if (sum > 255)
					{

						Console.WriteLine("Overflow");

						sum = sum % 256; //wraps around it, for now

					}

					cpuRegisters[x] = (byte)sum;


				}
				else if (opCode.StartsWith("D"))
				{

					canDraw = true;

					//need to redo this, its completely wrong lol

					//Draw
					int x = Int32.Parse(opCode.Substring(1, 1));
					int y = Int32.Parse(opCode.Substring(2, 1));
					char n = opCode[3];

					Console.WriteLine("Draw " + ((n=='F')?"White":"Black") + " at x:" + x + ", y:" + y);


					display[x, y] = n;

				}
				else
				{
					Console.WriteLine("Unknown opCode: " + opCode);
				}



			} catch(Exception e)
			{
				Console.WriteLine("Error: " + e);
			}

			Console.ReadLine();


		}


		void readRom(string filePath)
		{

			byte[] rom = File.ReadAllBytes(filePath);

			//load rom at 512
			Array.Copy(rom, 0, memory, 512, rom.Length);


			Console.WriteLine("Rom Size: " + rom.Length);
			Console.WriteLine("Memory Size: " + memory.Length+"\n");

		}

		public char[,] getDisplay()
		{
			return display;
		}


		public void clearScreen()
		{
			for (int i = 0; i < display.GetLength(0); i++)
			{
				for (int j = 0; j < display.GetLength(1); j++) 
				{
					display[i, j] = '0';
				}
			}
		}

	}
}
