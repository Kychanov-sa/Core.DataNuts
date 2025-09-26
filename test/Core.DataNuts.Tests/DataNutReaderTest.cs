using GlacialBytes.Core.DataNuts;
using GlacialBytes.Core.DataNuts.IO;
using System.Text;

namespace Core.DataNuts.Tests
{
  [TestClass]
  public sealed class DataNutReaderTest
  {
    [TestMethod]
    public void Read()
    {
      using var stream = File.OpenRead("TestData/test1.nut");
      using var reader = new DataNutReader(stream);

      Assert.AreEqual(Guid.Parse("{f06f31f3-d2d2-c097-7b00-000002000080}"), reader.NutId);
      Assert.AreEqual(123, reader.CoreSize);
      Assert.AreEqual(0xc097d2d2f06f31f3, reader.CoreHash);
      Assert.AreEqual("2025-09-21T08:32:59.4615547", reader.NutCreated.ToString("O"));
      Assert.AreEqual(DataNutCoreType.Utf8Text, reader.CoreType);

      Span<byte> buffer = stackalloc byte[(int)reader.CoreSize];
      int readSize = reader.Read(buffer);
      Assert.AreEqual(reader.CoreSize, readSize);

      string text = Encoding.UTF8.GetString(buffer);
      Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", text);
    }
  }
}
