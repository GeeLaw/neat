using System.Runtime.CompilerServices;

namespace Neat.Unicode
{
  /// <summary>
  /// Provides type-safe access to UTF.
  /// </summary>
  public static class Utf
  {
    #region decoding or encoding 1 code point

    /// <summary>
    /// Tries to decode a 1-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try1Char8ToChar32(Char8 lead1, out Char32 char32)
    {
      char32 = new Char32(UtfUnsafe.Char8ToChar32Unchecked1(lead1.Value));
      return UtfUnsafe.Char8Leads1(lead1.Value);
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
      int value = UtfUnsafe.Char8ToChar32Unchecked2(lead2.Value, cont1.Value);
      char32 = new Char32(value);
      return UtfUnsafe.Char8Leads2(lead2.Value)
        && UtfUnsafe.Char8Continues(cont1.Value)
        && UtfUnsafe.Char32From2Char8sIsValid(value);
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
      int value = UtfUnsafe.Char8ToChar32Unchecked3(lead3.Value, cont1.Value, cont2.Value);
      char32 = new Char32(value);
      return UtfUnsafe.Char8Leads3(lead3.Value)
        && UtfUnsafe.Char8Continues(cont1.Value)
        && UtfUnsafe.Char8Continues(cont2.Value)
        && UtfUnsafe.Char32From3Char8sIsValid(value);
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
      int value = UtfUnsafe.Char8ToChar32Unchecked4(lead4.Value, cont1.Value, cont2.Value, cont3.Value);
      char32 = new Char32(value);
      return UtfUnsafe.Char8Leads4(lead4.Value)
        && UtfUnsafe.Char8Continues(cont1.Value)
        && UtfUnsafe.Char8Continues(cont2.Value)
        && UtfUnsafe.Char8Continues(cont3.Value)
        && UtfUnsafe.Char32From4Char8sIsValid(value);
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
      char32 = new Char32(UtfUnsafe.Char16ToChar32Unchecked1(char16));
      return UtfUnsafe.Char16IsNotSurrogate(char16);
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
      char32 = new Char32(UtfUnsafe.Char16ToChar32Unchecked2(high, low));
      return UtfUnsafe.Char16IsHighSurrogate(high) && UtfUnsafe.Char16IsLowSurrogate(low);
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
      lead1 = UtfUnsafe.Char32To1Char8UncheckedLead1(char32.Value);
      return UtfUnsafe.Char32Is1Char8(char32.Value);
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
      lead2 = UtfUnsafe.Char32To2Char8sUncheckedLead2(char32.Value);
      cont1 = UtfUnsafe.Char32To2Char8sUncheckedCont1(char32.Value);
      return UtfUnsafe.Char32Is2Char8s(char32.Value);
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
      lead3 = UtfUnsafe.Char32To3Char8sUncheckedLead3(char32.Value);
      cont1 = UtfUnsafe.Char32To3Char8sUncheckedCont1(char32.Value);
      cont2 = UtfUnsafe.Char32To3Char8sUncheckedCont2(char32.Value);
      return UtfUnsafe.Char32Is3Char8s(char32.Value);
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
      lead4 = UtfUnsafe.Char32To4Char8sUncheckedLead4(char32.Value);
      cont1 = UtfUnsafe.Char32To4Char8sUncheckedCont1(char32.Value);
      cont2 = UtfUnsafe.Char32To4Char8sUncheckedCont2(char32.Value);
      cont3 = UtfUnsafe.Char32To4Char8sUncheckedCont3(char32.Value);
      return UtfUnsafe.Char32Is4Char8s(char32.Value);
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
      char16 = UtfUnsafe.Char32To1Char16Unchecked(char32.Value);
      return UtfUnsafe.Char32Is1Char16(char32.Value);
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
      int prepared = UtfUnsafe.Char32To2Char16sUncheckedPrepare(char32.Value);
      high = UtfUnsafe.Char32To1Char16Unchecked(prepared);
      low = UtfUnsafe.Char32To1Char16Unchecked(prepared);
      return UtfUnsafe.Char32Is2Char16s(char32.Value);
    }

    #endregion decoding or encoding 1 code point
  }
}
