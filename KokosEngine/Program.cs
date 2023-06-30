namespace KokosEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var board = new Board();
            board.SetStartingPosition();
            Console.WriteLine(board.AsFEN());
            Move move;
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("e2");
            move.SquareTo = Util.AlgebraicToIndex("e4");
            move.PieceMoved = Piece.WP;
            move.Type = MoveType.DoublePawnPush;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("d7");
            move.SquareTo = Util.AlgebraicToIndex("d5");
            move.PieceMoved = Piece.BP;
            move.Type = MoveType.DoublePawnPush;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("f1");
            move.SquareTo = Util.AlgebraicToIndex("a6");
            move.PieceMoved = Piece.WB;
            move.Type = MoveType.Quiet;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("e7");
            move.SquareTo = Util.AlgebraicToIndex("e5");
            move.PieceMoved = Piece.BP;
            move.Type = MoveType.DoublePawnPush;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("g1");
            move.SquareTo = Util.AlgebraicToIndex("f3");
            move.PieceMoved = Piece.WN;
            move.Type = MoveType.Quiet;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("c7");
            move.SquareTo = Util.AlgebraicToIndex("c5");
            move.PieceMoved = Piece.BP;
            move.Type = MoveType.DoublePawnPush;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("e1");
            move.SquareTo = Util.AlgebraicToIndex("g1");
            move.PieceMoved = Piece.WK;
            move.Type = MoveType.KingCastle;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("d5");
            move.SquareTo = Util.AlgebraicToIndex("d4");
            move.PieceMoved = Piece.BP;
            move.Type = MoveType.Quiet;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("c2");
            move.SquareTo = Util.AlgebraicToIndex("c4");
            move.PieceMoved = Piece.WP;
            move.Type = MoveType.DoublePawnPush;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
            move = new Move();
            move.SquareFrom = Util.AlgebraicToIndex("d4");
            move.SquareTo = Util.AlgebraicToIndex("c3");
            move.PieceMoved = Piece.BP;
            move.Type = MoveType.EnPassant;
            board.MakeMove(move);
            Console.WriteLine(board.AsFEN());
        }
    }
}