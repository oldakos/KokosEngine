using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine.Moves
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
            if (coordTo.GetRankIndex() > 0)
                return new PromotionCapture[]
                {
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.WhiteKnight, pieceCaptured),
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.WhiteBishop, pieceCaptured),
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.WhiteRook, pieceCaptured),
                new PromotionCapture(irreversibles,coordFrom,coordTo, Piece.WhiteQueen, pieceCaptured)
                };
            else
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

        private bool IsWhite() => coordTo.GetRankIndex() > 0;

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
                    coordTo.ImpactsCastleWhiteShort() ? false : irrBefore.CastleWhiteShort,
                    coordTo.ImpactsCastleWhiteLong() ? false : irrBefore.CastleWhiteLong,
                    coordTo.ImpactsCastleBlackShort() ? false : irrBefore.CastleBlackShort,
                    coordTo.ImpactsCastleBlackLong() ? false : irrBefore.CastleBlackLong,
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
            if (IsWhite())
                return new Square[]
                {
                new Square(coordTo, pieceCaptured),
                new Square(coordFrom, Piece.WhitePawn)
                };
            else
                return new Square[]
                {
                new Square(coordTo, pieceCaptured),
                new Square(coordFrom, Piece.BlackPawn)
                };
        }

        Coordinate IMove.CoordinateFrom()
        {
            return coordFrom;
        }

        Coordinate IMove.CoordinateTo()
        {
            return coordTo;
        }
    }
}
