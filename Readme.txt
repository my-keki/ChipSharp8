#CHIPSHARP8 - Chip8 Emulator Written in C# .NET8.0 and MonoGame 3.8.1.303

##What Works
    ->Only the space invaders rom works. It must run at 100hz or 500hz to accomodate how slow the keyboard responds.
    ->The enemies appear after being destroyed - TBD
    ->It passes the test_opcode rom.

##What To Add
    ->Sound
    ->Refactor DXYN so drawing the pixel doesn't go out of bounds on some games.

##What I Learned
    ->A better understanding of cpu architecture
    ->Organizing code when projects get big.
    ->Reading through documentation.
    ->Debugging code through terminal and or debugger

##Keyboard Controls
    ->Key 4 = Left
    ->Key W = Right
    ->Key Q = Fire
    ->Key Q = Start game after splash screen
    ->Key S = Retry after Game Over appears