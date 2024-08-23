namespace ChipSharp8.ChipSharp8Core;

public class CPU
{
    public const uint MemorySize = 4096;
    public const uint RegisterSize = 16;
    public const uint StackSize = 16;
    public const uint MaxWidth = 64;
    public const uint MaxHeight = 32;
    public const ushort ProgramStartAddress = 0x200;
    public const ushort SpriteStartAddress = 0x50;
    public ushort stackPointer;
    public byte delayTimer;
    public byte soundTimer;
    public byte[] register;
    public byte[] ram;
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

    public bool[] keyboard;
    public CPU()
    {
        InitizalizeCPU();
        LoadSprite();
        OpenROM();
    }

    public void InitizalizeCPU()
    {
        display = new byte[MaxWidth, MaxHeight];
        register = new byte[RegisterSize];
        ram = new byte[MemorySize];
        stack = new ushort[StackSize];
        programCounter = ProgramStartAddress;  
        keyboard = new bool[RegisterSize];
        opcode = 0;
        stackPointer = stack[0];
        delayTimer = 0;
        soundTimer = 0;
    }

    public void CycleCPU(ALU alu)
    {
        opcode = FetchOpcode();
        DecodeExecuteOpcode(alu);
        DecrementTimers();
    }

    public ushort FetchOpcode()
    {
        opcode = (ushort)((ram[programCounter] * 0x100u) | ram[programCounter + 0x1u]);
        programCounter += 2;
        return opcode;
    }

    public void DecodeExecuteOpcode(ALU alu)
    {
        switch ((opcode & 0xF000u) >> 12)  //MSB
        {
            case 0x0:
                switch (opcode & 0x0FFFu)
                {
                    case 0x0E0: alu.Opcode00E0(); break;
                    case 0x0EE: alu.Opcode00EE(); break;
                    default: alu.Opcode0NNN(); break;
                }
                break;
            case 0x1: alu.Opcode1NNN(); break;
            case 0x2: alu.Opcode2NNN(); break;
            case 0x3: alu.Opcode3XKK(); break;
            case 0x4: alu.Opcode4XKK(); break;
            case 0x5: alu.Opcode5XY0(); break;
            case 0x6: alu.Opcode6XKK(); break;
            case 0x7: alu.Opcode7XKK(); break;
            case 0x8:
                switch (opcode & 0x000Fu)
                {
                    case 0x0: alu.Opcode8XY0(); break;
                    case 0x1: alu.Opcode8XY1(); break;
                    case 0x2: alu.Opcode8XY2(); break;
                    case 0x3: alu.Opcode8XY3(); break;
                    case 0x4: alu.Opcode8XY4(); break;
                    case 0x5: alu.Opcode8XY5(); break;
                    case 0x6: alu.Opcode8XY6(); break;
                    case 0x7: alu.Opcode8XY7(); break;
                    case 0xE: alu.Opcode8XYE(); break;
                    default: break;
                }
                break;
            case 0x9: alu.Opcode9XY0(); break;
            case 0xA: alu.OpcodeANNN(); break;
            case 0xB: alu.OpcodeBNNN(); break;
            case 0xC: alu.OpcodeCXKK(); break;
            case 0xD: alu.OpcodeDXYN(); break;
            case 0xE:
                switch (opcode & 0x00FFu)
                {
                    case 0x9E: alu.OpcodeEX9E(); break;
                    case 0xA1: alu.OpcodeEXA1(); break;
                    default: break;
                }
                break;
            case 0xF:
                switch (opcode & 0x00FFu)
                {
                    case 0x07: alu.OpcodeFx07(); break;
                    case 0x0A: alu.OpcodeFx0A(); break;
                    case 0x15: alu.OpcodeFx15(); break;
                    case 0x18: alu.OpcodeFx18(); break;
                    case 0x1E: alu.OpcodeFx1E(); break;
                    case 0x29: alu.OpcodeFX29(); break;
                    case 0x33: alu.OpcodeFX33(); break;
                    case 0x55: alu.OpcodeFX55(); break;
                    case 0x65: alu.OpcodeFx65(); break;
                    default: break;
                }
                break;
            default: break;
        }
    }

    public void DecrementTimers()
    {
        if (delayTimer > 0)
        {
            delayTimer--;
        }

        if (soundTimer > 0)
        {
            soundTimer--;
        }
    }

    public void LoadSprite()
    {
        for (int i = 0; i < sprite.Length; i++)
        {
            ram[SpriteStartAddress + i] = sprite[i];
        }
    }

    public void OpenROM()
    {
        string path = @"..\Roms\Space Invaders.ch8";

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }
        else
        {
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    byte[] buffer = new byte[(int)fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);  //num of bytes greater than 0

                    for (int i = 0; i < (int)fs.Length; i++)
                    {
                        ram[ProgramStartAddress + i] = buffer[i];
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
