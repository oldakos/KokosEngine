using System.Text;

namespace KokosEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var gs = new Gamestate();

            //gs.LoadFEN("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1"); //perft 6
            gs.LoadFEN("r3k2r/Pppp1ppp/1b3nbN/nPP5/BB2P3/q4N2/Pp1P2PP/R2Q1RK1 b kq - 0 1"); //5
            //gs.LoadFEN("r3k2r/Ppp2ppp/1b3nbN/nPPp4/BB2P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 2"); //4
            //gs.LoadFEN("r3k2r/Ppp2ppp/1P3nbN/nP1p4/BB2P3/q4N2/Pp1P2PP/R2Q1RK1 b kq - 0 2"); //3
            //gs.LoadFEN("r3k2r/Ppp2ppp/1P3nbN/nP1p4/BB2P3/q4N2/P2P2PP/n2Q1RK1 w kq - 0 3"); //2
            //gs.LoadFEN("r3k2r/PpP2ppp/5nbN/nP1p4/BB2P3/q4N2/P2P2PP/n2Q1RK1 b kq - 0 3"); //1

            //gs.SetStartingPosition();
            int depth = 5;
            Console.WriteLine("Perft " + depth.ToString());
            var start = DateTime.Now;
            Console.WriteLine("Total nodes: " + gs.Perft(depth, true).ToString());
            Console.WriteLine("Duration: " + (DateTime.Now - start).ToString());
        }
    }
}