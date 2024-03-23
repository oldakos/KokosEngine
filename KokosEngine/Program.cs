using System.Text;

namespace KokosEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //seznam chyb perft
            //var gs = new Gamestate("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1"); //6 - 706045033

            //Console.WriteLine(gs.AsFEN());
            //foreach (var item in gs.GetPseudoLegalMoves())
            //{
            //    Console.WriteLine(Util.IndexToAlgebraic(item.SquareFrom) + Util.IndexToAlgebraic(item.SquareTo));
            //}

            //gs.MakeMove(MoveFactory.QuietMove(59, 51, Piece.BQ));            

            //var gs = new Gamestate().LoadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            var gs = new Gamestate();
            gs.SetStartingPosition();
            //Console.WriteLine(gs.PrintMailbox());
            //gs.MakeMove(MoveFactory.QuietMove(54, 9, Piece.BB));
            int depth = 5;
            Console.WriteLine("Perft " + depth.ToString());
            var start = DateTime.Now;
            Console.WriteLine("Total nodes: " + gs.Perft(depth, true).ToString());
            Console.WriteLine("Duration: " + (DateTime.Now - start).ToString());
        }
    }
}