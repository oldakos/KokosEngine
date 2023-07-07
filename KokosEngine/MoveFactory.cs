using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    internal static class MoveFactory
    {
        internal static Move QuietMove(int squareFrom, int squareTo, Piece pieceMoved)
        {
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareTo;
            move.Type = MoveType.Quiet;
            move.PieceMoved = pieceMoved;

            return move;
        }
        internal static Move Capture(int squareFrom, int squareTo, Piece pieceMoved, Piece pieceCaptured)
        {
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareTo;
            move.Type = MoveType.Capture;
            move.PieceMoved = pieceMoved;
            move.PieceCaptured = pieceCaptured;

            return move;
        }
        internal static Move WhiteDoublePawnPush(int squareFrom)
        {
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareFrom + 16;
            move.Type = MoveType.DoublePawnPush;
            move.PieceMoved = Piece.WP;

            return move;
        }
        internal static Move BlackDoublePawnPush(int squareFrom)
        {
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareFrom - 16;
            move.Type = MoveType.DoublePawnPush;
            move.PieceMoved = Piece.BP;

            return move;
        }
        internal static Move[] WhitePushPromotions(int squareFrom)
        {
            Move[] moves = new Move[4];
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareFrom + 8;
            move.PieceMoved = Piece.WP;

            moves[0] = move;
            moves[0].Type = MoveType.PromoKnight;

            moves[1] = move;
            moves[1].Type = MoveType.PromoBishop;

            moves[2] = move;
            moves[2].Type = MoveType.PromoRook;

            moves[3] = move;
            moves[3].Type = MoveType.PromoQueen;

            return moves;
        }
        internal static Move[] BlackPushPromotions(int squareFrom)
        {
            Move[] moves = new Move[4];
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareFrom - 8;
            move.PieceMoved = Piece.BP;

            moves[0] = move;
            moves[0].Type = MoveType.PromoKnight;

            moves[1] = move;
            moves[1].Type = MoveType.PromoBishop;

            moves[2] = move;
            moves[2].Type = MoveType.PromoRook;

            moves[3] = move;
            moves[3].Type = MoveType.PromoQueen;

            return moves;
        }
        internal static Move[] WhiteCapturePromotions(int squareFrom, int squareTo, Piece capturedPiece)
        {
            Move[] moves = new Move[4];
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareTo;
            move.PieceMoved = Piece.WP;
            move.PieceCaptured = capturedPiece;

            moves[0] = move;
            moves[0].Type = MoveType.PromoCaptureKnight;

            moves[1] = move;
            moves[1].Type = MoveType.PromoCaptureBishop;

            moves[2] = move;
            moves[2].Type = MoveType.PromoCaptureRook;

            moves[3] = move;
            moves[3].Type = MoveType.PromoCaptureQueen;

            return moves;
        }
        internal static Move[] BlackCapturePromotions(int squareFrom, int squareTo, Piece capturedPiece)
        {
            Move[] moves = new Move[4];
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareTo;
            move.PieceMoved = Piece.BP;
            move.PieceCaptured = capturedPiece;

            moves[0] = move;
            moves[0].Type = MoveType.PromoCaptureKnight;

            moves[1] = move;
            moves[1].Type = MoveType.PromoCaptureBishop;

            moves[2] = move;
            moves[2].Type = MoveType.PromoCaptureRook;

            moves[3] = move;
            moves[3].Type = MoveType.PromoCaptureQueen;

            return moves;
        }
        internal static Move WhiteEnPassant(int squareFrom, int squareTo)
        {
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareTo;
            move.Type = MoveType.EnPassant;
            move.PieceMoved = Piece.WP;
            move.PieceCaptured = Piece.BP;

            return move;
        }
        internal static Move BlackEnPassant(int squareFrom, int squareTo)
        {
            Move move = new Move();

            move.SquareFrom = squareFrom;
            move.SquareTo = squareTo;
            move.Type = MoveType.EnPassant;
            move.PieceMoved = Piece.BP;
            move.PieceCaptured = Piece.WP;

            return move;
        }
    }
}
