using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.Black
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

        string IMove.GetPieceNotation()
        {
            return coordFrom.GetFileString() + "x" + irrBefore.PossibleEnPassant.ToString();
        }

        string IMove.GetSquareNotation()
        {
            return coordFrom.ToString() + irrBefore.PossibleEnPassant.ToString();
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
            return new Square[]
            {
                new Square(irrBefore.PossibleEnPassant, Piece.BlackPawn),
                new Square(irrBefore.PossibleEnPassant.North, Piece.None),
                new Square(coordFrom, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            return new Square[]
            {
                new Square(irrBefore.PossibleEnPassant, Piece.None),
                new Square(irrBefore.PossibleEnPassant.North, Piece.WhitePawn),
                new Square(coordFrom, Piece.BlackPawn)
            };
        }
    }
}
