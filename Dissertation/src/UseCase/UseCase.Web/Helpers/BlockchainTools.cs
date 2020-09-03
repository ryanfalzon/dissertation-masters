using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace UseCase.Web.Helpers
{
    public static class BlockchainTools
    {
        public static byte[] HashObject(object @object)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@object)));
            }
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
    }
}