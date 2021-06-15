using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;

namespace OndatoCacheSolution.Domain.Helpers
{
    public static class SerializeHelpers
    {
        public static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static object Deserialize(string objectString)
        {
            return JsonSerializer.Deserialize<object>(objectString);
        }
    }
}
