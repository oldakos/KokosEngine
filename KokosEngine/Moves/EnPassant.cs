using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves
{
    internal class EnPassant : IMove
    {
        private IrreversibleInformation irrBefore;
        private Coordinate coordFrom;

        public EnPassant(IrreversibleInformation irreversibles, Coordinate coordFrom)
        {
            irrBefore = irreversibles;
            this.coordFrom = coordFrom;
        }
        private bool IsWhite()
        {
            return irrBefore.PossibleEnPassant.IsNorthOf(coordFrom);
        }

        Coordinate IMove.CoordinateFrom()
        {
            return coordFrom;
        }

        Coordinate IMove.CoordinateTo()
        {
            return irrBefore.PossibleEnPassant;
        }

        string IMove.GetPieceNotation()
        {
            return coordFrom.GetFileString() + "x" + irrBefore.PossibleEnPassant.ToString();
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
                new Square(irrBefore.PossibleEnPassant, Piece.WhitePawn),
                new Square(irrBefore.PossibleEnPassant.South, Piece.None),
                new Square(coordFrom, Piece.None)
                };
            else
                return new Square[]
                {
                new Square(irrBefore.PossibleEnPassant, Piece.BlackPawn),
                new Square(irrBefore.PossibleEnPassant.North, Piece.None),
                new Square(coordFrom, Piece.None)
                };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            if (IsWhite())
                return new Square[]
                {
                new Square(irrBefore.PossibleEnPassant, Piece.None),
                new Square(irrBefore.PossibleEnPassant.South, Piece.BlackPawn),
                new Square(coordFrom, Piece.WhitePawn)
                };
            else
                return new Square[]
                {
                new Square(irrBefore.PossibleEnPassant, Piece.None),
                new Square(irrBefore.PossibleEnPassant.North, Piece.WhitePawn),
                new Square(coordFrom, Piece.BlackPawn)
                };
        }
    }
}
