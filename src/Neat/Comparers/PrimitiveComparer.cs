using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Neat.Comparers
{
  public struct PrimitiveComparer
    : IEqualityComparer2<bool>, IComparer<bool>,
      IEqualityComparer2<byte>, IComparer<byte>,
      IEqualityComparer2<sbyte>, IComparer<sbyte>,
      IEqualityComparer2<char>, IComparer<char>,
      IEqualityComparer2<decimal>, IComparer<decimal>,
      IEqualityComparer2<double>, IComparer<double>,
      IEqualityComparer2<float>, IComparer<float>,
      IEqualityComparer2<int>, IComparer<int>,
      IEqualityComparer2<uint>, IComparer<uint>,
      IEqualityComparer2<nint>, IComparer<nint>,
      IEqualityComparer2<nuint>, IComparer<nuint>,
      IEqualityComparer2<long>, IComparer<long>,
      IEqualityComparer2<ulong>, IComparer<ulong>,
      IEqualityComparer2<short>, IComparer<short>,
      IEqualityComparer2<ushort>, IComparer<ushort>,
      IEqualityComparer2<string>, IComparer<string>
  {
    public static PrimitiveComparer Instance
    {
      [MethodImpl(Helper.OptimizeInline)]
      get { return default(PrimitiveComparer); }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(bool x, bool y)
    {
      return x ^ y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(bool obj)
    {
      return obj ? 0 : 1;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(bool x, bool y)
    {
      return x ? y ? 0 : 1 : y ? -1 : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(byte x, byte y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(byte obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(byte x, byte y)
    {
      return (int)x - (int)y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(sbyte x, sbyte y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(sbyte obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(sbyte x, sbyte y)
    {
      return (int)x - (int)y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(char x, char y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(char obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-65536 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(char x, char y)
    {
      return (int)x - (int)y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(decimal x, decimal y)
    {
      return decimal.Equals(x, y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(decimal obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(decimal x, decimal y)
    {
      return decimal.Compare(x, y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(double x, double y)
    {
      /* This handles NaN correctly. */
      return x.Equals(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(double obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(double x, double y)
    {
      /* This handles NaN correctly. */
      return x.CompareTo(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(float x, float y)
    {
      /* This handles NaN correctly. */
      return x.Equals(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(float obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(float x, float y)
    {
      /* This handles NaN correctly. */
      return x.CompareTo(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(int x, int y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(int obj)
    {
      return obj;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(int x, int y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(uint x, uint y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(uint obj)
    {
      return (int)obj;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(uint x, uint y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(nint x, nint y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(nint obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(nint x, nint y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(nuint x, nuint y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(nuint obj)
    {
      return obj.GetHashCode();
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(nuint x, nuint y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(long x, long y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(long obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(long x, long y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(ulong x, ulong y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(ulong obj)
    {
      return obj.GetHashCode();
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(ulong x, ulong y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(short x, short y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(short obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-65536 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(short x, short y)
    {
      return (int)x - (int)y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(ushort x, ushort y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(ushort obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-65536 subtraction.")]
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(ushort x, ushort y)
    {
      return (int)x - (int)y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(string x, string y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(string obj)
    {
      return obj is null ? 0 : obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(string x, string y)
    {
      return string.CompareOrdinal(x, y);
    }
  }
}
