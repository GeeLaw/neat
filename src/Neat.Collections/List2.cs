using System;
using System.Collections;
using System.Collections.Generic;

namespace Neat.Collections
{
  public sealed class List2<T> : IEnumerable2<T, List2<T>.Enumerator>, IReadOnlyList<T>, IList<T>, IList
  {
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

    public int Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    int IReadOnlyCollection<T>.Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    int ICollection<T>.Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    int ICollection.Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    #endregion Count, IReadOnlyCollection<T>.Count, ICollection<T>.Count, ICollection.Count

    #region this[int index], IReadOnlyList<T>.this[int index], IList<T>.this[int index], IList.this[int index]

    public T this[int index]
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    T IReadOnlyList<T>.this[int index]
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    T IList<T>.this[int index]
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    object IList.this[int index]
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    #endregion this[int index], IReadOnlyList<T>.this[int index], IList<T>.this[int index], IList.this[int index]

    #region Reverse

    public void Reverse()
    {
      throw new NotImplementedException();
    }

    public void Reverse(int start, int length)
    {
      throw new NotImplementedException();
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

    public struct Enumerator : IEnumerator2<T>
    {
      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }

      public bool MoveNext(out T item)
      {
        throw new NotImplementedException();
      }

      bool IEnumerator2.MoveNext(out object item)
      {
        throw new NotImplementedException();
      }

      public bool MoveNext()
      {
        throw new NotImplementedException();
      }

      public T Current
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      object IEnumerator.Current
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      void IDisposable.Dispose()
      {
        throw new NotImplementedException();
      }
    }

    #region GetEnumerator

    public Enumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }

    Enumerator IEnumerable2<T, Enumerator>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator2<T> IEnumerable2<T>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion GetEnumerator
  }
}
