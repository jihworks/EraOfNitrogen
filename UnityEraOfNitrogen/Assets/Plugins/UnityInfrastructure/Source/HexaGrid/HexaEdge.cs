// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.HexaGrid
{
    public class HexaEdge
    {
        public HexaMap Map => Vertex0.Map;

        public HexaVertex Vertex0 { get; }
        public HexaVertex Vertex1 { get; }

        public HexaEdgeIndex Index { get; }

        /// <summary>
        /// Cell on the right side of the edge, when looking from <see cref="Vertex0"/> to <see cref="Vertex1"/>.
        /// </summary>
        /// <remarks>
        /// This value is always NOT <c>null</c>.<br/>
        /// If see the edge from center of this cell, the edge direction(from 0 to 1) is always CW.
        /// </remarks>
        public HexaCell RightCell { get; }
        /// <summary>
        /// Cell on the left side of the edge, when looking from <see cref="Vertex0"/> to <see cref="Vertex1"/>.
        /// </summary>
        /// <remarks>
        /// If see the edge from center of this cell, the edge direction(from 0 to 1) is always CCW.<br/>
        /// Can be <c>null</c> if there is no cell on the left side of the edge such as border edge of the map.
        /// </remarks>
        public HexaCell? LeftCell { get; internal set; }

        public HexaEdge(HexaVertex vertex0, HexaVertex vertex1, HexaEdgeIndex index, HexaCell rightCell)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
            Index = index;
            RightCell = rightCell;
        }

        public bool Is0(HexaVertex vertex)
        {
            return Vertex0 == vertex;
        }
        public bool Is1(HexaVertex vertex)
        {
            return Vertex1 == vertex;
        }

        public bool IsSharedEdgeBetween(HexaCell cell0, HexaCell cell1)
        {
            if (LeftCell is null)
            {
                return false;
            }
            if ((RightCell == cell0 && LeftCell == cell1) ||
                (RightCell == cell1 && LeftCell == cell0))
            {
                return true;
            }
            return false;
        }

        public bool TryGetIsCwOrder(HexaVertex vertex0, HexaVertex vertex1, out bool CwOrCcw)
        {
            if (Vertex0 == vertex0 && Vertex1 == vertex1)
            {
                CwOrCcw = true;
                return true;
            }
            if (Vertex1 == vertex0 && Vertex0 == vertex1)
            {
                CwOrCcw = false;
                return true;
            }
            CwOrCcw = default;
            return false;
        }
        public bool IsCwOrder(HexaVertex vertex0, HexaVertex vertex1)
        {
            if (!TryGetIsCwOrder(vertex0, vertex1, out bool result))
            {
                throw new InvalidOperationException($"Vertex {vertex0.Index} and vertex {vertex1.Index} is not element of the edge {Index}");
            }
            return result;
        }

        public void GetCwOrder(HexaCell cell, out HexaVertex vertex0, out HexaVertex vertex1)
        {
            if (!TryGetCwOrder(cell, out vertex0, out vertex1))
            {
                throw new ArgumentException($"The cell {cell.Coord} is not adjacent to edge {Index}.", nameof(cell));
            }
        }

        public bool TryGetCwOrder(HexaCell cell, out HexaVertex vertex0, out HexaVertex vertex1)
        {
            if (RightCell == cell)
            {
                vertex0 = Vertex0;
                vertex1 = Vertex1;
                return true;
            }
            if (LeftCell == cell)
            {
                vertex0 = Vertex1;
                vertex1 = Vertex0;
                return true;
            }

            vertex0 = Vertex0;
            vertex1 = Vertex1;
            return false;
        }

        public IEnumerable<HexaCell> EnumerateCells()
        {
            yield return RightCell;

            if (LeftCell is not null)
            {
                yield return LeftCell;
            }
        }

        public IEnumerable<HexaVertex> EnumerateVertices()
        {
            yield return Vertex0;
            yield return Vertex1;
        }
    }
}
