using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.Black
{
    internal class Quiet : IMove
    {
        private IrreversibleInformation irrBefore;
        private Coordinate coordFrom;
        private Coordinate coordTo;
        private Piece pieceMoved;

        public Quiet(IrreversibleInformation irreversibles, Coordinate coordFrom, Coordinate coordTo, Piece pieceMoved)
        {
            irrBefore = irreversibles;
            this.coordFrom = coordFrom;
            this.coordTo = coordTo;
            this.pieceMoved = pieceMoved;
        }

        string IMove.GetPieceNotation()
        {
            if (pieceMoved == Piece.BlackPawn)
            {
                return coordTo.ToString();
            }
            else
            {
                return pieceMoved.ToNotation() + coordTo.ToString();
            }
        }

        string IMove.GetSquareNotation()
        {
            return coordFrom.ToString() + coordTo.ToString();
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                    irrBefore.CastleWhiteShort,
                    irrBefore.CastleWhiteLong,
                    (coordFrom == Coordinate.e8 || coordFrom == Coordinate.h8) ? false : irrBefore.CastleBlackShort,
                    (coordFrom == Coordinate.e8 || coordFrom == Coordinate.a8) ? false : irrBefore.CastleBlackLong,
                    Coordinate.None,
                    (pieceMoved == Piece.BlackPawn) ? 0 : irrBefore.HalfmovesSinceCaptureOrPawnMove + 1);
        }

        IrreversibleInformation IMove.IrreversiblesAfterUndo()
        {
            return irrBefore;
        }

        IEnumerable<Square> IMove.UpdatesAfterDo()
        {
            return new Square[]
            {
                new Square(coordTo, pieceMoved),
                new Square(coordFrom, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            return new Square[]
            {
                new Square(coordTo, Piece.None),
                new Square(coordFrom, pieceMoved)
            };
        }
    }
}
