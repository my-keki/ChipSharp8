// See https://aka.ms/new-console-template for more information
CPU cpu = new CPU();

ALU alu = new ALU(cpu);

alu.Opcode8XY4();
Console.WriteLine(cpu.register[0xF]);
