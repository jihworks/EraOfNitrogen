// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.HexaGrid
{
    public class HexaVertex
    {
        public HexaMap Map { get; }

        public HexaVertexIndex Index { get; }
        public HexaCoordF Coord { get; }

        internal HexaCell?[] CellsInternal { get; } = new HexaCell?[6];
        /// <remarks>
        /// Index: <see cref="HexaVertexPosition"/><br/>
        /// Means contacting direction to the cell from this vertex.<br/>
        /// Can be <c>null</c> if there is no cell in that direction.
        /// </remarks>
        public IReadOnlyList<HexaCell?> Cells => CellsInternal;

        internal HexaEdge?[] EdgesInternal { get; } = new HexaEdge?[6];
        /// <remarks>
        /// Index: <see cref="HexaVertexPosition"/><br/>
        /// Means connecting direction of another vertex on the edge from this vertex.<br/>
        /// Can be <c>null</c> if there is no edge in that direction.
        /// </remarks>
        public IReadOnlyList<HexaEdge?> Edges => EdgesInternal;

        public HexaVertex(HexaMap map, HexaVertexIndex index, HexaCoordF coord)
        {
            Map = map;
            Index = index;
            Coord = coord;
        }

        public HexaCell? GetCell(HexaVertexPosition position)
        {
            return CellsInternal[(int)position];
        }
        public bool TryGetPosition(HexaCell cell, out HexaVertexPosition position)
        {
            int index = CellsInternal.IndexOf(cell);
            if (index < 0)
            {
                position = default;
                return false;
            }

            position = (HexaVertexPosition)index;
            return true;
        }
        public HexaVertexPosition GetPosition(HexaCell cell)
        {
            if (!TryGetPosition(cell, out HexaVertexPosition result))
            {
                throw new InvalidOperationException($"Cell {cell.Coord} is not contact with the vertex {Index}.");
            }
            return result;
        }

        public HexaEdge? GetEdge(HexaVertexPosition position)
        {
            return EdgesInternal[(int)position];
        }
        public bool TryGetPosition(HexaEdge edge, out HexaVertexPosition position)
        {
            int index = EdgesInternal.IndexOf(edge);
            if (index < 0)
            {
                position = default;
                return false;
            }

            position = (HexaVertexPosition)index;
            return true;
        }
        public HexaVertexPosition GetPosition(HexaEdge edge)
        {
            if (!TryGetPosition(edge, out HexaVertexPosition result))
            {
                throw new InvalidOperationException($"Edge {edge.Index} is not connected with the vertex {Index}.");
            }
            return result;
        }

        public IEnumerable<HexaCell> EnumerateCells()
        {
            foreach (var cell in Cells)
            {
                if (cell is not null)
                {
                    yield return cell;
                }
            }
        }

        public IEnumerable<HexaEdge> EnumerateEdges()
        {
            foreach (var edge in Edges)
            {
                if (edge is not null)
                {
                    yield return edge;
                }
            }
        }
    }
}
