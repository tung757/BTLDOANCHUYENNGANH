using System;
using System.Text;
using System.Web.Security;

public static class CryptoHelper
{
    public static string Encrypt(string plain)
    {
        byte[] data = Encoding.UTF8.GetBytes(plain);
        byte[] encrypted = MachineKey.Protect(data, "OTP_PROTECT");
        return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string cipher)
    {
        try
        {
            byte[] data = Convert.FromBase64String(cipher);
            byte[] decrypted = MachineKey.Unprotect(data, "OTP_PROTECT");
            return Encoding.UTF8.GetString(decrypted);
        }
        catch
        {
            return null;
        }
    }
}
