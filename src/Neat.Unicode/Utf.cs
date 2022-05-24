using System.Runtime.CompilerServices;

namespace Neat.Unicode
{
  public static class Utf
  {
    internal const int ReplacementCharacter32 = 0xFFFD;
    internal const char ReplacementCharacter16 = (char)0xFFFD;

    #region generic property determination for Char8

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char8Leads1(byte value)
    {
      return value < 0x80u;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char8Continues(byte value)
    {
      return (value & 0xC0) == 0x80;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char8Leads2(byte value)
    {
      return (value & 0xE0) == 0xC0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char8Leads3(byte value)
    {
      return (value & 0xF0) == 0xE0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char8Leads4(byte value)
    {
      return (value & 0xF8) == 0xF0;
    }

    #endregion generic property determination for Char8

    #region Char8 to Char32

    [MethodImpl(Helper.OptimizeInline)]
    internal static int Char8ToChar32Unchecked1(byte lead1)
    {
      return lead1;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static int Char8ToChar32Unchecked2(byte lead2, byte cont1)
    {
      return ((lead2 & 0x1F) << 6) | (cont1 & 0x3F);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static int Char8ToChar32Unchecked3(byte lead3, byte cont1, byte cont2)
    {
      return ((lead3 & 0x0F) << 12) | ((cont1 & 0x3F) << 6) | (cont2 & 0x3F);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static int Char8ToChar32Unchecked4(byte lead4, byte cont1, byte cont2, byte cont3)
    {
      return ((lead4 & 0x07) << 18) | ((cont1 & 0x3F) << 12) | ((cont2 & 0x3F) << 6) | (cont3 & 0x3F);
    }

    #endregion Char8 to Char32

    #region generic property determination for Char16

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char16IsSurrogate(char value)
    {
      return (value & 0xFFFFF800) == 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char16IsHighSurrogate(char value)
    {
      return (value & 0xFFFFFC00) == 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char16IsLowSurrogate(char value)
    {
      return (value & 0xFFFFFC00) == 0xDC00;
    }

    #endregion generic property determination for Char16

    #region Char16 to Char32

    [MethodImpl(Helper.OptimizeInline)]
    internal static int Char16ToChar32Unchecked1(char value)
    {
      return value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static int Char16ToChar32Unchecked2(char high, char low)
    {
      return (((high & 0x3FF) << 10) | (low & 0x3FF)) + 0x10000;
    }

    #endregion Char16 to Char32

    #region range check for Char32

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsBelow0x80(int value)
    {
      return (uint)value < 0x80u;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsBelow0x800(int value)
    {
      return (uint)value < 0x800u;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsBelow0x10000(int value)
    {
      return (uint)value < 0x10000u;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsBelow0x110000(int value)
    {
      return (uint)value < 0x110000u;
    }

    #endregion range check for Char32

    #region generic property determination for Char32

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsValid(int value)
    {
      return (uint)value < 0x110000u && (value & 0xFFFFF800) != 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsSurrogate(int value)
    {
      return (value & 0xFFFFF800) == 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsHighSurrogate(int value)
    {
      return (value & 0xFFFFFC00) == 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsLowSurrogate(int value)
    {
      return (value & 0xFFFFFC00) == 0xDC00;
    }

    #endregion generic property determination for Char32

    #region generic encoding length counting for Char32 in Char8s

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is1Char8(int value)
    {
      return (uint)value < 0x80u;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is2Char8s(int value)
    {
      return 0x80u <= (uint)value && (uint)value < 0x800u;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is3Char8s(int value)
    {
      return 0x800u <= (uint)value && (uint)value < 0x10000u
        && (value & 0xFFFFF800) != 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is4Char8s(int value)
    {
      return 0x10000u <= (uint)value && (uint)value < 0x110000u;
    }

    #endregion generic encoding length counting for Char32 in Char8s

    #region generic encoding length counting for Char32 in Char16s

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is1Char16(int value)
    {
      return (uint)value < 0x10000u && (value & 0xFFFFF800) != 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is2Char16s(int value)
    {
      return 0x10000u <= (uint)value && (uint)value < 0x110000u;
    }

    #endregion generic encoding length counting for Char32 in Char16s

    #region Char32 to Char8

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To1Char8UncheckedLead1(int value)
    {
      return (byte)value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To1Char8Unchecked(int value)
    {
      return (byte)value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To2Char8sUncheckedLead2(int value)
    {
      return (byte)((value >> 6) | 0xC0);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To2Char8sUncheckedCont1(int value)
    {
      return (byte)((value & 0x3F) | 0x80);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To3Char8sUncheckedLead3(int value)
    {
      return (byte)((value >> 12) | 0xE0);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To3Char8sUncheckedCont1(int value)
    {
      return (byte)(((value >> 6) & 0x3F) | 0x80);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To3Char8sUncheckedCont2(int value)
    {
      return (byte)((value & 0x3F) | 0x80);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To4Char8sUncheckedLead4(int value)
    {
      return (byte)((value >> 18) | 0xF0);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To4Char8sUncheckedCont1(int value)
    {
      return (byte)(((value >> 12) & 0x3F) | 0x80);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To4Char8sUncheckedCont2(int value)
    {
      return (byte)(((value >> 6) & 0x3F) | 0x80);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To4Char8sUncheckedCont3(int value)
    {
      return (byte)((value & 0x3F) | 0x80);
    }

    #endregion Char32 to Char8

    #region Char32 to Char16

    [MethodImpl(Helper.OptimizeInline)]
    internal static char Char32To1Char16Unchecked(int value)
    {
      return (char)value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static int Char32To2Char16sUncheckedPrepare(int value)
    {
      return value - 0x10000;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static char Char32PreparedTo2Char16sUncheckedHigh(int value)
    {
      return (char)((value >> 10) | 0xD800);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static char Char32PreparedTo2Char16sUncheckedLow(int value)
    {
      return (char)((value & 0x3FF) | 0xDC00);
    }

    #endregion Char32 to Char16

    #region validity of Char32 from Char8 (overlong, surrogate, above 0x10FFFF)

    internal static bool Char32From2Char8sIsValid(int value)
    {
      return 0x80u <= (uint)value;
    }

    internal static bool Char32From3Char8sIsValid(int value)
    {
      return 0x800u <= (uint)value && (value & 0xFFFFF800) != 0xD800;
    }

    internal static bool Char32From4Char8sIsValid(int value)
    {
      return 0x10000u <= (uint)value && (uint)value < 0x110000u;
    }

    #endregion validity of Char32 from Char8 (overlong, surrogate, above 0x10FFFF)

    /// <summary>
    /// Finds the first invalid <see cref="Char8"/> instance.
    /// This method returns <c>-1</c> if the stream is valid.
    /// </summary>
    internal static int FindFirstInvalidChar8(ref byte src0, int src8s)
    {
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid;
          }
          goto Invalid;
        }
      Invalid:
        return i;
      Valid:
        i = j;
      }
      return -1;
    }

    /// <summary>
    /// Counts the number of invalid <see cref="Char8"/> instances.
    /// </summary>
    internal static int CountInvalidChar8s(ref byte src0, int src8s)
    {
      int invalids = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid;
          }
          goto Invalid;
        }
      Invalid:
        ++invalids;
        continue;
      Valid:
        i = j;
      }
      return invalids;
    }

    /// <summary>
    /// Replaces invalid <see cref="Char8"/> instances by the replacement character.
    /// </summary>
    internal static void SanitizeChar8s(ref byte src0, int src8s, ref byte dst0, int dst8s)
    {
      int k = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j = 0, value; i != src8s && k != dst8s; j = ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          Unsafe.Add(ref dst0, k++) = lead;
          continue;
        }
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(value = Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Below0x800;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(value = Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Below0x10000;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(value = Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Below0x110000;
          }
          goto Invalid;
        }
      Invalid:
        value = ReplacementCharacter32;
        goto IsReplacement;
      Below0x800:
        i = j;
        if (dst8s == k + 1)
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = Char32To2Char8sUncheckedLead2(value);
        Unsafe.Add(ref dst0, k++) = Char32To2Char8sUncheckedCont1(value);
        continue;
      Below0x10000:
        i = j;
      IsReplacement:
        if (dst8s <= k + 2)
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedLead3(value);
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedCont1(value);
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedCont2(value);
        continue;
      Below0x110000:
        i = j;
        if (dst8s <= k + 3)
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedLead4(value);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedCont1(value);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedCont2(value);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedCont3(value);
        continue;
      }
      while (k != dst8s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    /// <summary>
    /// Finds the first invalid <see langword="char"/> instance.
    /// This method returns <c>-1</c> if the stream is valid.
    /// </summary>
    internal static int FindFirstInvalidChar16(ref char src0, int src16s)
    {
      char first;
      for (int i = 0; i != src16s; ++i)
      {
        if (Char16IsLowSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          goto Invalid;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i == src16s || !Char16IsLowSurrogate(Unsafe.Add(ref src0, i)))
          {
            goto InvalidDecrease;
          }
          continue;
        }
        continue;
      InvalidDecrease:
        return --i;
      Invalid:
        return i;
      }
      return -1;
    }

    /// <summary>
    /// Counts the number of invalid <see langword="char"/> instances.
    /// </summary>
    internal static int CountInvalidChar16s(ref char src0, int src16s)
    {
      int invalids = 0;
      char first;
      for (int i = 0; i != src16s; ++i)
      {
        if (Char16IsLowSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          goto Invalid;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i == src16s || !Char16IsLowSurrogate(Unsafe.Add(ref src0, i)))
          {
            goto InvalidDecrease;
          }
          continue;
        }
        continue;
      InvalidDecrease:
        --i;
      Invalid:
        ++invalids;
      }
      return invalids;
    }

    /// <summary>
    /// Replaces invalid <see langword="char"/> instances by the replacement character.
    /// </summary>
    internal static void SanitizeChar16s(ref char src0, int src16s, ref char dst0, int dst16s)
    {
      int k = 0;
      char first, low;
      for (int i = 0; i != src16s && k != dst16s; ++i)
      {
        if (Char16IsLowSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          goto Invalid;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i == src16s || !Char16IsLowSurrogate(low = Unsafe.Add(ref src0, i)))
          {
            goto InvalidDecrease;
          }
          goto Valid2;
        }
        /* Valid1: */
        Unsafe.Add(ref dst0, k++) = first;
        continue;
      Valid2:
        if (dst16s == k + 1)
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = first;
        Unsafe.Add(ref dst0, k++) = low;
        continue;
      InvalidDecrease:
        --i;
      Invalid:
        Unsafe.Add(ref dst0, k++) = ReplacementCharacter16;
      }
      while (k != dst16s)
      {
        Unsafe.Add(ref dst0, k++) = (char)0;
      }
    }

    /// <summary>
    /// Finds the first invalid <see cref="Char32"/> instance.
    /// This method returns <c>-1</c> if the stream is valid.
    /// </summary>
    internal static int FindFirstInvalidChar32(ref int src0, int src32s)
    {
      for (int i = 0; i != src32s; ++i)
      {
        if (!Char32IsValid(Unsafe.Add(ref src0, i)))
        {
          return i;
        }
      }
      return -1;
    }

    /// <summary>
    /// Counts the number of invalid <see cref="Char32"/> instances.
    /// </summary>
    internal static int CountInvalidChar32s(ref int src0, int src32s)
    {
      int invalids = 0;
      for (int i = 0; i != src32s; ++i)
      {
        if (!Char32IsValid(Unsafe.Add(ref src0, i)))
        {
          ++invalids;
        }
      }
      return invalids;
    }

    /// <summary>
    /// Replaces invalid <see langword="Char32"/> instances by the replacement character.
    /// </summary>
    internal static void SanitizeChar32s(ref int src0, int src32s, ref int dst0, int dst32s)
    {
      int k = 0;
      for (int i = 0, value; i != src32s && k != dst32s; ++i)
      {
        Unsafe.Add(ref dst0, k++) = Char32IsValid(value = Unsafe.Add(ref src0, i))
          ? value
          : ReplacementCharacter32;
      }
      while (k != dst32s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }
  }
}
