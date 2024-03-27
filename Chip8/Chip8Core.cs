using System;
using System.Collections;
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
	
	    ushort opCode;
		ushort index; //I
		ushort programCounter = 512;

		ushort carryFlag;//VF

		byte[] memory = new byte[4096];

		byte[] cpuRegisters = new byte[16];//V

		Stack<ushort> stack = new Stack<ushort>(16);

		byte stackPosition = 0;


	    ushort[,] display = new ushort[64,32]; //1 or 0, black or white


		byte delayTimer = 0;
		byte soundTimer =0;


		public bool CanDraw { get; set; }

		public bool CanWaitForInput { get; set; }

		public bool KeyEntered { get; set; }

		public bool CanStepThroughProcess { get; set; }

		public bool PauseIfUnknownOpCode { get; set; }

		public bool PauseIfException { get; set; }
		public bool OldChip8Behaviour { get; set; }

		public bool DebugMode { get; set; }

		public int KeyCpuIndex { get; set; }

		public int KeyBeingPressed { get; set; }


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

			CanStepThroughProcess = false;
			PauseIfUnknownOpCode = false;
			OldChip8Behaviour = false;
			DebugMode = false;
			PauseIfException = false;
			KeyEntered = false;
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

				//delayTimer += 60;


				// Convert it to Big Endian if its in little endian, should fix read bug
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(chunk);
				}

				string opCodeStr = BitConverter.ToUInt16(chunk, 0).ToString("X4"); // "X4" for 4 digits

				Console.WriteLine("Opcode: "+opCodeStr);

				 opCode = Convert.ToUInt16(opCodeStr, 16);


				//okay so we use the fancy bitwise operations to still have a switch statement 
				//Note: 0xNumber to show it in hexadecimal

				//first number
				int nibble = opCode >> 12;

				string nnn,nn;

				int x, y, jump;

				switch (nibble)
				{
					case 0x0:

						if(opCode == 0x00E0)
						{
							Console.WriteLine("Clearing Screen");
							clearScreen();
							CanDraw = true;
						} else if (opCode == 0x00EE)
						{
							programCounter =  stack.Pop();
							Console.WriteLine("Getting Program Counter from stack and setting it to " + programCounter);
						}


					break;


					case 0x1:

						//Set the PC to nnn
						 nnn = opCodeStr.Substring(1);

						if(DebugMode)
						{

							Console.WriteLine("Set the Program Counter to " + nnn);
						}


						programCounter = (ushort)Convert.ToInt32(nnn, 16);


					break;


					case 0x2:

						//Set the PC to nnn
						nnn = opCodeStr.Substring(1);

						Console.WriteLine("Save the current Program Counter to Stack and it to " + nnn);

						stack.Push(programCounter);

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


					case 0x8:
						//8750
						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

						y = Convert.ToInt32(opCodeStr.Substring(2, 1), 16);

						int value = Convert.ToInt32(opCodeStr.Substring(3, 1), 16);

						switch(value)
						{
							case 0x0:

								//VX is set to the value of VY.

								cpuRegisters[x] = cpuRegisters[y];

								break;


							case 0x1:

								//VX is set to the bitwise/binary logical disjunction (OR) of VX and VY. VY is not affected.

								cpuRegisters[x] = (byte)(cpuRegisters[x] | cpuRegisters[y]);


								break;


							case 0x2:

								//VX is set to the bitwise/binary logical conjunction (AND) of VX and VY. VY is not affected.

								cpuRegisters[x] = (byte)(cpuRegisters[x] & cpuRegisters[y]);


								break;


							case 0x3:

								//VX is set to the bitwise/binary exclusive OR (XOR) of VX and VY. VY is not affected.

								cpuRegisters[x] = (byte)(cpuRegisters[x] ^ cpuRegisters[y]);

	
							break;



							case 0x4:

								//VX is set to the value of VX plus the value of VY. VY is not affected.

								int result = cpuRegisters[x] + cpuRegisters[y];

								

								//flag is set here..
								//if overflows

								if (result > 255)
								{
									result = result % 256;

									carryFlag = 1;
								} else
								{
									carryFlag = 0;
								}

								cpuRegisters[x] = (byte)(result); //must fix overflow here

								break;


							case 0x5:

								//Sets VX to the result of VX - VY

								cpuRegisters[x] = (byte)(cpuRegisters[x] - cpuRegisters[y]);

								//flag is set here..




							break;


							case 0x7:

								//Sets VX to the result of VX - VY

								cpuRegisters[x] = (byte)(cpuRegisters[y] - cpuRegisters[x]);

								//flag is set here..


							break;


							//Ambiguous
							case 0x6:
								//Shift the value in VX, 1 bit to the right

								if (OldChip8Behaviour)
								{
									cpuRegisters[x] = cpuRegisters[y];
								}

								cpuRegisters[x] >>= 1;

								//Set VF to 1 if the bit that was shifted out was 1, or 0 if it was 0

								break;

							//Ambiguous
							case 0x0E:
								//Shift the value in VX, 1 bit to the left

								if (OldChip8Behaviour)
								{
									cpuRegisters[x] = cpuRegisters[y];
								}

								cpuRegisters[x] <<= 1;

								//Set VF to 1 if the bit that was shifted out was 1, or 0 if it was 0
								break;

							default:
								Console.WriteLine("Unknown 8 opCode: " + opCodeStr);

								if (PauseIfUnknownOpCode)
								{
									Console.ReadLine();

								}

							break;


						}


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



					case 0xB:

						//

						if(OldChip8Behaviour)
						{
							nnn = opCodeStr.Substring(1);
						    jump = (ushort)Convert.ToInt32(nnn, 16);

							programCounter = (ushort)(cpuRegisters[0] + jump);

							Console.WriteLine("BNNN: Jumping to " + programCounter);
						} else
						{
							nnn = opCodeStr.Substring(1);
							jump = (ushort)Convert.ToInt32(nnn, 16);
							x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

							programCounter = (ushort)(cpuRegisters[x] + jump);


							Console.WriteLine("BXNN: Jumping to " + programCounter);

						}

						

		



						break;

					case 0xC:

						//
						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);
						nn = opCodeStr.Substring(2);

						Console.WriteLine("Generating Random Number to V["+x+"] & "+nn);

						Random random = new Random();

						int randNum = random.Next(255);

						cpuRegisters[x] = (byte)(randNum & Convert.ToByte(nn, 16));

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

						break;

					case 0xF:

						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

						int fvalue = Convert.ToInt32(opCodeStr.Substring(2, 2), 16);

					//	Console.WriteLine("fvalue "+ fvalue);

						switch (fvalue)
						{

							//timers

							case 0x07:

								cpuRegisters[x] = delayTimer;

							break;


							case 0x15:

								delayTimer = cpuRegisters[x];

							break;


							case 0x18:

								soundTimer = cpuRegisters[x];

							break;



							//font time!
							case 0x29:

								Console.WriteLine("Getting font: "+x);

								index =  (ushort)(80 + x);

								break;



							case 0x33:

								int decimalNum = cpuRegisters[x];
								//Console.WriteLine("Num: " + decimalNum);



								int counter = 2;

								while (decimalNum > 0)
								{
									int digit = decimalNum % 10;
									//Console.WriteLine("digit: "+digit);

									memory[index + counter] = (byte)digit;
									counter--;

									decimalNum /= 10; 
								}



								break;



							//amb
							case 0x55:

							//	Console.WriteLine("Storing "+x);
								
									for (int i = 0; i <=x; i++)
									{
										memory[index + i] = cpuRegisters[i];
									}

									if (OldChip8Behaviour)
									{
										index = (ushort)(index + x + 1);
									}


								break;


							case 0x65:
							
									for (int i = 0; i <=x; i++)
									{
										cpuRegisters[i] = memory[index + i];
									}

									if (OldChip8Behaviour)
									{
										index = (ushort)(index + x + 1);
									}


								break;


							case 0x0A:
								//KeyEntered = false;
								//this is going to be interesting...

								Console.WriteLine("Awaiting Key Input");


								//test mode
								//string word = Console.ReadLine();
								KeyCpuIndex = x;

								if (!KeyEntered)
								{

									CanWaitForInput = true;
									programCounter -= 2;
								}


								//	cpuRegisters[x] = 0;

								break;




							//FX1E
							case 0x1E:

								Console.WriteLine("Index will get the value in VX added to it");
								

								index = (ushort)(index + cpuRegisters[x]);

								// i need to double check what happens for an overflow here
								break;




							default:
								Console.WriteLine("Unknown FXXX opCode: " + opCodeStr);

								if (PauseIfUnknownOpCode)
								{
									Console.ReadLine();

								}

							break;

						}

						break;

					case 0xE:

						x = Convert.ToInt32(opCodeStr.Substring(1, 1), 16);

						int evalue = Convert.ToInt32(opCodeStr.Substring(2, 2), 16);

						//	Console.WriteLine("evalue "+ evalue);

						switch (evalue)
						{
							case 0x9E:
								//EX9E will skip one instruction (increment PC by 2) if the key corresponding to the value in VX is pressed.

								value = cpuRegisters[x];

								if(KeyBeingPressed==value)
								{
									programCounter += 2;
								}

								break;

							case 0xA1:
								//EXA1 skips (increment PC by 2) if the key corresponding to the value in VX is not pressed.
								value = cpuRegisters[x];

								if (KeyBeingPressed != value)
								{
									programCounter += 2;
								}


								break;
						}
						break;



					/*opcodes that still have to added


					 * EX9E, EXA1 - Skip if key(test)
					 * FX07, FX15, FX18 - timers ( test, implement timers)
					 * FX0A - key (test)
					 * FX29 - font (test)

					 * */


					default:
					Console.WriteLine("Unknown opCode: " + opCodeStr);

					if(PauseIfUnknownOpCode)
					{
						Console.ReadLine();
						
					}

					break;

				}






			} catch(Exception e)
			{
				Console.WriteLine("Exception, Opcode:" + opCode);
				Console.WriteLine("Error: " + e);

				if(PauseIfException)
				{
					Console.ReadLine();
				}
			}

			//I'll need to implemnent the delay timer here


			if (CanStepThroughProcess)
			{
				Console.ReadLine(); //Debug
			}


		}


		void readRom(string filePath)
		{

			byte[] rom = File.ReadAllBytes(filePath);

			//load rom at 512
			Array.Copy(rom, 0, memory, 512, rom.Length);


			Console.WriteLine("Rom Size: " + rom.Length);
			//Console.WriteLine("Memory Size: " + memory.Length+"\n");

		}

		public ushort[,] getDisplay()
		{
			return display;
		}


		public void clearScreen()
		{
			for (int x = 0; x < display.GetLength(0); x++)
			{
				for (int y = 0; y < display.GetLength(1); y++) 
				{
					display[x, y] = '0';
				}
			}
		}

		public void updateCpuRegistry(int index, byte value)
		{
			cpuRegisters[index] = value;
		}

		public void updateCpuRegistry(byte value)
		{
			updateCpuRegistry(KeyCpuIndex, value);
		}
		

	}
}
