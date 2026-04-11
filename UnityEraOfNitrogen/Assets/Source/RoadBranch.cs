// © 2026 Jong-il Hong
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>. 
//
// SPDX-License-Identifier: GPL-3.0-or-later

#nullable enable

using System;
using System.Collections.Generic;
using System.Text;

namespace Jih.Unity.EraOfNitrogen
{
    public struct RoadBranch : IEquatable<RoadBranch>
    {
        public static RoadBranch None => new(false, false, false, false, false, false);
        public static RoadBranch Branch0 => new(true, false, false, false, false, false);
        public static RoadBranch Branch01 => new(true, true, false, false, false, false);
        public static RoadBranch Branch02 => new(true, false, true, false, false, false);
        public static RoadBranch Branch03 => new(true, false, false, true, false, false);
        public static RoadBranch Branch012 => new(true, true, true, false, false, false);
        public static RoadBranch Branch013 => new(true, true, false, true, false, false);
        public static RoadBranch Branch024 => new(true, false, true, false, true, false);
        public static RoadBranch Branch035 => new(true, false, false, true, false, true);
        public static RoadBranch Branch0123 => new(true, true, true, true, false, false);
        public static RoadBranch Branch0234 => new(true, false, true, true, true, false);
        public static RoadBranch Branch0235 => new(true, false, true, true, false, true);
        public static RoadBranch Branch01234 => new(true, true, true, true, true, false);
        public static RoadBranch Branch012345 => new(true, true, true, true, true, true);

        public static IReadOnlyList<RoadBranch> Branches = new RoadBranch[]
        {
            Branch0,
            Branch01,
            Branch02,
            Branch03,
            Branch012,
            Branch013,
            Branch024,
            Branch035,
            Branch0123,
            Branch0234,
            Branch0235,
            Branch01234,
            Branch012345,
        };

        public static RoadBranch ShiftCw(RoadBranch value)
        {
            return new RoadBranch(value.B5, value.B0, value.B1, value.B2, value.B3, value.B4);
        }
        public static RoadBranch ShiftCcw(RoadBranch value)
        {
            return new RoadBranch(value.B1, value.B2, value.B3, value.B4, value.B5, value.B0);
        }

        public bool this[int index]
        {
            readonly get => index switch
            {
                0 => B0,
                1 => B1,
                2 => B2,
                3 => B3,
                4 => B4,
                5 => B5,
                _ => throw new ArgumentOutOfRangeException(nameof(index)),
            };
            set
            {
                switch (index)
                {
                    case 0: B0 = value; break;
                    case 1: B1 = value; break;
                    case 2: B2 = value; break;
                    case 3: B3 = value; break;
                    case 4: B4 = value; break;
                    case 5: B5 = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public bool B0, B1, B2, B3, B4, B5;

        public RoadBranch(bool b0, bool b1, bool b2, bool b3, bool b4, bool b5)
        {
            B0 = b0;
            B1 = b1;
            B2 = b2;
            B3 = b3;
            B4 = b4;
            B5 = b5;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is RoadBranch branch && Equals(branch);
        }
        public readonly bool Equals(RoadBranch other)
        {
            return B0 == other.B0 &&
                   B1 == other.B1 &&
                   B2 == other.B2 &&
                   B3 == other.B3 &&
                   B4 == other.B4 &&
                   B5 == other.B5;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(B0, B1, B2, B3, B4, B5);
        }

        public readonly override string ToString()
        {
            if (this == None)
            {
                return "<None>";
            }

            StringBuilder builder = new();
            builder.Append('<');
            if (B0)
            {
                builder.Append('0');
            }
            if (B1)
            {
                builder.Append('1');
            }
            if (B2)
            {
                builder.Append('2');
            }
            if (B3)
            {
                builder.Append('3');
            }
            if (B4)
            {
                builder.Append('4');
            }
            if (B5)
            {
                builder.Append('5');
            }
            builder.Append('>');

            return builder.ToString();
        }

        public static bool operator ==(RoadBranch left, RoadBranch right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(RoadBranch left, RoadBranch right)
        {
            return !(left == right);
        }

        public static RoadBranch operator >>(RoadBranch value, int shift)
        {
            RoadBranch result = value;
            for (int i = 0; i < shift; i++)
            {
                result = ShiftCw(result);
            }
            return result;
        }
        public static RoadBranch operator <<(RoadBranch value, int shift)
        {
            RoadBranch result = value;
            for (int i = 0; i < shift; i++)
            {
                result = ShiftCcw(result);
            }
            return result;
        }

        public const int MaxCount = 6;
    }
}
