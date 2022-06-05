using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Neat.Collections
{
  /// <summary>
  /// A dynamically growing array.
  /// All static members of this class are thread-safe.
  /// No instance member of this class is thread-safe unless explicitly stated otherwise.
  /// </summary>
  public sealed class List2<T> : IEnumerable2<T, List2<T>.Enumerator>, IReadOnlyList<T>, IList<T>, IList
  {
    private T[] myData;
    private int myCount;

#if LIST2_ENUMERATION_VERSION
    private uint myVersion;
#endif

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly T[] theEmptyArray = new T[0];

    /// <summary>
    /// The minimum non-zero capacity favored by <see cref="List2{T}"/>.
    /// This value is <see cref="List2.StartingCapacity"/>.
    /// </summary>
    public const int StartingCapacity = List2.StartingCapacity;

    /// <summary>
    /// The maximum capacity of a list of <typeparamref name="T"/>.
    /// This value is <see cref="List2.MaximumCapacityOneByte"/>
    /// for <see langword="bool"/>, <see langword="byte"/>, <see langword="sbyte"/>, and other one-byte structures.
    /// For all other types, this value is <see cref="List2.MaximumCapacityOther"/>.
    /// </summary>
    public static readonly int MaximumCapacity;

    static List2()
    {
      MaximumCapacity = (!RuntimeHelpers.IsReferenceOrContainsReferences<T>() && Unsafe.SizeOf<T>() == 1
        ? List2.MaximumCapacityOneByte
        : List2.MaximumCapacityOther);
    }

    #region public constructors

    /// <summary>
    /// Initializes a new, empty list.
    /// </summary>
    public List2()
    {
      myData = theEmptyArray;
      myCount = 0;
#if LIST2_ENUMERATION_VERSION
      myVersion = 0;
#endif
    }

    /// <summary>
    /// Initializes a new, empty list whose <see cref="Capacity"/> is exactly <paramref name="capacity"/>.
    /// </summary>
    /// <param name="capacity">This value must be non-negative and not exceed <see cref="MaximumCapacity"/>.</param>
    public List2(int capacity)
    {
      if ((uint)capacity > (uint)MaximumCapacity)
      {
        List2.ThrowCapacity();
      }
      myData = (capacity == 0 ? theEmptyArray : new T[capacity]);
      myCount = 0;
#if LIST2_ENUMERATION_VERSION
      myVersion = 0;
#endif
    }

    public List2(IEnumerable<T> items)
    {
      throw new NotImplementedException();
    }

    #endregion public constructors

    /// <summary>
    /// This constructor does not validate its arguments.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    private List2(T[] data, int count)
    {
      myData = data;
      myCount = count;
#if LIST2_ENUMERATION_VERSION
      myVersion = 0;
#endif
    }

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
    /// <param name="start">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <see cref="Count"/> minus <paramref name="start"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
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

    /// <summary>
    /// This method does not validate its arguments.
    /// </summary>
    /// <param name="least">This number should be positive and not exceed <see cref="MaximumCapacity"/>.</param>
    /// <param name="suggested">This number should be greater than or equal to <paramref name="least"/> and not exceed <see cref="MaximumCapacity"/>.</param>
    [MethodImpl(Helper.JustOptimize)]
    private static T[] AllocImpl(int least, int suggested)
    {
    Retry:
      try
      {
        return new T[suggested];
      }
      catch (OutOfMemoryException)
      {
        if (suggested != least)
        {
          suggested = least + (suggested - least) / 2;
          goto Retry;
        }
        throw;
      }
    }

    #region Capacity, SetCapacity, EnsureCapacity, TrimExcess

    /// <summary>
    /// Gets the capacity of the list.
    /// </summary>
    public int Capacity
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData.Length;
      }
    }

    /// <summary>
    /// Sets the capacity of the list.
    /// </summary>
    /// <param name="capacity">The new capacity.
    /// This value must be at least <see cref="Count"/> and not exceed <see cref="MaximumCapacity"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="capacity"/> is out of range.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public void SetCapacity(int capacity)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (capacity < count || capacity > MaximumCapacity)
      {
        List2.ThrowCapacity();
      }
      if (capacity == 0)
      {
        myData = theEmptyArray;
        return;
      }
      if (capacity != data.Length)
      {
        T[] newData = new T[capacity];
        Array.ConstrainedCopy(data, 0, newData, 0, count);
        /* No more exception is possible beyond this point. */
        myData = newData;
      }
    }

    /// <summary>
    /// Ensures the capacity is at least the specified value and amortizes the cost of growth.
    /// </summary>
    /// <param name="capacity">The minimum value of the new capacity.
    /// This value can be less than <see cref="Count"/> (including being negative),
    /// but must not exceed <see cref="MaximumCapacity"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="capacity"/> is out of range.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public void EnsureCapacity(int capacity)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (capacity > MaximumCapacity)
      {
        List2.ThrowCapacity();
      }
      if (capacity > data.Length)
      {
        int suggested = (count > MaximumCapacity / 2
          ? MaximumCapacity
          : count <= StartingCapacity / 2
          ? StartingCapacity
          : count * 2);
        suggested = (suggested < capacity ? capacity : suggested);
        T[] newData = AllocImpl(capacity, suggested);
        Array.ConstrainedCopy(data, 0, newData, 0, count);
        /* No more exception is possible beyond this point. */
        myData = newData;
      }
    }

    /// <summary>
    /// Sets the capacity to <see cref="Count"/> opportunistically.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void TrimExcess()
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (count == 0)
      {
        myData = theEmptyArray;
        return;
      }
      if (count < (int)(data.Length * 0.9))
      {
        T[] newData;
        try
        {
          /* We are going to overwrite every element before the newly allocated array
          /* becomes available to the user, so it is fine to allocate it uninitialized. */
          newData = GC.AllocateUninitializedArray<T>(count, false);
        }
        catch (OutOfMemoryException)
        {
          /* There is no guaratee that the list will shrink
          /* and a no-op is acceptable, although arguably
          /* more out-of-memory issues are arriving soon. */
          return;
        }
        Array.ConstrainedCopy(data, 0, newData, 0, count);
        /* No more exception is possible beyond this point. */
        myData = newData;
      }
    }

    #endregion Capacity, SetCapacity, EnsureCapacity, TrimExcess

    #region CopyTo, ICollection<T>.CopyTo, ICollection.CopyTo

    /// <summary>
    /// Except for allocation efficiency,
    /// the effect is equivalent to calling <see cref="Array.ConstrainedCopy(Array, int, Array, int, int)"/>
    /// (see that method for possible exceptions) with the following arguments:
    /// <see cref="ToArray"/>, <c>0</c>, <paramref name="array"/>, <paramref name="arrayIndex"/>, <see cref="Count"/>.
    /// This means <paramref name="array"/> cannot be a covariant reference.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void CopyTo(T[] array, int arrayIndex)
    {
      Array.ConstrainedCopy(myData, 0, array, arrayIndex, myCount);
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      Array.ConstrainedCopy(myData, 0, array, arrayIndex, myCount);
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection.CopyTo(Array array, int index)
    {
      Array.ConstrainedCopy(myData, 0, array, index, myCount);
    }

    #endregion CopyTo, ICollection<T>.CopyTo, ICollection.CopyTo

    #region ToArray, GetRange

    /// <summary>
    /// Gets an array of length <see cref="Count"/> containing the items in this list.
    /// This array is guaranteed to be newly allocated if <see cref="Count"/> is positive.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public T[] ToArray()
    {
      T[] data = myData;
      int count = myCount;
      if (count == 0)
      {
        return theEmptyArray;
      }
      /* We will copy to every item in this array before it is made available to the user,
      /* therefore, it is fine to allocate it uninitialized. */
      T[] results = GC.AllocateUninitializedArray<T>(count, false);
      Array.ConstrainedCopy(data, 0, results, 0, count);
      return results;
    }

    /// <summary>
    /// Gets an array of length <see cref="Count"/> containing the items in the specified range of this list.
    /// This array is guaranteed to be newly allocated if <paramref name="length"/> is positive.
    /// </summary>
    /// <param name="start">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <see cref="Count"/> minus <paramref name="start"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public T[] ToArray(int start, int length)
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
      if (length == 0)
      {
        return theEmptyArray;
      }
      /* We will copy to every item in this array before it is made available to the user,
      /* therefore, it is fine to allocate it uninitialized. */
      T[] results = GC.AllocateUninitializedArray<T>(length, false);
      Array.ConstrainedCopy(data, start, results, 0, length);
      return results;
    }

    /// <summary>
    /// Creates a newly allocated list containing the items in the specified range of this list.
    /// </summary>
    /// <param name="start">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <see cref="Count"/> minus <paramref name="start"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public List2<T> GetRange(int start, int length)
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
      if (length == 0)
      {
        return new List2<T>(theEmptyArray, 0);
      }
      /* We will copy to every item in this array before it is made available to the user,
      /* therefore, it is fine to allocate it uninitialized. */
      T[] results = GC.AllocateUninitializedArray<T>(length, false);
      Array.ConstrainedCopy(data, start, results, 0, length);
      return new List2<T>(results, length);
    }

    #endregion ToArray, GetRange

    #region Clear, ICollection<T>.Clear, IList.Clear

    /// <summary>
    /// Clears the list.
    /// This method does not perform any allocation.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void Clear()
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      {
        Array.Clear(myData, 0, myCount);
      }
      myCount = 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection<T>.Clear()
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      {
        Array.Clear(myData, 0, myCount);
      }
      myCount = 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IList.Clear()
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      {
        Array.Clear(myData, 0, myCount);
      }
      myCount = 0;
    }

    #endregion Clear, ICollection<T>.Clear, IList.Clear

    [MethodImpl(Helper.OptimizeNoInline)]
    private void AddRareImpl(T item)
    {
      T[] data = myData;
      int count = myCount;
      int least = count + 1;
      if (least <= data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      if (least > MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      int suggested = (count > MaximumCapacity / 2
        ? MaximumCapacity
        : count <= StartingCapacity / 2
        ? StartingCapacity
        : count * 2);
      T[] newData = AllocImpl(least, suggested);
      Array.ConstrainedCopy(data, 0, newData, 0, count);
      newData[count] = item;
      /* No more exception is possible beyond this point. */
      myData = newData;
      myCount = least;
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private int IListAddRareImpl(T item)
    {
      T[] data = myData;
      int count = myCount;
      int least = count + 1;
      if (least <= data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = least;
        return count;
      }
      if (least > MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      int suggested = (count > MaximumCapacity / 2
        ? MaximumCapacity
        : count <= StartingCapacity / 2
        ? StartingCapacity
        : count * 2);
      T[] newData = AllocImpl(least, suggested);
      Array.ConstrainedCopy(data, 0, newData, 0, count);
      newData[count] = item;
      /* No more exception is possible beyond this point. */
      myData = newData;
      myCount = least;
      return count;
    }

    #region Add, ICollection<T>.Add, IList.Add

    /// <summary>
    /// Adds an item to the end of the list.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void Add(T item)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = ++count;
      }
      else
      {
        AddRareImpl(item);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection<T>.Add(T item)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = ++count;
      }
      else
      {
        AddRareImpl(item);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    int IList.Add(object value)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T item = (T)value;
      T[] data = myData;
      int count = myCount;
      if (count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = count + 1;
        return count;
      }
      else
      {
        return IListAddRareImpl(item);
      }
    }

    #endregion Add, ICollection<T>.Add, IList.Add

    /// <summary>
    /// This method validates its arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private void AddRangeImpl(T[] source, int start, int length, int sourceCount)
    {
      if ((uint)start > (uint)sourceCount)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(sourceCount - start))
      {
        List2.ThrowLength();
      }
      if (length == 0)
      {
        return;
      }
      T[] data = myData;
      int count = myCount;
      int least = count + length;
      /* This comparison must be unsigned because "least = count + length" might have overflown. */
      if ((uint)least > (uint)MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      if (least <= data.Length)
      {
        Array.ConstrainedCopy(source, start, data, count, length);
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      int suggested = (count > MaximumCapacity / 2
        ? MaximumCapacity
        : count <= StartingCapacity / 2
        ? StartingCapacity
        : count * 2);
      suggested = (suggested < least ? least : suggested);
      T[] newData = AllocImpl(least, suggested);
      Array.ConstrainedCopy(data, 0, newData, 0, count);
      Array.ConstrainedCopy(source, start, newData, count, length);
      /* No more exception is possible beyond this point. */
      myData = newData;
      myCount = least;
    }

    #region AddRange

    /// <summary>
    /// Adds an array of items to the end of this list.
    /// </summary>
    /// <exception cref="NullReferenceException">If <paramref name="array"/> is <see langword="null"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void AddRange(T[] array)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      int count = array.Length;
      AddRangeImpl(array, 0, count, count);
    }

    /// <summary>
    /// Adds the items in the specified range of the array to the end of this list.
    /// </summary>
    /// <param name="start">This value must be non-negative and not exceed the length of <paramref name="array"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed the length of <paramref name="array"/> minus <paramref name="start"/>.</param>
    /// <exception cref="NullReferenceException">If <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void AddRange(T[] array, int start, int length)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      AddRangeImpl(array, start, length, array.Length);
    }

    /// <summary>
    /// Adds a list of items to the end of this list.
    /// </summary>
    /// <param name="list">This argument can be the list being appended.</param>
    /// <exception cref="NullReferenceException">If <paramref name="list"/> is <see langword="null"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void AddRange(List2<T> list)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = list.myData;
      int count = list.myCount;
      AddRangeImpl(data, 0, count, count);
    }

    /// <summary>
    /// Adds the items in the specified range of the a list to the end of this list.
    /// </summary>
    /// <param name="list">This argument can be the list being appended.</param>
    /// <param name="start">This value must be non-negative and not exceed the <see cref="Count"/> of <paramref name="list"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed the <see cref="Count"/> of <paramref name="list"/> minus <paramref name="start"/>.</param>
    /// <exception cref="NullReferenceException">If <paramref name="list"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void AddRange(List2<T> list, int start, int length)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      AddRangeImpl(list.myData, start, length, list.myCount);
    }

    #endregion AddRange

    [MethodImpl(Helper.OptimizeNoInline)]
    private void InsertRareImpl(int index, T item)
    {
      T[] data = myData;
      int count = myCount;
      if ((uint)index > (uint)count)
      {
        List2.ThrowIndex();
      }
      int least = count + 1;
      if (least <= data.Length)
      {
        Array.ConstrainedCopy(data, index, data, index + 1, count - index);
        data[index] = item;
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      if (least > MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      int suggested = (count > MaximumCapacity / 2
        ? MaximumCapacity
        : count <= StartingCapacity / 2
        ? StartingCapacity
        : count * 2);
      T[] newData = AllocImpl(least, suggested);
      Array.ConstrainedCopy(data, 0, newData, 0, index);
      newData[index] = item;
      Array.ConstrainedCopy(data, index, newData, index + 1, count - index);
      /* No more exception is possible beyond this point. */
      myData = newData;
      myCount = least;
    }

    #region Insert, IList<T>.Insert, IList.Insert

    /// <summary>
    /// Inserts the specified item at the specified index.
    /// </summary>
    /// <param name="index">The new index of the inserted item.
    /// This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void Insert(int index, T item)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (index == count && count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = ++count;
      }
      else
      {
        InsertRareImpl(index, item);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IList<T>.Insert(int index, T item)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (index == count && count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = ++count;
      }
      else
      {
        InsertRareImpl(index, item);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IList.Insert(int index, object value)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T item = (T)value;
      T[] data = myData;
      int count = myCount;
      if (index == count && count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = ++count;
      }
      else
      {
        InsertRareImpl(index, item);
      }
    }

    #endregion Insert, IList<T>.Insert, IList.Insert

    /// <summary>
    /// This method validates its arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private void InsertRangeImpl(int index, T[] source, int start, int length, int sourceCount)
    {
      T[] data = myData;
      int count = myCount;
      if ((uint)index > (uint)count)
      {
        List2.ThrowIndex();
      }
      if ((uint)start > (uint)sourceCount)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(sourceCount - start))
      {
        List2.ThrowLength();
      }
      if (length == 0)
      {
        return;
      }
      int least = count + length;
      /* This comparison must be unsigned because "least = count + length" might have overflown. */
      if ((uint)least > (uint)MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      /* Case 1: Reallocate. It does matter if "source" is "data". */
      if (least > data.Length)
      {
        int suggested = (count > MaximumCapacity / 2
          ? MaximumCapacity
          : count <= StartingCapacity / 2
          ? StartingCapacity
          : count * 2);
        suggested = (suggested < least ? least : suggested);
        T[] newData = AllocImpl(least, suggested);
        Array.ConstrainedCopy(data, 0, newData, 0, index);
        Array.ConstrainedCopy(source, start, newData, index, length);
        Array.ConstrainedCopy(data, index, newData, index + length, count - index);
        /* No more exception is possible beyond this point. */
        myData = newData;
        myCount = least;
        return;
      }
      /* Case 2: "source" is not "data". */
      if (!ReferenceEquals(source, data))
      {
        Array.ConstrainedCopy(data, index, data, index + length, count - index);
        Array.ConstrainedCopy(source, start, data, index, length);
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      /* Below, 1234/pq/PQ/x represent other/source/inserted/uninitialized items.
      /* Case 3: 12pq34   => 12PQpq34    or    12pq34 => 12pqPQ34    (identical effect).
      /* Handle: 12pq34xx => 12pqPQ34
      /*           ^^^^          ^^^^ */
      if (index == start || index == start + length)
      {
        Array.ConstrainedCopy(data, start, data, start + length, count - start);
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      /* Case   4: 12pq34   => 1PQ2pq34
      /*   Handle: 12pq34xx => 12p2pq34    then    12p2pq34 => 1pq2pq34
      /*            ^^^^^         ^^^^^                ^^       ^^
      /* Case 5.1: 12pq34   => 12pq3PQ4
      /*   Handle: 12pq34xx => 12pq34x4    then    12pq34x4 => 12pq3PQ4
      /*                ^             ^              ^^             ^^
      /* Case 5.2: 12pq34   => 12pPQq34
      /*   Handle: 12pq34xx => 12pq3q34    then    12pq3q34 => 12pPQq34
      /*              ^^^           ^^^              ^^           ^^
      /* The first step of Case 4/5.1/5.2 is the same.
      /* The second step of Case 5.1/5.2 is the same. */
      Array.ConstrainedCopy(data, index, data, index + length, count - index);
      Array.ConstrainedCopy(data,
        index < start
        ? start + length /* Case 4 --- pq sits at the moved location */
        : start /* Case 5.1 and Case 5.2 --- pq sits at the original location */,
        data, index, length);
      /* No more exception is possible beyond this point. */
      myCount = least;
    }

    #region InsertRange

    /// <summary>
    /// Inserts an array at the specified index.
    /// </summary>
    /// <param name="index">This value must be non-negative and not exceed <see cref="Count"/>.
    /// This is the new index of the first newly inserted item.</param>
    /// <exception cref="NullReferenceException">If <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If <see cref="Count"/> will exceed <see cref="MaximumCapacity"/> after inserting the items.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void InsertRange(int index, T[] source)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      int length = source.Length;
      InsertRangeImpl(index, source, 0, length, length);
    }

    /// <summary>
    /// Inserts the specified range of an array at the specified index.
    /// </summary>
    /// <param name="index">This value must be non-negative and not exceed <see cref="Count"/>.
    /// This is the new index of the first newly inserted item.</param>
    /// <param name="start">This value must be non-negative and not exceed the length of <paramref name="source"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed the length of <paramref name="source"/> minus <paramref name="start"/>.</param>
    /// <exception cref="NullReferenceException">If <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/>, <paramref name="start"/>, or <paramref name="length"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If <see cref="Count"/> will exceed <see cref="MaximumCapacity"/> after inserting the items.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void InsertRange(int index, T[] source, int start, int length)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      InsertRangeImpl(index, source, start, length, source.Length);
    }

    /// <summary>
    /// Inserts a list at the specified index.
    /// </summary>
    /// <param name="index">This value must be non-negative and not exceed <see cref="Count"/>.
    /// This is the new index of the first newly inserted item.</param>
    /// <param name="source">It is allowed to insert a list into itself.</param>
    /// <exception cref="NullReferenceException">If <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If <see cref="Count"/> will exceed <see cref="MaximumCapacity"/> after inserting the items.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void InsertRange(int index, List2<T> source)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = source.myData;
      int count = source.myCount;
      InsertRangeImpl(index, data, 0, count, count);
    }

    /// <summary>
    /// Inserts the specified range of a list at the specified index.
    /// </summary>
    /// <param name="index">This value must be non-negative and not exceed <see cref="Count"/>.
    /// This is the new index of the first newly inserted item.</param>
    /// <param name="source">It is allowed to insert a list into itself.</param>
    /// <param name="start">This value must be non-negative and not exceed the length of <paramref name="source"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed the length of <paramref name="source"/> minus <paramref name="start"/>.</param>
    /// <exception cref="NullReferenceException">If <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/>, <paramref name="start"/>, or <paramref name="length"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If <see cref="Count"/> will exceed <see cref="MaximumCapacity"/> after inserting the items.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void InsertRange(int index, List2<T> source, int start, int length)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      InsertRangeImpl(index, source.myData, start, length, source.myCount);
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
    public const int StartingCapacity = 8;
    public const int MaximumCapacityOneByte = 0x7FFFFFC7;
    public const int MaximumCapacityOther = 0x7FEFFFFF;

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowStart()
    {
      throw new ArgumentOutOfRangeException("start");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowLength()
    {
      throw new ArgumentOutOfRangeException("length");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowCapacity()
    {
      throw new ArgumentOutOfRangeException("capacity");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowTooMany()
    {
      throw new InvalidOperationException("There will be more than MaximumCapacity number of items in the list.");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowIndex()
    {
      throw new ArgumentOutOfRangeException("index");
    }

#if LIST2_ENUMERATION_VERSION

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowVersion()
    {
      throw new InvalidOperationException("The list is modified during enumeration. (This check is enabled by LIST2_ENUMERATION_VERSION.)");
    }

#endif

#if LIST2_ENUMERATOR_DISPOSE

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowDisposed()
    {
      throw new ObjectDisposedException(typeof(List2<>.Enumerator).FullName,
        "The enumerator is already disposed. (This check is enabled by LIST2_ENUMERATOR_DISPOSE.)");
    }

#endif

  }
}
