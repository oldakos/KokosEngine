using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves
{
    internal class DoublePawnPush : IMove
    {
        private IrreversibleInformation irrBefore;
        private Coordinate coordTo;

        public DoublePawnPush(IrreversibleInformation irreversibles, Coordinate coordTo)
        {
            irrBefore = irreversibles;
            this.coordTo = coordTo;
        }

        private bool IsWhite() => coordTo.GetRankIndex() <= 3;

        Coordinate IMove.CoordinateFrom()
        {
            if (IsWhite()) return coordTo.South.South;
            else return coordTo.North.North;
        }

        Coordinate IMove.CoordinateTo()
        {
            return coordTo;
        }

        string IMove.GetPieceNotation()
        {
            return coordTo.ToString();
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                irrBefore,
                IsWhite() ? coordTo.South : coordTo.North,
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
                new Square(coordTo, Piece.WhitePawn),
                new Square(((IMove)this).CoordinateFrom(), Piece.None)
                };
            else
                return new Square[]
                {
                new Square(coordTo, Piece.BlackPawn),
                new Square(((IMove)this).CoordinateFrom(), Piece.None)
                };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            if (IsWhite())
                return new Square[]
                {
                new Square(coordTo, Piece.None),
                new Square(((IMove)this).CoordinateFrom(), Piece.WhitePawn)
                };
            else
                return new Square[]
                {
                new Square(coordTo, Piece.None),
                new Square(((IMove)this).CoordinateFrom(), Piece.BlackPawn)
                };
        }
    }
}
