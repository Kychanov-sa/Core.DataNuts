namespace GlacialBytes.Core.DataNuts;

/// <summary>
/// Заголовок ореха данных.
/// </summary>
public struct DataNutHeader
{
  public UInt32 Signature;
  public UInt16 Version;
  public UInt64 CoreHash;
  public UInt32 CoreSize;
  public UInt16 CoreType;
}
