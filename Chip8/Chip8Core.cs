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


	    ushort opCode;
		ushort index; //I
		ushort programCounter = 512;

		byte[] memory = new byte[4096];

		byte[] cpuRegisters = new byte[16];//V

		ushort[] stack = new ushort[16];

		byte stackPosition = 0;


		//ushort[] display = new ushort[64 * 32]; //or 128 * 64

	    ushort[,] display = new ushort[64,32]; //1 or 0, black or white


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



		public void init(string filename)
		{
			// I need to add fonts and other functions in the first 512
			//512 onwards where I'm supposed to load rom

			//index 80 (hex 50) 0x050

			Array.Copy(fontData, 0, memory, 80, fontData.Length);


			readRom(filename);
			
			//readRom("C:/Projects/Chip8/IBM_Logo.ch8");
			//readRom("C:/Projects/Chip8/test_opcode.ch8");


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

				string opCodeStr = BitConverter.ToUInt16(chunk, 0).ToString("X4"); // "X4" for 4 digits

				Console.WriteLine(opCodeStr);

				 opCode = Convert.ToUInt16(opCodeStr, 16);


				//okay so we use the fancy bitwise operations to still have a switch statement 
				//Note: 0xNumber to show it in hexadecimal

				//first number
				int nibble = opCode >> 12;

				string nnn,nn;

				int x, y;

				switch (nibble)
				{
					case 0x0:

						if(opCode == 0x00E0)
						{
							Console.WriteLine("Clearing Screen");
							clearScreen();
							CanDraw = true;
						}

					break;


					case 0x1:

						//Set the PC to nnn
						 nnn = opCodeStr.Substring(1);

						Console.WriteLine("Set the Program Counter to " + nnn);

						programCounter = (ushort)Convert.ToInt32(nnn, 16);


					break;


					case 0x3:

						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

						nn = opCodeStr.Substring(2);

						Console.WriteLine("Will skip next instruction if V["+x+"] equals "+nn);


						//if (Vx == NN)
						if (cpuRegisters[x] == Convert.ToByte(nn, 16))
						{
							Console.WriteLine("skipping...");
							programCounter += 2;
						}


					break;


					case 0x4:

						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

						nn = opCodeStr.Substring(2);

						Console.WriteLine("Will skip next instruction if V[" + x + "] does not equal " + nn);


						//if (Vx == NN)
						if (cpuRegisters[x] != Convert.ToByte(nn, 16))
						{
							Console.WriteLine("skipping...");
							programCounter += 2;
						}


					break;


					case 0x5:

						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

						y = Convert.ToInt32(opCodeStr.Substring(2, 1), 16);


						Console.WriteLine("Will skip next instruction if V[" + x + "] equals " + "V[" + y + "]");


						//if (Vx == NN)
						if (cpuRegisters[x] == cpuRegisters[y])
						{
							Console.WriteLine("skipping...");
							programCounter += 2;
						}


					break;




					case 0x6:

						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

						nn = opCodeStr.Substring(2);

						Console.WriteLine("Set Cpu Register " + x + " to " + nn);

						//set register VX
						cpuRegisters[x] = Convert.ToByte(nn, 16);


					break;


					case 0x7:

						//add value to register VX

						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

						nn = opCodeStr.Substring(2);


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


					break;


					case 0x9:

						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

						y = Convert.ToInt32(opCodeStr.Substring(2, 1), 16);


						Console.WriteLine("Will skip next instruction if V[" + x + "] does not equal " + "V[" + y + "]");


						//if (Vx == NN)
						if (cpuRegisters[x] != cpuRegisters[y])
						{
							Console.WriteLine("skipping...");
							programCounter += 2;
						}


					break;



					case 0xA:

						//Set I to NNN

						 nnn = opCodeStr.Substring(1);

						Console.WriteLine("Set Index to " + nnn);

						index = (ushort)Convert.ToInt32(nnn, 16);


					break;



					case 0xD:


						CanDraw = true;

						//Draw

						 x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);
						 y = Convert.ToInt32(opCodeStr.Substring(2, 1), 16);

						//char n = opCode[3];
						int n = Convert.ToInt32(opCodeStr[3] + "", 16);


						Console.WriteLine("Drawing " + n + " pixels long ");

						int newX = cpuRegisters[x] & 63;
						int newY = cpuRegisters[y] & 31;


						int startNewX = newX;


						//	Console.WriteLine(" newX " + newX);
						//	Console.WriteLine(" newY " + newY);

						//Console.WriteLine( " n " + n);

						//	Console.WriteLine(" index is " + index);


						for (int rows = 0; rows < n; rows++)
						{
							//memory[index]

							byte spriteRow = memory[index + rows];

							// Convert the byte to a binary string
							//string binaryString = Convert.ToString(spriteRow, 2).PadLeft(8, '0');



							//for (int col = 0; col < 8; col++)
							//	{

							//}



							byte memoryValue = memory[index + n];

							//Console.WriteLine("memoryValue " + memoryValue);
							string binaryString = Convert.ToString(spriteRow, 2).PadLeft(8, '0');
							//Console.WriteLine(binaryString);

							string bits = binaryString.ToString();

							newX = startNewX;

							//will need to use bitwise operations here too instead of char spilt

							foreach (char bit in bits)
							{
								int number = int.Parse(bit.ToString());
								//Console.WriteLine(number);

								//	Console.WriteLine("newX" + newX);


								if (number == 1)
								{

									if (display[newX, newY] == 1)
									{
										display[newX, newY] = 0;
										//CanDraw = true;
									}
									else
									{
										display[newX, newY] = 1;
									}


								}
								else
								{

								}

								if (newX >= 63)
								{
									break;
								}
								else
								{
									newX++;
								}


							}


							if (newY >= 31)
							{
								break;
							}
							else
							{
								newY++;
							}




						}


						//	Console.WriteLine("Draw " + ((n=='F')?"White":"Black") + " at x:" + x + ", y:" + y);


						//	display[newX, newY] = n;

						break;





					default:
					Console.WriteLine("Unknown opCode: " + opCodeStr);
					break;

				}






			} catch(Exception e)
			{
				Console.WriteLine("Error: " + e);
			}

			Console.ReadLine(); //Debug


		}


		void readRom(string filePath)
		{

			byte[] rom = File.ReadAllBytes(filePath);

			//load rom at 512
			Array.Copy(rom, 0, memory, 512, rom.Length);


			Console.WriteLine("Rom Size: " + rom.Length);
			Console.WriteLine("Memory Size: " + memory.Length+"\n");

		}

		public ushort[,] getDisplay()
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
