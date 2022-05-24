using System.Runtime.CompilerServices;

namespace Neat.Unicode
{
  /// <summary>
  /// Provides extension methods for <see langword="char"/>.
  /// </summary>
  public static class Char16
  {
    /// <summary>
    /// Returns <see langword="true"/> if and only if <paramref name="char16"/> is a surrogate.
    /// This means that <paramref name="char16"/> is between <c>0xD800</c> and <c>0xDFFF</c> (both inclusive).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsSurrogate(this char char16)
    {
      return Utf.Char16IsSurrogate(char16);
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if <paramref name="char16"/> is a high (leading) surrogate.
    /// This means that <paramref name="char16"/> is between <c>0xD800</c> and <c>0xDBFF</c> (both inclusive).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsHighSurrogate(this char char16)
    {
      return Utf.Char16IsHighSurrogate(char16);
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if <paramref name="char16"/> is a low (trailing) surrogate.
    /// This means that <paramref name="char16"/> is between <c>0xDC00</c> and <c>0xDFFF</c> (both inclusive).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsLowSurrogate(this char char16)
    {
      return Utf.Char16IsLowSurrogate(char16);
    }
  }
}