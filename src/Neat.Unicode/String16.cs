using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Neat.Collections;

namespace Neat.Unicode
{
  /// <summary>
  /// Provides extension methods for <see langword="string"/>.
  /// </summary>
  public static class String16
  {
    /// <summary>
    /// Gets whether the instance is the <see langword="null"/> reference.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsDefault(this string string16)
    {
      return ReferenceEquals(string16, null);
    }

    /// <summary>
    /// Gets whether the instance is the <see langword="null"/> reference or empty (of <see cref="string.Length"/> zero).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsDefaultOrEmpty(this string string16)
    {
      return ReferenceEquals(string16, null) || string16.Length == 0;
    }

    /// <summary>
    /// Equivalent to <see cref="Utf.SanitizeString16(string)"/>.
    /// This method can be called on the <see langword="null"/> reference.
    /// </summary>
    public static string Sanitize(this string string16)
    {
      return Utf.SanitizeString16(string16);
    }

    /// <summary>
    /// Enumerates <see langword="char"/> instances in <see langword="string"/>.
    /// </summary>
    public struct Enumerator : IEnumerator2<char>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private string myData;
      private int myIndex;

      [MethodImpl(Helper.OptimizeInline)]
      internal Enumerator(string data)
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
      public bool MoveNext(out char item)
      {
        string data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
        item = default(char);
        return false;
      }

      [MethodImpl(Helper.JustOptimize)]
      bool IEnumerator2.MoveNext(out object item)
      {
        string data = myData;
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

      public char Current
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
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> reference).
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static Enumerator GetEnumerator2(this string string16)
    {
      int throwIfNull = string16.Length;
      return new Enumerator(string16);
    }

    /// <summary>
    /// Standard implementation of <see cref="IComparer{T}"/> and <see cref="IEqualityComparer2{T}"/> for <see langword="string"/>.
    /// The comparison is ordinal.
    /// </summary>
    public struct Comparer : IComparer<string>, IEqualityComparer2<string>
    {
      [MethodImpl(Helper.OptimizeInline)]
      public int Compare(string x, string y)
      {
        return string.CompareOrdinal(x, y);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Equals(string x, string y)
      {
        return string.Equals(x, y);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public int GetHashCode(string obj)
      {
        return ReferenceEquals(obj, null) ? 0 : obj.GetHashCode();
      }
    }
  }
}
