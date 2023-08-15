// See https://aka.ms/new-console-template for more information
using Testy;

class Test
{
    static void Main(string[] args)
    {
        // Display the number of command line arguments.
        using (Game game = new Game(800, 600, "LearnOpenTK"))
        {
            game.Run();
        }
    }
}