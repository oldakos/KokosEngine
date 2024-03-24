using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves
{
    internal class Promotion : IMove
    {
        private IrreversibleInformation irrBefore;
        private Piece piecePromotedTo;
        private Coordinate coordTo;

        public static Promotion[] GetAll(IrreversibleInformation irreversibles, Coordinate coordTo)
        {
            if (coordTo.GetRankIndex() > 0)
                return new Promotion[]
                {
                new Promotion(irreversibles, Piece.WhiteKnight, coordTo),
                new Promotion(irreversibles, Piece.WhiteBishop, coordTo),
                new Promotion(irreversibles, Piece.WhiteRook, coordTo),
                new Promotion(irreversibles, Piece.WhiteQueen, coordTo)
                };
            else
                return new Promotion[]
                {
                new Promotion(irreversibles, Piece.BlackKnight, coordTo),
                new Promotion(irreversibles, Piece.BlackBishop, coordTo),
                new Promotion(irreversibles, Piece.BlackRook, coordTo),
                new Promotion(irreversibles, Piece.BlackQueen, coordTo)
                };
        }
        public Promotion(IrreversibleInformation irreversibles, Piece piecePromotedTo, Coordinate coordTo)
        {
            irrBefore = irreversibles;
            this.piecePromotedTo = piecePromotedTo;
            this.coordTo = coordTo;
        }

        private bool IsWhite() => coordTo.GetRankIndex() > 0;

        string IMove.GetPieceNotation()
        {
            return coordTo.ToString() + "=" + piecePromotedTo.ToNotation();
        }

        string IMove.GetSquareNotation()
        {
            return ((IMove)this).CoordinateFrom() + coordTo.ToString() + piecePromotedTo.ToNotation();
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                irrBefore,
                Coordinate.None,
                0);
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
                new Square(coordTo, piecePromotedTo),
                new Square(coordTo.South, Piece.None)
                };
            else
                return new Square[]
                {
                new Square(coordTo, piecePromotedTo),
                new Square(coordTo.North, Piece.None)
                };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            if (IsWhite())
                return new Square[]
                {
                new Square(coordTo, Piece.None),
                new Square(coordTo.South, Piece.WhitePawn)
                };
            else
                return new Square[]
                {
                new Square(coordTo, Piece.None),
                new Square(coordTo.North, Piece.BlackPawn)
                };
        }

        Coordinate IMove.CoordinateFrom()
        {
            if (IsWhite()) return coordTo.South;
            else return coordTo.North;
        }

        Coordinate IMove.CoordinateTo()
        {
            return coordTo;
        }
    }
}
