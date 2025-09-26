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

      using var fileStream = File.OpenWrite(@"TestData\test1.nut");
      nut.NutStream.Position = 0;
      nut.NutStream.CopyTo(fileStream);
    }
  }
}
