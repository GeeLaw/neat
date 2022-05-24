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
  public readonly struct String32
    : IComparable<String32>, IComparable, IEquatable<String32>,
      IEnumerable2<Char32, String32.Enumerator>, IReadOnlyList<Char32>
  {
    [FieldOffset(0)]
    private readonly Char32[] myData;

    /// <summary>
    /// Initializes a new instance of <see cref="String32"/>.
    /// </summary>
    /// <param name="immutableData">This array must not escape to the user.</param>
    [MethodImpl(Helper.OptimizeInline)]
    internal String32(Char32[] immutableData)
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

    public Char32 this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[index];
      }
    }

    int IReadOnlyCollection<Char32>.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData.Length;
      }
    }

    #region Compare, order operators, IComparable<String32> members, IComparable members

    [MethodImpl(Helper.JustOptimize)]
    private static int Compare(Char32[] x, Char32[] y)
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
      ref int x0 = ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(x));
      ref int y0 = ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(y));
      for (int i = 0, length = (xlength < ylength ? xlength : ylength), xi, yi; i != length; ++i)
      {
        xi = Unsafe.Add(ref x0, i);
        yi = Unsafe.Add(ref y0, i);
        if (xi < yi)
        {
          return -1;
        }
        if (xi > yi)
        {
          return 1;
        }
      }
      return xlength - ylength;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static int Compare(String32 x, String32 y)
    {
      return Compare(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <=(String32 x, String32 y)
    {
      return Compare(x.myData, y.myData) <= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >=(String32 x, String32 y)
    {
      return Compare(x.myData, y.myData) >= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <(String32 x, String32 y)
    {
      return Compare(x.myData, y.myData) < 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >(String32 x, String32 y)
    {
      return Compare(x.myData, y.myData) > 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int CompareTo(String32 other)
    {
      return Compare(myData, other.myData);
    }

    [MethodImpl(Helper.JustOptimize)]
    int IComparable.CompareTo(object obj)
    {
      return ReferenceEquals(obj, null)
        ? 1
        : obj is String32 other
        ? Compare(myData, other.myData)
        : throw new ArgumentException("The argument '" + nameof(obj) + "' must be Neat.Unicode.String32 or null.", nameof(obj));
    }

    #endregion Compare, order operators, IComparable<String32> members, IComparable members

    #region Equals, equality operators, IEquatable<String32> members, object members

    [MethodImpl(Helper.JustOptimize)]
    private static bool Equals(Char32[] x, Char32[] y)
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
      ref int x0 = ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(x));
      ref int y0 = ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(y));
      for (int i = 0; i != length; i++)
      {
        if (Unsafe.Add(ref x0, i) != Unsafe.Add(ref y0, i))
        {
          return false;
        }
      }
      return true;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool Equals(String32 x, String32 y)
    {
      return Equals(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(String32 x, String32 y)
    {
      return Equals(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(String32 x, String32 y)
    {
      return !Equals(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(String32 other)
    {
      return Equals(myData, other.myData);
    }

    [MethodImpl(Helper.JustOptimize)]
    public override bool Equals(object obj)
    {
      return (obj is String32 other) && Equals(myData, other.myData);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static int ComputeHashCode(Char32[] x)
    {
      if (ReferenceEquals(x, null))
      {
        return 0;
      }
      int hash = Helper.FnvOffsetBasis;
      for (int i = 0, value; i < x.Length; ++i)
      {
        value = x[i].Value;
        hash = (hash ^ (value & 0xFF)) * Helper.FnvPrime;
        value >>= 8;
        hash = (hash ^ (value & 0xFF)) * Helper.FnvPrime;
        value >>= 8;
        hash = (hash ^ (value & 0xFF)) * Helper.FnvPrime;
        value >>= 8;
        hash = (hash ^ (value & 0xFF)) * Helper.FnvPrime;
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

    #endregion Equals, equality operators, IEquatable<String32> members, object members

    /// <summary>
    /// Enumerates <see cref="Char32"/> instances in a <see cref="String32"/>.
    /// </summary>
    public struct Enumerator : IEnumerator2<Char32>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Char32[] myData;
      private int myIndex;

      [MethodImpl(Helper.OptimizeInline)]
      internal Enumerator(Char32[] data)
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
      public bool MoveNext(out Char32 item)
      {
        Char32[] data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
        item = default(Char32);
        return false;
      }

      [MethodImpl(Helper.JustOptimize)]
      bool IEnumerator2.MoveNext(out object item)
      {
        Char32[] data = myData;
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

      public Char32 Current
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
      Char32[] data = myData;
      int throwIfNull = data.Length;
      return new Enumerator(data);
    }

    [MethodImpl(Helper.JustOptimize)]
    IEnumerator2<Char32> IEnumerable2<Char32>.GetEnumerator()
    {
      return GetEnumerator();
    }

    [MethodImpl(Helper.JustOptimize)]
    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      return GetEnumerator();
    }

    [MethodImpl(Helper.JustOptimize)]
    IEnumerator<Char32> IEnumerable<Char32>.GetEnumerator()
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
