using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.White
{
    internal struct LongCastle : IMove
    {
        private IrreversibleInformation irrBefore;

        public LongCastle(IrreversibleInformation irreversibles)
        {
            irrBefore = irreversibles;
        }

        string IMove.GetPieceNotation()
        {
            return "O-O-O";
        }

        string IMove.GetSquareNotation()
        {
            return "e1c1";
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                    false,
                    false,
                    irrBefore.CastleBlackShort,
                    irrBefore.CastleBlackLong,
                    Coordinate.None,
                    irrBefore.HalfmovesSinceCaptureOrPawnMove + 1);
        }

        IrreversibleInformation IMove.IrreversiblesAfterUndo()
        {
            return irrBefore;
        }

        IEnumerable<Square> IMove.UpdatesAfterDo()
        {
            return new Square[]
            {
                new Square(Coordinate.a1, Piece.None),
                new Square(Coordinate.c1, Piece.WhiteKing),
                new Square(Coordinate.d1, Piece.WhiteRook),
                new Square(Coordinate.e1, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            return new Square[]
            {
                new Square(Coordinate.a1, Piece.WhiteRook),
                new Square(Coordinate.c1, Piece.None),
                new Square(Coordinate.d1, Piece.None),
                new Square(Coordinate.e1, Piece.WhiteKing)
            };
        }
    }
}
