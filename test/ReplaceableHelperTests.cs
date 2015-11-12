using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using Knowit.EPiModules.Replaceables.Helper;
using Moq;
using NUnit.Framework;

namespace Knowit.EPiModules.Replaceables.Test
{
    [TestFixture]
    public class ReplaceableHelperTests
    {
        [Test]
        public void ShouldSetCorrectReplaceable()
        {
            //Arrange - episerver
            var mockLocator = new Mock<IServiceLocator>();
            var mockManager = new Mock<IReplaceablesManager>();
            ServiceLocator.SetLocator(mockLocator.Object);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            var context = new Mock<ContextCache>();
            context.Setup(x => x["EPiServer:ContentLanguage"]).Returns(new CultureInfo("en"));
            ContextCache.Current = context.Object;

            //Arrange - test
            var keys = new Dictionary<string, string>
            {
                {"test", "testing is essensial"},
                {"test2", "testing is really cool"}
            };
            mockManager.Setup(x => x.GetByLanguage(It.IsAny<string>())).Returns(() => Task.Run(() => keys));
            mockLocator.Setup(x => x.GetInstance<IReplaceablesManager>()).Returns(mockManager.Object);


            const string inputstring = "<!doctype HTML><html>" +
                                       "<head>" +
                                       "<title>" + 
                                            ReplaceableHelper.SearchTagStart + "test" + ReplaceableHelper.SearchTagEnd + 
                                        "</title>" +
                                       "</head>" +
                                        "<body>" + 
                                            ReplaceableHelper.SearchTagStart + "test2" + ReplaceableHelper.SearchTagEnd + 
                                        "</body>" +
                                       "<html>";

            const string expectedString = "<!doctype HTML><html><head><title>testing is essensial</title></head><body>testing is really cool</body><html>";
            
            //Act
            var result = ReplaceableHelper.SetReplacables(inputstring);

            //Assert
            Assert.That(result, Is.EqualTo(expectedString));

        }
    }
}
