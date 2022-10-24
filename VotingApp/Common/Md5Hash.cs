using System.Security.Cryptography;
using System.Text;

namespace VotingApp.Common
{
    public static class Md5Hash
    {
        public static string GetHash(string data)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.Unicode.GetBytes(data));
            return Encoding.Unicode.GetString(hash);
        }
    }
}