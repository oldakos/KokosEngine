using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.Black
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
            return coordTo.North.North.ToString() + coordTo.ToString();
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                irrBefore,
                coordTo.North,
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
                new Square(coordTo, Piece.BlackPawn),
                new Square(coordTo.North.North, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            return new Square[]
            {
                new Square(coordTo, Piece.None),
                new Square(coordTo.North.North, Piece.BlackPawn)
            };
        }
    }
}
