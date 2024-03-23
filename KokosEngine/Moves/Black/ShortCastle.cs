using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.Black
{
    internal struct ShortCastle : IMove
    {
        private IrreversibleInformation irrBefore;

        public ShortCastle(IrreversibleInformation irreversibles)
        {
            irrBefore = irreversibles;
        }

        string IMove.GetPieceNotation()
        {
            return "O-O";
        }

        string IMove.GetSquareNotation()
        {
            return "e8g8";
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
                new Square(Coordinate.e8, Piece.None),
                new Square(Coordinate.f8, Piece.BlackRook),
                new Square(Coordinate.g8, Piece.BlackKing),
                new Square(Coordinate.h8, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            return new Square[]
            {
                new Square(Coordinate.e8, Piece.BlackKing),
                new Square(Coordinate.f8, Piece.None),
                new Square(Coordinate.g8, Piece.None),
                new Square(Coordinate.h8, Piece.BlackRook)
            };
        }
    }
}
