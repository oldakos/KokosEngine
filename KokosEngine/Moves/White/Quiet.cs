using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.White
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
            if (pieceMoved == Piece.WhitePawn)
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
                    (coordFrom == Coordinate.e1 || coordFrom == Coordinate.h1) ? false : irrBefore.CastleWhiteShort,
                    (coordFrom == Coordinate.e1 || coordFrom == Coordinate.a1) ? false : irrBefore.CastleWhiteLong,
                    irrBefore.CastleBlackShort,
                    irrBefore.CastleBlackLong,
                    Coordinate.None,
                    (pieceMoved == Piece.WhitePawn) ? 0 : irrBefore.HalfmovesSinceCaptureOrPawnMove + 1);
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
