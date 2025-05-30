// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Stride.Core.Mathematics;

/// <summary>
/// Structure providing Width, Height and Depth.
/// </summary>
[DataContract("!Size3")]
[DataStyle(DataStyle.Compact)]
[StructLayout(LayoutKind.Sequential)]
public struct Size3 : IEquatable<Size3>, IComparable<Size3>, ISpanFormattable
{
    /// <summary>
    /// A zero size with (width, height, depth) = (0,0,0)
    /// </summary>
    public static readonly Size3 Zero = new(0, 0, 0);

    /// <summary>
    /// A one size with (width, height, depth) = (1,1,1)
    /// </summary>
    public static readonly Size3 One = new(1, 1, 1);

    /// <summary>
    /// A zero size with (width, height, depth) = (0,0,0)
    /// </summary>
    public static readonly Size3 Empty = Zero;

    /// <summary>
    /// Initializes a new instance of the <see cref="Size3" /> struct.
    /// </summary>
    /// <param name="width">The x.</param>
    /// <param name="height">The y.</param>
    /// <param name="depth">The depth.</param>
    public Size3(int width, int height, int depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
    }

    /// <summary>
    /// Width.
    /// </summary>
    [DataMember(0)]
    public int Width;

    /// <summary>
    /// Height.
    /// </summary>
    [DataMember(1)]
    public int Height;

    /// <summary>
    /// Height.
    /// </summary>
    [DataMember(2)]
    public int Depth;

    /// <summary>
    /// Gets a volume size.
    /// </summary>
    private readonly long VolumeSize
    {
        get
        {
            return (long)Width * Height * Depth;
        }
    }

    /// <inheritdoc/>
    public readonly bool Equals(Size3 other)
    {
        return Width == other.Width && Height == other.Height && Depth == other.Depth;
    }

    /// <inheritdoc/>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Size3 size && Equals(size);
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Width, Height, Depth);
    }

    /// <inheritdoc/>
    public readonly int CompareTo(Size3 other)
    {
        return Math.Sign(this.VolumeSize - other.VolumeSize);
    }

    /// <inheritdoc/>
    public override readonly string ToString() => $"{this}";

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
    {
        var handler = new DefaultInterpolatedStringHandler(4, 3, formatProvider);
        handler.AppendLiteral("(");
        handler.AppendFormatted(Width, format);
        handler.AppendLiteral(",");
        handler.AppendFormatted(Height, format);
        handler.AppendLiteral(",");
        handler.AppendFormatted(Depth, format);
        handler.AppendLiteral(")");
        return handler.ToStringAndClear();
    }

    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var format1 = format.Length > 0 ? format.ToString() : null;
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(4, 3, destination, provider, out _);
        handler.AppendLiteral("(");
        handler.AppendFormatted(Width, format1);
        handler.AppendLiteral(",");
        handler.AppendFormatted(Height, format1);
        handler.AppendLiteral(",");
        handler.AppendFormatted(Depth, format1);
        handler.AppendLiteral(")");
        return destination.TryWrite(ref handler, out charsWritten);
    }

    /// <summary>
    /// Implements the &lt;.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <(Size3 left, Size3 right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Implements the &lt;.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <=(Size3 left, Size3 right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Implements the &lt; or ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >(Size3 left, Size3 right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Implements the &gt; or ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >=(Size3 left, Size3 right)
    {
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    /// Implements the ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(Size3 left, Size3 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Implements the !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(Size3 left, Size3 right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Calculates the next up mip-level (*2) of this size.
    /// </summary>
    /// <returns>A next up mip-level Size3.</returns>
    public readonly Size3 Up2(int count = 1)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Must be >= 0");
        }

        return new Size3(Math.Max(1, Width << count), Math.Max(1, Height << count), Math.Max(1, Depth << count));
    }

    /// <summary>
    /// Calculates the next down mip-level (/2) of this size.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <returns>A next down mip-level Size3.</returns>
    public readonly Size3 Down2(int count = 1)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Must be >= 0");
        }

        return new Size3(Math.Max(1, Width >> count), Math.Max(1, Height >> count), Math.Max(1, Depth >> count));
    }

    /// <summary>
    /// Calculates the mip size based on a direction.
    /// </summary>
    /// <param name="direction">The direction &lt; 0 then <see cref="Down2"/>, &gt; 0  then <see cref="Up2"/>, else this unchanged.</param>
    /// <returns>Size3.</returns>
    public readonly Size3 Mip(int direction)
    {
        return direction == 0 ? this : direction < 0 ? Down2() : Up2();
    }

    /// <summary>
    /// Deconstructs the vector's components into named variables.
    /// </summary>
    /// <param name="width">The Width component</param>
    /// <param name="height">The Height component</param>
    /// <param name="depth">The Depth component</param>
    public readonly void Deconstruct(out int width, out int height, out int depth)
    {
        width = Width;
        height = Height;
        depth = Depth;
    }
}
