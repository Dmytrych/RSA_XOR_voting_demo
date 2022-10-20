namespace VotingApp.Common;

public static class XorEncryption
{
    private static char XorKey = 'a';
    
    public static string EncryptDecrypt(string text)
    {
        return new string(text.Select(letter => (char)(letter ^ XorKey)).ToArray());
    }
}