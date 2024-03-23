using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.Black
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
            this.irrBefore = irreversibles;
            this.coordFrom = from;
            this.coordTo = to;
            this.pieceMoved = pieceMoved;
            this.pieceCaptured = pieceCaptured;
        }
        string IMove.GetPieceNotation()
        {
            if (pieceMoved == Piece.BlackPawn)
            {
                return coordFrom.GetFileString() + "x" + coordTo.ToString();
            }
            else
            {
                return pieceMoved.ToNotation() + "x" + coordTo.ToString();
            }
        }
        string IMove.GetSquareNotation()
        {
            return coordFrom.ToString() + coordTo.ToString();
        }
        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                    (coordTo == Coordinate.e1 || coordTo == Coordinate.h1) ? false : irrBefore.CastleWhiteShort,
                    (coordTo == Coordinate.e1 || coordTo == Coordinate.a1) ? false : irrBefore.CastleWhiteLong,
                    (coordFrom == Coordinate.e8 || coordFrom == Coordinate.h8) ? false : irrBefore.CastleBlackShort,
                    (coordFrom == Coordinate.e8 || coordFrom == Coordinate.a8) ? false : irrBefore.CastleBlackLong,
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
