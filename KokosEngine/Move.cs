﻿using System;
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
        static readonly uint m_Promotion = 0b00000000_00000000_10000000_11000000;
        static readonly uint m_Capture = 0b00000000_00000000_01000000_11000000;
        static readonly uint m_Special = 0b00000000_00000000_00110000_11000000;

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
    }
}
