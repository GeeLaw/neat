using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.Collections;

namespace Neat.Unicode
{
  [StructLayout(LayoutKind.Explicit)]
  public readonly struct String8
    : IComparable<String8>, IComparable, IEquatable<String8>,
      IEnumerable2<Char8, String8.Enumerator>, IReadOnlyList<Char8>
  {
    [FieldOffset(0)]
    private readonly Char8[] myData;

    /// <summary>
    /// Initializes a new instance of <see cref="String8"/>.
    /// </summary>
    /// <param name="immutableData">This array must not escape to the user.</param>
    [MethodImpl(Helper.OptimizeInline)]
    internal String8(Char8[] immutableData)
    {
      myData = immutableData;
    }

    public int Length
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData.Length;
      }
    }

    public Char8 this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[index];
      }
    }

    int IReadOnlyCollection<Char8>.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData.Length;
      }
    }

    #region Compare, order operators, IComparable<String8> members, IComparable members

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.JustOptimize)]
    private static int Compare(Char8[] x, Char8[] y)
    {
      if (ReferenceEquals(x, y))
      {
        return 0;
      }
      if (ReferenceEquals(x, null))
      {
        return -1;
      }
      if (ReferenceEquals(y, null))
      {
        return 1;
      }
      int xlength = x.Length, ylength = y.Length;
      if (xlength == 0 || ylength == 0)
      {
        goto SharePrefix;
      }
      ref byte x0 = ref Unsafe.As<Char8, byte>(ref x[0]);
      ref byte y0 = ref Unsafe.As<Char8, byte>(ref y[0]);
      for (int i = 0, length = (xlength < ylength ? xlength : ylength), result; i != length; ++i)
      {
        result = (int)Unsafe.Add(ref x0, i) - (int)Unsafe.Add(ref y0, i);
        if (result != 0)
        {
          return result;
        }
      }
    SharePrefix:
      return xlength - ylength;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static int Compare(String8 x, String8 y)
    {
      return Compare(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <=(String8 x, String8 y)
    {
      return Compare(x.myData, y.myData) <= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >=(String8 x, String8 y)
    {
      return Compare(x.myData, y.myData) >= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <(String8 x, String8 y)
    {
      return Compare(x.myData, y.myData) < 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >(String8 x, String8 y)
    {
      return Compare(x.myData, y.myData) > 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int CompareTo(String8 other)
    {
      return Compare(myData, other.myData);
    }

    [MethodImpl(Helper.JustOptimize)]
    int IComparable.CompareTo(object obj)
    {
      return ReferenceEquals(obj, null)
        ? 1
        : obj is String8 other
        ? Compare(myData, other.myData)
        : throw new ArgumentException("The argument '" + nameof(obj) + "' must be Neat.Unicode.String8 or null.", nameof(obj));
    }

    #endregion Compare, order operators, IComparable<String8> members, IComparable members

    #region Equals, equality operators, IEquatable<String8> members, object members

    [MethodImpl(Helper.JustOptimize)]
    private static bool Equals(Char8[] x, Char8[] y)
    {
      if (ReferenceEquals(x, y))
      {
        return true;
      }
      if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
      {
        return false;
      }
      int length = x.Length;
      if (length != y.Length)
      {
        return false;
      }
      if (length == 0)
      {
        return true;
      }
      ref int x0 = ref Unsafe.As<Char8, int>(ref x[0]);
      ref int y0 = ref Unsafe.As<Char8, int>(ref y[0]);
      int mask = (length & 3);
      length >>= 2;
      for (int i = 0; i != length; i++)
      {
        if (Unsafe.Add(ref x0, i) != Unsafe.Add(ref y0, i))
        {
          return false;
        }
      }
      switch (mask)
      {
      case 1:
        mask = Helper.TrailingMask1;
        break;
      case 2:
        mask = Helper.TrailingMask2;
        break;
      case 3:
        mask = Helper.TrailingMask3;
        break;
      default:
        return true;
      }
      return ((Unsafe.Add(ref x0, length) ^ Unsafe.Add(ref y0, length)) & mask) == 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool Equals(String8 x, String8 y)
    {
      return Equals(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(String8 x, String8 y)
    {
      return Equals(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(String8 x, String8 y)
    {
      return !Equals(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(String8 other)
    {
      return Equals(myData, other.myData);
    }

    [MethodImpl(Helper.JustOptimize)]
    public override bool Equals(object obj)
    {
      return (obj is String8 other) && Equals(myData, other.myData);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static int ComputeHashCode(Char8[] x)
    {
      if (ReferenceEquals(x, null))
      {
        return 0;
      }
      int hash = Helper.FnvOffsetBasis;
      for (int i = 0; i < x.Length; ++i)
      {
        hash = (hash ^ x[i].Value) * Helper.FnvPrime;
      }
      return hash ^ x.Length;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override int GetHashCode()
    {
      return ComputeHashCode(myData);
    }

    public override string ToString()
    {
      throw new NotImplementedException();
    }

    #endregion Equals, equality operators, IEquatable<String8> members, object members

    /// <summary>
    /// Enumerates <see cref="Char8"/> instances in a <see cref="String8"/>.
    /// </summary>
    public struct Enumerator : IEnumerator2<Char8>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Char8[] myData;
      private int myIndex;

      [MethodImpl(Helper.OptimizeInline)]
      internal Enumerator(Char8[] data)
      {
        myData = data;
        myIndex = -1;
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
        myIndex = -1;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext(out Char8 item)
      {
        Char8[] data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
        item = default(Char8);
        return false;
      }

      [MethodImpl(Helper.JustOptimize)]
      bool IEnumerator2.MoveNext(out object item)
      {
        Char8[] data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
        item = null;
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext()
      {
        return ++myIndex < myData.Length;
      }

      public Char8 Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
          return myData[myIndex];
        }
      }

      object IEnumerator.Current
      {
        [MethodImpl(Helper.JustOptimize)]
        get
        {
          return myData[myIndex];
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IDisposable.Dispose()
      {
      }
    }

    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public Enumerator GetEnumerator()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return new Enumerator(data);
    }

    [MethodImpl(Helper.JustOptimize)]
    IEnumerator2<Char8> IEnumerable2<Char8>.GetEnumerator()
    {
      return GetEnumerator();
    }

    [MethodImpl(Helper.JustOptimize)]
    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      return GetEnumerator();
    }

    [MethodImpl(Helper.JustOptimize)]
    IEnumerator<Char8> IEnumerable<Char8>.GetEnumerator()
    {
      return GetEnumerator();
    }

    [MethodImpl(Helper.JustOptimize)]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
