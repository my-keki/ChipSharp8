using Microsoft.Xna.Framework.Input;

namespace ChipSharp8.ChipSharp8Core
{
    public class ALU
    {
        private ushort Address { get => (ushort)(cpu.opcode & 0x0FFFu); }
        private byte Nibble { get => (byte)(cpu.opcode & 0x000Fu); }
        private byte LowByte { get => (byte)(cpu.opcode & 0x00FFu); }
        private byte X { get => (byte)((cpu.opcode & 0x0F00u) / 0x100u); }
        private byte Y { get => (byte)((cpu.opcode & 0x00F0u) / 0x10u); }
        private CPU cpu;
        private static Random rand = new Random();

        public ALU(CPU cpu)
        {
            this.cpu = cpu;
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
            cpu.programCounter = cpu.stack[--cpu.stackPointer];
        }

        public void Opcode1NNN()
        {
            cpu.programCounter = Address;
        }

        public void Opcode2NNN()
        {
            cpu.stack[cpu.stackPointer++] = cpu.programCounter;
            cpu.programCounter = Address;
        }

        public void Opcode3XKK()
        {
            if (cpu.register[X] == LowByte)
            {
                cpu.programCounter += 2;
            }
        }

        public void Opcode4XKK()
        {
            if (cpu.register[X] != LowByte)
            {
                cpu.programCounter += 2;
            }
        }

        public void Opcode5XY0()
        {
            if (cpu.register[X] == cpu.register[Y])
            {
                cpu.programCounter += 2;
            }
        }

        public void Opcode6XKK()
        {
            cpu.register[X] = LowByte;
        }

        public void Opcode7XKK()
        {
            cpu.register[X] += LowByte;
        }

        public void Opcode8XY0()
        {
            cpu.register[X] = cpu.register[Y];
        }

        public void Opcode8XY1()
        {
            cpu.register[X] |= cpu.register[Y];
        }

        public void Opcode8XY2()
        {
            cpu.register[X] &= cpu.register[Y];
        }

        public void Opcode8XY3()
        {
            cpu.register[X] ^= cpu.register[Y];
        }

        public void Opcode8XY4()
        {
            ushort result = (ushort)(cpu.register[X] + cpu.register[Y]);

            if (result > 255u)
            {
                cpu.register[0xFu] = 1;
            }
            else
            {
                cpu.register[0xFu] = 0;
            }
            cpu.register[X] = (byte)result;
        }

        public void Opcode8XY5()
        {

            if (cpu.register[X] > cpu.register[Y])
            {
                cpu.register[0xFu] = 1;
            }
            else
            {
                cpu.register[0xFu] = 0;
            }
            cpu.register[X] -= cpu.register[Y];
        }

        public void Opcode8XY6()
        {

            if ((cpu.register[X] & 0x1u) == 0x1u)
            {
                cpu.register[0xFu] = 1;
            }
            else
            {
                cpu.register[0xFu] = 0;
            }

            cpu.register[X] /= 2;
        }

        public void Opcode8XY7()
        {

            if (cpu.register[Y] > cpu.register[X])
            {
                cpu.register[0xFu] = 1;
            }
            else
            {
                cpu.register[0xFu] = 0;
            }
            cpu.register[X] = (byte)(cpu.register[Y] - cpu.register[Y]);
        }

        public void Opcode8XYE()
        {
            if ((cpu.register[X] & 0x80u) == 0x80u)
            {
                cpu.register[0xFu] = 1;
            }
            else
            {
                cpu.register[0xFu] = 0;
            }
            cpu.register[X] *= 2;
        }

        public void Opcode9XY0()
        {
            if (cpu.register[X] != cpu.register[Y])
            {
                cpu.programCounter += 2;
            }
        }

        public void OpcodeANNN()
        {
            cpu.indexRegister = Address;
        }

        public void OpcodeBNNN()
        {
            cpu.programCounter = (ushort)(Address + cpu.register[0x0u]);
        }

        public void OpcodeCXKK()
        {
            cpu.register[X] = (byte)(rand.Next(0, 256) & LowByte);
        }

        public void OpcodeDXYN()
        {
            byte numOfBytes = (byte)(cpu.opcode & Nibble);

            cpu.register[0xFu] = 0;

            for (int row = 0; row < numOfBytes; row++)
            {
                byte buffer = cpu.ram[cpu.indexRegister + row];

                for (int col = 0; col < 0x8; col++)
                {
                    byte pixel = (byte)(buffer & (0b10000000 >> col));  //iterate MSB to LSB

                    int posX = (cpu.register[X] + col) % 64;
                    int posY = (cpu.register[Y] + row) % 32;

                    byte displayPixel = cpu.display[posX, posY];

                    if (displayPixel == 0)
                    {
                        cpu.display[posX, posY] = pixel;
                    }
                    else
                    {
                        cpu.register[0xFu] = 1;
                        cpu.display[posX, posY] ^= pixel;
                    }
                }
            }
        }

        public void OpcodeEX9E()
        {
            bool isKeyPressed = cpu.keyboard[cpu.register[X]];

            if (isKeyPressed)
            {
                cpu.programCounter += 2;
                cpu.keyboard[cpu.register[X]] = false;

                Console.WriteLine($"EX9E, 0x{cpu.register[X],0:X2}");
            }
        }

        public void OpcodeEXA1()
        {
            bool isKeyPressed = cpu.keyboard[cpu.register[X]];

            if (!isKeyPressed)
            {
                cpu.programCounter += 2;
                Console.WriteLine($"EXA1, 0x{cpu.register[X],0:X2}");
            }
        }

        public void OpcodeFx07()
        {
            cpu.register[X] = cpu.delayTimer;
        }

        public void OpcodeFx0A()    //wait for a key press and a release, if true, set Vx to the value of the key
        {
            bool isKeyPressed = cpu.keyboard[cpu.register[X]];

            Console.WriteLine($"Debug: waiting for key 0x{cpu.register[X]}");

            KeyboardState keyboardState = Keyboard.GetState();

            foreach (var key in Renderer._keyboard)
            {
                if (isKeyPressed && keyboardState.IsKeyUp(key.Key))
                {
                    cpu.register[X] = key.Value;
                    cpu.keyboard[cpu.register[X]] = false;
                    break;
                }
            }
            if (!isKeyPressed)
            {
                cpu.programCounter -= 2;
            }
        }

        public void OpcodeFx15()
        {
            cpu.delayTimer = cpu.register[X];
        }

        public void OpcodeFx18()
        {
            cpu.soundTimer = cpu.register[X];
        }

        public void OpcodeFx1E()
        {
            cpu.indexRegister += cpu.register[X];
        }

        public void OpcodeFX29()
        {
            cpu.indexRegister = (ushort)((CPU.SpriteStartAddress + 5) * cpu.register[X]);
        }

        public void OpcodeFX33()
        {
            cpu.ram[cpu.indexRegister + 2] = (byte)(cpu.register[X] % 10); //lsb
            cpu.ram[cpu.indexRegister + 1] = (byte)(cpu.register[X] % 100 / 10);   //middle
            cpu.ram[cpu.indexRegister] = (byte)(cpu.register[X] / 100); //msb
        }

        public void OpcodeFX55()
        {
            for (int i = 0; i <= X; i++)
            {
                cpu.ram[cpu.indexRegister + i] = cpu.register[i];
            }
        }

        public void OpcodeFx65()
        {
            for (int i = 0; i <= X; i++)
            {
                cpu.register[i] = cpu.ram[cpu.indexRegister + i];
            }
        }
    }
}