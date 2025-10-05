using System.Runtime.InteropServices;

namespace GlacialBytes.Core.DataNuts;


/// <summary>
/// Структура для кодирования четырёх символов расширения имени файла в двойном слове.
/// </summary>
/// <remarks>
/// Конструктор.
/// </remarks>
/// <param name="value">Инициализирующее значение.</param>
[StructLayout(LayoutKind.Sequential)]
public readonly struct FourCC(int value) : IEquatable<FourCC>
{
  /// <summary>
  /// Целочисленное значение.
  /// </summary>
  public readonly int Value = value;

  /// <summary>
  /// Создаёт экземпляр структуры из четырёх символов.
  /// </summary>
  /// <param name="c1">Символ 1.</param>
  /// <param name="c2">Символ 2.</param>
  /// <param name="c3">Символ 3.</param>
  /// <param name="c4">Символ 4.</param>
  /// <returns>Экземпляр структуры FourCC.</returns>
  public static FourCC FromFourChars(char c1, char c2, char c3, char c4)
  {
    int value = (int)c1 << 24 | (int)c2 << 16 | (int)c3 << 8 | (int)c4;
    return new FourCC(value);
  }

  /// <summary>
  /// Создаёт  экземпляр структуры из строки длинной четыре символа
  /// </summary>
  /// <param name="value">Строковое значение.</param>
  /// <returns>Экземпляр структуры FourCC.</returns>
  /// <exception cref="ArgumentNullException">Значение value не должно быть null.</exception>
  /// <exception cref="ArgumentException">Значение длины строки value не должно превышать 4.</exception>
  public static FourCC FromString(string value)
  {
    ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));
    if (value.Length > 4)
      throw new ArgumentException("value.Length > 4", nameof(value));

    if (value.Length < 4)
      value = value.PadRight(4, ' ');

    return FourCC.FromFourChars(value[0], value[1], value[2], value[3]);
  }

  /// <summary>
  /// implicit cast to int
  /// </summary>
  /// <param name="fourcc"></param>
  public static implicit operator int(FourCC fourcc)
  {
    return fourcc.Value;
  }

  /// <summary>
  /// Преобразование в Int32.
  /// </summary>
  /// <returns>Целочисленное значение.</returns>
  public int ToInt32()
  {
    return Value;
  }

  /// <summary>
  /// Неявное преобразование из Int32.
  /// </summary>
  /// <param name="value">Целочисленное значение.</param>
  public static implicit operator FourCC(int value)
  {
    return new FourCC(value);
  }

  /// <summary>
  /// Преобразование из Int32.
  /// </summary>
  /// <param name="value">Целочисленное значение.</param>
  /// <returns>Экземпляр структуры FourCC.</returns>
  public static FourCC FromInt32(int value)
  {
    return new FourCC(value);
  }

  /// <summary>
  /// Оператор равенства.
  /// </summary>
  /// <param name="left">Левый операнд.</param>
  /// <param name="right">Правый операнд.</param>
  /// <returns>Результат сравнения.</returns>
  public static bool operator ==(FourCC left, FourCC right)
  {
    return left.Equals(right);
  }

  /// <summary>
  /// Оператор неравенства.
  /// </summary>
  /// <param name="left">Левый операнд.</param>
  /// <param name="right">Правый операнд.</param>
  /// <returns>Результат сравнения.</returns>
  public static bool operator !=(FourCC left, FourCC right)
  {
    return !(left == right);
  }

  #region Object

  /// <inheritdoc />
  public override bool Equals(object? obj)
  {
    if (obj is FourCC enumValue)
      return Equals(enumValue);
    return false;
  }

  /// <inheritdoc />
  public override int GetHashCode()
  {
    return Value.GetHashCode();
  }

  #endregion

  #region IEquatable


#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
  /// <summary>
  /// <see cref="IEquatable{T}.Equals(T?)"/>
  /// </summary>
  public bool Equals(FourCC other)
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
  {
    return Value == other.Value;
  }

  #endregion

  
}
