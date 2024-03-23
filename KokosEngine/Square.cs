using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    /// <summary>
    /// A tuple of a Coordinate and a Piece.
    /// </summary>
    internal struct Square
    {
        internal Square(Coordinate coordinate, Piece piece)
        {
            Coordinate = coordinate;
            Piece = piece;
        }
        internal Coordinate Coordinate { get; set; }
        internal Piece Piece { get; set; }
    }
}
