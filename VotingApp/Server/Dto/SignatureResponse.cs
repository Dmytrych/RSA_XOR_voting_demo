using Org.BouncyCastle.Math;

namespace VotingApp.Server.Dto
{
    public class SignatureResponse
    {
        public IReadOnlyCollection<BigInteger> SignedData { get; set; }

        public string ServerRsaKey { get; set; }
    }
}