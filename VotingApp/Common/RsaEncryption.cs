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

    public string Encrypt(string text, string publicKeyText)
    {
        var publicKey = GetParametersFromKey(publicKeyText);
        var cspTemp = new RSACryptoServiceProvider();
        cspTemp.ImportParameters(publicKey);
        var data = Encoding.Unicode.GetBytes(text);
        var cypher = cspTemp.Encrypt(data, false);
        return Convert.ToBase64String(cypher);
    }

    public string Decrypt(string cypherText, string privateKeyText)
    {
        var dataBytes = Convert.FromBase64String(cypherText);
        csp.ImportParameters(GetParametersFromKey(privateKeyText));
        var result = csp.Decrypt(dataBytes, false);
        return Encoding.Unicode.GetString(result);
    }

    private string SerializeKey(RSAParameters key)
    {
        var sw = new StringWriter();
        var xmlSerializer = new XmlSerializer(typeof(RSAParameters));
        xmlSerializer.Serialize(sw, key);
        return sw.ToString();
    }

    private RSAParameters GetParametersFromKey(string publicKey)
    {
        var keyBytes = Encoding.Unicode.GetBytes(publicKey);
        var textReader = new XmlTextReader(new MemoryStream(keyBytes));
        var xmlSerializer = new XmlSerializer(typeof(RSAParameters));
        return (RSAParameters) xmlSerializer.Deserialize(textReader);
    }
}