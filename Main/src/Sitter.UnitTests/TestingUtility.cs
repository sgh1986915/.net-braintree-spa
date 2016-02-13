using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace MySitterHub.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class TestingUtility
    {
        public static void AreEqualByJson(object expected, object actual)
        {
            string expectedJson = JsonConvert.SerializeObject(expected);
            string actualJson = JsonConvert.SerializeObject(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}