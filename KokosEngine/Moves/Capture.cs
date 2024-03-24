using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves
{
    internal class Capture : IMove
    {
        private IrreversibleInformation irrBefore;
        private Coordinate coordFrom;
        private Coordinate coordTo;
        private Piece pieceMoved;
        private Piece pieceCaptured;

        public Capture(IrreversibleInformation irreversibles, Coordinate from, Coordinate to, Piece pieceMoved, Piece pieceCaptured)
        {
            irrBefore = irreversibles;
            coordFrom = from;
            coordTo = to;
            this.pieceMoved = pieceMoved;
            this.pieceCaptured = pieceCaptured;
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
                return coordFrom.GetFileString() + "x" + coordTo.ToString();
            }
            else
            {
                return pieceMoved.ToNotation() + "x" + coordTo.ToString();
            }
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                    coordFrom.ImpactsCastleWhiteShort() || coordTo.ImpactsCastleWhiteShort() ? false : irrBefore.CastleWhiteShort,
                    coordFrom.ImpactsCastleWhiteLong() || coordTo.ImpactsCastleWhiteLong() ? false : irrBefore.CastleWhiteLong,
                    coordFrom.ImpactsCastleBlackShort() || coordTo.ImpactsCastleBlackShort() ? false : irrBefore.CastleBlackShort,
                    coordFrom.ImpactsCastleBlackLong() || coordTo.ImpactsCastleBlackLong() ? false : irrBefore.CastleBlackLong,
                    Coordinate.None,
                    0);
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
                new Square(coordTo, pieceCaptured),
                new Square(coordFrom, pieceMoved)
            };
        }
    }
}
