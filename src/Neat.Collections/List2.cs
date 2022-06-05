using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Neat.Collections
{
  public sealed class List2<T> : IEnumerable2<T, List2<T>.Enumerator>, IReadOnlyList<T>, IList<T>, IList
  {
    private T[] myData;
    private int myCount;

#if LIST2_ENUMERATION_VERSION
    private uint myVersion;
#endif

    #region constructors

    public List2()
    {
      throw new NotImplementedException();
    }

    public List2(int capacityAtLeast)
    {
      throw new NotImplementedException();
    }

    public List2(IEnumerable<T> items)
    {
      throw new NotImplementedException();
    }

    #endregion constructors

    #region Count, IReadOnlyCollection<T>.Count, ICollection<T>.Count, ICollection.Count

    /// <summary>
    /// Gets the number of items in this list.
    /// </summary>
    public int Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myCount;
      }
    }

    int IReadOnlyCollection<T>.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myCount;
      }
    }

    int ICollection<T>.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myCount;
      }
    }

    int ICollection.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myCount;
      }
    }

    #endregion Count, IReadOnlyCollection<T>.Count, ICollection<T>.Count, ICollection.Count

    #region this[int index], IReadOnlyList<T>.this[int index], IList<T>.this[int index], IList.this[int index]

    /// <summary>
    /// Gets or sets the item at the specified index.
    /// </summary>
    public T this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[(uint)index < (uint)myCount ? index : -1];
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
#if LIST2_ENUMERATION_VERSION
        ++myVersion;
#endif
        myData[(uint)index < (uint)myCount ? index : -1] = value;
      }
    }

    T IReadOnlyList<T>.this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[(uint)index < (uint)myCount ? index : -1];
      }
    }

    T IList<T>.this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[(uint)index < (uint)myCount ? index : -1];
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
#if LIST2_ENUMERATION_VERSION
        ++myVersion;
#endif
        myData[(uint)index < (uint)myCount ? index : -1] = value;
      }
    }

    object IList.this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[(uint)index < (uint)myCount ? index : -1];
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
#if LIST2_ENUMERATION_VERSION
        ++myVersion;
#endif
        myData[(uint)index < (uint)myCount ? index : -1] = (T)value;
      }
    }

    #endregion this[int index], IReadOnlyList<T>.this[int index], IList<T>.this[int index], IList.this[int index]

    #region Reverse

    /// <summary>
    /// Reverses the list.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void Reverse()
    {
      Array.Reverse(myData, 0, myCount);
    }

    /// <summary>
    /// Reverses the specified range of the list.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void Reverse(int start, int length)
    {
      T[] data = myData;
      int count = myCount;
      if ((uint)start > (uint)count)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(count - start))
      {
        List2.ThrowLength();
      }
      Array.Reverse(data, start, length);
    }

    #endregion Reverse

    #region Capacity, SetCapacity, EnsureCapacity, TrimExcess

    public int Capacity
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public void SetCapacity(int capacity)
    {
      throw new NotImplementedException();
    }

    public void EnsureCapacity(int capacityAtLeast)
    {
      throw new NotImplementedException();
    }

    public void TrimExcess()
    {
      throw new NotImplementedException();
    }

    #endregion Capacity, SetCapacity, EnsureCapacity, TrimExcess

    #region CopyTo, ICollection<T>.CopyTo, ICollection.CopyTo

    public void CopyTo(T[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }

    #endregion CopyTo, ICollection<T>.CopyTo, ICollection.CopyTo

    #region ToArray, GetRange

    public T[] ToArray()
    {
      throw new NotImplementedException();
    }

    public T[] ToArray(int start, int length)
    {
      throw new NotImplementedException();
    }

    public List2<T> GetRange(int start, int length)
    {
      throw new NotImplementedException();
    }

    #endregion ToArray, GetRange

    #region Clear, ICollection<T>.Clear, IList.Clear

    public void Clear()
    {
      throw new NotImplementedException();
    }

    void ICollection<T>.Clear()
    {
      throw new NotImplementedException();
    }

    void IList.Clear()
    {
      throw new NotImplementedException();
    }

    #endregion Clear, ICollection<T>.Clear, IList.Clear

    #region Add, ICollection<T>.Add, IList.Add

    public void Add(T item)
    {
      throw new NotImplementedException();
    }

    void ICollection<T>.Add(T item)
    {
      throw new NotImplementedException();
    }

    int IList.Add(object value)
    {
      throw new NotImplementedException();
    }

    #endregion Add, ICollection<T>.Add, IList.Add

    #region AddRange

    public void AddRange(T[] array)
    {
      throw new NotImplementedException();
    }

    public void AddRange(T[] array, int start, int length)
    {
      throw new NotImplementedException();
    }

    public void AddRange(List2<T> list)
    {
      throw new NotImplementedException();
    }

    public void AddRange(List2<T> list, int start, int length)
    {
      throw new NotImplementedException();
    }

    public void AddRange(IEnumerable<T> enumerable)
    {
      throw new NotImplementedException();
    }

    #endregion AddRange

    #region Insert, IList<T>.Insert, IList.Insert

    public void Insert(int index, T item)
    {
      throw new NotImplementedException();
    }

    void IList<T>.Insert(int index, T item)
    {
      throw new NotImplementedException();
    }

    void IList.Insert(int index, object value)
    {
      throw new NotImplementedException();
    }

    #endregion Insert, IList<T>.Insert, IList.Insert

    #region InsertRange

    public void InsertRange(int index, T[] source)
    {
      throw new NotImplementedException();
    }

    public void InsertRange(int index, T[] source, int start, int length)
    {
      throw new NotImplementedException();
    }

    public void InsertRange(int index, List2<T> source)
    {
      throw new NotImplementedException();
    }

    public void InsertRange(int index, List2<T> source, int start, int length)
    {
      throw new NotImplementedException();
    }

    public void InsertRange(int index, IEnumerable<T> source)
    {
      throw new NotImplementedException();
    }

    #endregion InsertRange

    #region RemoveAt, RemoveRange, IList<T>.RemoveAt, IList.RemoveAt

    public void RemoveAt(int index)
    {
      throw new NotImplementedException();
    }

    public void RemoveRange(int start, int length)
    {
      throw new NotImplementedException();
    }

    void IList<T>.RemoveAt(int index)
    {
      throw new NotImplementedException();
    }

    void IList.RemoveAt(int index)
    {
      throw new NotImplementedException();
    }

    #endregion RemoveAt, RemoveRange, IList<T>.RemoveAt, IList.RemoveAt

    #region FirstIndexOf, LastIndexOf, IList<T>.IndexOf, IList.IndexOf

    public int FirstIndexOf(T item)
    {
      throw new NotImplementedException();
    }

    public int LastIndexOf(T item)
    {
      throw new NotImplementedException();
    }

    int IList<T>.IndexOf(T item)
    {
      throw new NotImplementedException();
    }

    int IList.IndexOf(object value)
    {
      throw new NotImplementedException();
    }

    #endregion FirstIndexOf, LastIndexOf, IList<T>.IndexOf, IList.IndexOf

    #region Contains, ICollection<T>.Contains, IList.Contains

    public bool Contains(T item)
    {
      throw new NotImplementedException();
    }

    bool ICollection<T>.Contains(T item)
    {
      throw new NotImplementedException();
    }

    bool IList.Contains(object value)
    {
      throw new NotImplementedException();
    }

    #endregion Contains, ICollection<T>.Contains, IList.Contains

    #region RemoveFirst, RemoveLast, RemoveAll, ICollection<T>.Remove, IList.Remove

    public int RemoveFirst(T item)
    {
      throw new NotImplementedException();
    }

    public int RemoveLast(T item)
    {
      throw new NotImplementedException();
    }

    public int RemoveAll(T item)
    {
      throw new NotImplementedException();
    }

    bool ICollection<T>.Remove(T item)
    {
      throw new NotImplementedException();
    }

    void IList.Remove(object value)
    {
      throw new NotImplementedException();
    }

    #endregion RemoveFirst, RemoveLast, RemoveAll, ICollection<T>.Remove, IList.Remove

    #region IPredicate, ThereExists, ForAll, CountSuchThat

    public interface IPredicate
    {
      bool Invoke(List2<T> list, int index, T item);
    }

    public bool ThereExists<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    public bool ForAll<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    public int CountSuchThat<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    #endregion IPredicate, ThereExists, ForAll, CountSuchThat

    #region FirstSuchThat, LastSuchThat

    public int FirstSuchThat<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    public int FirstSuchThat<TPredicate>(TPredicate predicate, int afterInclusive) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    public int FirstSuchThat<TPredicate>(TPredicate predicate, int afterInclusive, int length) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    public int LastSuchThat<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    public int LastSuchThat<TPredicate>(TPredicate predicate, int beforeExclusive) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    public int LastSuchThat<TPredicate>(TPredicate predicate, int beforeExclusive, int length) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    #endregion FirstSuchThat, LastSuchThat

    #region RemoveAllSuchThat

    public int RemoveAllSuchThat<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    public int RemoveAllSuchThat<TPredicate>(TPredicate predicate, int start, int length) where TPredicate : IPredicate
    {
      throw new NotImplementedException();
    }

    #endregion RemoveAllSuchThat

    #region IList.IsFixedSize, ICollection<T>.IsReadOnly, IList.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

    bool IList.IsFixedSize
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    bool IList.IsReadOnly
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    #endregion IList.IsFixedSize, ICollection<T>.IsReadOnly, IList.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

    /// <summary>
    /// Enumerates items in <see cref="List2{T}"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public struct Enumerator : IEnumerator2<T>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private T[] myData;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private int myCount;

      private int myIndex;

#if LIST2_ENUMERATION_VERSION

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private List2<T> myTarget;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private uint myVersion;

#endif

#if LIST2_ENUMERATOR_DISPOSE
      private bool myNotDisposed;
#endif

      private string DebuggerDisplay()
      {
        return "Count = " + myCount.ToString(CultureInfo.InvariantCulture)
          + ", Index = " + myIndex.ToString(CultureInfo.InvariantCulture)
#if LIST2_ENUMERATION_VERSION
          + ", Version = " + myVersion.ToString(CultureInfo.InvariantCulture)
#endif
#if LIST2_ENUMERATOR_DISPOSE
          + (myNotDisposed ? "" : " <disposed>")
#endif
          ;
      }

      [MethodImpl(Helper.OptimizeInline)]
      internal Enumerator(List2<T> target)
      {
        myData = target.myData;
        myCount = target.myCount;
        myIndex = -1;
#if LIST2_ENUMERATION_VERSION
        myTarget = target;
        myVersion = target.myVersion;
#endif
#if LIST2_ENUMERATOR_DISPOSE
        myNotDisposed = true;
#endif
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
#if LIST2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          List2.ThrowDisposed();
        }
#endif
#if LIST2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          List2.ThrowVersion();
        }
#endif
        myIndex = -1;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext(out T item)
      {
#if LIST2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          List2.ThrowDisposed();
        }
#endif
#if LIST2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          List2.ThrowVersion();
        }
#endif
        T[] data = myData;
        int count = myCount;
        int index = myIndex;
        myIndex = ++index;
        if (index < count)
        {
          item = data[index];
          return true;
        }
#if LIST2_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
        item = default(T);
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      bool IEnumerator2.MoveNext(out object item)
      {
#if LIST2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          List2.ThrowDisposed();
        }
#endif
#if LIST2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          List2.ThrowVersion();
        }
#endif
        T[] data = myData;
        int count = myCount;
        int index = myIndex;
        myIndex = ++index;
        if (index < count)
        {
          item = data[index];
          return true;
        }
#if LIST2_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
        item = null;
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext()
      {
#if LIST2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          List2.ThrowDisposed();
        }
#endif
#if LIST2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          List2.ThrowVersion();
        }
#endif
        return ++myIndex < myCount;
      }

      public T Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if LIST2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            List2.ThrowDisposed();
          }
#endif
#if LIST2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            List2.ThrowVersion();
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
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if LIST2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            List2.ThrowDisposed();
          }
#endif
#if LIST2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            List2.ThrowVersion();
          }
#endif
          return myData[myIndex];
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IDisposable.Dispose()
      {
#if LIST2_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
      }
    }

    #region GetEnumerator

    [MethodImpl(Helper.OptimizeInline)]
    public Enumerator GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    Enumerator IEnumerable2<T, Enumerator>.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator2<T> IEnumerable2<T>.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new Enumerator(this);
    }

    #endregion GetEnumerator
  }

  public static class List2
  {

    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowStart()
    {
      throw new ArgumentOutOfRangeException("start");
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowLength()
    {
      throw new ArgumentOutOfRangeException("length");
    }

#if LIST2_ENUMERATION_VERSION

    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowVersion()
    {
      throw new InvalidOperationException("The list is modified during enumeration. (This check is enabled by LIST2_ENUMERATION_VERSION.)");
    }

#endif

#if LIST2_ENUMERATOR_DISPOSE

    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowDisposed()
    {
      throw new ObjectDisposedException(typeof(List2<>.Enumerator).FullName,
        "The enumerator is already disposed. (This check is enabled by LIST2_ENUMERATOR_DISPOSE.)");
    }

#endif

  }
}
