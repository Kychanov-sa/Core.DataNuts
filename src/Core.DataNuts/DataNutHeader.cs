using System.Runtime.InteropServices;

namespace GlacialBytes.Core.DataNuts;

/// <summary>
/// Заголовок ореха данных.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
public struct DataNutHeader
{
  /// <summary>
  /// Значение подписи ореха.
  /// </summary>
  public const UInt16 DataNutSignature = 0x4E_44;

  /// <summary>
  /// Подпись ореха.
  /// </summary>
  [FieldOffset(0)]
  public UInt16 Signature;

  /// <summary>
  /// Версия формата.
  /// </summary>
  [FieldOffset(2)]
  public UInt16 Version;

  /// <summary>
  /// Дата создания.
  /// </summary>
  [FieldOffset(4)]
  public Int64 Created;

  /// <summary>
  /// Идентификатор ореха.
  /// </summary>
  [FieldOffset(12)]
  public Guid Id;

  /// <summary>
  /// Хэш ядра.
  /// </summary>
  [FieldOffset(12)]
  public UInt64 CoreHash;

  /// <summary>
  /// Размер ядра.
  /// </summary>
  [FieldOffset(20)]
  public UInt32 CoreSize;

  /// <summary>
  /// Тип ядра.
  /// </summary>
  [FieldOffset(24)]
  public UInt32 CoreType;

  /// <summary>
  /// Смещение данных ядра.
  /// </summary>
  [FieldOffset(28)]
  public UInt32 CoreOffset;
}
