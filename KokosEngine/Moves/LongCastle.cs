using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves
{
    internal struct LongCastle : IMove
    {
        private IrreversibleInformation irrBefore;
        private Color color;

        public LongCastle(IrreversibleInformation irreversibles, Color color)
        {
            irrBefore = irreversibles;
            this.color = color;
        }

        private bool IsWhite() => color == Color.White;

        Coordinate IMove.CoordinateFrom()
        {
            if (IsWhite()) return Coordinate.e1;
            else return Coordinate.e8;
        }

        Coordinate IMove.CoordinateTo()
        {
            if (IsWhite()) return Coordinate.c1;
            else return Coordinate.c8;
        }

        string IMove.GetPieceNotation()
        {
            return "O-O-O";
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            if (IsWhite())
                return new IrreversibleInformation(
                        false,
                        false,
                        irrBefore.CastleBlackShort,
                        irrBefore.CastleBlackLong,
                        Coordinate.None,
                        irrBefore.HalfmovesSinceCaptureOrPawnMove + 1);
            else
                return new IrreversibleInformation(
                        irrBefore.CastleWhiteShort,
                        irrBefore.CastleWhiteLong,
                        false,
                        false,
                        Coordinate.None,
                        irrBefore.HalfmovesSinceCaptureOrPawnMove + 1);
        }

        IrreversibleInformation IMove.IrreversiblesAfterUndo()
        {
            return irrBefore;
        }

        IEnumerable<Square> IMove.UpdatesAfterDo()
        {
            if (IsWhite())
                return new Square[]
                {
                new Square(Coordinate.a1, Piece.None),
                new Square(Coordinate.c1, Piece.WhiteKing),
                new Square(Coordinate.d1, Piece.WhiteRook),
                new Square(Coordinate.e1, Piece.None)
                };
            else
                return new Square[]
            {
                new Square(Coordinate.a8, Piece.None),
                new Square(Coordinate.c8, Piece.BlackKing),
                new Square(Coordinate.d8, Piece.BlackRook),
                new Square(Coordinate.e8, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            if (IsWhite())
                return new Square[]
                {
                new Square(Coordinate.a1, Piece.WhiteRook),
                new Square(Coordinate.c1, Piece.None),
                new Square(Coordinate.d1, Piece.None),
                new Square(Coordinate.e1, Piece.WhiteKing)
                };
            else
                return new Square[]
            {
                new Square(Coordinate.a8, Piece.BlackRook),
                new Square(Coordinate.c8, Piece.None),
                new Square(Coordinate.d8, Piece.None),
                new Square(Coordinate.e8, Piece.BlackKing)
            };
        }
    }
}
