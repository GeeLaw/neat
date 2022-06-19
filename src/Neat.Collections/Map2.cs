using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

    internal struct Entry
    {
      /// <summary>
      /// Points to the next entry in the bucket or the free entry list (negative if there are no more entries),
      /// if this entry is initialized (among the first <see cref="myTouchedCount"/> entries).
      /// Otherwise, this field is indeterminate.
      /// </summary>
      public int Next;

      /// <summary>
      /// Non-negative if initialized and in use, negative if initialized and free.
      /// Otherwise, this field is indeterminate.
      /// </summary>
      public int HashCode;

      /// <summary>
      /// The key if initialized and in use.
      /// Otherwise, this field is indeterminate.
      /// </summary>
      public TKey Key;

      /// <summary>
      /// The value if initialized and in use.
      /// Otherwise, this field is indeterminate.
      /// </summary>
      public TValue Value;

      public KeyValuePair<TKey, TValue> KeyValuePair
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
          return new KeyValuePair<TKey, TValue>(Key, Value);
        }
      }

      public DictionaryEntry DictionaryEntry
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
          return new DictionaryEntry(Key, Value);
        }
      }
    }

    /// <summary>
    /// <c>myBuckets[entry.HashCode % myBuckets.Length]</c> points to the first entry in this bucket
    /// (negative if there are no entries in this bucket).
    /// </summary>
    private protected int[] myBuckets;

    /// <summary>
    /// The first <see cref="myTouchedCount"/> entries are initialized, among which <see cref="myActiveCount"/> are in use (not free).
    /// The entries beyond the <see cref="myTouchedCount"/> ones are not initialized.
    /// </summary>
    private protected Entry[] myEntries;

    /// <summary>
    /// The number of key/value pairs in this map.
    /// </summary>
    private protected int myActiveCount;

    /// <summary>
    /// The number of initialized entries.
    /// </summary>
    private protected int myTouchedCount;

    /// <summary>
    /// Points to the first free entry
    /// (negative if there are no initialized free entries).
    /// </summary>
    private protected int myFirstFreeEntry;

    /// <summary>
    /// Index into <see cref="Map2.theSizes"/>.
    /// Negative if no storage has been allocated.
    /// </summary>
    private protected int mySizeIndex;

#if MAP2_ENUMERATION_VERSION

    /// <summary>
    /// The version number to invalidate <see cref="Enumerator"/>, <see cref="KeyEnumerator"/>, <see cref="ValueEnumerator"/>.
    /// </summary>
    private protected uint myVersion;

    /// <summary>
    /// The version number to detect rude equality comparers.
    /// </summary>
    private protected uint myVersion2;

#endif

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    private static readonly Entry[] theEmptyEntryList = new Entry[0];

    [MethodImpl(Helper.OptimizeInline)]
    private protected Map2()
    {
      myBuckets = Map2.theEmptyBuckets;
      myEntries = theEmptyEntryList;
      myActiveCount = 0;
      myTouchedCount = 0;
      myFirstFreeEntry = 0;
      mySizeIndex = -1;
#if MAP2_ENUMERATION_VERSION
      myVersion = 0;
      myVersion2 = 0;
#endif
    }

    /// <summary>
    /// Gets the number of key/value pairs.
    /// </summary>
    public int Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myActiveCount;
      }
    }

    /// <summary>
    /// Clears the key/value pairs.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void Clear()
    {
#if MAP2_ENUMERATION_VERSION
      ++myVersion;
#endif
      Map2.ResetBuckets(myBuckets);
      if (RuntimeHelpers.IsReferenceOrContainsReferences<Entry>())
      {
        Array.Clear(myEntries, 0, myTouchedCount);
      }
      myActiveCount = 0;
      myTouchedCount = 0;
      myFirstFreeEntry = -1;
    }

    /// <summary>
    /// Reduces the memory usage opportunistically.
    /// </summary>
    /// <returns><see langword="true"/> if reallocation happened.</returns>
    [MethodImpl(Helper.JustOptimize)]
    public bool TrimExcess()
    {
#if MAP2_ENUMERATION_VERSION
      ++myVersion;
#endif
      int activeCount = myActiveCount;
      if (activeCount == 0)
      {
        myBuckets = Map2.theEmptyBuckets;
        myEntries = theEmptyEntryList;
        myTouchedCount = 0;
        myFirstFreeEntry = -1;
        mySizeIndex = -1;
        return true;
      }
      int oldSizeIndex = mySizeIndex;
      int newSizeIndex = oldSizeIndex;
      Map2.Size[] sizes = Map2.theSizes;
      while (newSizeIndex >= 0 && sizes[newSizeIndex].EntryCount >= activeCount)
      {
        --newSizeIndex;
      }
      ++newSizeIndex;
      if (newSizeIndex == oldSizeIndex)
      {
        return false;
      }
      return TrimExcessImpl(myEntries, activeCount, newSizeIndex);
    }

    /// <summary>
    /// Race conditions could have corrupted this instance, and
    /// we want unsafe code in this method to be safe as far as CLR is concerned.
    /// However, unsafe access after user-performed bounds checks
    /// might still break CLR type safety due to read introduction
    /// (e.g., when an index is re-read after it has been checked).
    /// This method is non-inlining to reduce such risk.
    /// See <a href="https://github.com/dotnet/docs/issues/29696">dotnet/docs#29696</a>.
    /// </summary>
    [MethodImpl(Helper.OptimizeNoInline)]
    private bool TrimExcessImpl(Entry[] oldEntries, int activeCount, int sizeIndex)
    {
      Map2.Size size = Map2.theSizes[sizeIndex];
      /* This check ensures that unsafe access to "entries" is safe. */
      if ((uint)size.EntryCount < (uint)activeCount)
      {
        return false;
      }
      int[] buckets = GC.AllocateUninitializedArray<int>(size.BucketCount, false);
      Entry[] entries = new Entry[size.EntryCount];
      Map2.ResetBuckets(buckets);
      ref Entry entry0 = ref MemoryMarshal.GetArrayDataReference(entries);
      for (int i = 0, j = 0, k; i != activeCount; ++i, ++j)
      {
        /* Access to "oldEntries" and "buckets" should be safe,
        /* due to potential corruption of the entries.
        /* So "j" could be out of range, and "k" could be negative. */
        while (oldEntries[j].HashCode < 0)
        {
          ++j;
        }
        Unsafe.Add(ref entry0, i) = oldEntries[j];
        k = Unsafe.Add(ref entry0, i).HashCode % size.BucketCount;
        Unsafe.Add(ref entry0, i).Next = buckets[k];
        buckets[k] = i;
      }
      /* No more exception is possible beyond this point. */
      myBuckets = buckets;
      myEntries = entries;
      /* myActiveCount = activeCount; */
      myTouchedCount = activeCount;
      myFirstFreeEntry = -1;
      mySizeIndex = sizeIndex;
      return true;
    }

    /// <summary>
    /// Improves enumeration efficiency.
    /// </summary>
    /// <param name="force"><see langword="true"/> if defragmentation is performed as long as there is any fragmentation.</param>
    /// <returns><see langword="true"/> if defragmentation happened.
    /// The return value can be <see langword="false"/> even if <paramref name="force"/> is <see langword="true"/>,
    /// when there is no fragmentation at all.</returns>
    [MethodImpl(Helper.JustOptimize)]
    public bool Defragment(bool force)
    {
#if MAP2_ENUMERATION_VERSION
      ++myVersion;
#endif
      int activeCount = myActiveCount, touchedCount = myTouchedCount;
      if (myFirstFreeEntry < 0 || (!force && activeCount >= (int)(touchedCount * 0.9)))
      {
        return false;
      }
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      Map2.ResetBuckets(buckets);
      /* Rewire the entries up to the first inactive one. */
      int i = 0, k, bucketsLength = buckets.Length;
      for (; entries[i].HashCode >= 0; ++i)
      {
        k = entries[i].HashCode % bucketsLength;
        entries[i].Next = buckets[k];
        buckets[k] = i;
      }
      /* Move active entries to the front and rewire them. */
      for (int j = i + 1; i != activeCount; ++i, ++j)
      {
        while (entries[j].HashCode < 0)
        {
          ++j;
        }
        entries[i] = entries[j];
        k = entries[i].HashCode % bucketsLength;
        entries[i].Next = buckets[k];
        buckets[k] = i;
      }
      if (RuntimeHelpers.IsReferenceOrContainsReferences<Entry>())
      {
        Array.Clear(entries, activeCount, touchedCount - activeCount);
      }
      myTouchedCount = activeCount;
      myFirstFreeEntry = -1;
      return true;
    }

    /// <summary>
    /// Copies the keys to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="arrayIndex"/> is negative,
    /// or greater than the length of <paramref name="array"/> minus <see cref="Count"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void CopyKeysTo(TKey[] array, int arrayIndex)
    {
      if (array is null)
      {
        Map2.ThrowCopyToArrayNull();
      }
      CopyKeysToImpl(myEntries, array, arrayIndex, myActiveCount, nameof(arrayIndex));
    }

    /// <summary>
    /// This method is non-inlining to reduce the risk of read introduction.
    /// It also assumes that <paramref name="array"/> is not <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeNoInline)]
    private protected static void CopyKeysToImpl(Entry[] entries, TKey[] array, int arrayIndex, int activeCount, string arrayIndexOrIndex)
    {
      if ((uint)arrayIndex >= (uint)array.Length || (uint)(array.Length - arrayIndex) < (uint)activeCount)
      {
        throw new ArgumentOutOfRangeException(arrayIndexOrIndex);
      }
      ref TKey dest0 = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), arrayIndex);
      /* At this point, "activeCount" must be non-negative due to the second condition of "if". */
      for (int i = 0, j = 0; i != activeCount; ++i, ++j)
      {
        while (entries[j].HashCode < 0)
        {
          ++j;
        }
        Unsafe.Add(ref dest0, i) = entries[j].Key;
      }
    }

    /// <summary>
    /// Copies the values to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="arrayIndex"/> is negative,
    /// or greater than the length of <paramref name="array"/> minus <see cref="Count"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void CopyValuesTo(TValue[] array, int arrayIndex)
    {
      if (array is null)
      {
        Map2.ThrowCopyToArrayNull();
      }
      CopyValuesToImpl(myEntries, array, arrayIndex, myActiveCount, nameof(arrayIndex));
    }

    /// <summary>
    /// This method is non-inlining to reduce the risk of read introduction.
    /// It also assumes that <paramref name="array"/> is not <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeNoInline)]
    private protected static void CopyValuesToImpl(Entry[] entries, TValue[] array, int arrayIndex, int activeCount, string arrayIndexOrIndex)
    {
      if ((uint)arrayIndex >= (uint)array.Length || (uint)(array.Length - arrayIndex) < (uint)activeCount)
      {
        throw new ArgumentOutOfRangeException(arrayIndexOrIndex);
      }
      ref TValue dest0 = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), arrayIndex);
      /* At this point, "activeCount" must be non-negative due to the second condition of "if". */
      for (int i = 0, j = 0; i != activeCount; ++i, ++j)
      {
        while (entries[j].HashCode < 0)
        {
          ++j;
        }
        Unsafe.Add(ref dest0, i) = entries[j].Value;
      }
    }

    /// <summary>
    /// Copies the key/value pairs to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="arrayIndex"/> is negative,
    /// or greater than the length of <paramref name="array"/> minus <see cref="Count"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      if (array is null)
      {
        Map2.ThrowCopyToArrayNull();
      }
      CopyPairsToImpl(myEntries, array, arrayIndex, myActiveCount, nameof(arrayIndex));
    }

    /// <summary>
    /// This method is non-inlining to reduce the risk of read introduction.
    /// It also assumes that <paramref name="array"/> is not <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeNoInline)]
    private protected static void CopyPairsToImpl(Entry[] entries, KeyValuePair<TKey, TValue>[] array, int arrayIndex, int activeCount, string arrayIndexOrIndex)
    {
      if ((uint)arrayIndex >= (uint)array.Length || (uint)(array.Length - arrayIndex) < (uint)activeCount)
      {
        throw new ArgumentOutOfRangeException(arrayIndexOrIndex);
      }
      ref KeyValuePair<TKey, TValue> dest0 = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), arrayIndex);
      /* At this point, "activeCount" must be non-negative due to the second condition of "if". */
      for (int i = 0, j = 0; i != activeCount; ++i, ++j)
      {
        while (entries[j].HashCode < 0)
        {
          ++j;
        }
        Unsafe.Add(ref dest0, i) = entries[j].KeyValuePair;
      }
    }

    /// <summary>
    /// This method is non-inlining to reduce the risk of read introduction.
    /// It also assumes that <paramref name="array"/> is not <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeNoInline)]
    private protected static void CopyEntriesToImpl(Entry[] entries, DictionaryEntry[] array, int arrayIndex, int activeCount, string arrayIndexOrIndex)
    {
      if ((uint)arrayIndex >= (uint)array.Length || (uint)(array.Length - arrayIndex) < (uint)activeCount)
      {
        throw new ArgumentOutOfRangeException(arrayIndexOrIndex);
      }
      ref DictionaryEntry dest0 = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), arrayIndex);
      /* At this point, "activeCount" must be non-negative due to the second condition of "if". */
      for (int i = 0, j = 0; i != activeCount; ++i, ++j)
      {
        while (entries[j].HashCode < 0)
        {
          ++j;
        }
        Unsafe.Add(ref dest0, i) = entries[j].DictionaryEntry;
      }
    }

    #region public members that call virtual methods and are hidden in the derived classes

    /// <summary>
    /// Gets or sets the value corresponding to the specified key.
    /// </summary>
    /// <exception cref="KeyNotFoundException">If the key does not exist when getting the value.</exception>
    public TValue this[TKey key]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        TValue value;
        Unsafe.SkipInit(out value);
        if (!TryGetOverride(key, ref value))
        {
          Map2.ThrowKeyNotFound();
        }
        return value;
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
        AddOrReplaceOverride(key, value);
      }
    }

    /// <returns><see langword="true"/> if the key exists.</returns>
    [MethodImpl(Helper.OptimizeInline)]
    public bool ContainsKey(TKey key)
    {
      return ContainsKeyOverride(key);
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is written to once, with the value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is not written to.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    [MethodImpl(Helper.OptimizeInline)]
    public bool TryGet(TKey key, ref TValue value)
    {
      return TryGetOverride(key, ref value);
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is written to once, with the value being the value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is written to once, with the value <see langword="default"/>.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    [MethodImpl(Helper.OptimizeInline)]
    public bool GetOrDefault(TKey key, out TValue value)
    {
      return GetOrDefaultOverride(key, out value);
    }

    /// <summary>
    /// Tries to swap the value corresponding to <paramref name="key"/> with <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is read once, whose value becomes the new value corresponding to <paramref name="key"/>,
    /// and <paramref name="value"/> is written to once afterwards, with the old value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is neither read nor written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    [MethodImpl(Helper.OptimizeInline)]
    public bool TrySwap(TKey key, ref TValue value)
    {
      return TrySwapOverride(key, ref value);
    }

    /// <returns><see langword="true"/> if the key existed.</returns>
    [MethodImpl(Helper.OptimizeInline)]
    public bool Remove(TKey key)
    {
      return RemoveOverride(key);
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/> and remove <paramref name="key"/> from the map.
    /// If the key existed, <paramref name="value"/> is written to once, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is not written to.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key existed.</returns>
    [MethodImpl(Helper.OptimizeInline)]
    public bool TryGetAndRemove(TKey key, ref TValue value)
    {
      return TryGetAndRemoveOverride(key, ref value);
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/> and remove <paramref name="key"/> from the map.
    /// If the key existed, <paramref name="value"/> is written to once, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is written to once, with the value <see langword="default"/>.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key existed.</returns>
    [MethodImpl(Helper.OptimizeInline)]
    public bool GetAndRemoveOrDefault(TKey key, out TValue value)
    {
      return GetAndRemoveOrDefaultOverride(key, out value);
    }

    /// <summary>
    /// Tries to add <paramref name="key"/> with <paramref name="value"/>.
    /// If <paramref name="key"/> already exists, its existing value is not updated.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public bool TryAddNew(TKey key, TValue value)
    {
      return TryAddNewOverride(key, value);
    }

    /// <summary>
    /// Adds or replaces the value correponding to <paramref name="key"/> as/by <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public bool AddOrReplace(TKey key, TValue value)
    {
      return AddOrReplaceOverride(key, value);
    }

    /// <summary>
    /// If the key existed, <paramref name="value"/> is not read, but written to once, with the existing value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is read once, whose value becomes the value corresponding to <paramref name="key"/>, and is never written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public bool AddOrGet(TKey key, ref TValue value)
    {
      return AddOrGetOverride(key, ref value);
    }

    /// <summary>
    /// If the key existed, <paramref name="value"/> is read once, whose value becomes the new value corresponding to <paramref name="key"/>,
    /// and <paramref name="value"/> is written to once afterwards, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is read once, whose value becomes the value corresponding to <paramref name="key"/>, and is never written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public bool AddOrSwap(TKey key, ref TValue value)
    {
      return AddOrSwapOverride(key, ref value);
    }

    /// <summary>
    /// Gets a view of the keys.
    /// Obtaining such a view is thread-safe.
    /// </summary>
    public ICollection<TKey> Keys
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return KeysOverride();
      }
    }

    #endregion public members that call virtual methods and are hidden in the derived classes

    /// <summary>
    /// Gets a view of the values.
    /// Obtaining such a view is thread-safe.
    /// </summary>
    public ValueView Values
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return new ValueView(this);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public KeyEnumerator GetKeyEnumerator()
    {
      return new KeyEnumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public ValueEnumerator GetValueEnumerator()
    {
      return new ValueEnumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public Enumerator GetEnumerator()
    {
      return new Enumerator(this);
    }

    /// <summary>
    /// The entry at the returned index should be treated as uninitialized.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    private protected int AllocEntry(out int[] outBuckets, out Entry[] outEntries)
    {
      Entry[] entries = myEntries;
      int firstFreeEntry = myFirstFreeEntry;
      if (firstFreeEntry >= 0)
      {
        outBuckets = myBuckets;
        outEntries = entries;
        myFirstFreeEntry = entries[firstFreeEntry].Next;
        return firstFreeEntry;
      }
      if ((firstFreeEntry = myTouchedCount) < entries.Length)
      {
        outBuckets = myBuckets;
        outEntries = entries;
        myTouchedCount = firstFreeEntry + 1;
        return firstFreeEntry;
      }
      return AllocEntryRareImpl(out outBuckets, out outEntries, entries, mySizeIndex + 1);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private int AllocEntryRareImpl(out int[] outBuckets, out Entry[] outEntries, Entry[] oldEntries, int newSizeIndex)
    {
      Map2.Size[] sizes = Map2.theSizes;
      if (newSizeIndex >= sizes.Length)
      {
        Map2.ThrowTooMany();
      }
      Map2.Size sz = sizes[newSizeIndex];
      int[] buckets = GC.AllocateUninitializedArray<int>(sz.BucketCount, false);
      Entry[] entries = new Entry[sz.EntryCount];
      Map2.ResetBuckets(buckets);
      Array.ConstrainedCopy(oldEntries, 0, entries, 0, oldEntries.Length);
      ref Entry entry0 = ref MemoryMarshal.GetArrayDataReference(entries);
      for (int i = 0, j, oldEntriesLength = oldEntries.Length; i != oldEntriesLength; ++i)
      {
        /* We should not use unsafe access to "buckets",
        /* because the entries could have been corrupted by race conditions
        /* and "j" could be negative. */
        j = Unsafe.Add(ref entry0, i).HashCode % sz.BucketCount;
        Unsafe.Add(ref entry0, i).Next = buckets[j];
        buckets[j] = i;
      }
      outBuckets = buckets;
      outEntries = entries;
      /* No more exception is possible beyond this point. */
      myBuckets = buckets;
      myEntries = entries;
      mySizeIndex = newSizeIndex;
      /* Now, newSizeIndex is the newly allocated entry. */
      newSizeIndex = oldEntries.Length;
      myTouchedCount = newSizeIndex + 1;
      return newSizeIndex;
    }

    #region virtual methods

    private protected abstract bool ContainsKeyOverride(TKey key);
    private protected abstract bool TryGetOverride(TKey key, ref TValue value);
    private protected abstract bool GetOrDefaultOverride(TKey key, out TValue value);
    private protected abstract bool TrySwapOverride(TKey key, ref TValue value);
    private protected abstract bool RemoveOverride(TKey key);
    private protected abstract bool TryGetAndRemoveOverride(TKey key, ref TValue value);
    private protected abstract bool GetAndRemoveOrDefaultOverride(TKey key, out TValue value);
    private protected abstract bool TryAddNewOverride(TKey key, TValue value);
    private protected abstract bool AddOrReplaceOverride(TKey key, TValue value);
    private protected abstract bool AddOrGetOverride(TKey key, ref TValue value);
    private protected abstract bool AddOrSwapOverride(TKey key, ref TValue value);
    private protected abstract ICollection<TKey> KeysOverride();

    #endregion virtual methods

    #region IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count, ICollection<KeyValuePair<TKey, TValue>>.Count, ICollection.Count

    int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    int ICollection<KeyValuePair<TKey, TValue>>.Count
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    int ICollection.Count
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    #endregion IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count, ICollection<KeyValuePair<TKey, TValue>>.Count, ICollection.Count

    #region ICollection<KeyValuePair<TKey, TValue>>.Clear, IDictionary.Clear

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
      throw new NotReimplementedException();
    }

    void IDictionary.Clear()
    {
      throw new NotReimplementedException();
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.Clear, IDictionary.Clear

    #region IReadOnlyDictionary<TKey, TValue>.this[TKey key], IDictionary<TKey, TValue>.this[TKey key], IDictionary.this[object key]

    TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
      get
      {
        throw new NotReimplementedException();
      }
      set
      {
        throw new NotReimplementedException();
      }
    }

    object IDictionary.this[object key]
    {
      get
      {
        throw new NotReimplementedException();
      }
      set
      {
        throw new NotReimplementedException();
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.this[TKey key], IDictionary<TKey, TValue>.this[TKey key], IDictionary.this[object key]

    #region IDictionary<TKey, TValue>.Add, ICollection<KeyValuePair<TKey, TValue>>.Add, IDictionary.Add

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
      throw new NotReimplementedException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      throw new NotReimplementedException();
    }

    void IDictionary.Add(object key, object value)
    {
      throw new NotReimplementedException();
    }

    #endregion IDictionary<TKey, TValue>.Add, ICollection<KeyValuePair<TKey, TValue>>.Add, IDictionary.Add

    #region IReadOnlyDictionary<TKey, TValue>.TryGetValue, IDictionary<TKey, TValue>.TryGetValue

    bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
    {
      throw new NotReimplementedException();
    }

    bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
    {
      throw new NotReimplementedException();
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.TryGetValue, IDictionary<TKey, TValue>.TryGetValue

    #region IReadOnlyDictionary<TKey, TValue>.ContainsKey, IDictionary<TKey, TValue>.ContainsKey, ICollection<KeyValuePair<TKey, TValue>>.Contains, IDictionary.Contains

    bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
    {
      throw new NotReimplementedException();
    }

    bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
    {
      throw new NotReimplementedException();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
      throw new NotReimplementedException();
    }

    bool IDictionary.Contains(object key)
    {
      throw new NotReimplementedException();
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.ContainsKey, IDictionary<TKey, TValue>.ContainsKey, ICollection<KeyValuePair<TKey, TValue>>.Contains, IDictionary.Contains

    #region IDictionary<TKey, TValue>.Remove, ICollection<KeyValuePair<TKey, TValue>>.Remove, IDictionary.Remove

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
      throw new NotReimplementedException();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
      throw new NotReimplementedException();
    }

    void IDictionary.Remove(object key)
    {
      throw new NotReimplementedException();
    }

    #endregion IDictionary<TKey, TValue>.Remove, ICollection<KeyValuePair<TKey, TValue>>.Remove, IDictionary.Remove

    #region ICollection<KeyValuePair<TKey, TValue>>.CopyTo, ICollection.CopyTo

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      throw new NotReimplementedException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      throw new NotReimplementedException();
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.CopyTo, ICollection.CopyTo

    #region IReadOnlyDictionary<TKey, TValue>.Keys, IDictionary<TKey, TValue>.Keys, IDictionary.Keys

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    ICollection IDictionary.Keys
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.Keys, IDictionary<TKey, TValue>.Keys, IDictionary.Keys

    #region IReadOnlyDictionary<TKey, TValue>.Values, IDictionary<TKey, TValue>.Values, IDictionary.Values

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    ICollection IDictionary.Values
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.Values, IDictionary<TKey, TValue>.Values, IDictionary.Values

    #region ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly, IDictionary.IsReadOnly, IDictionary.IsFixedSize, ICollection.IsSynchronized, ICollection.SyncRoot

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool IDictionary.IsReadOnly
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool IDictionary.IsFixedSize
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool ICollection.IsSynchronized
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// This member is not supported.
    /// </summary>
    object ICollection.SyncRoot
    {
      get
      {
        throw new NotReimplementedException();
      }
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly, IDictionary.IsReadOnly, IDictionary.IsFixedSize, ICollection.IsSynchronized, ICollection.SyncRoot

    #region GetEnumerator (explicit implementations)

    Enumerator IEnumerable2<KeyValuePair<TKey, TValue>, Enumerator>.GetEnumerator()
    {
      throw new NotReimplementedException();
    }

    IEnumerator2<KeyValuePair<TKey, TValue>> IEnumerable2<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      throw new NotReimplementedException();
    }

    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      throw new NotReimplementedException();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      throw new NotReimplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotReimplementedException();
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      throw new NotReimplementedException();
    }

    #endregion GetEnumerator (explicit implementations)

    /// <summary>
    /// Enumerates key/value pairs in <see cref="Map2{TKey, TValue}"/>.
    /// </summary>
    public struct Enumerator : IEnumerator2<KeyValuePair<TKey, TValue>>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Entry[] myEntries;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private int myTouchedCount;

      private int myIndexAfterCurrent;

      private KeyValuePair<TKey, TValue> myCurrent;

#if MAP2_ENUMERATION_VERSION

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Map2<TKey, TValue> myTarget;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private uint myVersion;

#endif

#if MAP2_ENUMERATOR_DISPOSE
      private bool myNotDisposed;
#endif

      [MethodImpl(Helper.OptimizeInline)]
      internal Enumerator(Map2<TKey, TValue> target)
      {
        myEntries = target.myEntries;
        myTouchedCount = target.myTouchedCount;
        myIndexAfterCurrent = 0;
        myCurrent = default(KeyValuePair<TKey, TValue>);
#if MAP2_ENUMERATION_VERSION
        myTarget = target;
        myVersion = target.myVersion;
#endif
#if MAP2_ENUMERATOR_DISPOSE
        myNotDisposed = true;
#endif
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        myIndexAfterCurrent = 0;
      }

      [MethodImpl(Helper.JustOptimize)]
      public bool MoveNext(out KeyValuePair<TKey, TValue> item)
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
#if MAP2_ENUMERATOR_DISPOSE
          myNotDisposed = false;
#endif
          item = default(KeyValuePair<TKey, TValue>);
          return false;
        }
        myIndexAfterCurrent = index + 1;
        item = myCurrent = entries[index].KeyValuePair;
        return true;
      }

      [MethodImpl(Helper.JustOptimize)]
      bool IEnumerator2.MoveNext(out object item)
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
#if MAP2_ENUMERATOR_DISPOSE
          myNotDisposed = false;
#endif
          item = null;
          return false;
        }
        myIndexAfterCurrent = index + 1;
        item = myCurrent = entries[index].KeyValuePair;
        return true;
      }

      [MethodImpl(Helper.JustOptimize)]
      public bool MoveNext()
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
          return false;
        }
        myIndexAfterCurrent = index + 1;
        myCurrent = entries[index].KeyValuePair;
        return true;
      }

      public KeyValuePair<TKey, TValue> Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            Map2.ThrowEnumeratorDisposed();
          }
#endif
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent;
        }
      }

      object IEnumerator.Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            Map2.ThrowEnumeratorDisposed();
          }
#endif
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent;
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IDisposable.Dispose()
      {
#if MAP2_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
      }
    }

    /// <summary>
    /// Enumerates keys in <see cref="Map2{TKey, TValue}"/>.
    /// </summary>
    public struct KeyEnumerator : IEnumerator2<TKey>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Entry[] myEntries;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private int myTouchedCount;

      private int myIndexAfterCurrent;

      private TKey myCurrent;

#if MAP2_ENUMERATION_VERSION

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Map2<TKey, TValue> myTarget;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private uint myVersion;

#endif

#if MAP2_ENUMERATOR_DISPOSE
      private bool myNotDisposed;
#endif

      [MethodImpl(Helper.OptimizeInline)]
      internal KeyEnumerator(Map2<TKey, TValue> target)
      {
        myEntries = target.myEntries;
        myTouchedCount = target.myTouchedCount;
        myIndexAfterCurrent = 0;
        myCurrent = default(TKey);
#if MAP2_ENUMERATION_VERSION
        myTarget = target;
        myVersion = target.myVersion;
#endif
#if MAP2_ENUMERATOR_DISPOSE
        myNotDisposed = true;
#endif
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowKeyEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        myIndexAfterCurrent = 0;
      }

      [MethodImpl(Helper.JustOptimize)]
      public bool MoveNext(out TKey item)
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowKeyEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
#if MAP2_ENUMERATOR_DISPOSE
          myNotDisposed = false;
#endif
          item = default(TKey);
          return false;
        }
        myIndexAfterCurrent = index + 1;
        item = myCurrent = entries[index].Key;
        return true;
      }

      [MethodImpl(Helper.JustOptimize)]
      bool IEnumerator2.MoveNext(out object item)
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowKeyEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
#if MAP2_ENUMERATOR_DISPOSE
          myNotDisposed = false;
#endif
          item = null;
          return false;
        }
        myIndexAfterCurrent = index + 1;
        item = myCurrent = entries[index].Key;
        return true;
      }

      [MethodImpl(Helper.JustOptimize)]
      public bool MoveNext()
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowKeyEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
          return false;
        }
        myIndexAfterCurrent = index + 1;
        myCurrent = entries[index].Key;
        return true;
      }

      public TKey Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            Map2.ThrowKeyEnumeratorDisposed();
          }
#endif
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent;
        }
      }

      object IEnumerator.Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            Map2.ThrowKeyEnumeratorDisposed();
          }
#endif
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent;
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IDisposable.Dispose()
      {
#if MAP2_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
      }
    }

    /// <summary>
    /// Enumerates values in <see cref="Map2{TKey, TValue}"/>.
    /// </summary>
    public struct ValueEnumerator : IEnumerator2<TValue>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Entry[] myEntries;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private int myTouchedCount;

      private int myIndexAfterCurrent;

      private TValue myCurrent;

#if MAP2_ENUMERATION_VERSION

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Map2<TKey, TValue> myTarget;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private uint myVersion;

#endif

#if MAP2_ENUMERATOR_DISPOSE
      private bool myNotDisposed;
#endif

      [MethodImpl(Helper.OptimizeInline)]
      internal ValueEnumerator(Map2<TKey, TValue> target)
      {
        myEntries = target.myEntries;
        myTouchedCount = target.myTouchedCount;
        myIndexAfterCurrent = 0;
        myCurrent = default(TValue);
#if MAP2_ENUMERATION_VERSION
        myTarget = target;
        myVersion = target.myVersion;
#endif
#if MAP2_ENUMERATOR_DISPOSE
        myNotDisposed = true;
#endif
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowValueEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        myIndexAfterCurrent = 0;
      }

      [MethodImpl(Helper.JustOptimize)]
      public bool MoveNext(out TValue item)
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowValueEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
#if MAP2_ENUMERATOR_DISPOSE
          myNotDisposed = false;
#endif
          item = default(TValue);
          return false;
        }
        myIndexAfterCurrent = index + 1;
        item = myCurrent = entries[index].Value;
        return true;
      }

      [MethodImpl(Helper.JustOptimize)]
      bool IEnumerator2.MoveNext(out object item)
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowValueEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
#if MAP2_ENUMERATOR_DISPOSE
          myNotDisposed = false;
#endif
          item = null;
          return false;
        }
        myIndexAfterCurrent = index + 1;
        item = myCurrent = entries[index].Value;
        return true;
      }

      [MethodImpl(Helper.JustOptimize)]
      public bool MoveNext()
      {
#if MAP2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          Map2.ThrowValueEnumeratorDisposed();
        }
#endif
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
          return false;
        }
        myIndexAfterCurrent = index + 1;
        myCurrent = entries[index].Value;
        return true;
      }

      public TValue Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            Map2.ThrowValueEnumeratorDisposed();
          }
#endif
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent;
        }
      }

      object IEnumerator.Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            Map2.ThrowValueEnumeratorDisposed();
          }
#endif
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent;
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IDisposable.Dispose()
      {
#if MAP2_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
      }
    }

    private protected sealed class DictionaryEnumerator : IDictionaryEnumerator
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Entry[] myEntries;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private int myTouchedCount;

      private int myIndexAfterCurrent;

      private DictionaryEntry myCurrent;

#if MAP2_ENUMERATION_VERSION

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Map2<TKey, TValue> myTarget;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private uint myVersion;

#endif

      [MethodImpl(Helper.OptimizeInline)]
      internal DictionaryEnumerator(Map2<TKey, TValue> target)
      {
        myEntries = target.myEntries;
        myTouchedCount = target.myTouchedCount;
        myIndexAfterCurrent = 0;
        myCurrent = default(DictionaryEntry);
#if MAP2_ENUMERATION_VERSION
        myTarget = target;
        myVersion = target.myVersion;
#endif
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        myIndexAfterCurrent = 0;
      }

      [MethodImpl(Helper.JustOptimize)]
      public bool MoveNext()
      {
#if MAP2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          Map2.ThrowVersion();
        }
#endif
        Entry[] entries = myEntries;
        int touchedCount = myTouchedCount;
        int index = myIndexAfterCurrent;
        while (index != touchedCount && entries[index].HashCode < 0)
        {
          ++index;
        }
        if (index == touchedCount)
        {
          return false;
        }
        myIndexAfterCurrent = index + 1;
        myCurrent = entries[index].DictionaryEntry;
        return true;
      }

      public DictionaryEntry Entry
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent;
        }
      }

      public object Key
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent.Key;
        }
      }

      public object Value
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent.Value;
        }
      }

      public object Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if MAP2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            Map2.ThrowVersion();
          }
#endif
          return myCurrent;
        }
      }
    }

    /// <summary>
    /// Represents a view of the values in a <see cref="Map2{TKey, TValue}"/> instance.
    /// Among instance members,
    /// only those of <see cref="IEquatable{T}"/> and <see cref="object"/> can be invoked on <see langword="default"/> instances.
    /// </summary>
    public readonly struct ValueView : IEquatable<ValueView>, IEnumerable2<TValue, ValueEnumerator>, ICollection<TValue>, IReadOnlyCollection<TValue>, ICollection
    {
      private readonly Map2<TKey, TValue> myTarget;

      [MethodImpl(Helper.OptimizeInline)]
      internal ValueView(Map2<TKey, TValue> target)
      {
        myTarget = target;
      }

      public int Count
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
          return myTarget.myActiveCount;
        }
      }

      public bool Contains(TValue item)
      {
        throw new NotImplementedException();
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void CopyTo(TValue[] array, int arrayIndex)
      {
        myTarget.CopyValuesTo(array, arrayIndex);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public ValueEnumerator GetEnumerator()
      {
        return new ValueEnumerator(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Equals(ValueView other)
      {
        return ReferenceEquals(myTarget, other.myTarget);
      }

      #region static Equals, operator ==, operator !=

      [MethodImpl(Helper.OptimizeInline)]
      public static bool Equals(ValueView x, ValueView y)
      {
        return ReferenceEquals(x.myTarget, y.myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public static bool operator ==(ValueView x, ValueView y)
      {
        return ReferenceEquals(x.myTarget, y.myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public static bool operator !=(ValueView x, ValueView y)
      {
        return !ReferenceEquals(x.myTarget, y.myTarget);
      }

      #endregion static Equals, operator ==, operator !=

      #region object members

      [MethodImpl(Helper.OptimizeInline)]
      public override bool Equals(object obj)
      {
        return (obj is ValueView other) && ReferenceEquals(myTarget, other.myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public override int GetHashCode()
      {
        return RuntimeHelpers.GetHashCode(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public override string ToString()
      {
        return typeof(ValueView).FullName;
      }

      #endregion object members

      #region ICollection.CopyTo

      [MethodImpl(Helper.OptimizeInline)]
      void ICollection.CopyTo(Array array, int index)
      {
        /* First, (implicitly) check whether "myTarget" is null. */
        Map2<TKey, TValue> target = myTarget;
        int activeCount = target.myActiveCount;
        if (array is TValue[] vArray)
        {
          CopyValuesToImpl(target.myEntries, vArray, index, activeCount, nameof(index));
        }
        else
        {
          Map2.ThrowCopyValuesToArrayType();
        }
      }

      #endregion ICollection.CopyTo

      #region ICollection<TValue>.Add, ICollection<TValue>.Clear, ICollection<TValue>.Remove

      [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
      [MethodImpl(Helper.OptimizeNoInline)]
      void ICollection<TValue>.Add(TValue item)
      {
        int unused = myTarget.myActiveCount;
        throw new NotSupportedException();
      }

      [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
      [MethodImpl(Helper.OptimizeNoInline)]
      void ICollection<TValue>.Clear()
      {
        int unused = myTarget.myActiveCount;
        throw new NotSupportedException();
      }

      [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
      [MethodImpl(Helper.OptimizeNoInline)]
      bool ICollection<TValue>.Remove(TValue item)
      {
        int unused = myTarget.myActiveCount;
        throw new NotSupportedException();
      }

      #endregion ICollection<TValue>.Add, ICollection<TValue>.Clear, ICollection<TValue>.Remove

      #region ICollection<TValue>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      /// <summary>
      /// This member is thread-safe.
      /// </summary>
      bool ICollection<TValue>.IsReadOnly
      {
        [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
          int unused = myTarget.myActiveCount;
          return true;
        }
      }

      /// <summary>
      /// This member is thread-safe.
      /// </summary>
      bool ICollection.IsSynchronized
      {
        [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
          int unused = myTarget.myActiveCount;
          return false;
        }
      }

      /// <summary>
      /// This member is thread-safe.
      /// This member is not supported.
      /// </summary>
      object ICollection.SyncRoot
      {
        [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
        [MethodImpl(Helper.JustOptimize)]
        get
        {
          int unused = myTarget.myActiveCount;
          throw new NotSupportedException("SyncRoot is not supported.");
        }
      }

      #endregion ICollection<TValue>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      #region GetEnumerator (explicit implementations)

      [MethodImpl(Helper.OptimizeInline)]
      IEnumerator2<TValue> IEnumerable2<TValue>.GetEnumerator()
      {
        return new ValueEnumerator(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      IEnumerator2 IEnumerable2.GetEnumerator()
      {
        return new ValueEnumerator(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
      {
        return new ValueEnumerator(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      IEnumerator IEnumerable.GetEnumerator()
      {
        return new ValueEnumerator(myTarget);
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
    : Map2<TKey, TValue>,
      IEnumerable2<KeyValuePair<TKey, TValue>, Map2<TKey, TValue>.Enumerator>,
      IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary
    where TEqualityComparer : IEqualityComparer2<TKey>
  {
    [SuppressMessage("Style", "IDE0044", Justification = "The equality comparer could be a mutable structure.")]
    private TEqualityComparer myComparer;

    /// <summary>
    /// Initializes the instance with a specific equality comparer.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Map2(TEqualityComparer comparer)
    {
      myComparer = comparer;
    }

    /// <summary>
    /// Initializes the instance with the <see langword="default"/> comparer.
    /// This constructor should only be used
    /// if <typeparamref name="TEqualityComparer"/> is a value type and requires no initialization.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Map2()
    {
      myComparer = default(TEqualityComparer);
    }

    private protected sealed override void InternalInheritance()
    {
    }

    #region public members that hide those from the base class

    /// <summary>
    /// Gets or sets the value corresponding to the specified key.
    /// </summary>
    /// <exception cref="KeyNotFoundException">If the key does not exist when getting the value.</exception>
    public new TValue this[TKey key]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        TValue value;
        Unsafe.SkipInit(out value);
        if (!TryGet(key, ref value))
        {
          Map2.ThrowKeyNotFound();
        }
        return value;
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
        AddOrReplace(key, value);
      }
    }

    /// <returns><see langword="true"/> if the key exists.</returns>
    [MethodImpl(Helper.JustOptimize)]
    public new bool ContainsKey(TKey key)
    {
      /* We guard against the rude behavior of mutating the map instance in the user-implemented IEqualityComparer2. */
#if MAP2_ENUMERATION_VERSION
      uint version = myVersion, version2 = myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      int currentEntry = buckets[hashCode % buckets.Length];
      while (currentEntry >= 0)
      {
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          return true;
        }
        currentEntry = entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      return false;
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is written to once, with the value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is not written to.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    [MethodImpl(Helper.JustOptimize)]
    public new bool TryGet(TKey key, ref TValue value)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = myVersion, version2 = myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      int currentEntry = buckets[hashCode % buckets.Length];
      while (currentEntry >= 0)
      {
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          value = entries[currentEntry].Value;
          return true;
        }
        currentEntry = entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      return false;
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is written to once, with the value being the value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is written to once, with the value <see langword="default"/>.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    [MethodImpl(Helper.JustOptimize)]
    public new bool GetOrDefault(TKey key, out TValue value)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = myVersion, version2 = myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      int currentEntry = buckets[hashCode % buckets.Length];
      while (currentEntry >= 0)
      {
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          value = entries[currentEntry].Value;
          return true;
        }
        currentEntry = entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      value = default(TValue);
      return false;
    }

    /// <summary>
    /// Tries to swap the value corresponding to <paramref name="key"/> with <paramref name="value"/>.
    /// If the key exists, <paramref name="value"/> is read once, whose value becomes the new value corresponding to <paramref name="key"/>,
    /// and <paramref name="value"/> is written to once afterwards, with the old value corresponding to <paramref name="key"/>.
    /// If the key does not exist, <paramref name="value"/> is neither read nor written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key exists.</returns>
    [SuppressMessage("Style", "IDE0180", Justification = "I do not like this fancy stuff.")]
    [MethodImpl(Helper.JustOptimize)]
    public new bool TrySwap(TKey key, ref TValue value)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = ++myVersion, version2 = myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      int currentEntry = buckets[hashCode % buckets.Length];
      while (currentEntry >= 0)
      {
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          TValue tmp = entries[currentEntry].Value;
          entries[currentEntry].Value = value;
          value = tmp;
          return true;
        }
        currentEntry = entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      return false;
    }

    /// <returns><see langword="true"/> if the key existed.</returns>
    [MethodImpl(Helper.JustOptimize)]
    public new bool Remove(TKey key)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = myVersion, version2 = ++myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      ref int refNextOfPreviousEntry = ref buckets[hashCode % buckets.Length];
      while (true)
      {
        int currentEntry = refNextOfPreviousEntry;
        if (currentEntry < 0)
        {
          break;
        }
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          refNextOfPreviousEntry = entries[currentEntry].Next;
          entries[currentEntry].Next = myFirstFreeEntry;
          entries[currentEntry].HashCode = -1;
          entries[currentEntry].Key = default(TKey);
          entries[currentEntry].Value = default(TValue);
          myFirstFreeEntry = currentEntry;
          return true;
        }
        refNextOfPreviousEntry = ref entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      return false;
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/> and remove <paramref name="key"/> from the map.
    /// If the key existed, <paramref name="value"/> is written to once, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is not written to.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key existed.</returns>
    [MethodImpl(Helper.JustOptimize)]
    public new bool TryGetAndRemove(TKey key, ref TValue value)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = myVersion, version2 = ++myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      ref int refNextOfPreviousEntry = ref buckets[hashCode % buckets.Length];
      while (true)
      {
        int currentEntry = refNextOfPreviousEntry;
        if (currentEntry < 0)
        {
          break;
        }
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          value = entries[currentEntry].Value;
          refNextOfPreviousEntry = entries[currentEntry].Next;
          entries[currentEntry].Next = myFirstFreeEntry;
          entries[currentEntry].HashCode = -1;
          entries[currentEntry].Key = default(TKey);
          entries[currentEntry].Value = default(TValue);
          myFirstFreeEntry = currentEntry;
          return true;
        }
        refNextOfPreviousEntry = ref entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      return false;
    }

    /// <summary>
    /// Tries to copy the value corresponding to <paramref name="key"/> into <paramref name="value"/> and remove <paramref name="key"/> from the map.
    /// If the key existed, <paramref name="value"/> is written to once, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is written to once, with the value <see langword="default"/>.
    /// This method never reads <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key existed.</returns>
    [MethodImpl(Helper.JustOptimize)]
    public new bool GetAndRemoveOrDefault(TKey key, out TValue value)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = myVersion, version2 = ++myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      ref int refNextOfPreviousEntry = ref buckets[hashCode % buckets.Length];
      while (true)
      {
        int currentEntry = refNextOfPreviousEntry;
        if (currentEntry < 0)
        {
          break;
        }
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          value = entries[currentEntry].Value;
          refNextOfPreviousEntry = entries[currentEntry].Next;
          entries[currentEntry].Next = myFirstFreeEntry;
          entries[currentEntry].HashCode = -1;
          entries[currentEntry].Key = default(TKey);
          entries[currentEntry].Value = default(TValue);
          myFirstFreeEntry = currentEntry;
          return true;
        }
        refNextOfPreviousEntry = ref entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      value = default(TValue);
      return false;
    }

    /// <summary>
    /// Tries to add <paramref name="key"/> with <paramref name="value"/>.
    /// If <paramref name="key"/> already exists, its existing value is not updated.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public new bool TryAddNew(TKey key, TValue value)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = ++myVersion, version2 = myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      int currentEntry = buckets[hashCode % buckets.Length];
      while (currentEntry >= 0)
      {
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          return false;
        }
        currentEntry = entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      currentEntry = AllocEntry(out buckets, out entries);
      entries[currentEntry].HashCode = hashCode;
      entries[currentEntry].Key = key;
      entries[currentEntry].Value = value;
      hashCode %= buckets.Length;
      entries[currentEntry].Next = buckets[hashCode];
      buckets[hashCode] = currentEntry;
      return true;
    }

    /// <summary>
    /// Adds or replaces the value correponding to <paramref name="key"/> as/by <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public new bool AddOrReplace(TKey key, TValue value)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = ++myVersion, version2 = myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      int currentEntry = buckets[hashCode % buckets.Length];
      while (currentEntry >= 0)
      {
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          entries[currentEntry].Value = value;
          return false;
        }
        currentEntry = entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      currentEntry = AllocEntry(out buckets, out entries);
      entries[currentEntry].HashCode = hashCode;
      entries[currentEntry].Key = key;
      entries[currentEntry].Value = value;
      hashCode %= buckets.Length;
      entries[currentEntry].Next = buckets[hashCode];
      buckets[hashCode] = currentEntry;
      return true;
    }

    /// <summary>
    /// If the key existed, <paramref name="value"/> is not read, but written to once, with the existing value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is read once, whose value becomes the value corresponding to <paramref name="key"/>, and is never written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public new bool AddOrGet(TKey key, ref TValue value)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = ++myVersion, version2 = myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      int currentEntry = buckets[hashCode % buckets.Length];
      while (currentEntry >= 0)
      {
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          value = entries[currentEntry].Value;
          return false;
        }
        currentEntry = entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      currentEntry = AllocEntry(out buckets, out entries);
      entries[currentEntry].HashCode = hashCode;
      entries[currentEntry].Key = key;
      entries[currentEntry].Value = value;
      hashCode %= buckets.Length;
      entries[currentEntry].Next = buckets[hashCode];
      buckets[hashCode] = currentEntry;
      return true;
    }

    /// <summary>
    /// If the key existed, <paramref name="value"/> is read once, whose value becomes the new value corresponding to <paramref name="key"/>,
    /// and <paramref name="value"/> is written to once afterwards, with the old value corresponding to <paramref name="key"/>.
    /// If the key did not exist, <paramref name="value"/> is read once, whose value becomes the value corresponding to <paramref name="key"/>, and is never written to.
    /// </summary>
    /// <returns><see langword="true"/> if the key did not exist.</returns>
    /// <exception cref="InvalidOperationException">If the number of key/value pairs will exceed <see cref="Map2.MaximumCapacity"/>.</exception>
    [SuppressMessage("Style", "IDE0180", Justification = "I do not like this fancy stuff.")]
    [MethodImpl(Helper.JustOptimize)]
    public new bool AddOrSwap(TKey key, ref TValue value)
    {
#if MAP2_ENUMERATION_VERSION
      uint version = ++myVersion, version2 = myVersion2;
#endif
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = myComparer.GetHashCode(key) & Map2.HashCodeMask;
      int currentEntry = buckets[hashCode % buckets.Length];
      while (currentEntry >= 0)
      {
        if (entries[currentEntry].HashCode == hashCode && myComparer.Equals(key, entries[currentEntry].Key))
        {
#if MAP2_ENUMERATION_VERSION
          if (version != myVersion || version2 != myVersion2)
          {
            Map2.ThrowVersion();
          }
#endif
          TValue tmp = entries[currentEntry].Value;
          entries[currentEntry].Value = value;
          value = tmp;
          return false;
        }
        currentEntry = entries[currentEntry].Next;
      }
#if MAP2_ENUMERATION_VERSION
      if (version != myVersion || version2 != myVersion2)
      {
        Map2.ThrowVersion();
      }
#endif
      currentEntry = AllocEntry(out buckets, out entries);
      entries[currentEntry].HashCode = hashCode;
      entries[currentEntry].Key = key;
      entries[currentEntry].Value = value;
      hashCode %= buckets.Length;
      entries[currentEntry].Next = buckets[hashCode];
      buckets[hashCode] = currentEntry;
      return true;
    }

    /// <summary>
    /// Gets a view of the keys.
    /// Obtaining such a view is thread-safe.
    /// </summary>
    public new KeyView Keys
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return new KeyView(this);
      }
    }

    #endregion public members that hide those from the base class

    #region virtual methods

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool ContainsKeyOverride(TKey key)
    {
      return ContainsKey(key);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool TryGetOverride(TKey key, ref TValue value)
    {
      return TryGet(key, ref value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool GetOrDefaultOverride(TKey key, out TValue value)
    {
      return GetOrDefault(key, out value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool TrySwapOverride(TKey key, ref TValue value)
    {
      return TrySwap(key, ref value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool RemoveOverride(TKey key)
    {
      return Remove(key);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool TryGetAndRemoveOverride(TKey key, ref TValue value)
    {
      return TryGetAndRemove(key, ref value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool GetAndRemoveOrDefaultOverride(TKey key, out TValue value)
    {
      return GetAndRemoveOrDefault(key, out value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool TryAddNewOverride(TKey key, TValue value)
    {
      return TryAddNew(key, value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool AddOrReplaceOverride(TKey key, TValue value)
    {
      return AddOrReplace(key, value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool AddOrGetOverride(TKey key, ref TValue value)
    {
      return AddOrGet(key, ref value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override bool AddOrSwapOverride(TKey key, ref TValue value)
    {
      return AddOrSwap(key, ref value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override ICollection<TKey> KeysOverride()
    {
      return new KeyView(this);
    }

    #endregion virtual methods

    #region IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count, ICollection<KeyValuePair<TKey, TValue>>.Count, ICollection.Count

    int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myActiveCount;
      }
    }

    int ICollection<KeyValuePair<TKey, TValue>>.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myActiveCount;
      }
    }

    int ICollection.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myActiveCount;
      }
    }

    #endregion IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count, ICollection<KeyValuePair<TKey, TValue>>.Count, ICollection.Count

    #region ICollection<KeyValuePair<TKey, TValue>>.Clear, IDictionary.Clear

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
      Clear();
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IDictionary.Clear()
    {
      Clear();
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.Clear, IDictionary.Clear

    #region IReadOnlyDictionary<TKey, TValue>.this[TKey key], IDictionary<TKey, TValue>.this[TKey key], IDictionary.this[object key]

    TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        TValue value;
        Unsafe.SkipInit(out value);
        if (!TryGet(key, ref value))
        {
          Map2.ThrowKeyNotFound();
        }
        return value;
      }
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        TValue value;
        Unsafe.SkipInit(out value);
        if (!TryGet(key, ref value))
        {
          Map2.ThrowKeyNotFound();
        }
        return value;
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
        AddOrReplace(key, value);
      }
    }

    object IDictionary.this[object key]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        TValue value;
        Unsafe.SkipInit(out value);
        if (!(key is TKey tKey
          ? TryGet(tKey, ref value)
          : default(TKey) is null && key is null && TryGet(default(TKey), ref value)))
        {
          Map2.ThrowKeyNotFound();
        }
        return value;
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
        /* The attempt is modify the map has been made upon entering this method,
        /* even if the key or the value cannot be cast to the right type(s). */
#if MAP2_ENUMERATION_VERSION
        ++myVersion;
#endif
        AddOrReplace((TKey)key, (TValue)value);
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.this[TKey key], IDictionary<TKey, TValue>.this[TKey key], IDictionary.this[object key]

    #region IDictionary<TKey, TValue>.Add, ICollection<KeyValuePair<TKey, TValue>>.Add, IDictionary.Add

    [MethodImpl(Helper.OptimizeInline)]
    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
      if (!TryAddNew(key, value))
      {
        Map2.ThrowKeyExists();
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      if (!TryAddNew(item.Key, item.Value))
      {
        Map2.ThrowKeyExists();
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IDictionary.Add(object key, object value)
    {
#if MAP2_ENUMERATION_VERSION
      ++myVersion;
#endif
      if (!TryAddNew((TKey)key, (TValue)value))
      {
        Map2.ThrowKeyExists();
      }
    }

    #endregion IDictionary<TKey, TValue>.Add, ICollection<KeyValuePair<TKey, TValue>>.Add, IDictionary.Add

    #region IReadOnlyDictionary<TKey, TValue>.TryGetValue, IDictionary<TKey, TValue>.TryGetValue

    [MethodImpl(Helper.OptimizeInline)]
    bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
    {
      return GetOrDefault(key, out value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
    {
      return GetOrDefault(key, out value);
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.TryGetValue, IDictionary<TKey, TValue>.TryGetValue

    #region IReadOnlyDictionary<TKey, TValue>.ContainsKey, IDictionary<TKey, TValue>.ContainsKey, ICollection<KeyValuePair<TKey, TValue>>.Contains, IDictionary.Contains

    [MethodImpl(Helper.OptimizeInline)]
    bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
    {
      return ContainsKey(key);
    }

    [MethodImpl(Helper.OptimizeInline)]
    bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
    {
      return ContainsKey(key);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
      throw new NotImplementedException();
    }

    [MethodImpl(Helper.OptimizeInline)]
    bool IDictionary.Contains(object key)
    {
      return key is TKey tKey
        ? ContainsKey(tKey)
        : default(TKey) is null && key is null && ContainsKey(default(TKey));
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.ContainsKey, IDictionary<TKey, TValue>.ContainsKey, ICollection<KeyValuePair<TKey, TValue>>.Contains, IDictionary.Contains

    #region IDictionary<TKey, TValue>.Remove, ICollection<KeyValuePair<TKey, TValue>>.Remove, IDictionary.Remove

    [MethodImpl(Helper.OptimizeInline)]
    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
      return Remove(key);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
      throw new NotImplementedException();
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IDictionary.Remove(object key)
    {
      if (key is TKey tKey)
      {
        Remove(tKey);
      }
      else if (default(TKey) is null && key is null)
      {
        Remove(default(TKey));
      }
    }

    #endregion IDictionary<TKey, TValue>.Remove, ICollection<KeyValuePair<TKey, TValue>>.Remove, IDictionary.Remove

    #region ICollection<KeyValuePair<TKey, TValue>>.CopyTo, ICollection.CopyTo

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      if (array is null)
      {
        Map2.ThrowCopyToArrayNull();
      }
      CopyPairsToImpl(myEntries, array, arrayIndex, myActiveCount, nameof(arrayIndex));
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection.CopyTo(Array array, int index)
    {
      if (array is KeyValuePair<TKey, TValue>[] kvpArray)
      {
        CopyPairsToImpl(myEntries, kvpArray, index, myActiveCount, nameof(index));
      }
      else if (array is DictionaryEntry[] deArray)
      {
        CopyEntriesToImpl(myEntries, deArray, index, myActiveCount, nameof(index));
      }
      else
      {
        Map2.ThrowCopyPairsOrEntriesToArrayType();
      }
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.CopyTo, ICollection.CopyTo

    #region IReadOnlyDictionary<TKey, TValue>.Keys, IDictionary<TKey, TValue>.Keys, IDictionary.Keys

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return new KeyView(this);
      }
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return new KeyView(this);
      }
    }

    ICollection IDictionary.Keys
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return new KeyView(this);
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.Keys, IDictionary<TKey, TValue>.Keys, IDictionary.Keys

    #region IReadOnlyDictionary<TKey, TValue>.Values, IDictionary<TKey, TValue>.Values, IDictionary.Values

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return new ValueView(this);
      }
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return new ValueView(this);
      }
    }

    ICollection IDictionary.Values
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return new ValueView(this);
      }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>.Values, IDictionary<TKey, TValue>.Values, IDictionary.Values

    #region ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly, IDictionary.IsReadOnly, IDictionary.IsFixedSize, ICollection.IsSynchronized, ICollection.SyncRoot

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool IDictionary.IsReadOnly
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool IDictionary.IsFixedSize
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool ICollection.IsSynchronized
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// This member is not supported.
    /// </summary>
    object ICollection.SyncRoot
    {
      [MethodImpl(Helper.JustOptimize)]
      get
      {
        throw new NotSupportedException("SyncRoot is not supported.");
      }
    }

    #endregion ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly, IDictionary.IsReadOnly, IDictionary.IsFixedSize, ICollection.IsSynchronized, ICollection.SyncRoot

    #region GetEnumerator (explicit implementations)

    [MethodImpl(Helper.OptimizeInline)]
    Enumerator IEnumerable2<KeyValuePair<TKey, TValue>, Enumerator>.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator2<KeyValuePair<TKey, TValue>> IEnumerable2<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return new DictionaryEnumerator(this);
    }

    #endregion GetEnumerator (explicit implementations)

    /// <summary>
    /// Represents a view of the keys in a <see cref="Map2{TKey, TValue, TEqualityComparer}"/> instance.
    /// Among instance members,
    /// only those of <see cref="IEquatable{T}"/> and <see cref="object"/> can be invoked on <see langword="default"/> instances.
    /// </summary>
    public readonly struct KeyView : IEquatable<KeyView>, IEnumerable2<TKey, KeyEnumerator>, ICollection<TKey>, IReadOnlyCollection<TKey>, ICollection
    {
      private readonly Map2<TKey, TValue, TEqualityComparer> myTarget;

      [MethodImpl(Helper.OptimizeInline)]
      internal KeyView(Map2<TKey, TValue, TEqualityComparer> target)
      {
        myTarget = target;
      }

      public int Count
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
          return myTarget.myActiveCount;
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Contains(TKey item)
      {
        return myTarget.ContainsKey(item);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void CopyTo(TKey[] array, int arrayIndex)
      {
        myTarget.CopyKeysTo(array, arrayIndex);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public KeyEnumerator GetEnumerator()
      {
        return new KeyEnumerator(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Equals(KeyView other)
      {
        return ReferenceEquals(myTarget, other.myTarget);
      }

      #region static Equals, operator ==, operator !=

      [MethodImpl(Helper.OptimizeInline)]
      public static bool Equals(KeyView x, KeyView y)
      {
        return ReferenceEquals(x.myTarget, y.myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public static bool operator ==(KeyView x, KeyView y)
      {
        return ReferenceEquals(x.myTarget, y.myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public static bool operator !=(KeyView x, KeyView y)
      {
        return !ReferenceEquals(x.myTarget, y.myTarget);
      }

      #endregion static Equals, operator ==, operator !=

      #region object members

      [MethodImpl(Helper.OptimizeInline)]
      public override bool Equals(object obj)
      {
        return (obj is KeyView other) && ReferenceEquals(myTarget, other.myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public override int GetHashCode()
      {
        return RuntimeHelpers.GetHashCode(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public override string ToString()
      {
        return typeof(KeyView).FullName;
      }

      #endregion object members

      #region ICollection.CopyTo

      [MethodImpl(Helper.OptimizeInline)]
      void ICollection.CopyTo(Array array, int index)
      {
        Map2<TKey, TValue, TEqualityComparer> target = myTarget;
        int activeCount = target.myActiveCount;
        if (array is TKey[] kArray)
        {
          CopyKeysToImpl(target.myEntries, kArray, index, activeCount, nameof(index));
        }
        else
        {
          Map2.ThrowCopyKeysToArrayType();
        }
      }

      #endregion ICollection.CopyTo

      #region ICollection<TKey>.Add, ICollection<TKey>.Clear, ICollection<TKey>.Remove

      [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
      [MethodImpl(Helper.OptimizeNoInline)]
      void ICollection<TKey>.Add(TKey item)
      {
        int unused = myTarget.myActiveCount;
        throw new NotSupportedException();
      }

      [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
      [MethodImpl(Helper.OptimizeNoInline)]
      void ICollection<TKey>.Clear()
      {
        int unused = myTarget.myActiveCount;
        throw new NotSupportedException();
      }

      [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
      [MethodImpl(Helper.OptimizeNoInline)]
      bool ICollection<TKey>.Remove(TKey item)
      {
        int unused = myTarget.myActiveCount;
        throw new NotSupportedException();
      }

      #endregion ICollection<TKey>.Add, ICollection<TKey>.Clear, ICollection<TKey>.Remove

      #region ICollection<TKey>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      /// <summary>
      /// This member is thread-safe.
      /// </summary>
      bool ICollection<TKey>.IsReadOnly
      {
        [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
          int unused = myTarget.myActiveCount;
          return true;
        }
      }

      /// <summary>
      /// This member is thread-safe.
      /// </summary>
      bool ICollection.IsSynchronized
      {
        [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
          int unused = myTarget.myActiveCount;
          return false;
        }
      }

      /// <summary>
      /// This member is thread-safe.
      /// This member is not supported.
      /// </summary>
      object ICollection.SyncRoot
      {
        [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
        [MethodImpl(Helper.JustOptimize)]
        get
        {
          int unused = myTarget.myActiveCount;
          throw new NotSupportedException("SyncRoot is not supported.");
        }
      }

      #endregion ICollection<TKey>.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

      #region GetEnumerator (explicit implementations)

      [MethodImpl(Helper.OptimizeInline)]
      IEnumerator2<TKey> IEnumerable2<TKey>.GetEnumerator()
      {
        return new KeyEnumerator(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      IEnumerator2 IEnumerable2.GetEnumerator()
      {
        return new KeyEnumerator(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
      {
        return new KeyEnumerator(myTarget);
      }

      [MethodImpl(Helper.OptimizeInline)]
      IEnumerator IEnumerable.GetEnumerator()
      {
        return new KeyEnumerator(myTarget);
      }

      #endregion GetEnumerator (explicit implementations)
    }
  }

  public static class Map2
  {
    internal const int HashCodeMask = 0x7FFFFFFF;

    public const int MaximumCapacity = 881646013;

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    internal static readonly int[] theEmptyBuckets = new int[0];

    internal readonly struct Size
    {
      public readonly int BucketCount;
      public readonly int EntryCount;

      public Size(int bucketCount, int entryCount)
      {
        BucketCount = bucketCount;
        EntryCount = entryCount;
      }
    }

    internal static readonly Size[] theSizes = new Size[]
    {
      new Size(17, 12),
      new Size(37, 27),
      new Size(79, 59),
      new Size(163, 122),
      new Size(331, 248),
      new Size(673, 504),
      new Size(1361, 1020),
      new Size(2729, 2046),
      new Size(5471, 4103),
      new Size(10949, 8211),
      new Size(21911, 16433),
      new Size(43853, 32889),
      new Size(87719, 65789),
      new Size(175447, 131585),
      new Size(350899, 263174),
      new Size(701819, 526364),
      new Size(1052731, 894821),
      new Size(1579099, 1342234),
      new Size(2368649, 2013351),
      new Size(3552977, 3020030),
      new Size(5329469, 4530048),
      new Size(7994213, 6795081),
      new Size(11991373, 10192667),
      new Size(17987089, 15289025),
      new Size(26980697, 22933592),
      new Size(40471061, 34400401),
      new Size(60706627, 51600632),
      new Size(91059949, 77400956),
      new Size(136590001, 116101500),
      new Size(204885013, 174152261),
      new Size(307327523, 261228394),
      new Size(460991303, 391842607),
      new Size(691487003, 587763952),
      new Size(881646013, 881646013)
    };

    /// <summary>
    /// This method assumes <paramref name="buckets"/> is within <see cref="theSizes"/>.
    /// It is non-inlining to reduce the risk of read introduction.
    /// See <a href="https://github.com/dotnet/docs/issues/29696">dotnet/docs#29696</a>.
    /// </summary>
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ResetBuckets(int[] buckets)
    {
      Unsafe.InitBlockUnaligned(
        ref Unsafe.As<int, byte>(ref MemoryMarshal.GetArrayDataReference(buckets)),
        0xFF,
        (uint)buckets.Length * 4u);
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowKeyNotFound()
    {
      throw new KeyNotFoundException();
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowKeyExists()
    {
      throw new ArgumentException("The key already exists in the map.", "key");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowTooMany()
    {
      throw new InvalidOperationException("There will be more than MaximumCapacity number of key/value pairs in the map.");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowCopyToArrayNull()
    {
      throw new ArgumentNullException("array");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowCopyKeysToArrayType()
    {
      throw new ArgumentException("The array must be TKey[].", "array");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowCopyValuesToArrayType()
    {
      throw new ArgumentException("The array must be TValue[].", "array");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowCopyPairsOrEntriesToArrayType()
    {
      throw new ArgumentException("The array must be either KeyValuePair<TKey, TValue>[] or DictionaryEntry[].", "array");
    }

#if MAP2_ENUMERATION_VERSION

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowVersion()
    {
      throw new InvalidOperationException("The map is modified during enumeration. (This check is enabled by MAP2_ENUMERATION_VERSION.)");
    }

#endif


#if MAP2_ENUMERATOR_DISPOSE

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowEnumeratorDisposed()
    {
      throw new ObjectDisposedException(typeof(Map2<,>.Enumerator).FullName,
        "The enumerator is already disposed. (This check is enabled by MAP2_ENUMERATOR_DISPOSE.)");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowKeyEnumeratorDisposed()
    {
      throw new ObjectDisposedException(typeof(Map2<,>.KeyEnumerator).FullName,
        "The enumerator is already disposed. (This check is enabled by MAP2_ENUMERATOR_DISPOSE.)");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowValueEnumeratorDisposed()
    {
      throw new ObjectDisposedException(typeof(Map2<,>.ValueEnumerator).FullName,
        "The enumerator is already disposed. (This check is enabled by MAP2_ENUMERATOR_DISPOSE.)");
    }

#endif
  }
}
