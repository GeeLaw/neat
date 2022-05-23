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
      return ((lead3 & 0x1F) << 12) | ((cont1 & 0x3F) << 6) | (cont2 & 0x3F);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static int Char8ToChar32Unchecked4(byte lead4, byte cont1, byte cont2, byte cont3)
    {
      return ((lead4 & 0x1F) << 18) | ((cont1 & 0x3F) << 12) | ((cont2 & 0x3F) << 6) | (cont3 & 0x3F);
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
    internal static byte Char32To1Char8Unchecked(int value)
    {
      return (byte)value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static void Char32To1Char8Unchecked(int value, out byte lead1)
    {
      lead1 = (byte)value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static void Char32To2Char8sUnchecked(int value, out byte lead2, out byte cont1)
    {
      lead2 = (byte)((value >> 6) | 0xC0);
      cont1 = (byte)((value & 0x3F) | 0x80);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static void Char32To3Char8sUnchecked(int value, out byte lead3, out byte cont1, out byte cont2)
    {
      lead3 = (byte)((value >> 12) | 0xE0);
      cont1 = (byte)(((value >> 6) & 0x3F) | 0x80);
      cont2 = (byte)((value & 0x3F) | 0x80);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static void Char32To4Char8sUnchecked(int value, out byte lead4, out byte cont1, out byte cont2, out byte cont3)
    {
      lead4 = (byte)((value >> 18) | 0xF0);
      cont1 = (byte)(((value >> 12) & 0x3F) | 0x80);
      cont2 = (byte)(((value >> 6) & 0x3F) | 0x80);
      cont3 = (byte)((value & 0x3F) | 0x80);
    }

    #endregion Char32 to Char8

    #region Char32 to Char16

    [MethodImpl(Helper.OptimizeInline)]
    internal static char Char32To1Char16Unchecked(int value)
    {
      return (char)value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static void Char32To1Char16Unchecked(int value, out char ch)
    {
      ch = (char)value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static void Char32To2Char16sUnchecked(int value, out char high, out char low)
    {
      value -= 0x10000;
      high = (char)((value >> 10) | 0xD800);
      low = (char)((value & 0x3FF) | 0xDC00);
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
  }
}
