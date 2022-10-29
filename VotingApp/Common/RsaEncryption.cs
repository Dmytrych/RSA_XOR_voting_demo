using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace VotingApp.Common;

public class RsaEncryption
{
    private readonly RSACryptoServiceProvider csp;

    public RsaEncryption()
    {
        csp = new RSACryptoServiceProvider();
        PublicKey = csp.ExportParameters(false);
        PrivateKey = csp.ExportParameters(true);
    }

    public RSAParameters PublicKey { get; }

    public RSAParameters PrivateKey { get; }

    public string GetPrivateKey() => SerializeKey(PrivateKey);

    public string GetPublicKey() => SerializeKey(PublicKey);

    public string Encrypt(string text)
    {
        var data = Encoding.Unicode.GetBytes(text);
        return Encrypt(data, csp);
    }

    public static string Encrypt(string text, string publicKeyText)
    {
        var publicKey = GetParametersFromKey(publicKeyText);
        var cspTemp = new RSACryptoServiceProvider();
        cspTemp.ImportParameters(publicKey);
        var data = Encoding.Unicode.GetBytes(text);
        return Encrypt(data, cspTemp);
    }

    public static string Encrypt(byte[] data, RSACryptoServiceProvider rsa)
    {
        var cypher = rsa.Encrypt(data, false);
        return Convert.ToBase64String(cypher);
    }

    public string Decrypt(string encryptedData)
    {
        var dataBytes = Convert.FromBase64String(encryptedData);
        return Decrypt(dataBytes, csp);
    }

    public static string Decrypt(string cypherText, string privateKeyText)
    {
        var dataBytes = Convert.FromBase64String(cypherText);
        var tempCsp = new RSACryptoServiceProvider();
        tempCsp.ImportParameters(GetParametersFromKey(privateKeyText));
        return Decrypt(dataBytes, tempCsp);
    }

    public static string Decrypt(byte[] data, RSACryptoServiceProvider rsa)
    {
        var result = rsa.Decrypt(data, false);
        return Encoding.Unicode.GetString(result);
    }

    private string SerializeKey(RSAParameters key)
    {
        var sw = new StringWriter();
        var xmlSerializer = new XmlSerializer(typeof(RSAParameters));
        xmlSerializer.Serialize(sw, key);
        return sw.ToString();
    }

    private static RSAParameters GetParametersFromKey(string publicKey)
    {
        var keyBytes = Encoding.Unicode.GetBytes(publicKey);
        var textReader = new XmlTextReader(new MemoryStream(keyBytes));
        var xmlSerializer = new XmlSerializer(typeof(RSAParameters));
        return (RSAParameters) xmlSerializer.Deserialize(textReader);
    }

    public string SignData(string dataText)
    {
        var data = Encoding.Unicode.GetBytes(dataText);
        return SignData(data, csp);
    }

    public static string SignData(string privateKeyText, string dataText)
    {
        var privateKey = GetParametersFromKey(privateKeyText);
        RSACryptoServiceProvider cspTemp = new RSACryptoServiceProvider();
        cspTemp.ImportParameters(privateKey);
        var data = Encoding.Unicode.GetBytes(dataText);
        return SignData(data, cspTemp);
    }

    public static string SignData(byte[] data, RSACryptoServiceProvider rsa)
    {
        var signature = rsa.SignData(data, SHA256.Create());
        return Convert.ToBase64String(signature);
    }

    public bool VerifySignature(string dataToVerify, string signature)
    {
        var signatureData = Encoding.Unicode.GetBytes(signature);
        var data = Encoding.Unicode.GetBytes(dataToVerify);
        return VerifySignature(data, signatureData, csp);
    }

    private static bool VerifySignature(byte[] dataToVerify, byte[] signature, RSACryptoServiceProvider rsa)
    {
        return rsa.VerifyData(dataToVerify, SHA256.Create(), signature);
    }
}