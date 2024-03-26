using System.Text;

namespace KokosEngine
{
    internal class Program
    {
        const string STARTPOS = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        static void Main(string[] args)
        {
            string fen = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
            int depth = 6;
            bool verbose = true;

            //Perft(depth, verbose);
            Perft(fen, depth, verbose);
        }

        static long Perft(int depth, bool verbose = false)
        {
            return Perft(STARTPOS, depth, verbose);
        }

        static long Perft(string fen, int depth, bool verbose = false)
        {
            var gs = new Gamestate();
            gs.LoadFEN(fen);
            if (verbose) Console.WriteLine("perft " + depth.ToString());
            long nodes;
            var start = DateTime.Now;
            nodes = gs.Perft(depth, verbose);
            var end = DateTime.Now;
            var duration = end - start;
            if (verbose)
            {
                Console.WriteLine($"Nodes searched: {nodes}");
                Console.WriteLine($"Duration: {duration}");
                Console.WriteLine();
                Console.WriteLine($"knps ~ {nodes / (duration.TotalSeconds * 1000)}");
            }
            return nodes;
        }
    }
}