using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace UnifiedModel.Connectors
{
    public static class Extensions
    {
        public static string Hash(this object @object)
        {
            using SHA256 sha256Hash = SHA256.Create();
            return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@object))).ToHexString();
        }

        public static string ToHexString(this byte[] data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        public static void Assert(bool assertion)
        {
            if (!assertion)
            {
                throw new InvalidDataException("Null values passed as parameters...");
            }
        }

        public static bool IsNotNull(this object @object)
        {
            foreach (PropertyInfo propertyInfo in @object.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(@object);
                if(value == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}