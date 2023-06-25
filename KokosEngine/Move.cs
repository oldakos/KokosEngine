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

        private uint data;

        internal uint SquareFrom
        {
            get
            {
                return data & m_SquareFrom;
            }
            set
            {
                data = (data & ~m_SquareFrom) | (value & m_SquareFrom);
            }
        }
        internal uint SquareTo
        {
            get
            {
                return (data & m_SquareTo) >> 6;
            }
            set
            {
                data = (data & ~m_SquareTo) | ((value << 6) & m_SquareTo);
            }
        }
        internal bool IsCapture
        {
            get
            {
                return (data & m_Capture) != 0;
            }
            set
            {
                if (value == true)
                {
                    data |= m_Capture;
                }
                else
                {
                    data &= ~m_Capture;
                }
            }
        }
        internal bool IsPromotion
        {
            get
            {
                return (data & m_Promotion) != 0;
            }
            set
            {
                if (value == true)
                {
                    data |= m_Promotion;
                }
                else
                {
                    data &= ~m_Promotion;
                }
            }
        }
        internal uint Special
        {
            get
            {
                return (data & m_Special) >> 12;
            }
            set
            {
                data = (data & ~m_Special) | ((value << 12) & m_Special);
            }
        }
        internal Piece PieceMoved
        {
            get
            {
                return (Piece)((data & m_PieceMoved) >> 16);
            }
            set
            {
                data = (data & ~m_PieceMoved) | (((uint)value << 16) & m_PieceMoved);
            }
        }
        internal Piece PieceCaptured
        {
            get
            {
                return (Piece)((data & m_PieceCaptured) >> 19);
            }
            set
            {
                data = (data & ~m_PieceCaptured) | (((uint)value << 19) & m_PieceCaptured);
            }
        }
        internal MoveType Type
        {
            get
            {
                return (MoveType)((data & m_MoveType) >> 12);
            }
            set
            {
                data = (data & ~m_MoveType) | (((uint)value << 12) & m_MoveType);
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
        Promotion = 0b1000,
        PromoCapture = 0b1100
    }
}
