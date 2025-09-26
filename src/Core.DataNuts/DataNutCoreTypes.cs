namespace GlacialBytes.Core.DataNuts;

/// <summary>
/// Типы данных ядра ореха.
/// </summary>
public enum DataNutCoreType : UInt32
{
  /// <summary>
  /// Обобщённый бинарный тип.
  /// </summary>
  GenericBinary = 0x80000000,

  /// <summary>
  /// Обобщённый текстовый тип.
  /// </summary>
  GenericText = 0x80000001,

  /// <summary>
  /// Текст в кодировке UTF-8.
  /// </summary>
  Utf8Text = 0x80000002,
}
