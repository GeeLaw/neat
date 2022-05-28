using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Neat.Unicode
{
  public static class Utf
  {
    #region public methods for decoding or encoding 1 code point

    /// <summary>
    /// Tries to decode a 1-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try1Char8ToChar32(Char8 lead1, out Char32 char32)
    {
      char32 = new Char32(Char8ToChar32Unchecked1(lead1.Value));
      return Char8Leads1(lead1.Value);
    }

    /// <summary>
    /// Tries to decode a 2-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try2Char8sToChar32(Char8 lead2, Char8 cont1, out Char32 char32)
    {
      int value = Char8ToChar32Unchecked2(lead2.Value, cont1.Value);
      char32 = new Char32(value);
      return Char8Leads2(lead2.Value)
        && Char8Continues(cont1.Value)
        && Char32From2Char8sIsValid(value);
    }

    /// <summary>
    /// Tries to decode a 3-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try3Char8sToChar32(Char8 lead3, Char8 cont1, Char8 cont2, out Char32 char32)
    {
      int value = Char8ToChar32Unchecked3(lead3.Value, cont1.Value, cont2.Value);
      char32 = new Char32(value);
      return Char8Leads3(lead3.Value)
        && Char8Continues(cont1.Value)
        && Char8Continues(cont2.Value)
        && Char32From3Char8sIsValid(value);
    }

    /// <summary>
    /// Tries to decode a 4-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try4Char8sToChar32(Char8 lead4, Char8 cont1, Char8 cont2, Char8 cont3, out Char32 char32)
    {
      int value = Char8ToChar32Unchecked4(lead4.Value, cont1.Value, cont2.Value, cont3.Value);
      char32 = new Char32(value);
      return Char8Leads4(lead4.Value)
        && Char8Continues(cont1.Value)
        && Char8Continues(cont2.Value)
        && Char8Continues(cont3.Value)
        && Char32From4Char8sIsValid(value);
    }

    /// <summary>
    /// Tries to decode a 1-unit UTF-16 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try1Char16ToChar32(char char16, out Char32 char32)
    {
      char32 = new Char32(Char16ToChar32Unchecked1(char16));
      return Char16IsNotSurrogate(char16);
    }

    /// <summary>
    /// Tries to decode a 2-unit UTF-16 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try2Char16sToChar32(char high, char low, out Char32 char32)
    {
      char32 = new Char32(Char16ToChar32Unchecked2(high, low));
      return Char16IsHighSurrogate(high) && Char16IsLowSurrogate(low);
    }

    /// <summary>
    /// Tries to encode a 1-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="lead1"/> contains the valid encoded UTF-8 sequence.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="lead1"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To1Char8(Char32 char32, out byte lead1)
    {
      lead1 = Char32To1Char8UncheckedLead1(char32.Value);
      return Char32Is1Char8(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 2-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="lead2"/>, <paramref name="cont1"/> contain the valid encoded UTF-8 sequence.
    /// Otherwise, there is no guarantee as to what are contained in <paramref name="lead2"/>, <paramref name="cont1"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To2Char8s(Char32 char32, out byte lead2, out byte cont1)
    {
      lead2 = Char32To2Char8sUncheckedLead2(char32.Value);
      cont1 = Char32To2Char8sUncheckedCont1(char32.Value);
      return Char32Is2Char8s(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 3-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="lead3"/>, <paramref name="cont1"/>, <paramref name="cont2"/> contain the valid encoded UTF-8 sequence.
    /// Otherwise, there is no guarantee as to what are contained in <paramref name="lead3"/>, <paramref name="cont1"/>, <paramref name="cont2"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To3Char8s(Char32 char32, out byte lead3, out byte cont1, out byte cont2)
    {
      lead3 = Char32To3Char8sUncheckedLead3(char32.Value);
      cont1 = Char32To3Char8sUncheckedCont1(char32.Value);
      cont2 = Char32To3Char8sUncheckedCont2(char32.Value);
      return Char32Is3Char8s(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 4-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="lead4"/>, <paramref name="cont1"/>, <paramref name="cont2"/>, <paramref name="cont3"/> contain the valid encoded UTF-8 sequence.
    /// Otherwise, there is no guarantee as to what are contained in <paramref name="lead4"/>, <paramref name="cont1"/>, <paramref name="cont2"/>, <paramref name="cont3"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To4Char8s(Char32 char32, out byte lead4, out byte cont1, out byte cont2, out byte cont3)
    {
      lead4 = Char32To4Char8sUncheckedLead4(char32.Value);
      cont1 = Char32To4Char8sUncheckedCont1(char32.Value);
      cont2 = Char32To4Char8sUncheckedCont2(char32.Value);
      cont3 = Char32To4Char8sUncheckedCont3(char32.Value);
      return Char32Is4Char8s(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 1-unit UTF-16 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="char16"/> contains the valid encoded UTF-16 sequence.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char16"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To1Char16(Char32 char32, out char char16)
    {
      char16 = Char32To1Char16Unchecked(char32.Value);
      return Char32Is1Char16(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 2-unit UTF-16 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="high"/>, <paramref name="low"/> contain the valid encoded UTF-16 sequence.
    /// Otherwise, there is no guarantee as to what are contained in <paramref name="high"/>, <paramref name="low"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To2Char16s(Char32 char32, out char high, out char low)
    {
      int prepared = Char32To2Char16sUncheckedPrepare(char32.Value);
      high = Char32To1Char16Unchecked(prepared);
      low = Char32To1Char16Unchecked(prepared);
      return Char32Is2Char16s(char32.Value);
    }

    #endregion public methods for decoding or encoding 1 code point

    internal const int ReplacementCharacter32 = 0xFFFD;
    internal const char ReplacementCharacter16 = (char)0xFFFD;
    internal const byte ReplacementCharacter8Lead3 = 0xEF;
    internal const byte ReplacementCharacter8Cont1 = 0xBF;
    internal const byte ReplacementCharacter8Cont2 = 0xBD;
    internal const int MaximumLength32 = 0x7FEFFFFF;
    internal const int MaximumLength16 = 0x7FEFFFFF;
    internal const int MaximumLength8 = 0x7FFFFFC7;
    internal const string String8WouldBeTooLong = "The string in UTF-8 would be too long.";
    internal const string String16WouldBeTooLong = "The string in UTF-16 would be too long.";
    internal const string String32WouldBeTooLong = "The string in UTF-32 would be too long.";
    internal const string String8IsNotValid = "The string in UTF-8 is not valid.";
    internal const string String16IsNotValid = "The string in UTF-16 is not valid.";
    internal const string String32IsNotValid = "The string in UTF-32 is not valid.";

    #region generic property determination for Char8

    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of byte explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char8Leads1(byte value)
    {
      return (uint)value < 0x80u;
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
    internal static bool Char16IsNotSurrogate(char value)
    {
      return (value & 0xFFFFF800) != 0xD800;
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

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char16IsNotLowSurrogate(char value)
    {
      return (value & 0xFFFFFC00) != 0xDC00;
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

    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsAbove0x10000AndBelow0x110000(int value)
    {
      return (uint)(value - 0x10000) < (uint)(0x110000 - 0x10000);
    }

    #endregion range check for Char32

    #region generic property determination for Char32

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsValid(int value)
    {
      return (uint)value < 0x110000u && (value & 0xFFFFF800) != 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsNotValid(int value)
    {
      return (uint)value >= 0x110000u || (value & 0xFFFFF800) == 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsSurrogate(int value)
    {
      return (value & 0xFFFFF800) == 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32IsNotSurrogate(int value)
    {
      return (value & 0xFFFFF800) != 0xD800;
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

    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is2Char8s(int value)
    {
      return (uint)(value - 0x80) < (uint)(0x800 - 0x80);
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is3Char8s(int value)
    {
      return (uint)(value - 0x800) < (uint)(0x10000 - 0x800) && (value & 0xFFFFF800) != 0xD800;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is4Char8s(int value)
    {
      return (uint)(value - 0x10000) < (uint)(0x110000 - 0x10000);
    }

    [MethodImpl(Helper.JustOptimize)]
    internal static int Char32LengthInChar8s(int value)
    {
      if (Char32IsBelow0x80(value))
      {
        return 1;
      }
      if (Char32IsBelow0x800(value))
      {
        return 2;
      }
      if (Char32IsBelow0x10000(value))
      {
        if (Char32IsNotSurrogate(value))
        {
          return 3;
        }
        goto Invalid;
      }
      if (Char32IsBelow0x110000(value))
      {
        return 4;
      }
    Invalid:
      return -1;
    }

    #endregion generic encoding length counting for Char32 in Char8s

    #region generic encoding length counting for Char32 in Char16s

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is1Char16(int value)
    {
      return (uint)value < 0x10000u && (value & 0xFFFFF800) != 0xD800;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32Is2Char16s(int value)
    {
      return (uint)(value - 0x10000) < (uint)(0x110000 - 0x10000);
    }

    [MethodImpl(Helper.JustOptimize)]
    internal static int Char32LengthInChar16s(int value)
    {
      if (Char32IsBelow0x10000(value))
      {
        if (Char32IsNotSurrogate(value))
        {
          return 1;
        }
        goto Invalid;
      }
      if (Char32IsBelow0x110000(value))
      {
        return 2;
      }
    Invalid:
      return -1;
    }

    #endregion generic encoding length counting for Char32 in Char16s

    #region Char32 to Char8

    [MethodImpl(Helper.OptimizeInline)]
    internal static byte Char32To1Char8UncheckedLead1(int value)
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
    internal static char Char32PreparedTo2Char16sUncheckedHigh(int prepared)
    {
      return (char)((prepared >> 10) | 0xD800);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static char Char32PreparedTo2Char16sUncheckedLow(int prepared)
    {
      return (char)((prepared & 0x3FF) | 0xDC00);
    }

    #endregion Char32 to Char16

    #region validity of Char32 from Char8 (overlong, surrogate, above 0x10FFFF)

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32From2Char8sIsValid(int value)
    {
      return 0x80u <= (uint)value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32From3Char8sIsValid(int value)
    {
      return 0x800u <= (uint)value && (value & 0xFFFFF800) != 0xD800;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    internal static bool Char32From4Char8sIsValid(int value)
    {
      return (uint)(value - 0x10000) < (uint)(0x110000 - 0x10000);
    }

    #endregion validity of Char32 from Char8 (overlong, surrogate, above 0x10FFFF)

    #region String8 to String8

    /// <summary>
    /// Finds the first invalid <see cref="Char8"/> instance.
    /// This method returns <c>-1</c> if the stream is valid.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static int FindFirstInvalidChar8(ref byte src0, int src8s)
    {
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
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
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        return i;
      Valid2:
      Valid3:
      Valid4:
        i = j;
      }
      return -1;
    }

    /// <summary>
    /// Counts the number of invalid <see cref="Char8"/> instances.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static int CountInvalidChar8s(ref byte src0, int src8s)
    {
      int invalids = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
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
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        ++invalids;
        continue;
      Valid2:
      Valid3:
      Valid4:
        i = j;
      }
      return invalids;
    }

    /// <summary>
    /// Replaces invalid <see cref="Char8"/> instances by the UTF-8 encoding of the replacement character.
    /// Each invalid instance becomes 3 valid instances.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static void SanitizeChar8s(ref byte src0, int src8s, ref byte dst0, int dst8s)
    {
      int k = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j = 0; i != src8s && k != dst8s; j = ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          Unsafe.Add(ref dst0, k++) = lead;
          continue;
        }
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
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
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        lead = ReplacementCharacter8Lead3;
        cont1 = ReplacementCharacter8Cont1;
        cont2 = ReplacementCharacter8Cont2;
        goto IsReplacement;
      Valid2:
        i = j;
        if (dst8s == k + 1)
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = lead;
        Unsafe.Add(ref dst0, k++) = cont1;
        continue;
      Valid3:
        i = j;
      IsReplacement:
        if ((uint)dst8s <= (uint)(k + 2))
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = lead;
        Unsafe.Add(ref dst0, k++) = cont1;
        Unsafe.Add(ref dst0, k++) = cont2;
        continue;
      Valid4:
        i = j;
        if ((uint)dst8s <= (uint)(k + 3))
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = lead;
        Unsafe.Add(ref dst0, k++) = cont1;
        Unsafe.Add(ref dst0, k++) = cont2;
        Unsafe.Add(ref dst0, k++) = cont3;
        continue;
      }
      while (k != dst8s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String8 to String8

    #region String16 to String16

    /// <summary>
    /// Finds the first invalid <see langword="char"/> instance.
    /// This method returns <c>-1</c> if the stream is valid.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static int FindFirstInvalidChar16(ref char src0, int src16s)
    {
      char first;
      for (int i = 0; i != src16s; ++i)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(Unsafe.Add(ref src0, i)))
          {
            /* Valid2 */
            continue;
          }
          /* InvalidDecrease */
          return --i;
        }
        /* Invalid */
        return i;
      }
      return -1;
    }

    /// <summary>
    /// Counts the number of invalid <see langword="char"/> instances.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static int CountInvalidChar16s(ref char src0, int src16s)
    {
      int invalids = 0;
      char first;
      for (int i = 0; i != src16s; ++i)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(Unsafe.Add(ref src0, i)))
          {
            /* Valid2 */
            continue;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        ++invalids;
      }
      return invalids;
    }

    /// <summary>
    /// Replaces invalid <see langword="char"/> instances by the UTF-16 encoding of the replacement character.
    /// Each invalid instance becomes 1 valid instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static void SanitizeChar16s(ref char src0, int src16s, ref char dst0, int dst16s)
    {
      int k = 0;
      char first, low;
      for (int i = 0; i != src16s && k != dst16s; ++i)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          goto Valid1;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(low = Unsafe.Add(ref src0, i)))
          {
            goto Valid2;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        first = ReplacementCharacter16;
      Valid1:
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
      }
      while (k != dst16s)
      {
        Unsafe.Add(ref dst0, k++) = (char)0;
      }
    }

    #endregion String16 to String16

    #region String32 to String32

    /// <summary>
    /// Finds the first invalid <see cref="Char32"/> instance.
    /// This method returns <c>-1</c> if the stream is valid.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static int FindFirstInvalidChar32(ref int src0, int src32s)
    {
      for (int i = 0; i != src32s; ++i)
      {
        if (Char32IsNotValid(Unsafe.Add(ref src0, i)))
        {
          return i;
        }
      }
      return -1;
    }

    /// <summary>
    /// Counts the number of invalid <see cref="Char32"/> instances.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static int CountInvalidChar32s(ref int src0, int src32s)
    {
      int invalids = 0;
      for (int i = 0; i != src32s; ++i)
      {
        if (Char32IsNotValid(Unsafe.Add(ref src0, i)))
        {
          ++invalids;
        }
      }
      return invalids;
    }

    /// <summary>
    /// Replaces invalid <see langword="Char32"/> instances by the UTF-32 encoding of the replacement character.
    /// Each invalid instance becomes 1 valid instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static void SanitizeChar32s(ref int src0, int src32s, ref int dst0, int dst32s)
    {
      int k = 0;
      for (int i = 0, value; i != src32s && k != dst32s; ++i, ++k)
      {
        Unsafe.Add(ref dst0, k) = Char32IsValid(value = Unsafe.Add(ref src0, i))
          ? value
          : ReplacementCharacter32;
      }
      while (k != dst32s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String32 to String32

    #region String8 to String32

    /// <summary>
    /// Given UTF-8, computes UTF-32 length.
    /// Returns <see langword="true"/> if and only if UTF-8 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see cref="Char32"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see cref="Char8"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static bool String8ToString32CountStrict(ref byte src0, int src8s, out int countIndex)
    {
      int dst32s = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i, ++dst32s)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
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
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        countIndex = i;
        return false;
      Valid2:
      Valid3:
      Valid4:
        i = j;
      }
      countIndex = dst32s;
      return true;
    }

    /// <summary>
    /// Given UTF-8, computes UTF-32 length, with invalid <see cref="Char8"/> instances replaced by the UTF-32 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static int String8ToString32CountReplace(ref byte src0, int src8s)
    {
      int dst32s = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i, ++dst32s)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          /* Invalid */
          continue;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          /* Invalid */
          continue;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          /* Invalid */
          continue;
        }
        /* Invalid */
        continue;
      Valid2:
      Valid3:
      Valid4:
        i = j;
      }
      return dst32s;
    }

    /// <summary>
    /// Transforms UTF-8 to UTF-32, with invalid <see cref="Char8"/> instances replaced by the UTF-32 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst32s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static void String8ToString32Transform(ref byte src0, int src8s, ref int dst0, int dst32s)
    {
      int k = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j, value; i != src8s && k != dst32s; ++i, ++k)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          Unsafe.Add(ref dst0, k) = Char8ToChar32Unchecked1(lead);
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(value = Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(value = Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
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
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        Unsafe.Add(ref dst0, k) = ReplacementCharacter32;
        continue;
      Valid2:
      Valid3:
      Valid4:
        i = j;
        Unsafe.Add(ref dst0, k) = value;
        continue;
      }
      while (k != dst32s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String8 to String32

    #region String16 to String32

    /// <summary>
    /// Given UTF-16, computes UTF-32 length.
    /// Returns <see langword="true"/> if and only if UTF-16 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see cref="Char32"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see langword="char"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static bool String16ToString32CountStrict(ref char src0, int src16s, out int countIndex)
    {
      int dst32s = 0;
      char first;
      for (int i = 0; i != src16s; ++i, ++dst32s)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(Unsafe.Add(ref src0, i)))
          {
            /* Valid2 */
            continue;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        countIndex = i;
        return false;
      }
      countIndex = dst32s;
      return true;
    }

    /// <summary>
    /// Given UTF-16, computes UTF-32 length, with invalid <see langword="char"/> instances replaced by the UTF-32 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static int String16ToString32CountReplace(ref char src0, int src16s)
    {
      int dst32s = 0;
      for (int i = 0; i != src16s; ++i, ++dst32s)
      {
        if (Char16IsHighSurrogate(Unsafe.Add(ref src0, i))
          && (++i == src16s || Char16IsNotLowSurrogate(Unsafe.Add(ref src0, i))))
        {
          /* InvalidDecrease */
          --i;
        }
        /* Valid1, Valid2, Invalid (including falling through from InvalidDecrease) */
      }
      return dst32s;
    }

    /// <summary>
    /// Transforms UTF-16 to UTF-32, with invalid <see langword="char"/> instances replaced by the UTF-32 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst32s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static void String16ToString32Transform(ref char src0, int src16s, ref int dst0, int dst32s)
    {
      int k = 0;
      char first, low;
      for (int i = 0; i != src16s && k != dst32s; ++i, ++k)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          goto Valid1;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(low = Unsafe.Add(ref src0, i)))
          {
            goto Valid2;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        first = ReplacementCharacter16;
      Valid1:
        Unsafe.Add(ref dst0, k) = Char16ToChar32Unchecked1(first);
        continue;
      Valid2:
        Unsafe.Add(ref dst0, k) = Char16ToChar32Unchecked2(first, low);
        continue;
      }
      while (k != dst32s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String16 to String32

    #region String32 to String8

    /// <summary>
    /// Given UTF-32, computes UTF-8 length.
    /// Returns <see langword="true"/> if and only if UTF-32 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see cref="Char8"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see cref="Char32"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static bool String32ToString8CountStrict(ref int src0, int src32s, out long countIndex)
    {
      int valid2s = 0, valid3s = 0, valid4s = 0;
      for (int i = 0, value; i != src32s; ++i)
      {
        if (Char32IsBelow0x80(value = Unsafe.Add(ref src0, i)))
        {
          continue;
        }
        if (Char32IsBelow0x800(value))
        {
          ++valid2s;
          continue;
        }
        if (Char32IsBelow0x10000(value))
        {
          if (Char32IsNotSurrogate(value))
          {
            ++valid3s;
            continue;
          }
          goto Invalid;
        }
        if (Char32IsBelow0x110000(value))
        {
          ++valid4s;
          continue;
        }
      Invalid:
        countIndex = i;
        return false;
      }
      countIndex = src32s + (long)valid2s + valid3s * 2L + valid4s * 3L;
      return true;
    }

    /// <summary>
    /// Given UTF-32, computes UTF-8 length, with invalid <see cref="Char32"/> instances replaced by the UTF-8 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static long String32ToString8CountReplace(ref int src0, int src32s)
    {
      int valid2s = 0, valid3invalids = 0, valid4s = 0;
      for (int i = 0, value; i != src32s; ++i)
      {
        if (Char32IsBelow0x80(value = Unsafe.Add(ref src0, i)))
        {
          continue;
        }
        if (Char32IsBelow0x800(value))
        {
          ++valid2s;
          continue;
        }
        if (Char32IsAbove0x10000AndBelow0x110000(value))
        {
          ++valid4s;
          continue;
        }
        ++valid3invalids;
        continue;
      }
      return src32s + (long)valid2s + valid3invalids * 2L + valid4s * 3L;
    }

    /// <summary>
    /// Transforms UTF-32 to UTF-8, with invalid <see cref="Char32"/> instances replaced by the UTF-8 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst8s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static void String32ToString8Transform(ref int src0, int src32s, ref byte dst0, int dst8s)
    {
      int k = 0;
      for (int i = 0, value; i != src32s && k != dst8s; ++i)
      {
        if (Char32IsBelow0x80(value = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          Unsafe.Add(ref dst0, k++) = Char32To1Char8UncheckedLead1(value);
          continue;
        }
        if (Char32IsBelow0x800(value))
        {
          goto Valid2;
        }
        if (Char32IsBelow0x10000(value))
        {
          if (Char32IsNotSurrogate(value))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char32IsBelow0x110000(value))
        {
          goto Valid4;
        }
      Invalid:
        value = ReplacementCharacter32;
        goto Valid3;
      Valid2:
        if (dst8s == k + 1)
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = Char32To2Char8sUncheckedLead2(value);
        Unsafe.Add(ref dst0, k++) = Char32To2Char8sUncheckedCont1(value);
        continue;
      Valid3:
        if ((uint)dst8s <= (uint)(k + 2))
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedLead3(value);
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedCont1(value);
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedCont2(value);
        continue;
      Valid4:
        if ((uint)dst8s <= (uint)(k + 3))
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

    #endregion String32 to String8

    #region String32 to String16

    /// <summary>
    /// Given UTF-32, computes UTF-16 length.
    /// Returns <see langword="true"/> if and only if UTF-32 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see langword="char"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see cref="Char32"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static bool String32ToString16CountStrict(ref int src0, int src32s, out long countIndex)
    {
      int dst16sMoreThanSrc32s = 0;
      for (int i = 0, value; i != src32s; ++i)
      {
        if (Char32IsBelow0x10000(value = Unsafe.Add(ref src0, i)))
        {
          if (Char32IsNotSurrogate(value))
          {
            /* Valid1 */
            continue;
          }
          goto Invalid;
        }
        if (Char32IsBelow0x110000(value))
        {
          /* Valid2 */
          ++dst16sMoreThanSrc32s;
          continue;
        }
      Invalid:
        countIndex = i;
        return false;
      }
      countIndex = (long)src32s + dst16sMoreThanSrc32s;
      return true;
    }

    /// <summary>
    /// Given UTF-32, computes UTF-16 length, with invalid <see cref="Char32"/> instances replaced by the UTF-16 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static long String32ToString16CountReplace(ref int src0, int src32s)
    {
      int dst16sMoreThanSrc32s = 0;
      for (int i = 0; i != src32s; ++i)
      {
        if (Char32Is2Char16s(Unsafe.Add(ref src0, i)))
        {
          /* Valid2 */
          ++dst16sMoreThanSrc32s;
        }
        /* Valid1, Invalid */
      }
      return (long)src32s + dst16sMoreThanSrc32s;
    }

    /// <summary>
    /// Transforms UTF-32 to UTF-16, with invalid <see cref="Char32"/> instances replaced by the UTF-16 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst16s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static void String32ToString16Transform(ref int src0, int src32s, ref char dst0, int dst16s)
    {
      int k = 0;
      for (int i = 0, value; i != src32s && k != dst16s; ++i)
      {
        if (Char32IsBelow0x10000(value = Unsafe.Add(ref src0, i)))
        {
          if (Char32IsNotSurrogate(value))
          {
            goto Valid1;
          }
          goto Invalid;
        }
        if (Char32IsBelow0x110000(value))
        {
          goto Valid2;
        }
      Invalid:
        value = ReplacementCharacter16;
      Valid1:
        Unsafe.Add(ref dst0, k++) = Char32To1Char16Unchecked(value);
        continue;
      Valid2:
        if (dst16s == k + 1)
        {
          break;
        }
        value = Char32To2Char16sUncheckedPrepare(value);
        Unsafe.Add(ref dst0, k++) = Char32PreparedTo2Char16sUncheckedHigh(value);
        Unsafe.Add(ref dst0, k++) = Char32PreparedTo2Char16sUncheckedLow(value);
        continue;
      }
      while (k != dst16s)
      {
        Unsafe.Add(ref dst0, k++) = (char)0;
      }
    }

    #endregion String32 to String16

    #region String8 to String16

    /// <summary>
    /// Given UTF-8, computes UTF-16 length.
    /// Returns <see langword="true"/> if and only if UTF-8 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see langword="char"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see cref="Char8"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static bool String8ToString16CountStrict(ref byte src0, int src8s, out int countIndex)
    {
      int dst16s = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i, ++dst16s)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
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
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        countIndex = i;
        return false;
      Valid4:
        ++dst16s;
      Valid2:
      Valid3:
        i = j;
        continue;
      }
      countIndex = dst16s;
      return true;
    }

    /// <summary>
    /// Given UTF-8, computes UTF-16 length, with invalid <see cref="Char8"/> instances replaced by the UTF-16 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static int String8ToString16CountReplace(ref byte src0, int src8s)
    {
      int dst16s = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i, ++dst16s)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
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
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        continue;
      Valid4:
        ++dst16s;
      Valid2:
      Valid3:
        i = j;
        continue;
      }
      return dst16s;
    }

    /// <summary>
    /// Transforms UTF-8 to UTF-16, with invalid <see cref="Char8"/> instances replaced by the UTF-16 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst16s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static void String8ToString16Transform(ref byte src0, int src8s, ref char dst0, int dst16s)
    {
      int k = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j, value; i != src8s && k != dst16s; ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          Unsafe.Add(ref dst0, k++) = Char32To1Char16Unchecked(Char8ToChar32Unchecked1(lead));
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(value = Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(value = Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
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
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        Unsafe.Add(ref dst0, k++) = ReplacementCharacter16;
        continue;
      Valid2:
      Valid3:
        i = j;
        Unsafe.Add(ref dst0, k++) = Char32To1Char16Unchecked(value);
        continue;
      Valid4:
        if (dst16s == k + 1)
        {
          break;
        }
        i = j;
        value = Char32To2Char16sUncheckedPrepare(value);
        Unsafe.Add(ref dst0, k++) = Char32PreparedTo2Char16sUncheckedHigh(value);
        Unsafe.Add(ref dst0, k++) = Char32PreparedTo2Char16sUncheckedLow(value);
        continue;
      }
      while (k != dst16s)
      {
        Unsafe.Add(ref dst0, k++) = (char)0;
      }
    }

    #endregion String8 to String16

    #region String16 to String8

    /// <summary>
    /// Given UTF-16, computes UTF-8 length.
    /// Returns <see langword="true"/> if and only if UTF-16 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see cref="Char8"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see langword="char"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static bool String16ToString8CountStrict(ref char src0, int src16s, out long countIndex)
    {
      throw new System.NotImplementedException();
    }

    /// <summary>
    /// Given UTF-16, computes UTF-8 length, with invalid <see langword="char"/> instances replaced by the UTF-8 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static long String16ToString8CountReplace(ref char src0, int src16s)
    {
      throw new System.NotImplementedException();
    }

    /// <summary>
    /// Transforms UTF-16 to UTF-8, with invalid <see langword="char"/> instances replaced by the UTF-8 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst8s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    internal static void String16ToString8Transform(ref char src0, int src16s, ref byte dst0, int dst8s)
    {
      throw new System.NotImplementedException();
    }

    #endregion String16 to String8
  }
}
