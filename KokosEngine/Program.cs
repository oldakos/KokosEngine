﻿namespace KokosEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var board = new Gamestate();
            board.SetStartingPosition();
            Console.WriteLine(board.AsFEN());
            var move1 = new Move();
            move1.SquareFrom = Util.AlgebraicToIndex("e2");
            move1.SquareTo = Util.AlgebraicToIndex("e4");
            move1.PieceMoved = Piece.WP;
            move1.Type = MoveType.DoublePawnPush;
            board.MakeMove(move1);
            Console.WriteLine(board.AsFEN());
            var move2 = new Move();
            move2.SquareFrom = Util.AlgebraicToIndex("d7");
            move2.SquareTo = Util.AlgebraicToIndex("d5");
            move2.PieceMoved = Piece.BP;
            move2.Type = MoveType.DoublePawnPush;
            board.MakeMove(move2);
            Console.WriteLine(board.AsFEN());
            var move3 = new Move();
            move3.SquareFrom = Util.AlgebraicToIndex("f1");
            move3.SquareTo = Util.AlgebraicToIndex("a6");
            move3.PieceMoved = Piece.WB;
            move3.Type = MoveType.Quiet;
            board.MakeMove(move3);
            Console.WriteLine(board.AsFEN());
            var move4 = new Move();
            move4.SquareFrom = Util.AlgebraicToIndex("e7");
            move4.SquareTo = Util.AlgebraicToIndex("e5");
            move4.PieceMoved = Piece.BP;
            move4.Type = MoveType.DoublePawnPush;
            board.MakeMove(move4);
            Console.WriteLine(board.AsFEN());
            var move5 = new Move();
            move5.SquareFrom = Util.AlgebraicToIndex("g1");
            move5.SquareTo = Util.AlgebraicToIndex("f3");
            move5.PieceMoved = Piece.WN;
            move5.Type = MoveType.Quiet;
            board.MakeMove(move5);
            Console.WriteLine(board.AsFEN());
            var move6 = new Move();
            move6.SquareFrom = Util.AlgebraicToIndex("c7");
            move6.SquareTo = Util.AlgebraicToIndex("c5");
            move6.PieceMoved = Piece.BP;
            move6.Type = MoveType.DoublePawnPush;
            board.MakeMove(move6);
            Console.WriteLine(board.AsFEN());
            var move7 = new Move();
            move7.SquareFrom = Util.AlgebraicToIndex("e1");
            move7.SquareTo = Util.AlgebraicToIndex("g1");
            move7.PieceMoved = Piece.WK;
            move7.Type = MoveType.KingCastle;
            board.MakeMove(move7);
            Console.WriteLine(board.AsFEN());
            var move8 = new Move();
            move8.SquareFrom = Util.AlgebraicToIndex("d5");
            move8.SquareTo = Util.AlgebraicToIndex("d4");
            move8.PieceMoved = Piece.BP;
            move8.Type = MoveType.Quiet;
            board.MakeMove(move8);
            Console.WriteLine(board.AsFEN());
            var move9 = new Move();
            move9.SquareFrom = Util.AlgebraicToIndex("c2");
            move9.SquareTo = Util.AlgebraicToIndex("c4");
            move9.PieceMoved = Piece.WP;
            move9.Type = MoveType.DoublePawnPush;
            board.MakeMove(move9);
            Console.WriteLine(board.AsFEN());
            var move0 = new Move();
            move0.SquareFrom = Util.AlgebraicToIndex("d4");
            move0.SquareTo = Util.AlgebraicToIndex("c3");
            move0.PieceMoved = Piece.BP;
            move0.Type = MoveType.EnPassant;
            board.MakeMove(move0);
            Console.WriteLine(board.AsFEN());
            Console.WriteLine(board.PrintMailbox());
            for (int i = 0; i < 10; i++)
            {
                board.UnmakeMove();
                Console.WriteLine(board.AsFEN());
            }
            Console.WriteLine(board.PrintMailbox());
        }
    }
}