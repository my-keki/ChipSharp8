namespace ChipSharp8.ChipSharp8Core
{
    public static class Program
    {       
        public static void Main()
        {
            using (var game = new Renderer())
                game.Run();
        }
    }
}
