public static class Program
{
    public static void Main()
    {
        using (var game = new Renderer())
            game.Run();
    }
}