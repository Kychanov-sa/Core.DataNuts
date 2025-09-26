using System.IO.Hashing;
using System.Runtime.InteropServices;
using System.Text;

namespace GlacialBytes.Core.DataNuts.IO
{
  /// <summary>
  /// Построитель ореха данных.
  /// </summary>
  public sealed class DataNutBuilder
  {
    /// <summary>
    /// Хэш.
    /// </summary>
    private readonly XxHash64 _hasher = new ();

    /// <summary>
    /// Данные ядра.
    /// </summary>
    private byte[] _coreData = [];

    /// <summary>
    /// Тип данных ядра.
    /// </summary>
    private DataNutCoreType _coreType = DataNutCoreType.GenericBinary;

    /// <summary>
    /// Хэш данных ядра.
    /// </summary>
    private ulong _coreHash = 0L;

    /// <summary>
    /// Создаёт построитель из потока.
    /// </summary>
    /// <param name="stream">Поток данных ядра.</param>
    /// <returns>Построитель ореха данных.</returns>
    public static DataNutBuilder FromStream(Stream stream)
    {
      ArgumentNullException.ThrowIfNull(stream, nameof(stream));

      var data = new Span<byte>();
      stream.Read(data);
      if (data.Length == 0)
        throw new InvalidOperationException("Empty core is not allowed while building data nut.");

      var builder = new DataNutBuilder();
      builder.SetDataCore(data, DataNutCoreType.GenericBinary);
      return builder;
    }

    public static DataNutBuilder FromText(string text)
    {
      ArgumentException.ThrowIfNullOrEmpty(text, nameof(text));

      var builder = new DataNutBuilder();
      builder.SetDataCore(Encoding.UTF8.GetBytes(text), DataNutCoreType.Utf8Text);
      return builder;
    }

    public static DataNutBuilder FromFile(string fileName)
    {
      ArgumentException.ThrowIfNullOrEmpty(fileName, nameof(fileName));

      var data = File.ReadAllBytes(fileName);
      if (data.Length == 0)
        throw new InvalidOperationException("Empty core is not allowed while building data nut.");

      var builder = new DataNutBuilder();
      builder.SetDataCore(data, DataNutCoreType.GenericBinary);
      return builder;
    }

    public static DataNutBuilder FromMemory(byte[] buffer)
    {
      ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));
      if (buffer.Length == 0)
        throw new InvalidOperationException("Empty core is not allowed while building data nut.");

      var builder = new DataNutBuilder();
      builder.SetDataCore(buffer, DataNutCoreType.GenericBinary);
      return builder;
    }

    public static DataNutBuilder FromMemory(Span<byte> span)
    {
      if (span.Length == 0)
        throw new InvalidOperationException("Empty core is not allowed while building data nut.");

      var builder = new DataNutBuilder();
      builder.SetDataCore(span, DataNutCoreType.GenericBinary);
      return builder;
    }

    /// <summary>
    /// Устанавливает ядро данных
    /// </summary>
    /// <param name="coreData">Ядро данных.</param>
    /// <param name="coreType">Тип ядра.</param>
    /// <returns>Экземпляр построителя.</returns>
    private void SetDataCore(ReadOnlySpan<byte> coreData, DataNutCoreType coreType)
    {
      _coreData = coreData.ToArray();
      _coreType = coreType;
      
      _hasher.Reset();
      _hasher.Append(coreData);
      _coreHash = _hasher.GetCurrentHashAsUInt64();
    }

    /// <summary>
    /// Выполняет построение.
    /// </summary>
    /// <returns>Поток с записанным орехом данных.</returns>
    public DataNut Build()
    {
      int headerSize = Marshal.SizeOf<DataNutHeader>();
      var stream = new MemoryStream(_coreData.Length + headerSize);

      var header = CreateHeader((uint)headerSize);
      Span<byte> headerBuffer = stackalloc byte[headerSize];
      MemoryMarshal.Write(headerBuffer, in header);

      stream.Write(headerBuffer);
      stream.Write(_coreData);
      return new DataNut()
      {
        Id = header.Id,
        Version = (DataNutVersion) header.Version,
        Created = new DateTime(header.Created),
        CoreHash = _coreHash,
        CoreOffset = headerSize,
        CoreSize = _coreData.Length,
        CoreType = _coreType,
        NutStream = stream,
      };
    }

    /// <summary>
    /// Создаёт заколовок ореха.
    /// </summary>
    /// <param name="coreOffset">Смещение данных ядра.</param>
    /// <returns>Заполненный заголовок.</returns>
    private DataNutHeader CreateHeader(uint coreOffset)
    {
      return new DataNutHeader()
      {
        Signature = DataNutHeader.DataNutSignature,
        Version = (ushort)DataNutVersion.Version_1_0,
        CoreSize = Convert.ToUInt32(_coreData.Length),
        CoreType = (uint)_coreType,
        CoreHash = _coreHash,
        Created = DateTime.UtcNow.Ticks,
        CoreOffset = coreOffset,
      };
    }
  }
}
