using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves
{
    internal class QuietMove : IMove
    {
        private IrreversibleInformation irrBefore;
        private Coordinate coordFrom;
        private Coordinate coordTo;
        private Piece pieceMoved;

        public QuietMove(IrreversibleInformation irreversibles, Coordinate coordFrom, Coordinate coordTo, Piece pieceMoved)
        {
            irrBefore = irreversibles;
            this.coordFrom = coordFrom;
            this.coordTo = coordTo;
            this.pieceMoved = pieceMoved;
        }

        Coordinate IMove.CoordinateFrom()
        {
            return coordFrom;
        }

        Coordinate IMove.CoordinateTo()
        {
            return coordTo;
        }

        string IMove.GetPieceNotation()
        {
            if (pieceMoved.IsPawn())
            {
                return coordTo.ToString();
            }
            else
            {
                return pieceMoved.ToNotation() + coordTo.ToString();
            }
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                    coordFrom.ImpactsCastleWhiteShort() ? false : irrBefore.CastleWhiteShort,
                    coordFrom.ImpactsCastleWhiteLong() ? false : irrBefore.CastleWhiteLong,
                    coordFrom.ImpactsCastleBlackShort() ? false : irrBefore.CastleBlackShort,
                    coordFrom.ImpactsCastleBlackLong() ? false : irrBefore.CastleBlackLong,
                    Coordinate.None,
                    pieceMoved.IsPawn() ? 0 : irrBefore.HalfmovesSinceCaptureOrPawnMove + 1);
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
