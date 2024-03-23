using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.White
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
            return "e1g1";
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
                new Square(Coordinate.e1, Piece.None),
                new Square(Coordinate.f1, Piece.WhiteRook),
                new Square(Coordinate.g1, Piece.WhiteKing),
                new Square(Coordinate.h1, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            return new Square[]
            {
                new Square(Coordinate.e1, Piece.WhiteKing),
                new Square(Coordinate.f1, Piece.None),
                new Square(Coordinate.g1, Piece.None),
                new Square(Coordinate.h1, Piece.WhiteRook)
            };
        }
    }
}
