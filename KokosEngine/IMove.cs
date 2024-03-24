using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    internal interface IMove
    {
        /// <summary>
        /// Get the square-based notation of the move, e.g. "a2a4" or "e7e8Q".
        /// </summary>
        internal string GetSquareNotation()
        {
            return CoordinateFrom().ToString() + CoordinateTo().ToString();
        }
        /// <summary>
        /// Get the piece-based notation of the move, e.g. "a4" or "e8=Q" or "Nxf5".
        /// </summary>
        internal string GetPieceNotation();
        /// <summary>
        /// Get the list of Square objects that are to be set on the board by doing the move (including setting empty squares).
        /// </summary>
        internal IEnumerable<Square> UpdatesAfterDo();
        /// <summary>
        /// Get the list of Square objects that are to be set on the board by undoing the move (including setting empty squares).
        /// </summary>
        internal IEnumerable<Square> UpdatesAfterUndo();
        /// <summary>
        /// Get the IrreversibleInformation resulting from the move.
        /// </summary>
        internal IrreversibleInformation IrreversiblesAfterDo();
        /// <summary>
        /// Get the IrreversibleInformation as it was before the move.
        /// </summary>
        internal IrreversibleInformation IrreversiblesAfterUndo();
        internal Coordinate CoordinateFrom();
        internal Coordinate CoordinateTo();
    }
}
