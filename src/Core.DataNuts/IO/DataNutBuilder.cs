using System.Collections.Specialized;
using System.IO;
using System.IO.Hashing;
using System.Runtime.InteropServices;
using System.Text;

namespace GlacialBytes.Core.DataNuts.IO;

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
  /// Метаданные.
  /// </summary>
  private Dictionary<string, string> _metadata = [];

  /// <summary>
  /// Длина метаданных.
  /// </summary>
  private int _metadataLength = 0;

  /// <summary>
  /// Создаёт построитель ореха из потока.
  /// </summary>
  /// <param name="stream">Поток данных ядра.</param>
  /// <returns>Построитель ореха данных.</returns>
  /// <exception cref="InvalidOperationException">Поток не содержит данных.</exception>
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

  /// <summary>
  /// Создаёт построитель ореха из строки текста.
  /// </summary>
  /// <param name="text">Строка текса данных ядра.</param>
  /// <returns>Построитель ореха данных.</returns>
  /// <exception cref="ArgumentNullException">Недопускается значение null для строки текста.</exception>
  /// <exception cref="ArgumentException">Недопускается пустая строка текста.</exception>
  public static DataNutBuilder FromText(string text)
  {
    ArgumentException.ThrowIfNullOrEmpty(text, nameof(text));

    var builder = new DataNutBuilder();
    builder.SetDataCore(Encoding.UTF8.GetBytes(text), DataNutCoreType.Utf8Text);
    return builder;
  }

  /// <summary>
  /// Создаёт построитель ореха из данных файла.
  /// </summary>
  /// <param name="fileName">Имя файла с данными ядра.</param>
  /// <returns>Построитель ореха данных.</returns>
  /// <exception cref="ArgumentNullException">Недопускается значение null для имени файла.</exception>
  /// <exception cref="ArgumentException">Недопускается пустая строка имени файла.</exception>
  /// <exception cref="InvalidOperationException">Файл не содержит данных.</exception>
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

  /// <summary>
  /// Создаёт построитель ореха из буфера.
  /// </summary>
  /// <param name="buffer">Буфер с данными ядра.</param>
  /// <returns>Построитель ореха данных.</returns>
  /// <exception cref="ArgumentNullException">Недопускается значение null для буфера.</exception>
  /// <exception cref="InvalidOperationException">Буфер не содержит данных.</exception>
  public static DataNutBuilder FromMemory(byte[] buffer)
  {
    ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));
    if (buffer.Length == 0)
      throw new InvalidOperationException("Empty core is not allowed while building data nut.");

    var builder = new DataNutBuilder();
    builder.SetDataCore(buffer, DataNutCoreType.GenericBinary);
    return builder;
  }

  /// <summary>
  /// Создаёт построитель ореха из спана.
  /// </summary>
  /// <param name="span">Спан с данными ядра.ы</param>
  /// <returns>Построитель ореха данных.</returns>
  /// <exception cref="InvalidOperationException">Спан не содержит данных.</exception>
  public static DataNutBuilder FromMemory(Span<byte> span)
  {
    if (span.Length == 0)
      throw new InvalidOperationException("Empty core is not allowed while building data nut.");

    var builder = new DataNutBuilder();
    builder.SetDataCore(span, DataNutCoreType.GenericBinary);
    return builder;
  }

  /// <summary>
  /// Добавляет элемент метаданных.
  /// </summary>
  /// <param name="key">Ключ метаданных.</param>
  /// <param name="value">Значение метаданных.</param>
  /// <returns>Экземпляр построителя.</returns>
  /// <exception cref="ArgumentNullException">key имеет значение null.</exception>
  /// <exception cref="ArgumentException">Элемент с таким ключом уже существует в метаданныз.</exception>
  public DataNutBuilder AddMetadata(string key, string value)
  {
    _metadata.Add(key, value);
    _metadataLength += key.Length;
    _metadataLength += value.Length;
    ++_metadataLength;
    return this;
  }

  /// <summary>
  /// Добавляет коллекцию метаданных.
  /// </summary>
  /// <param name="metadata">Метаданные.</param>
  /// <returns>Экземпляр построителя.</returns>
  /// <exception cref="ArgumentNullException">key имеет значение null.</exception>
  /// <exception cref="ArgumentException">Элемент с таким ключом уже существует в метаданныз.</exception>
  public DataNutBuilder AddMetadata(IEnumerable<KeyValuePair<string, string>> metadata)
  {
    foreach (var kv in metadata)
    {
      _metadata.Add(kv.Key, kv.Value);
      _metadataLength += kv.Key.Length;
      _metadataLength += kv.Value.Length;
      ++_metadataLength;
    }
    return this;
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
    var metadataBuffer = CreateMetadata();
    int headerSize = Marshal.SizeOf<DataNutHeader>();
    var header = CreateHeader(coreOffset: (uint)(headerSize + metadataBuffer.Length));
    Span<byte> headerBuffer = stackalloc byte[headerSize];
    MemoryMarshal.Write(headerBuffer, in header);

    var stream = new MemoryStream(_coreData.Length + headerSize + metadataBuffer.Length);
    stream.Write(headerBuffer);
    if (metadataBuffer.Length> 0)
    {
      stream.Write(metadataBuffer);
    }
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
      Metadata = _metadata,
    };
  }

  /// <summary>
  /// Создаёт набор метаданных.
  /// </summary>
  /// <returns>Массив байт с метаданными.</returns>
  private byte[] CreateMetadata()
  {
    if (_metadata.Any())
    {
      var metadataBuilder = new StringBuilder(_metadataLength);
      foreach (var kv in _metadata)
      {
        metadataBuilder.Append(kv.Key);
        metadataBuilder.Append(':');
        metadataBuilder.Append(kv.Value);
        metadataBuilder.Append('\n');
      }
      return Encoding.UTF8.GetBytes(metadataBuilder.ToString());
    }
    return [];
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
      Version = (ushort)(_metadata.Any() ? DataNutVersion.Version_1_1 : DataNutVersion.Version_1_0),
      CoreSize = Convert.ToUInt32(_coreData.Length),
      CoreType = (uint)_coreType,
      CoreHash = _coreHash,
      Created = DateTime.UtcNow.Ticks,
      CoreOffset = coreOffset,
    };
  }
}
