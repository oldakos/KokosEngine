using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.Black
{
    internal class PromotionCapture : IMove
    {
        private IrreversibleInformation irrBefore;
        private Coordinate coordFrom;
        private Coordinate coordTo;
        private Piece piecePromotedTo;
        private Piece pieceCaptured;

        public static PromotionCapture[] GetAll(IrreversibleInformation irreversibles, Coordinate coordFrom, Coordinate coordTo, Piece pieceCaptured)
        {
            return new PromotionCapture[]
            {
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.BlackKnight, pieceCaptured),
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.BlackBishop, pieceCaptured),
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.BlackRook, pieceCaptured),
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.BlackQueen, pieceCaptured)
            };
        }
        public PromotionCapture(IrreversibleInformation irreversibles, Coordinate coordFrom, Coordinate coordTo, Piece piecePromotedTo, Piece pieceCaptured)
        {
            irrBefore = irreversibles;
            this.coordFrom = coordFrom;
            this.coordTo = coordTo;
            this.piecePromotedTo = piecePromotedTo;
            this.pieceCaptured = pieceCaptured;
        }

        string IMove.GetPieceNotation()
        {
            return coordFrom.GetFileString() + "x" + coordTo.ToString() + "=" + piecePromotedTo.ToNotation();
        }

        string IMove.GetSquareNotation()
        {
            return coordFrom.ToString() + coordTo.ToString() + piecePromotedTo.ToNotation();
        }

        IrreversibleInformation IMove.IrreversiblesAfterDo()
        {
            return new IrreversibleInformation(
                    (coordTo == Coordinate.e1 || coordTo == Coordinate.h1) ? false : irrBefore.CastleWhiteShort,
                    (coordTo == Coordinate.e1 || coordTo == Coordinate.a1) ? false : irrBefore.CastleWhiteLong,
                    irrBefore.CastleBlackShort,
                    irrBefore.CastleBlackLong,
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
                new Square(coordFrom, Piece.None)
            };
        }

        IEnumerable<Square> IMove.UpdatesAfterUndo()
        {
            return new Square[]
            {
                new Square(coordTo, pieceCaptured),
                new Square(coordFrom, Piece.BlackPawn)
            };
        }
    }
}
