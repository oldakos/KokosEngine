using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.White
{
    internal class Promotion : IMove
    {
        private IrreversibleInformation irrBefore;
        private Piece piecePromotedTo;
        private Coordinate coordTo;

        public static Promotion[] GetAll(IrreversibleInformation irreversibles, Coordinate coordTo)
        {
            return new Promotion[]
            {
                new Promotion(irreversibles, Piece.WhiteKnight, coordTo),
                new Promotion(irreversibles, Piece.WhiteBishop, coordTo),
                new Promotion(irreversibles, Piece.WhiteRook, coordTo),
                new Promotion(irreversibles, Piece.WhiteQueen, coordTo)
            };
        }
        public Promotion(IrreversibleInformation irreversibles, Piece piecePromotedTo, Coordinate coordTo)
        {
            irrBefore = irreversibles;
            this.piecePromotedTo = piecePromotedTo;
            this.coordTo = coordTo;
        }

        string IMove.GetPieceNotation()
        {
            return coordTo.ToString() + "=" + piecePromotedTo.ToNotation();
        }

        string IMove.GetSquareNotation()
        {
            return coordTo.South.ToString() + coordTo.ToString() + piecePromotedTo.ToNotation();
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
                new Square(coordTo, piecePromotedTo),
                new Square(coordTo.South, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            return new Square[]
            {
                new Square(coordTo, Piece.None),
                new Square(coordTo.South, Piece.WhitePawn)
            };
        }
    }
}
