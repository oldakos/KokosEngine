using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    /// <summary>
    /// A container for Castling rights, EnPassant coordinate and Halfmoves since capture or pawn move
    /// </summary>
    internal struct IrreversibleInformation
    {
        //let's try to fit into 4 bytes
        private byte _castlingFlags;    //1b
        private Coordinate _epCoord;    //1b
        private ushort _halfmoves;      //2b - let's hope our players won't repeat for more than 65535 moves

        internal static IrreversibleInformation GetStartingState()
        {
            return new IrreversibleInformation(0b1111, Coordinate.None, 0);
        }

        private IrreversibleInformation(byte castlingFlags, Coordinate epCoord, ushort halfmoves)
        {
            _castlingFlags = castlingFlags;
            _epCoord = epCoord;
            _halfmoves = halfmoves;
        }

        /// <summary>
        /// Copy the castling rights from `original` and set the epCoordinate and halfmoves.
        /// </summary>
        internal IrreversibleInformation(IrreversibleInformation original, Coordinate epCoord, ushort halfmoves)
        {
            _castlingFlags = original._castlingFlags;
            _epCoord = epCoord;
            _halfmoves = halfmoves;
        }

        /// <summary>
        /// Set every property of this IrreversibleInformation.
        /// </summary>
        internal IrreversibleInformation(bool castleWhiteShort, bool castleWhiteLong, bool castleBlackShort, bool castleBlackLong, Coordinate possibleEnPassant, int halfmovesSinceCaptureOrPawnMove)
        {
            _castlingFlags = 0;
            _epCoord = possibleEnPassant;
            _halfmoves = (ushort)halfmovesSinceCaptureOrPawnMove;
            CastleWhiteShort = castleWhiteShort;
            CastleWhiteLong = castleWhiteLong;
            CastleBlackShort = castleBlackShort;
            CastleBlackLong = castleBlackLong;
        }

        #region Bool properties for castling rights

        private const byte MASK_WHITE_SHORT = 0b0001;
        private const byte MASK_WHITE_LONG = 0b0010;
        private const byte MASK_BLACK_SHORT = 0b0100;
        private const byte MASK_BLACK_LONG = 0b1000;
        private const byte NOT_MASK_WHITE_SHORT = 0b1110;
        private const byte NOT_MASK_WHITE_LONG = 0b1101;
        private const byte NOT_MASK_BLACK_SHORT = 0b1011;
        private const byte NOT_MASK_BLACK_LONG = 0b0111;

        internal bool CastleWhiteShort
        {
            get
            {
                return (_castlingFlags & MASK_WHITE_SHORT) != 0;
            }
            init
            {
                if (value) { _castlingFlags |= MASK_WHITE_SHORT; }
                else { _castlingFlags &= NOT_MASK_WHITE_SHORT; }
            }
        }
        internal bool CastleWhiteLong
        {
            get
            {
                return (_castlingFlags & MASK_WHITE_LONG) != 0;
            }
            init
            {
                if (value) { _castlingFlags |= MASK_WHITE_LONG; }
                else { _castlingFlags &= NOT_MASK_WHITE_LONG; }
            }
        }
        internal bool CastleBlackShort
        {
            get
            {
                return (_castlingFlags & MASK_BLACK_SHORT) != 0;
            }
            init
            {
                if (value) { _castlingFlags |= MASK_BLACK_SHORT; }
                else { _castlingFlags &= NOT_MASK_BLACK_SHORT; }
            }
        }
        internal bool CastleBlackLong
        {
            get
            {
                return (_castlingFlags & MASK_BLACK_LONG) != 0;
            }
            init
            {
                if (value) { _castlingFlags |= MASK_BLACK_LONG; }
                else { _castlingFlags &= NOT_MASK_BLACK_LONG; }
            }
        }

        #endregion
        internal Coordinate PossibleEnPassant { get => _epCoord; }
        internal Bitboard EPBitboard { get => new Bitboard(_epCoord); }
        internal ushort HalfmovesSinceCaptureOrPawnMove { get => _halfmoves; }
    }
}
