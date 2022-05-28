using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.Collections;

namespace Neat.Unicode
{
  /// <summary>
  /// <see cref="String32"/> is binary-compatbile with a reference to an array of <see cref="Char32"/>.
  /// Represents a string encoded in UTF-32, which is not necessarily valid.
  /// Similar to <see langword="string"/>, the underlying memory of any <see cref="String32"/> must be immutable.
  /// </summary>
  [DebuggerDisplay("{ToString(),nq}")]
  [StructLayout(LayoutKind.Explicit)]
  public readonly struct String32
    : IComparable<String32>, IComparable, IEquatable<String32>,
      IEnumerable2<Char32, String32.Enumerator>, IReadOnlyList<Char32>
  {
    [FieldOffset(0)]
    internal readonly Char32[] myData;

    /// <summary>
    /// Initializes a new instance of <see cref="String32"/>.
    /// </summary>
    /// <param name="immutableData">This array must not escape to the user.</param>
    [MethodImpl(Helper.OptimizeInline)]
    internal String32(Char32[] immutableData)
    {
      myData = immutableData;
    }

    /// <summary>
    /// Gets an instance of <see cref="String32"/> of <see cref="Length"/> zero.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static readonly String32 Empty = new String32(Utf.theEmptyChar32s);

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
    /// Gets whether the instance is <see langword="default"/> (the <see langword="null"/> wrapper) or empty (of <see cref="Length"/> zero).
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
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
    private static int CompareImpl(Char32[] x, Char32[] y)
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
      return CompareImpl(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <=(String32 x, String32 y)
    {
      return CompareImpl(x.myData, y.myData) <= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >=(String32 x, String32 y)
    {
      return CompareImpl(x.myData, y.myData) >= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <(String32 x, String32 y)
    {
      return CompareImpl(x.myData, y.myData) < 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >(String32 x, String32 y)
    {
      return CompareImpl(x.myData, y.myData) > 0;
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public int CompareTo(String32 other)
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
        : obj is String32 other
        ? CompareImpl(myData, other.myData)
        : throw new ArgumentException("The argument '" + nameof(obj) + "' must be Neat.Unicode.String32 or null.", nameof(obj));
    }

    #endregion Compare, order operators, IComparable<String32> members, IComparable members

    #region Equals, equality operators, IEquatable<String32> members, object members

    [MethodImpl(Helper.JustOptimize)]
    private static bool EqualsImpl(Char32[] x, Char32[] y)
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
      return EqualsImpl(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(String32 x, String32 y)
    {
      return EqualsImpl(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(String32 x, String32 y)
    {
      return !EqualsImpl(x.myData, y.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(String32 other)
    {
      return EqualsImpl(myData, other.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public override bool Equals(object obj)
    {
      return (obj is String32 other) && EqualsImpl(myData, other.myData);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static int GetHashCodeImpl(Char32[] x)
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
      return GetHashCodeImpl(myData);
    }

    /// <summary>
    /// Equivalent to <see cref="Utf.String32ToString16Replace(String32)"/>.
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper),
    /// in which case it returns <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public override string ToString()
    {
      return Utf.String32ToString16Replace(this);
    }

    #endregion Equals, equality operators, IEquatable<String32> members, object members

    /// <summary>
    /// Equivalent to <see cref="Utf.SanitizeString32(String32)"/>.
    /// This method can be called on the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public String32 Sanitize()
    {
      return Utf.SanitizeString32(this);
    }

    #region Create, GetPinnableReference, AsSpan, AsMemory

    /// <summary>
    /// Creates a new <see cref="String8"/> and initializes it with the specified delegate.
    /// Similar to <see cref="string.Create{TState}(int, TState, SpanAction{char, TState})"/>.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.JustOptimize)]
    public static String32 Create<TState>(int length, TState state, SpanAction<Char32, TState> action)
    {
      if ((uint)length > (uint)Utf.MaximumLength32)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }
      Char32[] data = new Char32[length];
      action(new Span<Char32>(data), state);
      return new String32(data);
    }

    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(Helper.OptimizeInline)]
    public readonly ref Char32 GetPinnableReference()
    {
      Char32[] data = myData;
      int throwIfNull = data.Length;
      return ref MemoryMarshal.GetArrayDataReference(data);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlySpan<Char32> AsSpan()
    {
      Char32[] data = myData;
      int throwIfNull = data.Length;
      return new ReadOnlySpan<Char32>(data);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public ReadOnlySpan<Char32> AsSpan(int start)
    {
      Char32[] data = myData;
      int dataLength = data.Length;
      if ((uint)start > (uint)dataLength)
      {
        throw new ArgumentOutOfRangeException(nameof(start));
      }
      return new ReadOnlySpan<Char32>(data, start, dataLength - start);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public ReadOnlySpan<Char32> AsSpan(int start, int length)
    {
      Char32[] data = myData;
      int dataLength = data.Length;
      if ((uint)start > (uint)dataLength)
      {
        throw new ArgumentOutOfRangeException(nameof(start));
      }
      if ((uint)length > (uint)(dataLength - start))
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }
      return new ReadOnlySpan<Char32>(data, start, length);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlyMemory<Char32> AsMemory()
    {
      Char32[] data = myData;
      int throwIfNull = data.Length;
      return new ReadOnlyMemory<Char32>(data);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public ReadOnlyMemory<Char32> AsMemory(int start)
    {
      Char32[] data = myData;
      int dataLength = data.Length;
      if ((uint)start > (uint)dataLength)
      {
        throw new ArgumentOutOfRangeException(nameof(start));
      }
      return new ReadOnlyMemory<Char32>(data, start, dataLength - start);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public ReadOnlyMemory<Char32> AsMemory(int start, int length)
    {
      Char32[] data = myData;
      int dataLength = data.Length;
      if ((uint)start > (uint)dataLength)
      {
        throw new ArgumentOutOfRangeException(nameof(start));
      }
      if ((uint)length > (uint)(dataLength - start))
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }
      return new ReadOnlyMemory<Char32>(data, start, length);
    }

    #endregion Create, GetPinnableReference, AsSpan, AsMemory

    /// <summary>
    /// Enumerates <see cref="Char32"/> instances in a <see cref="String32"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public struct Enumerator : IEnumerator2<Char32>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Char32[] myData;

      private int myIndex;

#if DEBUG
      private bool myDisposed;
#endif

      private string DebuggerDisplay()
      {
        return "Index = " + myIndex.ToString(CultureInfo.InvariantCulture)
#if DEBUG
          + (myDisposed ? ", Disposed = True" : ", Disposed = False")
#endif
          ;
      }

      [MethodImpl(Helper.OptimizeInline)]
      internal Enumerator(Char32[] data)
      {
        myData = data;
        myIndex = -1;
#if DEBUG
        myDisposed = false;
#endif
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
#if DEBUG
        if (myDisposed)
        {
          throw new ObjectDisposedException("Neat.Unicode.String32.Enumerator");
        }
#endif
        myIndex = -1;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext(out Char32 item)
      {
#if DEBUG
        if (myDisposed)
        {
          throw new ObjectDisposedException("Neat.Unicode.String32.Enumerator");
        }
#endif
        Char32[] data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
#if DEBUG
        myDisposed = true;
#endif
        item = default(Char32);
        return false;
      }

      [MethodImpl(Helper.JustOptimize)]
      bool IEnumerator2.MoveNext(out object item)
      {
#if DEBUG
        if (myDisposed)
        {
          throw new ObjectDisposedException("Neat.Unicode.String32.Enumerator");
        }
#endif
        Char32[] data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
#if DEBUG
        myDisposed = true;
#endif
        item = null;
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext()
      {
#if DEBUG
        if (myDisposed)
        {
          throw new ObjectDisposedException("Neat.Unicode.String32.Enumerator");
        }
#endif
        return ++myIndex < myData.Length;
      }

      public Char32 Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if DEBUG
          if (myDisposed)
          {
            throw new ObjectDisposedException("Neat.Unicode.String32.Enumerator");
          }
#endif
          return myData[myIndex];
        }
      }

      /// <summary>
      /// This property is hidden in the debugging view to work around
      /// <a href="https://developercommunity.visualstudio.com/t/Inspecting-a-property-returning-a-field/10056308">this bug of Visual Studio</a>.
      /// </summary>
      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      object IEnumerator.Current
      {
        [MethodImpl(Helper.JustOptimize)]
        get
        {
#if DEBUG
          if (myDisposed)
          {
            throw new ObjectDisposedException("Neat.Unicode.String32.Enumerator");
          }
#endif
          return myData[myIndex];
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IDisposable.Dispose()
      {
#if DEBUG
        myDisposed = true;
#endif
      }
    }

    #region GetEnumerator

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

    #endregion GetEnumerator

    /// <summary>
    /// Standard implementation of <see cref="IComparer{T}"/> and <see cref="IEqualityComparer2{T}"/> for <see cref="String32"/>.
    /// The comparison is ordinal.
    /// </summary>
    public struct Comparer : IComparer<String32>, IEqualityComparer2<String32>
    {
      [MethodImpl(Helper.OptimizeInline)]
      public int Compare(String32 x, String32 y)
      {
        return CompareImpl(x.myData, y.myData);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Equals(String32 x, String32 y)
      {
        return EqualsImpl(x.myData, y.myData);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public int GetHashCode(String32 obj)
      {
        return GetHashCodeImpl(obj.myData);
      }
    }
  }
}
