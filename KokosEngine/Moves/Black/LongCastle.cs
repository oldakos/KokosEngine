using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.Black
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
            return "e8c8";
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
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
