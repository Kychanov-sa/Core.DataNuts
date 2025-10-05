using GlacialBytes.Core.DataNuts;
using GlacialBytes.Core.DataNuts.IO;
using System.Text;

namespace Core.DataNuts.Tests
{
  [TestClass]
  public sealed class DataNutReaderTest
  {
    [TestMethod]
    public void Read_v1_0()
    {
      using var stream = File.OpenRead("TestData/test1.nut");
      using var reader = new DataNutReader(stream);

      Assert.AreEqual(Guid.Parse("{f06f31f3-d2d2-c097-7b00-000002000080}"), reader.NutId);
      Assert.AreEqual(123, reader.CoreSize);
      Assert.AreEqual(0xc097d2d2f06f31f3, reader.CoreHash);
      Assert.AreEqual(DataNutCoreType.Utf8Text, reader.CoreType);

      Span<byte> buffer = stackalloc byte[(int)reader.CoreSize];
      int readSize = reader.Read(buffer);
      Assert.AreEqual(reader.CoreSize, readSize);

      string text = Encoding.UTF8.GetString(buffer);
      Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", text);
    }

    [TestMethod]
    public void Read_v1_1()
    {
      using var stream = File.OpenRead("TestData/test2.nut");
      using var reader = new DataNutReader(stream);

      Assert.AreEqual(Guid.Parse("{f06f31f3-d2d2-c097-7b00-000002000080}"), reader.NutId);
      Assert.AreEqual(123, reader.CoreSize);
      Assert.AreEqual(0xc097d2d2f06f31f3, reader.CoreHash);
      Assert.AreEqual(DataNutCoreType.Utf8Text, reader.CoreType);
      Assert.AreEqual(2, reader.Metadata.Count);
      Assert.AreEqual("Stanislav Kychanov", reader.Metadata["Author"]);
      Assert.AreEqual("1.0", reader.Metadata["SupportedVersion"]);

      Span<byte> buffer = stackalloc byte[(int)reader.CoreSize];
      int readSize = reader.Read(buffer);
      Assert.AreEqual(reader.CoreSize, readSize);

      string text = Encoding.UTF8.GetString(buffer);
      Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", text);
    }
  }
}
