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
  public readonly struct String8
    : IComparable<String8>, IComparable, IEquatable<String8>,
      IEnumerable2<Char8, String8.Enumerator>, IReadOnlyList<Char8>
  {
    [FieldOffset(0)]
    internal readonly Char8[] myData;

    /// <summary>
    /// Initializes a new instance of <see cref="String8"/>.
    /// </summary>
    /// <param name="immutableData">This array must not escape to the user.</param>
    [MethodImpl(Helper.OptimizeInline)]
    internal String8(Char8[] immutableData)
    {
      myData = immutableData;
    }

    #region conversion methods

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    private static readonly Char8[] theEmptyArray = new Char8[0];

    /// <summary>
    /// Converts <see langword="string"/> to <see cref="String8"/>,
    /// throwing <see cref="ArgumentException"/> if the <see langword="string"/> is not valid UTF-16.
    /// A <see langword="null"/> <see langword="string"/> is converted to a <see langword="default"/> <see cref="String8"/> (the <see langword="null"/> wrapper),
    /// and an empty <see langword="string"/> is converted to a <see cref="String8"/> of length <c>0</c>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String8 FromString16Strict(string string16)
    {
      if (ReferenceEquals(string16, null))
      {
        return default(String8);
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        return new String8(theEmptyArray);
      }
      ref char src0 = ref MemoryMarshal.GetReference(string16.AsSpan());
      long dst8s;
      if (!UtfUnsafe.String16ToString8CountStrict(ref src0, src16s, out dst8s))
      {
        throw new ArgumentException(UtfUnsafe.String16IsNotValid, nameof(string16));
      }
      if (dst8s > UtfUnsafe.MaximumLength8)
      {
        throw new OutOfMemoryException(UtfUnsafe.String8WouldBeTooLong);
      }
      Char8[] string8 = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
      UtfUnsafe.String16ToString8Transform(ref src0, src16s,
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(string8)),
        (int)dst8s);
      return new String8(string8);
    }

    /// <summary>
    /// Converts <see langword="string"/> to <see cref="String8"/>,
    /// replacing invalid UTF-16 code units by the replacement character.
    /// A <see langword="null"/> <see langword="string"/> is converted to a <see langword="default"/> <see cref="String8"/> (the <see langword="null"/> wrapper),
    /// and an empty <see langword="string"/> is converted to a <see cref="String8"/> of length <c>0</c>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String8 FromString16Replace(string string16)
    {
      if (ReferenceEquals(string16, null))
      {
        return default(String8);
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        return new String8(theEmptyArray);
      }
      ref char src0 = ref MemoryMarshal.GetReference(string16.AsSpan());
      long dst8s = UtfUnsafe.String16ToString8CountReplace(ref src0, src16s);
      if (dst8s > UtfUnsafe.MaximumLength8)
      {
        throw new OutOfMemoryException(UtfUnsafe.String8WouldBeTooLong);
      }
      Char8[] string8 = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
      UtfUnsafe.String16ToString8Transform(ref src0, src16s,
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(string8)),
        (int)dst8s);
      return new String8(string8);
    }

    /// <summary>
    /// Converts <see cref="String8"/> to <see langword="string"/>,
    /// throwing <see cref="ArgumentException"/> if the <see cref="String8"/> is not valid UTF-8.
    /// A <see langword="default"/> <see cref="String8"/> (the <see langword="null"/> wrapper) is converted to the empty <see langword="string"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static string ToString16Strict(String8 string8)
    {
      return ToString16StrictImpl(string8.myData);
    }

    /// <summary>
    /// Converts <see cref="String8"/> to <see langword="string"/>,
    /// replacing invalid UTF-8 bytes by the replacement character.
    /// A <see langword="default"/> <see cref="String8"/> (the <see langword="null"/> wrapper) is converted to the empty <see langword="string"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static string ToString16Replace(String8 string8)
    {
      return ToString16ReplaceImpl(string8.myData);
    }

    private sealed class StringCreateHelper
    {
      [SuppressMessage("Performance", "CA1822", Justification = "Closed delegates are more performant.")]
      [MethodImpl(Helper.JustOptimize)]
      public void Invoke(Span<char> span, Char8[] arg)
      {
        UtfUnsafe.String8ToString16Transform(
          ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(arg)),
          arg.Length,
          ref MemoryMarshal.GetReference(span),
          span.Length);
      }
    }

    private static readonly SpanAction<char, Char8[]> theStringCreateAction = new StringCreateHelper().Invoke;

    [MethodImpl(Helper.JustOptimize)]
    private static string ToString16StrictImpl(Char8[] string8)
    {
      int src8s;
      if (ReferenceEquals(string8, null) || (src8s = string8.Length) == 0)
      {
        return "";
      }
      int dst16s;
      if (!UtfUnsafe.String8ToString16CountStrict(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(string8)),
        src8s,
        out dst16s))
      {
        throw new ArgumentException(UtfUnsafe.String8IsNotValid, nameof(string8));
      }
      if (dst16s > UtfUnsafe.MaximumLength16)
      {
        throw new OutOfMemoryException(UtfUnsafe.String16WouldBeTooLong);
      }
      return string.Create(dst16s, string8, theStringCreateAction);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static string ToString16ReplaceImpl(Char8[] string8)
    {
      int src8s;
      if (ReferenceEquals(string8, null) || (src8s = string8.Length) == 0)
      {
        return "";
      }
      int dst16s = UtfUnsafe.String8ToString16CountReplace(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(string8)),
        src8s);
      if (dst16s > UtfUnsafe.MaximumLength16)
      {
        throw new OutOfMemoryException(UtfUnsafe.String16WouldBeTooLong);
      }
      return string.Create(dst16s, string8, theStringCreateAction);
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
        Char8[] data = myData;
        return ReferenceEquals(data, null) || data.Length == 0;
      }
    }

    /// <summary>
    /// Gets the number of UTF-8 bytes.
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
    /// Gets the UTF-8 byte at the specified index.
    /// This indexer cannot be read if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    public Char8 this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[index];
      }
    }

    /// <summary>
    /// Gets the number of UTF-8 bytes.
    /// This property cannot be read if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
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
    private static int CompareImpl(Char8[] x, Char8[] y)
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
      ref byte x0 = ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(x));
      ref byte y0 = ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(y));
      for (int i = 0, length = (xlength < ylength ? xlength : ylength), result; i != length; ++i)
      {
        result = (int)Unsafe.Add(ref x0, i) - (int)Unsafe.Add(ref y0, i);
        if (result != 0)
        {
          return result;
        }
      }
      return xlength - ylength;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static int Compare(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <=(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData) <= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >=(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData) >= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData) < 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData) > 0;
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public int CompareTo(String8 other)
    {
      return CompareImpl(myData, other.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    int IComparable.CompareTo(object obj)
    {
      return ReferenceEquals(obj, null)
        ? 1
        : obj is String8 other
        ? CompareImpl(myData, other.myData)
        : throw new ArgumentException("The argument '" + nameof(obj) + "' must be Neat.Unicode.String8 or null.", nameof(obj));
    }

    #endregion Compare, order operators, IComparable<String8> members, IComparable members

    #region Equals, equality operators, IEquatable<String8> members, object members

    [MethodImpl(Helper.JustOptimize)]
    private static bool EqualsImpl(Char8[] x, Char8[] y)
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
      ref int x0 = ref Unsafe.As<Char8, int>(ref MemoryMarshal.GetArrayDataReference(x));
      ref int y0 = ref Unsafe.As<Char8, int>(ref MemoryMarshal.GetArrayDataReference(y));
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
      /* We take a leap of faith to assume that it is safe to read past the end
      /* on the belief that array data have alignment and packing of 4 bytes.
      /* We do NOT read past the end if the valid data are exactly a multiple of 4 bytes. */
      return ((Unsafe.Add(ref x0, length) ^ Unsafe.Add(ref y0, length)) & mask) == 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool Equals(String8 x, String8 y)
    {
      return EqualsImpl(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(String8 x, String8 y)
    {
      return EqualsImpl(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(String8 x, String8 y)
    {
      return !EqualsImpl(x.myData, y.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(String8 other)
    {
      return EqualsImpl(myData, other.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public override bool Equals(object obj)
    {
      return (obj is String8 other) && EqualsImpl(myData, other.myData);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static int GetHashCodeImpl(Char8[] x)
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

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public override int GetHashCode()
    {
      return GetHashCodeImpl(myData);
    }

    /// <summary>
    /// Equivalent to <see cref="ToString16Replace(String8)"/>.
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    public override string ToString()
    {
      return ToString16ReplaceImpl(myData);
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

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public Enumerator GetEnumerator()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return new Enumerator(data);
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    IEnumerator2<Char8> IEnumerable2<Char8>.GetEnumerator()
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
    IEnumerator<Char8> IEnumerable<Char8>.GetEnumerator()
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
