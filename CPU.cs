using System;
class CPU
{
    public const uint memorySize = 4096u;
    public const uint registerSize = 16u;
    public const uint stackSize = 16u;
    public const uint maxWidth = 64u;
    public const uint maxHeight = 32u;
    public const ushort programStartAddress = (ushort)0x200u;
    public const ushort spriteStartAddress = (ushort)0x50u;
    public byte stackPointer;
    public byte delayTimer;
    public byte soundTimer;
    public byte[] register;
    public byte[] ram;
    public byte[] keyPad = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    public byte[,] display;
    public ushort opcode;
    public ushort indexRegister;
    public ushort programCounter;
    public ushort[] stack;
    public byte[] sprite = {
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

    public CPU()
    {
        register = new byte[registerSize];

        ram = new byte[memorySize];

        stack = new ushort[stackSize];

        programCounter = programStartAddress;

        stackPointer = 0;

        delayTimer = 0;

        soundTimer = 0;

        LoadSprite();

        OpenROM();

        opcode = FetchOpcode();

        display = new byte[maxWidth, maxHeight];
    }

    public ushort FetchOpcode()
    {
        opcode = (ushort)((ram[programCounter] * 0x100u) | ram[programCounter + 0x1u]);

        Console.WriteLine($"ram[{programCounter}] -> {ram[programCounter], 0:X2}, ram[{programCounter + 1}] -> {ram[programCounter + 1], 0:X2} => 0x{opcode, 0:X4}");   //debug

        programCounter += 2;

        return opcode;
    }

    public void LoadSprite()
    {
        for (int i = 0; i < sprite.Length; i++)
        {
            ram[spriteStartAddress + i] = sprite[i];
        }
    }

    public void OpenROM()
    {
        const string path = @".\Space Invaders.ch8";

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"kek!!");
        }
        else
        {
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    byte[] buffer = new byte[(int)fs.Length];

                    Console.WriteLine(fs.Read(buffer, 0, (int)fs.Length));  //num of bytes greater than 0

                    for (int i = 0; i < (int)fs.Length; i++)
                    {
                        ram[programStartAddress + i] = buffer[i];
                        //Console.WriteLine($"ram[{programStartAddress + i}] -> " + $"0x{buffer[i], 0:X2}");  //debug
                    }
                    fs.Close();
                };

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}