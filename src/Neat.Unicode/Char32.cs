using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.Collections;

namespace Neat.Unicode
{
  /// <summary>
  /// <see cref="Char32"/> is binary-compatible with <see langword="int"/> and <see langword="uint"/>.
  /// An instance of <see cref="Char32"/> is not necessarily a valid Unicode code point.
  /// </summary>
  [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 4)]
  public readonly struct Char32 : IComparable<Char32>, IComparable, IEquatable<Char32>
  {
    [FieldOffset(0)]
    public readonly int Value;

    #region constructors, cast operators

    /// <summary>
    /// Initializes a new instance of <see cref="Char32"/>.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    public Char32(int value)
    {
      Value = value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Char32"/>.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    [CLSCompliant(false)]
    public Char32(uint value)
    {
      Value = (int)value;
    }

    /// <summary>
    /// Converts an <see langword="int"/> to a <see cref="Char32"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    public static explicit operator Char32(int value)
    {
      return new Char32(value);
    }

    /// <summary>
    /// Converts a <see langword="uint"/> to a <see cref="Char32"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    [CLSCompliant(false)]
    public static explicit operator Char32(uint value)
    {
      return new Char32((int)value);
    }

    /// <summary>
    /// Converts a <see cref="Char32"/> to an <see langword="int"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    public static explicit operator int(Char32 value)
    {
      return value.Value;
    }

    /// <summary>
    /// Converts a <see cref="Char32"/> to a <see langword="uint"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    [CLSCompliant(false)]
    public static explicit operator uint(Char32 value)
    {
      return (uint)value.Value;
    }

    #endregion constructors, cast operators

    #region Compare, order operators, IComparable<Char32> members, IComparable members

    public static int Compare(Char32 x, Char32 y)
    {
      int xx = x.Value, yy = y.Value;
      return xx < yy ? -1 : xx > yy ? 1 : 0;
    }

    public static bool operator <=(Char32 x, Char32 y)
    {
      return x.Value <= y.Value;
    }

    public static bool operator >=(Char32 x, Char32 y)
    {
      return x.Value >= y.Value;
    }

    public static bool operator <(Char32 x, Char32 y)
    {
      return x.Value < y.Value;
    }

    public static bool operator >(Char32 x, Char32 y)
    {
      return x.Value > y.Value;
    }

    public int CompareTo(Char32 other)
    {
      int xx = Value, yy = other.Value;
      return xx < yy ? -1 : xx > yy ? 1 : 0;
    }

    int IComparable.CompareTo(object obj)
    {
      if (ReferenceEquals(obj, null))
      {
        return 1;
      }
      if (obj is Char32 other)
      {
        int xx = Value, yy = other.Value;
        return xx < yy ? -1 : xx > yy ? 1 : 0;
      }
      throw new ArgumentException("The argument '" + nameof(obj) + "' must be Neat.Unicode.Char32 or null.", nameof(obj));
    }

    #endregion Compare, order operators, IComparable<Char32> members, IComparable members

    #region Equals, equality operators, IEquatable<Char32> members, object members

    public static bool Equals(Char32 x, Char32 y)
    {
      return x.Value == y.Value;
    }

    public static bool operator ==(Char32 x, Char32 y)
    {
      return x.Value == y.Value;
    }

    public static bool operator !=(Char32 x, Char32 y)
    {
      return x.Value != y.Value;
    }

    public bool Equals(Char32 other)
    {
      return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
      return (obj is Char32 other) && (Value == other.Value);
    }

    public override int GetHashCode()
    {
      return Value;
    }

    private static readonly string[] theToStringResults = new string[128]
    {
      "Char32(^@)", "Char32(^A)", "Char32(^B)", "Char32(^C)",
      "Char32(^D)", "Char32(^E)", "Char32(^F)", "Char32(^G)",
      "Char32(^H)", "Char32(^I)", "Char32(^J)", "Char32(^K)",
      "Char32(^L)", "Char32(^M)", "Char32(^N)", "Char32(^O)",
      "Char32(^P)", "Char32(^Q)", "Char32(^R)", "Char32(^S)",
      "Char32(^T)", "Char32(^U)", "Char32(^V)", "Char32(^W)",
      "Char32(^X)", "Char32(^Y)", "Char32(^Z)", "Char32(^[)",
      "Char32(^\\)", "Char32(^])", "Char32(^^)", "Char32(^_)",
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
      "x", "y", "z", "{", "|", "}", "~", "Char32(^?)"
    };

    [MethodImpl(Helper.OptimizeInline)]
    private static char GetHexit(int below16)
    {
      return (char)((below16 < 10 ? 48 : 55) + below16);
    }

    private static unsafe string GetString(int value)
    {
      char* char18 = stackalloc char[18];
      if (Utf.Char32IsBelow0x80(value))
      {
        return theToStringResults[value];
      }
      if (Utf.Char32IsBelow0x10000(value))
      {
        if (Utf.Char32IsSurrogate(value))
        {
          goto NotValidCodepoint;
        }
        return new string(Utf.Char32To1Char16Unchecked(value), 1);
      }
      if (Utf.Char32IsBelow0x110000(value))
      {
        value = Utf.Char32To2Char16sUncheckedPrepare(value);
        char18[0] = Utf.Char32PreparedTo2Char16sUncheckedHigh(value);
        char18[1] = Utf.Char32PreparedTo2Char16sUncheckedLow(value);
        return new string(char18, 0, 2);
      }
    NotValidCodepoint:
      char18[0] = 'C';
      char18[1] = 'h';
      char18[2] = 'a';
      char18[3] = 'r';
      char18[4] = '3';
      char18[5] = '2';
      char18[6] = '(';
      char18[7] = '0';
      char18[8] = 'x';
      char18[17] = ')';
      for (int i = 16; i != 8; --i)
      {
        char18[i] = GetHexit(value & 0xF);
        value >>= 1;
      }
      return new string(char18, 0, 18);
    }

    public override string ToString()
    {
      return GetString(Value);
    }

    #endregion Equals, equality operators, IEquatable<Char32> members, object members

    #region properties

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance is a valid Unicode code point.
    /// This means that <see cref="Value"/> is between either <c>0x0</c> and <c>0xD7FF</c> or <c>0xE000</c> and <c>0x10FFFF</c> (all inclusive).
    /// </summary>
    public bool IsValid
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32IsValid(Value);
      }
    }

    #endregion properties

    #region properties related to Char8

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as a 1-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is between <c>0x0</c> and <c>0x7F</c> (both inclusive).
    /// </summary>
    public bool Is1Char8
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32Is1Char8(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as a 2-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is between <c>0x80</c> and <c>0x7FF</c> (both inclusive).
    /// </summary>
    public bool Is2Char8s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32Is2Char8s(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as a 3-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is between either <c>0x800</c> and <c>0xDBFF</c> or <c>0xE000</c> and <c>0xFFFF</c> (all inclusive).
    /// </summary>
    public bool Is3Char8s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32Is3Char8s(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as a 4-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is between <c>0x10000</c> and <c>0x10FFFF</c> (both inclusive).
    /// </summary>
    public bool Is4Char8s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32Is4Char8s(Value);
      }
    }

    /// <summary>
    /// Gets the number of bytes used to encode the <see cref="Char32"/> instance in UTF-8.
    /// Returns <c>-1</c> if the instance is not a valid Unicode code point.
    /// </summary>
    public int LengthInChar8s
    {
      [MethodImpl(Helper.JustOptimize)]
      get
      {
        return Utf.Char32LengthInChar8s(Value);
      }
    }

    #endregion properties related to Char8

    #region properties related to Char16

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance is a surrogate code point (which is invalid).
    /// This means that <see cref="Value"/> is between <c>0xD800</c> and <c>0xDFFF</c> (both inclusive).
    /// </summary>
    public bool IsSurrogate
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32IsSurrogate(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance is a high (leading) surrogate code point (which is invalid).
    /// This means that <see cref="Value"/> is between <c>0xD800</c> and <c>0xDBFF</c> (both inclusive).
    /// </summary>
    public bool IsHighSurrogate
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32IsHighSurrogate(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance is a low (leading) surrogate code point (which is invalid).
    /// This means that <see cref="Value"/> is between <c>0xDC00</c> and <c>0xDFFF</c> (both inclusive).
    /// </summary>
    public bool IsLowSurrogate
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32IsLowSurrogate(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as one code unit in UTF-16.
    /// This means that <see cref="Value"/> is between either <c>0x0</c> and <c>0xD7FF</c> or <c>0xE000</c> and <c>0xFFFF</c> (all inclusive).
    /// </summary>
    public bool Is1Char16
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32Is1Char16(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as two code units in UTF-16.
    /// This means that <see cref="Value"/> is between <c>0x10000</c> and <c>0x10FFFF</c> (both inclusive).
    /// </summary>
    public bool Is2Char16s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32Is2Char16s(Value);
      }
    }

    /// <summary>
    /// Gets the number of code units used to encode the <see cref="Char32"/> instance in UTF-16.
    /// Returns <c>-1</c> if the instance is not a valid Unicode code point.
    /// </summary>
    public int LengthInChar16s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Utf.Char32LengthInChar16s(Value);
      }
    }

    #endregion properties related to Char16

    /// <summary>
    /// Standard implementation of <see cref="IComparer{T}"/> and <see cref="IEqualityComparer2{T}"/> for <see cref="Char32"/>.
    /// </summary>
    public struct Comparer : IComparer<Char32>, IEqualityComparer2<Char32>
    {
      public int Compare(Char32 x, Char32 y)
      {
        int xx = x.Value, yy = y.Value;
        return xx < yy ? -1 : xx > yy ? 1 : 0;
      }

      public bool Equals(Char32 x, Char32 y)
      {
        return x.Value == y.Value;
      }

      public int GetHashCode(Char32 obj)
      {
        return obj.Value;
      }
    }
  }
}
