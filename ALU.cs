
class ALU
{

    private ushort address;
    private byte nibble;
    private byte x;
    private byte y;
    private byte lowByte;
    private CPU cpu;
    private static Random rand = new Random();
    public ALU(CPU cpu)
    {
        this.cpu = cpu;
   
        nibble = (byte)(cpu.opcode & 0x000Fu);

        x = (byte)((cpu.opcode & 0x0F00u) / 0x100u);

        y = (byte)((cpu.opcode & 0x00F0u) / 0x10u);

        lowByte = (byte)(cpu.opcode & 0x00FFu);

        Console.WriteLine($"address -> 0x{address,0:X3}, nibble -> 0x{nibble,0:X1}, x -> 0x{x,0:X1}, y -> 0x{y,0:X1}, lowByte -> 0x{lowByte,0:X2}");
    }

    public ushort GetAddress()
    {
        return address = (ushort)(cpu.opcode & 0x0FFFu);
    }

    public byte GetNibble()
    {
        return nibble = (byte)(cpu.opcode & 0x000Fu);
    }

    public byte GetX()
    {
        return x = (byte)((cpu.opcode & 0x0F00u) / 0x100u);
    }

    public byte GetY()
    {
        return y = (byte)((cpu.opcode & 0x00F0u) / 0x10u);
    }

    public byte GetLowByte()
    {
        return lowByte = (byte)(cpu.opcode & 0x00FFu);
    }

    public void Opcode0NNN()
    {
        //DO NOTHING
    }

    public void Opcode00E0()
    {

        Array.Clear(cpu.display);
    }

    public void Opcode00EE()
    {

        cpu.programCounter = cpu.stack[cpu.stackPointer--];
    }

    public void Opcode1NNN()
    {

        cpu.programCounter = GetAddress();
    }

    public void Opcode2NNN()
    {

        cpu.stack[cpu.stackPointer++] = cpu.programCounter;

        cpu.programCounter = GetAddress();
    }

    public void Opcode3XKK()
    {

        if (cpu.register[GetX()] == GetLowByte())
        {

            cpu.programCounter += 2;
        }
    }

    public void Opcode4XKK()
    {

        if (cpu.register[GetX()] != GetLowByte())
        {

            cpu.programCounter += 2;
        }
    }

    public void Opcode5XY0()
    {

        if (cpu.register[GetX()] == cpu.register[GetY()])
        {

            cpu.programCounter += 2;
        }
    }

    public void Opcode6XKK()
    {

        cpu.register[GetX()] = GetLowByte();
    }

    public void Opcode7XKK()
    {

        cpu.register[GetX()] += GetLowByte();
    }

    public void Opcode8XY0()
    {

        cpu.register[GetX()] = cpu.register[GetY()];
    }

    public void Opcode8XY1()
    {

        cpu.register[GetX()] |= cpu.register[GetY()];
    }

    public void Opcode8XY2()
    {

        cpu.register[GetX()] &= cpu.register[GetY()];
    }

    public void Opcode8XY3()
    {

        cpu.register[GetX()] ^= cpu.register[GetY()];
    }

    public void Opcode8XY4()
    {

        ushort result = (ushort)(cpu.register[GetX()] + cpu.register[GetY()]);

        if (result > 255u)
        {

            cpu.register[0xFu] = 1;

        }
        else
        {

            cpu.register[0xFu] = 0;
        }
    }

    public void Opcode8XY5()
    {

        if (cpu.register[GetX()] > cpu.register[GetY()])
        {

            cpu.register[0xFu] = 1;

        }
        else
        {

            cpu.register[0xFu] = 0;
        }

        cpu.register[GetX()] -= cpu.register[GetY()];
    }

    public void Opcode8XY6()
    {

        if ((cpu.register[GetX()] & 0x1u) == 0x1u)
        {

            cpu.register[0xFu] = 1;

        }
        else
        {

            cpu.register[0xFu] = 0;
        }

        cpu.register[GetX()] /= 2;
    }

    public void Opcode8XY7()
    {

        if (cpu.register[GetY()] > cpu.register[GetX()])
        {
            cpu.register[0xFu] = 1;

        }
        else
        {
            cpu.register[0xFu] = 0;
        }

        cpu.register[GetX()] = (byte)(cpu.register[GetY()] - cpu.register[GetY()]);
    }

    public void Opcode8XYE()
    {

        if ((cpu.register[GetX()] & 0x80u) == 0x80u)
        {
            cpu.register[0xFu] = 1;

        }
        else
        {
            cpu.register[0xFu] = 0;
        }

        cpu.register[GetX()] *= 2;
    }

    public void Opcode9XY0()
    {
        if (cpu.register[GetX()] != cpu.register[GetY()])
        {

            cpu.programCounter += 2;
        }
    }

    public void OpcodeANNN()
    {
        cpu.indexRegister = GetAddress();
    }

    public void OpcodeBNNN()
    {
        cpu.programCounter = (ushort)(GetAddress() + cpu.register[0x0u]);
    }

    public void OpcodeCXKK()
    {
        cpu.register[GetX()] = (byte)(rand.Next(0, 256) & GetLowByte());
    }

    public void OpcodeDXYN()    //TODO
    {

        byte numOfBytes = (byte)(cpu.opcode & GetNibble());

        byte mostSignificantBit = 0x80;

        cpu.register[0xFu] = 0;

        for (byte row = 0; row < numOfBytes; row++)
        {
            byte writeBuffer = cpu.ram[cpu.indexRegister + row];

            for (byte col = 0; col < 0x8; col++)
            {

                byte pixel = (byte)(writeBuffer & (mostSignificantBit >> col));

                if (cpu.display[col + GetX(), row + GetY()] == 0)
                {

                    cpu.display[col + GetX(), row + GetY()] = pixel;
                }
                else
                {
                    cpu.register[0xFu] = 1;
                    
                    cpu.display[col + GetX(), row + GetY()] ^= pixel;
                }


            }
        }
    }

    public void OpcodeEX9E()
    {

        if (cpu.keysPressed[cpu.register[GetX()]])
        {

            cpu.programCounter += 2;
        }
    }

    public void OpcodeEXA1()
    {
        if (!cpu.keysPressed[cpu.register[GetX()]])
        {

            cpu.programCounter += 2;
        }
    }

    public void OpcodeFx07()
    {

        cpu.register[GetX()] = cpu.delayTimer;
    }

    public void OpcodeFx0A()
    {

        if (cpu.keysPressed[0])
        {
            cpu.register[GetX()] = 0;
        }
        else if (cpu.keysPressed[1])
        {
            cpu.register[GetX()] = 1;
        }
        else if (cpu.keysPressed[2])
        {
            cpu.register[GetX()] = 2;
        }
        else if (cpu.keysPressed[3])
        {
            cpu.register[GetX()] = 3;
        }
        else if (cpu.keysPressed[4])
        {
            cpu.register[GetX()] = 4;
        }
        else if (cpu.keysPressed[5])
        {
            cpu.register[GetX()] = 5;
        }
        else if (cpu.keysPressed[6])
        {
            cpu.register[GetX()] = 6;
        }
        else if (cpu.keysPressed[7])
        {
            cpu.register[GetX()] = 7;
        }
        else if (cpu.keysPressed[8])
        {
            cpu.register[GetX()] = 8;
        }
        else if (cpu.keysPressed[9])
        {
            cpu.register[GetX()] = 9;
        }
        else if (cpu.keysPressed[10])
        {
            cpu.register[GetX()] = 10;
        }
        else if (cpu.keysPressed[11])
        {
            cpu.register[GetX()] = 11;
        }
        else if (cpu.keysPressed[12])
        {
            cpu.register[GetX()] = 12;
        }
        else if (cpu.keysPressed[13])
        {
            cpu.register[GetX()] = 13;
        }
        else if (cpu.keysPressed[14])
        {
            cpu.register[GetX()] = 14;
        }
        else if (cpu.keysPressed[15])
        {
            cpu.register[GetX()] = 15;
        }
        else
        {
            cpu.programCounter -= 2;
        }
    }

    public void OpcodeFx15()
    {

        cpu.delayTimer = cpu.register[GetX()];
    }

    public void OpcodeFx18()
    {
        cpu.soundTimer = cpu.register[GetX()];
    }

    public void OpcodeFx1E()
    {

        cpu.indexRegister += cpu.register[GetX()];
    }

    public void OpcodeFX29()
    {

        cpu.indexRegister = (ushort)(CPU.spriteStartAddress + 5 * cpu.register[GetX()]);
    }

    public void OpcodeFX33()
    {

        cpu.ram[cpu.indexRegister + 2] = (byte)(cpu.register[GetX()] % 10);	//lsb

        cpu.ram[cpu.indexRegister + 1] = (byte)(cpu.register[GetX()] % 100 / 10);	//middle

        cpu.ram[cpu.indexRegister] = (byte)(cpu.register[GetX()] / 100); //msb
    }

    public void OpcodeFX55()
    {
        for (byte i = 0; i <= cpu.register[GetX()]; i++)
        {
            cpu.ram[cpu.indexRegister + i] = cpu.register[i];
        }
    }

    public void OpcodeFx65()
    {
        for (byte i = 0; i <= cpu.register[GetX()]; i++)
        {
            cpu.register[i] = cpu.ram[cpu.indexRegister + i];
        }
    }
}