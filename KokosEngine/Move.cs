using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    internal struct Move
    {
        static readonly uint m_SquareFrom = 0b00000000_00000000_00000000_00111111;
        static readonly uint m_SquareTo = 0b00000000_00000000_00001111_11000000;
        static readonly uint m_Promotion = 0b00000000_00000000_10000000_00000000;
        static readonly uint m_Capture = 0b00000000_00000000_01000000_00000000;
        static readonly uint m_Special = 0b00000000_00000000_00110000_00000000;
        static readonly uint m_PieceMoved = 0b00000000_00000111_00000000_00000000;
        static readonly uint m_PieceCaptured = 0b00000000_00111000_00000000_00000000;
        static readonly uint m_MoveType = 0b00000000_00000000_11110000_00000000;

        private uint _data;

        internal int SquareFrom
        {
            get
            {
                return (int)(_data & m_SquareFrom);
            }
            set
            {
                _data = (uint)((_data & ~m_SquareFrom) | (value & m_SquareFrom));
            }
        }
        internal int SquareTo
        {
            get
            {
                return (int)(_data & m_SquareTo) >> 6;
            }
            set
            {
                _data = (uint)((_data & ~m_SquareTo) | ((value << 6) & m_SquareTo));
            }
        }
        internal bool IsCapture
        {
            get
            {
                return (_data & m_Capture) != 0;
            }
            set
            {
                if (value == true)
                {
                    _data |= m_Capture;
                }
                else
                {
                    _data &= ~m_Capture;
                }
            }
        }
        internal bool IsPromotion
        {
            get
            {
                return (_data & m_Promotion) != 0;
            }
            set
            {
                if (value == true)
                {
                    _data |= m_Promotion;
                }
                else
                {
                    _data &= ~m_Promotion;
                }
            }
        }
        internal int Special
        {
            get
            {
                return (int)(_data & m_Special) >> 12;
            }
            set
            {
                _data = (uint)((_data & ~m_Special) | ((value << 12) & m_Special));
            }
        }
        internal Piece PieceMoved
        {
            get
            {
                return (Piece)((_data & m_PieceMoved) >> 16);
            }
            set
            {
                _data = (_data & ~m_PieceMoved) | (((uint)value << 16) & m_PieceMoved);
            }
        }
        internal Piece PieceCaptured
        {
            get
            {
                return (Piece)((_data & m_PieceCaptured) >> 19);
            }
            set
            {
                _data = (_data & ~m_PieceCaptured) | (((uint)value << 19) & m_PieceCaptured);
            }
        }
        internal MoveType Type
        {
            get
            {
                return (MoveType)((_data & m_MoveType) >> 12);
            }
            set
            {
                _data = (_data & ~m_MoveType) | (((uint)value << 12) & m_MoveType);
            }
        }
        internal Piece PiecePromotedTo
        {
            get
            {
                int index = Special;
                if (PieceMoved.IsBlack()) index += 6;
                return (Piece)index;
            }
        }
    }

    internal enum MoveType
    {
        Quiet = 0b0000,
        DoublePawnPush = 0b0001,
        KingCastle = 0b0010,
        QueenCastle = 0b0011,
        Capture = 0b0100,
        EnPassant = 0b0101,
        PromoKnight = 0b1000,
        PromoBishop = 0b1001,
        PromoRook = 0b1010,
        PromoQueen = 0b1011,
        PromoCaptureKnight = 0b1100,
        PromoCaptureBishop = 0b1101,
        PromoCaptureRook = 0b1110,
        PromoCaptureQueen = 0b1111,
    }
}
