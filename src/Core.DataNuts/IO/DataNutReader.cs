using System.Runtime.InteropServices;

namespace GlacialBytes.Core.DataNuts.IO;

/// <summary>
/// Ридер для орехов данных.
/// </summary>
public class DataNutReader : IDisposable, IAsyncDisposable
{
  /// <summary>
  /// Поток с данными ореха.
  /// </summary>
  protected Stream _coreStream;

  /// <summary>
  /// Признак утилизованного объекта.
  /// </summary>
  protected bool _disposed = false;

  /// <summary>
  /// Признак необходимости оставить исходный поток открытым при утилизации ридера.
  /// </summary>
  private readonly bool _leaveOpen;

  /// <summary>
  /// Дата создания ореха.
  /// </summary>
  public DateTime NutCreated { get; private set; }

  /// <summary>
  /// Идентификатор ореха.
  /// </summary>
  public Guid NutId { get; private set; }

  /// <summary>
  /// Размер ядра данных в байтах.
  /// </summary>
  public long CoreSize { get; private set; }

  /// <summary>
  /// Хэш ядра данных.
  /// </summary>
  public ulong CoreHash { get; private set; }

  /// <summary>
  /// Тип данных в ядре.
  /// </summary>
  public DataNutCoreType CoreType { get; private set; }

  /// <summary>
  /// Конструктор.
  /// </summary>
  /// <param name="stream">Поток с данными ореха.</param>
  /// <param name="leaveOpen">true, если необходимо оставить поток открытым, после удаления объекта DataNutStream; иначе false.</param>
  /// <returns>Поток ядра.</returns>
  /// <exception cref="EndOfStreamException">Был достигнут конец потока при чтении данных.</exception>
  public DataNutReader(Stream stream, bool leaveOpen = false)
  {
    int headerSize = Marshal.SizeOf<DataNutHeader>();
    if (stream.Length - stream.Position < headerSize)
      throw new EndOfStreamException();

    Span<byte> buffer = stackalloc byte[headerSize];
    _ = stream.Read(buffer);
    var header = MemoryMarshal.Read<DataNutHeader>(buffer);

    if (header.Signature != DataNutHeader.DataNutSignature)
      throw new InvalidNutSignatureException();

    NutCreated = new DateTime(header.Created);
    NutId = header.Id;

    CoreSize = header.CoreSize;
    CoreHash = header.CoreHash;
    CoreType = (DataNutCoreType)header.CoreType;

    stream.Seek(header.CoreOffset, SeekOrigin.Begin);
    _coreStream = stream;
    _leaveOpen = leaveOpen;
  }

  /// <summary>
  /// Читает указанное количество байт данных из ядра и записывает их в буфер, начиная с указанного смещения,
  /// </summary>
  /// <param name="buffer">Буфер для сохранения прочитанного.</param>
  /// <param name="offset">Смещение в буфере для записи.</param>
  /// <param name="count">Количество читаемых байт данных.</param>
  /// <returns>Количество прочитанных байт.</returns>
  public int Read(byte[] buffer, int offset, int count)
  {
    return _coreStream.Read(buffer, offset, count);
  }

  /// <summary>
  /// Читает указанное количество байт данных из ядра и записывает их в буфер.
  /// </summary>
  /// <param name="buffer">Буфер для сохранения прочитанного.</param>
  /// <returns>Количество прочитанных байт.</returns>
  public int Read(Span<byte> buffer)
  {
    return _coreStream.Read(buffer);
  }

  /// <summary>
  /// Асинхронно читает указанное количество байт данных из ядра и записывает их в буфер, начиная с указанного смещения,
  /// </summary>
  /// <param name="buffer">Буфер для сохранения прочитанного.</param>
  /// <param name="offset">Смещение в буфере для записи.</param>
  /// <param name="count">Количество читаемых байт данных.</param>
  /// <param name="cancellationToken">Токен отмены.</param>
  /// <returns>Количество прочитанных байт.</returns>
  public async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
  {
#pragma warning disable CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'
    return await _coreStream.ReadAsync(buffer, offset, count, cancellationToken);
#pragma warning restore CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'
  }

  /// <summary>
  /// Асинхронно читает указанное количество байт данных из ядра и записывает их в буфер.
  /// </summary>
  /// <param name="buffer">Буфер для сохранения прочитанного.</param>
  /// <param name="cancellationToken">Токен отмены.</param>
  /// <returns>Количество прочитанных байт.</returns>
  public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
  {
    return await _coreStream.ReadAsync(buffer, cancellationToken);
  }

  /// <summary>
  /// Перемещает позицию чтение внутри ядра данных.
  /// </summary>
  /// <param name="offset">Смещение относительно начала ядра данных.</param>
  /// <param name="origin">Направление перемещения позиции.</param>
  /// <returns>Значение новой позиции.</returns>
  public long Seek(long offset, SeekOrigin origin)
  {
    long newOffset = _coreStream.Seek(offset + Marshal.SizeOf<DataNutHeader>(), origin);
    return newOffset - Marshal.SizeOf<DataNutHeader>();
  }

  #region IDisposable

  /// <summary>
  /// <see cref="IDisposable.Dispose"/>
  /// </summary>
  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  #endregion

  #region IAsyncDisposable

  /// <summary>
  /// <see cref="IAsyncDisposable.DisposeAsync"/>
  /// </summary>
  public async ValueTask DisposeAsync()
  {
    await DisposeAsyncCore().ConfigureAwait(false);
    GC.SuppressFinalize(this);
  }

  #endregion

  /// <summary>
  /// Утилизирует объект.
  /// </summary>
  /// <param name="disposing">Утилизация вызвана вручную.</param>
  protected virtual void Dispose(bool disposing)
  {
    if (_disposed)
      return;

    if (disposing && !_leaveOpen)
    {
      _coreStream.Dispose();
    }
  }

  /// <summary>
  /// Асинхронно утилизирует объект.
  /// </summary>
  protected async virtual ValueTask DisposeAsyncCore()
  {
    if (_disposed)
      return;

    if (!_leaveOpen)
    {
      await _coreStream.DisposeAsync().ConfigureAwait(false);
    }
  }
}
