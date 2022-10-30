using Org.BouncyCastle.Math;

namespace VotingApp.Server.Dto
{
    public class SignatureResponse
    {
        public IReadOnlyCollection<byte[]> SignedData { get; set; }

        public string ServerRsaKey { get; set; }
    }
}