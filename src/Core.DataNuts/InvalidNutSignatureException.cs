namespace GlacialBytes.Core.DataNuts;

/// <summary>
/// Подпись ореха некорректна.
/// </summary>
public class InvalidNutSignatureException : Exception
{
  /// <summary>
  /// Конструктор.
  /// </summary>
  public InvalidNutSignatureException()
    : base("Invalid data nut signature.")
  {
  }
}
