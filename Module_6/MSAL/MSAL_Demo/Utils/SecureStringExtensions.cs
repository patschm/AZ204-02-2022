using System.Net;
using System.Security;

namespace MSAL_Demo
{
    static class SecureStringExtensions
    {
        public static string ToPlainString(this SecureString secureStr)
        {
            string plainStr = new NetworkCredential(string.Empty, secureStr).Password;
            return plainStr;
        }

        public static SecureString ToSecureString(this string plainStr)
        {
            var secStr = new SecureString(); 
            secStr.Clear();
            foreach (char c in plainStr.ToCharArray())
            {
                secStr.AppendChar(c);
            }
            return secStr;
        }
    }
}
