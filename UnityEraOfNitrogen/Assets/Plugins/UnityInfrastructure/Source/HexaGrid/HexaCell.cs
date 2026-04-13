// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.HexaGrid
{
    public class HexaCell
    {
        public HexaMap Map { get; }

        public HexaIndex Index { get; }
        public HexaCoord Coord { get; }

        internal HexaCell?[] NeighborsInternal { get; } = new HexaCell[6];
        /// <remarks>
        /// Index: <see cref="HexaNeighborPosition"/><br/>
        /// Can be <c>null</c> if there is no neighbor in that direction such as border cell of the map.
        /// </remarks>
        public IReadOnlyList<HexaCell?> Neighbors => NeighborsInternal;

        internal HexaCell?[] DiagonalsInternal { get; } = new HexaCell[6];
        /// <remarks>
        /// Index: <see cref="HexaDiagonalPosition"/><br/>
        /// Can be <c>null</c> if there is no diagonal neighbor in that direction such as border cell of the map.<br/>
        /// <br/>
        /// Diagonal means the cell which is not directly adjacent but can be reached by moving with direction of a vertex from the cell center through a connected edge to the vertex.
        /// </remarks>
        public IReadOnlyList<HexaCell?> Diagonals => DiagonalsInternal;

        internal HexaVertex[] VerticesInternal { get; } = new HexaVertex[6];
        /// <remarks>
        /// Index: <see cref="HexaVertexPosition"/>
        /// </remarks>
        public IReadOnlyList<HexaVertex> Vertices => VerticesInternal;

        internal HexaEdge[] EdgesInternal { get; } = new HexaEdge[6];
        /// <remarks>
        /// Index: <see cref="HexaEdgePosition"/>
        /// </remarks>
        public IReadOnlyList<HexaEdge> Edges => EdgesInternal;

        public HexaCell(HexaMap map, HexaIndex index, HexaCoord coord)
        {
            Map = map;
            Index = index;
            Coord = coord;
        }

        public HexaCell? GetNeighbor(HexaNeighborPosition position)
        {
            return NeighborsInternal[(int)position];
        }
        public bool TryGetNeighborPosition(HexaCell cell, out HexaNeighborPosition position)
        {
            int index = NeighborsInternal.IndexOf(cell);
            if (index < 0)
            {
                position = default;
                return false;
            }

            position = (HexaNeighborPosition)index;
            return true;
        }
        public HexaNeighborPosition GetNeighborPosition(HexaCell cell)
        {
            if (!TryGetNeighborPosition(cell, out HexaNeighborPosition result))
            {
                throw new InvalidOperationException($"Cell {cell.Coord} is not a neighbor of the cell {Coord}.");
            }
            return result;
        }

        public HexaCell? GetDiagonal(HexaDiagonalPosition position)
        {
            return DiagonalsInternal[(int)position];
        }
        public bool TryGetDiagonalPosition(HexaCell cell, out HexaDiagonalPosition position)
        {
            int index = DiagonalsInternal.IndexOf(cell);
            if (index < 0)
            {
                position = default;
                return false;
            }

            position = (HexaDiagonalPosition)index;
            return true;
        }
        public HexaDiagonalPosition GetDiagonalPosition(HexaCell cell)
        {
            if (!TryGetDiagonalPosition(cell, out HexaDiagonalPosition result))
            {
                throw new InvalidOperationException($"Cell {cell.Coord} is not a diagonal of the cell {Coord}.");
            }
            return result;
        }

        public HexaVertex GetVertex(HexaVertexPosition position)
        {
            return VerticesInternal[(int)position];
        }
        public bool TryGetPosition(HexaVertex vertex, out HexaVertexPosition position)
        {
            int index = VerticesInternal.IndexOf(vertex);
            if (index < 0)
            {
                position = default;
                return false;
            }

            position = (HexaVertexPosition)index;
            return true;
        }
        public HexaVertexPosition GetPosition(HexaVertex vertex)
        {
            if (!TryGetPosition(vertex, out HexaVertexPosition result))
            {
                throw new InvalidOperationException($"Vertex {vertex.Coord} is not on the cell {Coord}.");
            }
            return result;
        }

        public HexaEdge GetEdge(HexaEdgePosition position)
        {
            return EdgesInternal[(int)position];
        }
        public bool TryGetPosition(HexaEdge edge, out HexaEdgePosition position)
        {
            int index = EdgesInternal.IndexOf(edge);
            if (index < 0)
            {
                position = default;
                return false;
            }

            position = (HexaEdgePosition)index;
            return true;
        }
        public HexaEdgePosition GetPosition(HexaEdge edge)
        {
            if (!TryGetPosition(edge, out HexaEdgePosition result))
            {
                throw new InvalidOperationException($"Edge {edge.Coord} is not on the cell {Coord}.");
            }
            return result;
        }

        public IEnumerable<HexaCell> EnumerateNeighbors()
        {
            foreach (var neighbor in Neighbors)
            {
                if (neighbor is not null)
                {
                    yield return neighbor;
                }
            }
        }
    }
}
