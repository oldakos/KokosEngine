using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.White
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

        string IMove.GetPieceNotation()
        {
            return coordTo.ToString();
        }

        string IMove.GetSquareNotation()
        {
            return coordTo.South.South.ToString() + coordTo.ToString();
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                irrBefore,
                coordTo.South,
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
                new Square(coordTo, Piece.WhitePawn),
                new Square(coordTo.South.South, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            return new Square[]
            {
                new Square(coordTo, Piece.None),
                new Square(coordTo.South.South, Piece.WhitePawn)
            };
        }
    }
}
