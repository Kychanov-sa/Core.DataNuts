using GlacialBytes.Core.DataNuts;
using GlacialBytes.Core.DataNuts.IO;

namespace Core.DataNuts.Tests
{
  [TestClass]
  public sealed class DataNutBuilderTest
  {
    [TestMethod]
    public void FromText()
    {
      var builder = DataNutBuilder.FromText("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");
      var nut = builder.Build();

      Assert.AreEqual(Guid.Parse("{f06f31f3-d2d2-c097-7b00-000002000080}"), nut.Id);
      Assert.AreEqual(DateTime.UtcNow.ToShortDateString(), nut.Created.ToShortDateString());
      Assert.AreEqual(155, nut.NutStream.Length);
      Assert.AreEqual(123, nut.CoreSize);
      Assert.AreEqual(0xc097d2d2f06f31f3, nut.CoreHash);
      Assert.AreEqual(DataNutCoreType.Utf8Text, nut.CoreType);      
    }

    [TestMethod]
    public void FromText_WithMetadata()
    {
      var builder = DataNutBuilder.FromText("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");
      builder.AddMetadata("Author", "Stanislav Kychanov");
      builder.AddMetadata("SupportedVersion", "1.0");
      var nut = builder.Build();

      Assert.AreEqual(Guid.Parse("{f06f31f3-d2d2-c097-7b00-000002000080}"), nut.Id);
      Assert.AreEqual(DateTime.UtcNow.ToShortDateString(), nut.Created.ToShortDateString());
      Assert.AreEqual(202, nut.NutStream.Length);
      Assert.AreEqual(123, nut.CoreSize);
      Assert.AreEqual(0xc097d2d2f06f31f3, nut.CoreHash);
      Assert.AreEqual(DataNutCoreType.Utf8Text, nut.CoreType);
      Assert.AreEqual(2, nut.Metadata.Count);
      Assert.AreEqual("Stanislav Kychanov", nut.Metadata["Author"]);
      Assert.AreEqual("1.0", nut.Metadata["SupportedVersion"]);

      using var fileStream = File.OpenWrite(@"TestData\test2.nut");
      nut.NutStream.Position = 0;
      nut.NutStream.CopyTo(fileStream);
    }
  }
}
