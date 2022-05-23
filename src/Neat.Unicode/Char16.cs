using System.Runtime.CompilerServices;

namespace Neat.Unicode
{
  /// <summary>
  /// Highly efficient operations of <see langword="char"/>.
  /// </summary>
  public static class Char16
  {
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsSurrogate(char value)
    {
      return (value & 0xF800) == 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsHighSurrogate(char value)
    {
      return (value & 0xFC00) == 0xD800;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsLowSurrogate(char value)
    {
      return (value & 0xFC00) == 0xDC00;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static int ToChar32Unchecked1(char value)
    {
      return value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static int ToChar32Unchecked2(char high, char low)
    {
      return (((high & 0x3FF) << 10) | (low & 0x3FF)) + 0x10000;
    }
  }
}
