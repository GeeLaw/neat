using System;
using System.Buffers;
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

    #region conversion methods

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    private static readonly Char32[] theEmptyArray = new Char32[0];

    /// <summary>
    /// Converts <see langword="string"/> to <see cref="String32"/>,
    /// throwing <see cref="ArgumentException"/> if the <see langword="string"/> is not valid UTF-16.
    /// A <see langword="null"/> <see langword="string"/> is converted to a <see langword="default"/> <see cref="String32"/> (the <see langword="null"/> wrapper),
    /// and an empty <see langword="string"/> is converted to a <see cref="String32"/> of length <c>0</c>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String32 FromString16Strict(string string16)
    {
      if (ReferenceEquals(string16, null))
      {
        return default(String32);
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        return new String32(theEmptyArray);
      }
      ref char src0 = ref MemoryMarshal.GetReference(string16.AsSpan());
      int dst32s;
      if (!Utf.String16ToString32CountStrict(ref src0, src16s, out dst32s))
      {
        throw new ArgumentException(Utf.String16IsNotValid, nameof(string16));
      }
      if (dst32s > Utf.MaximumLength32)
      {
        throw new OutOfMemoryException(Utf.String32WouldBeTooLong);
      }
      Char32[] string32 = GC.AllocateUninitializedArray<Char32>(dst32s, false);
      Utf.String16ToString32Transform(ref src0, src16s,
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(string32)),
        dst32s);
      return new String32(string32);
    }

    /// <summary>
    /// Converts <see langword="string"/> to <see cref="String32"/>,
    /// replacing invalid UTF-16 code units by the replacement character.
    /// A <see langword="null"/> <see langword="string"/> is converted to a <see langword="default"/> <see cref="String32"/> (the <see langword="null"/> wrapper),
    /// and an empty <see langword="string"/> is converted to a <see cref="String32"/> of length <c>0</c>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String32 FromString16Replace(string string16)
    {
      if (ReferenceEquals(string16, null))
      {
        return default(String32);
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        return new String32(theEmptyArray);
      }
      ref char src0 = ref MemoryMarshal.GetReference(string16.AsSpan());
      int dst32s = Utf.String16ToString32CountReplace(ref src0, src16s);
      if (dst32s > Utf.MaximumLength32)
      {
        throw new OutOfMemoryException(Utf.String32WouldBeTooLong);
      }
      Char32[] string32 = GC.AllocateUninitializedArray<Char32>(dst32s, false);
      Utf.String16ToString32Transform(ref src0, src16s,
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(string32)),
        dst32s);
      return new String32(string32);
    }

    /// <summary>
    /// Converts <see cref="String32"/> to <see langword="string"/>,
    /// throwing <see cref="ArgumentException"/> if the <see cref="String32"/> is not valid UTF-32.
    /// A <see langword="default"/> <see cref="String32"/> (the <see langword="null"/> wrapper) is converted to the empty <see langword="string"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static string ToString16Strict(String32 string32)
    {
      return ToString16StrictImpl(string32.myData);
    }

    /// <summary>
    /// Converts <see cref="String32"/> to <see langword="string"/>,
    /// replacing invalid UTF-32 code points by the replacement character.
    /// A <see langword="default"/> <see cref="String32"/> (the <see langword="null"/> wrapper) is converted to the empty <see langword="string"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static string ToString16Replace(String32 string32)
    {
      return ToString16ReplaceImpl(string32.myData);
    }

    private sealed class StringCreateHelper
    {
      [SuppressMessage("Performance", "CA1822", Justification = "Closed delegates are more performant.")]
      [MethodImpl(Helper.JustOptimize)]
      public void Invoke(Span<char> span, Char32[] arg)
      {
        Utf.String32ToString16Transform(
          ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(arg)),
          arg.Length,
          ref MemoryMarshal.GetReference(span),
          span.Length);
      }
    }

    private static readonly SpanAction<char, Char32[]> theStringCreateAction = new StringCreateHelper().Invoke;

    [MethodImpl(Helper.JustOptimize)]
    private static string ToString16StrictImpl(Char32[] string32)
    {
      int src32s;
      if (ReferenceEquals(string32, null) || (src32s = string32.Length) == 0)
      {
        return "";
      }
      long dst16s;
      if (!Utf.String32ToString16CountStrict(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(string32)),
        src32s,
        out dst16s))
      {
        throw new ArgumentException(Utf.String32IsNotValid, nameof(string32));
      }
      if (dst16s > Utf.MaximumLength16)
      {
        throw new OutOfMemoryException(Utf.String16WouldBeTooLong);
      }
      return string.Create((int)dst16s, string32, theStringCreateAction);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static string ToString16ReplaceImpl(Char32[] string32)
    {
      int src32s;
      if (ReferenceEquals(string32, null) || (src32s = string32.Length) == 0)
      {
        return "";
      }
      long dst16s = Utf.String32ToString16CountReplace(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(string32)),
        src32s);
      if (dst16s > Utf.MaximumLength16)
      {
        throw new OutOfMemoryException(Utf.String16WouldBeTooLong);
      }
      return string.Create((int)dst16s, string32, theStringCreateAction);
    }

    #endregion conversion methods

    /// <summary>
    /// Gets whether the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    public bool IsDefault
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return ReferenceEquals(myData, null);
      }
    }

    /// <summary>
    /// Gets whether the instance is <see langword="default"/> (the <see langword="null"/> wrapper) or empty (of <see cref="Length"/> <c>0</c>).
    /// </summary>
    public bool IsDefaultOrEmpty
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        Char32[] data = myData;
        return ReferenceEquals(data, null) || data.Length == 0;
      }
    }

    /// <summary>
    /// Gets the number of UTF-32 code points.
    /// This property cannot be read if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    public int Length
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData.Length;
      }
    }

    /// <summary>
    /// Gets the UTF-32 code point at the specified index.
    /// This indexer cannot be read if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    public Char32 this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[index];
      }
    }

    /// <summary>
    /// Gets the number of UTF-32 code points.
    /// This property cannot be read if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
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

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public int CompareTo(String32 other)
    {
      return Compare(myData, other.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
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

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(String32 other)
    {
      return Equals(myData, other.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
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

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public override int GetHashCode()
    {
      return ComputeHashCode(myData);
    }

    /// <summary>
    /// Equivalent to <see cref="ToString16Replace(String32)"/>.
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public override string ToString()
    {
      return ToString16ReplaceImpl(myData);
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

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public Enumerator GetEnumerator()
    {
      Char32[] data = myData;
      int throwIfNull = data.Length;
      return new Enumerator(data);
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    IEnumerator2<Char32> IEnumerable2<Char32>.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    IEnumerator<Char32> IEnumerable<Char32>.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
