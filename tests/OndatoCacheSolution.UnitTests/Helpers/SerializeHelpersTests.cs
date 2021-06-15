using FluentAssertions;
using Newtonsoft.Json;
using OndatoCacheSolution.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OndatoCacheSolution.UnitTests.Helpers
{
    public class SerializeHelpersTests
    {
        [Fact]
        public void ByteArrayToObject_GivenAString_ItCanBeSerializedDeserialized()
        {
            var stringObject = "test";
            var deserializedObject = SerializeHelpers.Deserialize(
                SerializeHelpers.Serialize(stringObject)).ToString();

            deserializedObject.Should().Be(stringObject);

        }
    }
}
