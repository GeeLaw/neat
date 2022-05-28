using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.Collections;

namespace Neat.Unicode
{
  /// <summary>
  /// <see cref="Char8"/> is binary-compatible with <see langword="byte"/> and <see langword="sbyte"/>.
  /// An instance of <see cref="Char8"/> is not necessarily a possible byte in a valid UTF-8 sequence.
  /// </summary>
  [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1)]
  public readonly struct Char8 : IComparable<Char8>, IComparable, IEquatable<Char8>
  {
    [FieldOffset(0)]
    public readonly byte Value;

    #region constructors, cast operators

    /// <summary>
    /// Initializes a new instance of <see cref="Char8"/>.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Char8(byte value)
    {
      Value = value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Char8"/>.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public Char8(sbyte value)
    {
      Value = (byte)value;
    }

    /// <summary>
    /// Converts a <see langword="byte"/> to a <see cref="Char8"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator Char8(byte value)
    {
      return new Char8(value);
    }

    /// <summary>
    /// Converts a <see langword="sbyte"/> to a <see cref="Char8"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator Char8(sbyte value)
    {
      return new Char8((byte)value);
    }

    /// <summary>
    /// Converts a <see cref="Char8"/> to a <see langword="byte"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator byte(Char8 value)
    {
      return value.Value;
    }

    /// <summary>
    /// Converts a <see cref="Char8"/> to a <see langword="sbyte"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator sbyte(Char8 value)
    {
      return (sbyte)value.Value;
    }

    #endregion constructors, cast operators

    #region Compare, order operators, IComparable<Char8> members, IComparable members

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static int Compare(Char8 x, Char8 y)
    {
      return (int)x.Value - (int)y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <=(Char8 x, Char8 y)
    {
      return x.Value <= y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >=(Char8 x, Char8 y)
    {
      return x.Value >= y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <(Char8 x, Char8 y)
    {
      return x.Value < y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >(Char8 x, Char8 y)
    {
      return x.Value > y.Value;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public int CompareTo(Char8 other)
    {
      return (int)Value - (int)other.Value;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.JustOptimize)]
    int IComparable.CompareTo(object obj)
    {
      return ReferenceEquals(obj, null)
        ? 1
        : obj is Char8 other
        ? (int)Value - (int)other.Value
        : throw new ArgumentException("The argument '" + nameof(obj) + "' must be Neat.Unicode.Char8 or null.", nameof(obj));
    }

    #endregion Compare, order operators, IComparable<Char8> members, IComparable members

    #region Equals, equality operators, IEquatable<Char8> members, object members

    [MethodImpl(Helper.OptimizeInline)]
    public static bool Equals(Char8 x, Char8 y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(Char8 x, Char8 y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(Char8 x, Char8 y)
    {
      return x.Value != y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(Char8 other)
    {
      return Value == other.Value;
    }

    [MethodImpl(Helper.JustOptimize)]
    public override bool Equals(object obj)
    {
      return (obj is Char8 other) && (Value == other.Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override int GetHashCode()
    {
      return Value;
    }

    private static readonly string[] theToStringResults = new string[256]
    {
      "Char8(^@)", "Char8(^A)", "Char8(^B)", "Char8(^C)",
      "Char8(^D)", "Char8(^E)", "Char8(^F)", "Char8(^G)",
      "Char8(^H)", "Char8(^I)", "Char8(^J)", "Char8(^K)",
      "Char8(^L)", "Char8(^M)", "Char8(^N)", "Char8(^O)",
      "Char8(^P)", "Char8(^Q)", "Char8(^R)", "Char8(^S)",
      "Char8(^T)", "Char8(^U)", "Char8(^V)", "Char8(^W)",
      "Char8(^X)", "Char8(^Y)", "Char8(^Z)", "Char8(^[)",
      "Char8(^\\)", "Char8(^])", "Char8(^^)", "Char8(^_)",
      " ", "!", "\"", "#", "$", "%", "&", "'",
      "(", ")", "*", "+", ",", "-", ".", "/",
      "0", "1", "2", "3", "4", "5", "6", "7",
      "8", "9", ":", ";", "<", "=", ">", "?",
      "@", "A", "B", "C", "D", "E", "F", "G",
      "H", "I", "J", "K", "L", "M", "N", "O",
      "P", "Q", "R", "S", "T", "U", "V", "W",
      "X", "Y", "Z", "[", "\\", "]", "^", "_",
      "`", "a", "b", "c", "d", "e", "f", "g",
      "h", "i", "j", "k", "l", "m", "n", "o",
      "p", "q", "r", "s", "t", "u", "v", "w",
      "x", "y", "z", "{", "|", "}", "~", "Char8(^?)",
      "Char8(0x80)", "Char8(0x81)", "Char8(0x82)", "Char8(0x83)",
      "Char8(0x84)", "Char8(0x85)", "Char8(0x86)", "Char8(0x87)",
      "Char8(0x88)", "Char8(0x89)", "Char8(0x8A)", "Char8(0x8B)",
      "Char8(0x8C)", "Char8(0x8D)", "Char8(0x8E)", "Char8(0x8F)",
      "Char8(0x90)", "Char8(0x91)", "Char8(0x92)", "Char8(0x93)",
      "Char8(0x94)", "Char8(0x95)", "Char8(0x96)", "Char8(0x97)",
      "Char8(0x98)", "Char8(0x99)", "Char8(0x9A)", "Char8(0x9B)",
      "Char8(0x9C)", "Char8(0x9D)", "Char8(0x9E)", "Char8(0x9F)",
      "Char8(0xA0)", "Char8(0xA1)", "Char8(0xA2)", "Char8(0xA3)",
      "Char8(0xA4)", "Char8(0xA5)", "Char8(0xA6)", "Char8(0xA7)",
      "Char8(0xA8)", "Char8(0xA9)", "Char8(0xAA)", "Char8(0xAB)",
      "Char8(0xAC)", "Char8(0xAD)", "Char8(0xAE)", "Char8(0xAF)",
      "Char8(0xB0)", "Char8(0xB1)", "Char8(0xB2)", "Char8(0xB3)",
      "Char8(0xB4)", "Char8(0xB5)", "Char8(0xB6)", "Char8(0xB7)",
      "Char8(0xB8)", "Char8(0xB9)", "Char8(0xBA)", "Char8(0xBB)",
      "Char8(0xBC)", "Char8(0xBD)", "Char8(0xBE)", "Char8(0xBF)",
      "Char8(0xC0)", "Char8(0xC1)", "Char8(0xC2)", "Char8(0xC3)",
      "Char8(0xC4)", "Char8(0xC5)", "Char8(0xC6)", "Char8(0xC7)",
      "Char8(0xC8)", "Char8(0xC9)", "Char8(0xCA)", "Char8(0xCB)",
      "Char8(0xCC)", "Char8(0xCD)", "Char8(0xCE)", "Char8(0xCF)",
      "Char8(0xD0)", "Char8(0xD1)", "Char8(0xD2)", "Char8(0xD3)",
      "Char8(0xD4)", "Char8(0xD5)", "Char8(0xD6)", "Char8(0xD7)",
      "Char8(0xD8)", "Char8(0xD9)", "Char8(0xDA)", "Char8(0xDB)",
      "Char8(0xDC)", "Char8(0xDD)", "Char8(0xDE)", "Char8(0xDF)",
      "Char8(0xE0)", "Char8(0xE1)", "Char8(0xE2)", "Char8(0xE3)",
      "Char8(0xE4)", "Char8(0xE5)", "Char8(0xE6)", "Char8(0xE7)",
      "Char8(0xE8)", "Char8(0xE9)", "Char8(0xEA)", "Char8(0xEB)",
      "Char8(0xEC)", "Char8(0xED)", "Char8(0xEE)", "Char8(0xEF)",
      "Char8(0xF0)", "Char8(0xF1)", "Char8(0xF2)", "Char8(0xF3)",
      "Char8(0xF4)", "Char8(0xF5)", "Char8(0xF6)", "Char8(0xF7)",
      "Char8(0xF8)", "Char8(0xF9)", "Char8(0xFA)", "Char8(0xFB)",
      "Char8(0xFC)", "Char8(0xFD)", "Char8(0xFE)", "Char8(0xFF)"
    };

    [MethodImpl(Helper.OptimizeInline)]
    public override string ToString()
    {
      return theToStringResults[Value];
    }

    #endregion Equals, equality operators, IEquatable<Char8> members, object members

    #region properties

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid one-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is <c>0xxxxxxx</c>.
    /// </summary>
    public bool Leads1
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Leads1(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid continuation byte in UTF-8,
    /// without considering overlong sequences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// This means that <see cref="Value"/> is <c>10xxxxxx</c>.
    /// </summary>
    public bool Continues
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Continues(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid leading byte for a 2-byte sequence in UTF-8,
    /// without considering overlong sequences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// This means that <see cref="Value"/> is <c>110xxxxx</c>.
    /// (In particular, this property is <see langword="true"/> even if <see cref="Value"/> is <c>11000000</c> or <c>11000001</c>,
    /// which cannot appear in any valid UTF-8 sequence as overlong sequences are invalid.)
    /// </summary>
    public bool Leads2
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Leads2(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid leading byte for a 3-byte sequence in UTF-8,
    /// without considering overlong sequences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// This means that <see cref="Value"/> is <c>1110xxxx</c>.
    /// </summary>
    public bool Leads3
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Leads3(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid leading byte for a 4-byte sequence in UTF-8,
    /// without considering overlong sequences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// This means that <see cref="Value"/> is <c>11110xxx</c>.
    /// (In particular, this property is <see langword="true"/> even if <see cref="Value"/> is <c>11110111</c>,
    /// which cannot appear in any valid UTF-8 sequence.)
    /// </summary>
    public bool Leads4
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Leads4(Value);
      }
    }

    #endregion properties

    /// <summary>
    /// Standard implementation of <see cref="IComparer{T}"/> and <see cref="IEqualityComparer2{T}"/> for <see cref="Char8"/>.
    /// </summary>
    public struct Comparer : IComparer<Char8>, IEqualityComparer2<Char8>
    {
      [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
      [MethodImpl(Helper.OptimizeInline)]
      public int Compare(Char8 x, Char8 y)
      {
        return (int)x.Value - (int)y.Value;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Equals(Char8 x, Char8 y)
      {
        return x.Value == y.Value;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public int GetHashCode(Char8 obj)
      {
        return obj.Value;
      }
    }
  }
}
