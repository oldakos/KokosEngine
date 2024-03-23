using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves.White
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
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.WhiteKnight, pieceCaptured),
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.WhiteBishop, pieceCaptured),
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.WhiteRook, pieceCaptured),
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.WhiteQueen, pieceCaptured)
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
                    irrBefore.CastleWhiteShort,
                    irrBefore.CastleWhiteLong,
                    (coordTo == Coordinate.e8 || coordTo == Coordinate.h8) ? false : irrBefore.CastleBlackShort,
                    (coordTo == Coordinate.e8 || coordTo == Coordinate.a8) ? false : irrBefore.CastleBlackLong,
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
                new Square(coordFrom, Piece.WhitePawn)
            };
        }
    }
}
