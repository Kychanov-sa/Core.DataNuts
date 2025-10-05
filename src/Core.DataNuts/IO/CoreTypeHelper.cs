namespace GlacialBytes.Core.DataNuts.IO;

/// <summary>
/// Вспомогательный класс для работы с типами ядра.
/// </summary>
internal static class CoreTypeHelper
{
  /// <summary>
  /// Возвращает тип ядра по расширению файла.
  /// </summary>
  /// <param name="extension">Расширение файла.</param>
  /// <returns>Соответствующий расширению тип ядра.</returns>
  public static DataNutCoreType FromFileExtension(string extension)
  {
    if (String.IsNullOrEmpty(extension))
      return DataNutCoreType.GenericBinary;

    if (extension.StartsWith('.'))
      extension = extension[1..];
    if (extension.Length > 4)
      extension = extension[..4];
    var fourCC = FourCC.FromString(extension);

    return (DataNutCoreType)fourCC.Value;
  }
}
