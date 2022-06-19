using System;
using System.Collections;
using System.Collections.Generic;

namespace Neat.Collections
{
  /// <summary>
  /// All static members of this class are thread-safe.
  /// No instance member of this class is thread-safe unless explicitly stated otherwise.
  /// Any member that could mutate the map in any way other than removing a particular key will invalidate all existing enumeration operations,
  /// even if no actual change is made, unless explicitly stated otherwise.
  /// As an example, enumerator-invalidating operations include clearing, trimming, and defragmenting.
  /// The equality comparer must not mutate the instance to which it belongs, even if that mutation does not invalidate existing enumerators
  /// (i.e., even if the mutation is removing a particular key).
  /// </summary>
  public abstract class Map2<TKey, TValue>
    : IEnumerable2<KeyValuePair<TKey, TValue>, Map2<TKey, TValue>.Enumerator>,
      IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary
  {
    /// <summary>
    /// Makes it impossible to create non-abstract derived classes outside this assembly.
    /// </summary>
    private protected abstract void InternalInheritance();

    /// <summary>
    /// Gets the number of key/value pairs.
    /// </summary>
    public int Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Clears the key/value pairs.
    /// </summary>
    public void Clear()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Reduces the memory usage opportunistically.
    /// </summary>
    /// <returns><see langword="true"/> if reallocation happened.</returns>
    public bool TrimExcess()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Improves enumeration efficiency.
    /// </summary>
    /// <param name="force"><see langword="true"/> if defragmentation is performed as long as there is any fragmentation.</param>
    /// <returns><see langword="true"/> if defragmentation happened.
    /// The return value can be <see langword="false"/> even if <paramref name="force"/> is <see langword="true"/>,
    /// when there is no fragmentation at all.</returns>
    public bool Defragment(bool force)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Copies the key/value pairs to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="arrayIndex"/> is negative,
    /// or greater than the length of <paramref name="array"/> minus <see cref="Count"/>.</exception>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets or sets the value corresponding to the specified key.
    /// </summary>
    /// <exception cref="KeyNotFoundException">If the key does not exist when getting the value.</exception>
    public TValue this[TKey key]
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

    /// <returns><see langword="true"/> if the key exists.</returns>
    public bool ContainsKey(TKey key)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is written to once, with the value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is not written to.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    public bool TryGet(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is written to once, with the value being the value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is written to once, with the value <see langword="default"/>.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    public bool GetOrDefault(TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to swap the value corresponding to <paramref name="key"/> with <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is read once, whose value becomes the new value corresponding to <paramref name="key"/>,
    /// and <paramref name="value"/> is written to once afterwards, with the old value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is neither read nor written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    public bool TrySwap(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <returns><see langword="true"/> if the key existed.</returns>
    public bool Remove(TKey key)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/> and remove <paramref name="key"/> from the map.
    /// If the key existed, <paramref name="value"/> is written to once, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is not written to.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key existed.</returns>
    public bool TryGetAndRemove(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/> and remove <paramref name="key"/> from the map.
    /// If the key existed, <paramref name="value"/> is written to once, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is written to once, with the value <see langword="default"/>.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key existed.</returns>
    public bool GetAndRemoveOrDefault(TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to add <paramref name="key"/> with <paramref name="value"/>.
    /// If <paramref name="key"/> already exists, its existing value is not updated.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    public bool TryAddNew(TKey key, TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds or replaces the value correponding to <paramref name="key"/> as/by <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    public bool AddOrReplace(TKey key, TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// If the key existed, <paramref name="value"/> is not read, but written to once, with the existing value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is read once, whose value becomes the value corresponding to <paramref name="key"/>, and is never written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    public bool AddOrGet(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// If the key existed, <paramref name="value"/> is read once, whose value becomes the new value corresponding to <paramref name="key"/>,
    /// and <paramref name="value"/> is written to once afterwards, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is read once, whose value becomes the value corresponding to <paramref name="key"/>, and is never written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    public bool AddOrSwap(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets a view of the keys.
    /// Obtaining such a view is thread-safe.
    /// </summary>
    public KeyView Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets a view of the values.
    /// Obtaining such a view is thread-safe.
    /// </summary>
    public ValueView Values
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public Enumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #region IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count, ICollection<KeyValuePair<TKey, TValue>>.Count, ICollection.Count

    int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    int ICollection<KeyValuePair<TKey, TValue>>.Count
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

    #endregion IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count, ICollection<KeyValuePair<TKey, TValue>>.Count, ICollection.Count

    #region ICollection<KeyValuePair<TKey, TValue>>.Clear, IDictionary.Clear

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
      throw new NotImplementedException();
    }

    void IDictionary.Clear()
    {
      throw new NotImplementedException();
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.Clear, IDictionary.Clear

    #region IReadOnlyDictionary<TKey, TValue>.this[TKey key], IDictionary<TKey, TValue>.this[TKey key], IDictionary.this[object key]

    TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
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

    object IDictionary.this[object key]
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

    #endregion IReadOnlyDictionary<TKey, TValue>.this[TKey key], IDictionary<TKey, TValue>.this[TKey key], IDictionary.this[object key]

    #region IDictionary<TKey, TValue>.Add, ICollection<KeyValuePair<TKey, TValue>>.Add, IDictionary.Add

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
      throw new NotImplementedException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      throw new NotImplementedException();
    }

    void IDictionary.Add(object key, object value)
    {
      throw new NotImplementedException();
    }

    #endregion IDictionary<TKey, TValue>.Add, ICollection<KeyValuePair<TKey, TValue>>.Add, IDictionary.Add

    #region IReadOnlyDictionary<TKey, TValue>.TryGetValue, IDictionary<TKey, TValue>.TryGetValue

    bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.TryGetValue, IDictionary<TKey, TValue>.TryGetValue

    #region IReadOnlyDictionary<TKey, TValue>.ContainsKey, IDictionary<TKey, TValue>.ContainsKey, ICollection<KeyValuePair<TKey, TValue>>.Contains, IDictionary.Contains

    bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
    {
      throw new NotImplementedException();
    }

    bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
    {
      throw new NotImplementedException();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
      throw new NotImplementedException();
    }

    bool IDictionary.Contains(object key)
    {
      throw new NotImplementedException();
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.ContainsKey, IDictionary<TKey, TValue>.ContainsKey, ICollection<KeyValuePair<TKey, TValue>>.Contains, IDictionary.Contains

    #region IDictionary<TKey, TValue>.Remove, ICollection<KeyValuePair<TKey, TValue>>.Remove, IDictionary.Remove

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
      throw new NotImplementedException();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
      throw new NotImplementedException();
    }

    void IDictionary.Remove(object key)
    {
      throw new NotImplementedException();
    }

    #endregion IDictionary<TKey, TValue>.Remove, ICollection<KeyValuePair<TKey, TValue>>.Remove, IDictionary.Remove

    #region ICollection<KeyValuePair<TKey, TValue>>.CopyTo, ICollection.CopyTo

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.CopyTo, ICollection.CopyTo

    #region IReadOnlyDictionary<TKey, TValue>.Keys, IDictionary<TKey, TValue>.Keys, IDictionary.Keys

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    ICollection IDictionary.Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.Keys, IDictionary<TKey, TValue>.Keys, IDictionary.Keys

    #region IReadOnlyDictionary<TKey, TValue>.Values, IDictionary<TKey, TValue>.Values, IDictionary.Values

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    ICollection IDictionary.Values
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.Values, IDictionary<TKey, TValue>.Values, IDictionary.Values

    #region ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly, IDictionary.IsReadOnly, IDictionary.IsFixedSize, ICollection.IsSynchronized, ICollection.SyncRoot

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    bool IDictionary.IsReadOnly
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    bool IDictionary.IsFixedSize
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

    #endregion ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly, IDictionary.IsReadOnly, IDictionary.IsFixedSize, ICollection.IsSynchronized, ICollection.SyncRoot

    #region GetEnumerator (explicit implementations)

    Enumerator IEnumerable2<KeyValuePair<TKey, TValue>, Enumerator>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator2<KeyValuePair<TKey, TValue>> IEnumerable2<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion GetEnumerator (explicit implementations)

    /// <summary>
    /// Enumerates key/value pairs in <see cref="Map2{TKey, TValue}"/>.
    /// </summary>
    public struct Enumerator : IEnumerator2<KeyValuePair<TKey, TValue>>
    {
      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }

      public bool MoveNext(out KeyValuePair<TKey, TValue> item)
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

      public KeyValuePair<TKey, TValue> Current
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

    /// <summary>
    /// Enumerates keys in <see cref="Map2{TKey, TValue}"/>.
    /// </summary>
    public struct KeyEnumerator : IEnumerator2<TKey>
    {
      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }

      public bool MoveNext(out TKey item)
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

      public TKey Current
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

    /// <summary>
    /// Enumerates values in <see cref="Map2{TKey, TValue}"/>.
    /// </summary>
    public struct ValueEnumerator : IEnumerator2<TValue>
    {
      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }

      public bool MoveNext(out TValue item)
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

      public TValue Current
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

    /// <summary>
    /// Represents a view of the keys in a <see cref="Map2{TKey, TValue}"/> instance.
    /// </summary>
    public struct KeyView : IEquatable<KeyView>, IEnumerable2<TKey, KeyEnumerator>, ICollection<TKey>, IReadOnlyCollection<TKey>, ICollection
    {
      public int Count
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool Contains(TKey item)
      {
        throw new NotImplementedException();
      }

      public void CopyTo(TKey[] array, int arrayIndex)
      {
        throw new NotImplementedException();
      }

      public KeyEnumerator GetEnumerator()
      {
        throw new NotImplementedException();
      }

      public bool Equals(KeyView other)
      {
        throw new NotImplementedException();
      }

      #region static Equals, operator ==, operator !=

      public static bool Equals(KeyView x, KeyView y)
      {
        throw new NotImplementedException();
      }

      public static bool operator ==(KeyView x, KeyView y)
      {
        throw new NotImplementedException();
      }

      public static bool operator !=(KeyView x, KeyView y)
      {
        throw new NotImplementedException();
      }

      #endregion static Equals, operator ==, operator !=

      #region object members

      public override bool Equals(object obj)
      {
        throw new NotImplementedException();
      }

      public override int GetHashCode()
      {
        throw new NotImplementedException();
      }

      public override string ToString()
      {
        throw new NotImplementedException();
      }

      #endregion object members

      #region ICollection.CopyTo

      void ICollection.CopyTo(Array array, int index)
      {
        throw new NotImplementedException();
      }

      #endregion ICollection.CopyTo

      #region ICollection<TKey>.Add, ICollection<TKey>.Clear, ICollection<TKey>.Remove

      void ICollection<TKey>.Add(TKey item)
      {
        throw new NotImplementedException();
      }

      void ICollection<TKey>.Clear()
      {
        throw new NotImplementedException();
      }

      bool ICollection<TKey>.Remove(TKey item)
      {
        throw new NotImplementedException();
      }

      #endregion ICollection<TKey>.Add, ICollection<TKey>.Clear, ICollection<TKey>.Remove

      #region ICollection<TKey>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      bool ICollection<TKey>.IsReadOnly
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

      #endregion ICollection<TKey>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      #region GetEnumerator (explicit implementations)

      IEnumerator2<TKey> IEnumerable2<TKey>.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator2 IEnumerable2.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      #endregion GetEnumerator (explicit implementations)
    }

    /// <summary>
    /// Represents a view of the values in a <see cref="Map2{TKey, TValue}"/> instance.
    /// </summary>
    public struct ValueView : IEquatable<ValueView>, IEnumerable2<TValue, ValueEnumerator>, ICollection<TValue>, IReadOnlyCollection<TValue>, ICollection
    {
      public int Count
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool Contains(TValue item)
      {
        throw new NotImplementedException();
      }

      public void CopyTo(TValue[] array, int arrayIndex)
      {
        throw new NotImplementedException();
      }

      public ValueEnumerator GetEnumerator()
      {
        throw new NotImplementedException();
      }

      public bool Equals(ValueView other)
      {
        throw new NotImplementedException();
      }

      #region static Equals, operator ==, operator !=

      public static bool Equals(ValueView x, ValueView y)
      {
        throw new NotImplementedException();
      }

      public static bool operator ==(ValueView x, ValueView y)
      {
        throw new NotImplementedException();
      }

      public static bool operator !=(ValueView x, ValueView y)
      {
        throw new NotImplementedException();
      }

      #endregion static Equals, operator ==, operator !=

      #region object members

      public override bool Equals(object obj)
      {
        throw new NotImplementedException();
      }

      public override int GetHashCode()
      {
        throw new NotImplementedException();
      }

      public override string ToString()
      {
        throw new NotImplementedException();
      }

      #endregion object members

      #region ICollection.CopyTo

      void ICollection.CopyTo(Array array, int index)
      {
        throw new NotImplementedException();
      }

      #endregion ICollection.CopyTo

      #region ICollection<TValue>.Add, ICollection<TValue>.Clear, ICollection<TValue>.Remove

      void ICollection<TValue>.Add(TValue item)
      {
        throw new NotImplementedException();
      }

      void ICollection<TValue>.Clear()
      {
        throw new NotImplementedException();
      }

      bool ICollection<TValue>.Remove(TValue item)
      {
        throw new NotImplementedException();
      }

      #endregion ICollection<TValue>.Add, ICollection<TValue>.Clear, ICollection<TValue>.Remove

      #region ICollection<TValue>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      bool ICollection<TValue>.IsReadOnly
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

      #endregion ICollection<TValue>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      #region GetEnumerator (explicit implementations)

      IEnumerator2<TValue> IEnumerable2<TValue>.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator2 IEnumerable2.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      #endregion GetEnumerator (explicit implementations)
    }
  }

  /// <summary>
  /// All static members of this class are thread-safe.
  /// No instance member of this class is thread-safe unless explicitly stated otherwise.
  /// Any member that could mutate the map in any way other than removing a particular key will invalidate all existing enumeration operations,
  /// even if no actual change is made, unless explicitly stated otherwise.
  /// As an example, enumerator-invalidating operations include clearing, trimming, and defragmenting.
  /// The equality comparer must not mutate the instance to which it belongs, even if that mutation does not invalidate existing enumerators
  /// (i.e., even if the mutation is removing a particular key).
  /// </summary>
  public sealed class Map2<TKey, TValue, TEqualityComparer>
    : IEnumerable2<KeyValuePair<TKey, TValue>, Map2<TKey, TValue, TEqualityComparer>.Enumerator>,
      IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary
    where TEqualityComparer : IEqualityComparer2<TKey>
  {
    /// <summary>
    /// Gets the number of key/value pairs.
    /// </summary>
    public int Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Clears the key/value pairs.
    /// </summary>
    public void Clear()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Reduces the memory usage opportunistically.
    /// </summary>
    /// <returns><see langword="true"/> if reallocation happened.</returns>
    public bool TrimExcess()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Improves enumeration efficiency.
    /// </summary>
    /// <param name="force"><see langword="true"/> if defragmentation is performed as long as there is any fragmentation.</param>
    /// <returns><see langword="true"/> if defragmentation happened.
    /// The return value can be <see langword="false"/> even if <paramref name="force"/> is <see langword="true"/>,
    /// when there is no fragmentation at all.</returns>
    public bool Defragment(bool force)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Copies the key/value pairs to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="arrayIndex"/> is negative,
    /// or greater than the length of <paramref name="array"/> minus <see cref="Count"/>.</exception>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets or sets the value corresponding to the specified key.
    /// </summary>
    /// <exception cref="KeyNotFoundException">If the key does not exist when getting the value.</exception>
    public TValue this[TKey key]
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

    /// <returns><see langword="true"/> if the key exists.</returns>
    public bool ContainsKey(TKey key)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is written to once, with the value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is not written to.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    public bool TryGet(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is written to once, with the value being the value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is written to once, with the value <see langword="default"/>.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    public bool GetOrDefault(TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to swap the value corresponding to <paramref name="key"/> with <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is read once, whose value becomes the new value corresponding to <paramref name="key"/>,
    /// and <paramref name="value"/> is written to once afterwards, with the old value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is neither read nor written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    public bool TrySwap(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <returns><see langword="true"/> if the key existed.</returns>
    public bool Remove(TKey key)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/> and remove <paramref name="key"/> from the map.
    /// If the key existed, <paramref name="value"/> is written to once, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is not written to.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key existed.</returns>
    public bool TryGetAndRemove(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/> and remove <paramref name="key"/> from the map.
    /// If the key existed, <paramref name="value"/> is written to once, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is written to once, with the value <see langword="default"/>.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key existed.</returns>
    public bool GetAndRemoveOrDefault(TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to add <paramref name="key"/> with <paramref name="value"/>.
    /// If <paramref name="key"/> already exists, its existing value is not updated.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    public bool TryAddNew(TKey key, TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds or replaces the value correponding to <paramref name="key"/> as/by <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    public bool AddOrReplace(TKey key, TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// If the key existed, <paramref name="value"/> is not read, but written to once, with the existing value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is read once, whose value becomes the value corresponding to <paramref name="key"/>, and is never written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    public bool AddOrGet(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// If the key existed, <paramref name="value"/> is read once, whose value becomes the new value corresponding to <paramref name="key"/>,
    /// and <paramref name="value"/> is written to once afterwards, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is read once, whose value becomes the value corresponding to <paramref name="key"/>, and is never written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    public bool AddOrSwap(TKey key, ref TValue value)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets a view of the keys.
    /// Obtaining such a view is thread-safe.
    /// </summary>
    public KeyView Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets a view of the values.
    /// Obtaining such a view is thread-safe.
    /// </summary>
    public ValueView Values
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public Enumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #region IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count, ICollection<KeyValuePair<TKey, TValue>>.Count, ICollection.Count

    int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    int ICollection<KeyValuePair<TKey, TValue>>.Count
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

    #endregion IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count, ICollection<KeyValuePair<TKey, TValue>>.Count, ICollection.Count

    #region ICollection<KeyValuePair<TKey, TValue>>.Clear, IDictionary.Clear

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
      throw new NotImplementedException();
    }

    void IDictionary.Clear()
    {
      throw new NotImplementedException();
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.Clear, IDictionary.Clear

    #region IReadOnlyDictionary<TKey, TValue>.this[TKey key], IDictionary<TKey, TValue>.this[TKey key], IDictionary.this[object key]

    TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
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

    object IDictionary.this[object key]
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

    #endregion IReadOnlyDictionary<TKey, TValue>.this[TKey key], IDictionary<TKey, TValue>.this[TKey key], IDictionary.this[object key]

    #region IDictionary<TKey, TValue>.Add, ICollection<KeyValuePair<TKey, TValue>>.Add, IDictionary.Add

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
      throw new NotImplementedException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      throw new NotImplementedException();
    }

    void IDictionary.Add(object key, object value)
    {
      throw new NotImplementedException();
    }

    #endregion IDictionary<TKey, TValue>.Add, ICollection<KeyValuePair<TKey, TValue>>.Add, IDictionary.Add

    #region IReadOnlyDictionary<TKey, TValue>.TryGetValue, IDictionary<TKey, TValue>.TryGetValue

    bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
    {
      throw new NotImplementedException();
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.TryGetValue, IDictionary<TKey, TValue>.TryGetValue

    #region IReadOnlyDictionary<TKey, TValue>.ContainsKey, IDictionary<TKey, TValue>.ContainsKey, ICollection<KeyValuePair<TKey, TValue>>.Contains, IDictionary.Contains

    bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
    {
      throw new NotImplementedException();
    }

    bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
    {
      throw new NotImplementedException();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
      throw new NotImplementedException();
    }

    bool IDictionary.Contains(object key)
    {
      throw new NotImplementedException();
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.ContainsKey, IDictionary<TKey, TValue>.ContainsKey, ICollection<KeyValuePair<TKey, TValue>>.Contains, IDictionary.Contains

    #region IDictionary<TKey, TValue>.Remove, ICollection<KeyValuePair<TKey, TValue>>.Remove, IDictionary.Remove

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
      throw new NotImplementedException();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
      throw new NotImplementedException();
    }

    void IDictionary.Remove(object key)
    {
      throw new NotImplementedException();
    }

    #endregion IDictionary<TKey, TValue>.Remove, ICollection<KeyValuePair<TKey, TValue>>.Remove, IDictionary.Remove

    #region ICollection<KeyValuePair<TKey, TValue>>.CopyTo, ICollection.CopyTo

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.CopyTo, ICollection.CopyTo

    #region IReadOnlyDictionary<TKey, TValue>.Keys, IDictionary<TKey, TValue>.Keys, IDictionary.Keys

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    ICollection IDictionary.Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.Keys, IDictionary<TKey, TValue>.Keys, IDictionary.Keys

    #region IReadOnlyDictionary<TKey, TValue>.Values, IDictionary<TKey, TValue>.Values, IDictionary.Values

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    ICollection IDictionary.Values
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.Values, IDictionary<TKey, TValue>.Values, IDictionary.Values

    #region ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly, IDictionary.IsReadOnly, IDictionary.IsFixedSize, ICollection.IsSynchronized, ICollection.SyncRoot

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    bool IDictionary.IsReadOnly
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    bool IDictionary.IsFixedSize
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

    #endregion ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly, IDictionary.IsReadOnly, IDictionary.IsFixedSize, ICollection.IsSynchronized, ICollection.SyncRoot

    #region GetEnumerator (explicit implementations)

    Enumerator IEnumerable2<KeyValuePair<TKey, TValue>, Enumerator>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator2<KeyValuePair<TKey, TValue>> IEnumerable2<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion GetEnumerator (explicit implementations)

    /// <summary>
    /// Enumerates key/value pairs in <see cref="Map2{TKey, TValue, TEqualityComparer}"/>.
    /// </summary>
    public struct Enumerator : IEnumerator2<KeyValuePair<TKey, TValue>>
    {
      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }

      public bool MoveNext(out KeyValuePair<TKey, TValue> item)
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

      public KeyValuePair<TKey, TValue> Current
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

    /// <summary>
    /// Enumerates keys in <see cref="Map2{TKey, TValue, TEqualityComparer}"/>.
    /// </summary>
    public struct KeyEnumerator : IEnumerator2<TKey>
    {
      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }

      public bool MoveNext(out TKey item)
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

      public TKey Current
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

    /// <summary>
    /// Enumerates values in <see cref="Map2{TKey, TValue, TEqualityComparer}"/>.
    /// </summary>
    public struct ValueEnumerator : IEnumerator2<TValue>
    {
      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }

      public bool MoveNext(out TValue item)
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

      public TValue Current
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

    /// <summary>
    /// Represents a view of the keys in a <see cref="Map2{TKey, TValue, TEqualityComparer}"/> instance.
    /// </summary>
    public struct KeyView : IEquatable<KeyView>, IEnumerable2<TKey, KeyEnumerator>, ICollection<TKey>, IReadOnlyCollection<TKey>, ICollection
    {
      public int Count
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool Contains(TKey item)
      {
        throw new NotImplementedException();
      }

      public void CopyTo(TKey[] array, int arrayIndex)
      {
        throw new NotImplementedException();
      }

      public KeyEnumerator GetEnumerator()
      {
        throw new NotImplementedException();
      }

      public bool Equals(KeyView other)
      {
        throw new NotImplementedException();
      }

      #region static Equals, operator ==, operator !=

      public static bool Equals(KeyView x, KeyView y)
      {
        throw new NotImplementedException();
      }

      public static bool operator ==(KeyView x, KeyView y)
      {
        throw new NotImplementedException();
      }

      public static bool operator !=(KeyView x, KeyView y)
      {
        throw new NotImplementedException();
      }

      #endregion static Equals, operator ==, operator !=

      #region object members

      public override bool Equals(object obj)
      {
        throw new NotImplementedException();
      }

      public override int GetHashCode()
      {
        throw new NotImplementedException();
      }

      public override string ToString()
      {
        throw new NotImplementedException();
      }

      #endregion object members

      #region ICollection.CopyTo

      void ICollection.CopyTo(Array array, int index)
      {
        throw new NotImplementedException();
      }

      #endregion ICollection.CopyTo

      #region ICollection<TKey>.Add, ICollection<TKey>.Clear, ICollection<TKey>.Remove

      void ICollection<TKey>.Add(TKey item)
      {
        throw new NotImplementedException();
      }

      void ICollection<TKey>.Clear()
      {
        throw new NotImplementedException();
      }

      bool ICollection<TKey>.Remove(TKey item)
      {
        throw new NotImplementedException();
      }

      #endregion ICollection<TKey>.Add, ICollection<TKey>.Clear, ICollection<TKey>.Remove

      #region ICollection<TKey>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      bool ICollection<TKey>.IsReadOnly
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

      #endregion ICollection<TKey>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      #region GetEnumerator (explicit implementations)

      IEnumerator2<TKey> IEnumerable2<TKey>.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator2 IEnumerable2.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      #endregion GetEnumerator (explicit implementations)
    }

    /// <summary>
    /// Represents a view of the values in a <see cref="Map2{TKey, TValue, TEqualityComparer}"/> instance.
    /// </summary>
    public struct ValueView : IEquatable<ValueView>, IEnumerable2<TValue, ValueEnumerator>, ICollection<TValue>, IReadOnlyCollection<TValue>, ICollection
    {
      public int Count
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool Contains(TValue item)
      {
        throw new NotImplementedException();
      }

      public void CopyTo(TValue[] array, int arrayIndex)
      {
        throw new NotImplementedException();
      }

      public ValueEnumerator GetEnumerator()
      {
        throw new NotImplementedException();
      }

      public bool Equals(ValueView other)
      {
        throw new NotImplementedException();
      }

      #region static Equals, operator ==, operator !=

      public static bool Equals(ValueView x, ValueView y)
      {
        throw new NotImplementedException();
      }

      public static bool operator ==(ValueView x, ValueView y)
      {
        throw new NotImplementedException();
      }

      public static bool operator !=(ValueView x, ValueView y)
      {
        throw new NotImplementedException();
      }

      #endregion static Equals, operator ==, operator !=

      #region object members

      public override bool Equals(object obj)
      {
        throw new NotImplementedException();
      }

      public override int GetHashCode()
      {
        throw new NotImplementedException();
      }

      public override string ToString()
      {
        throw new NotImplementedException();
      }

      #endregion object members

      #region ICollection.CopyTo

      void ICollection.CopyTo(Array array, int index)
      {
        throw new NotImplementedException();
      }

      #endregion ICollection.CopyTo

      #region ICollection<TValue>.Add, ICollection<TValue>.Clear, ICollection<TValue>.Remove

      void ICollection<TValue>.Add(TValue item)
      {
        throw new NotImplementedException();
      }

      void ICollection<TValue>.Clear()
      {
        throw new NotImplementedException();
      }

      bool ICollection<TValue>.Remove(TValue item)
      {
        throw new NotImplementedException();
      }

      #endregion ICollection<TValue>.Add, ICollection<TValue>.Clear, ICollection<TValue>.Remove

      #region ICollection<TValue>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      bool ICollection<TValue>.IsReadOnly
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

      #endregion ICollection<TValue>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      #region GetEnumerator (explicit implementations)

      IEnumerator2<TValue> IEnumerable2<TValue>.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator2 IEnumerable2.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      #endregion GetEnumerator (explicit implementations)
    }
  }

  public static class Map2
  {
    /// <summary>
    /// Not implemented yet.
    /// </summary>
    public const int MaximumCapacity = 0;
  }
}
