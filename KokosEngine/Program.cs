namespace KokosEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var board = new Board();
            board.SetStartingPosition();
            Console.WriteLine(board.AsFEN());
        }
    }
}