using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace sfcat.Tests
{
    [TestClass]
    public class BasicCommandTests
    {
        [TestMethod]
        public void TestLogoCommand()
        {
            LogoCommand logoCommand = new LogoCommand();
            var conclusion = logoCommand.Run();
            Assert.IsInstanceOfType(conclusion, typeof(MultiOutputConclusion));
            var str = from s in ((MultiOutputConclusion)conclusion).GetOutputStrings()
                      where s.Contains("Azure CTO's Office")
                      select s;

            Assert.IsNotNull(str.Count() == 1);
        }       
    }
}
