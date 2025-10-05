namespace GlacialBytes.Core.DataNuts;

/// <summary>
/// Орех данных.
/// </summary>
public record DataNut
{
  /// <summary>
  /// Идентификатор.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Версия.
  /// </summary>
  public DataNutVersion Version { get; init; }

  /// <summary>
  /// Поток с данными ореха.
  /// </summary>
  public required Stream NutStream { get; init; }

  /// <summary>
  /// Дата создания.
  /// </summary>
  public DateTime Created { get; init; }

  /// <summary>
  /// Хэш данных ядра.
  /// </summary>
  public ulong CoreHash { get; init; }

  /// <summary>
  /// Тип данных ядра.
  /// </summary>
  public DataNutCoreType CoreType { get; init; }
  
  /// <summary>
  /// Смещение до ядра в потоке.
  /// </summary>
  public int CoreOffset { get; init; }

  /// <summary>
  /// Размер ядра данных.
  /// </summary>
  public int CoreSize { get; init; }

  /// <summary>
  /// Размер ядра данных.
  /// </summary>
  public required IReadOnlyDictionary<string, string> Metadata { get; init; }
}
