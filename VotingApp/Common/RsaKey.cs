using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace VotingApp.Common
{
    public class RsaKey
    {
        public RsaKey(RsaKeyParameters keyParameters)
        {
            EFactor = keyParameters.Exponent;
            NFactor = keyParameters.Modulus;
        }

        public RsaKey(BigInteger exponent, BigInteger modulus)
        {
            EFactor = exponent;
            NFactor = modulus;
        }

        public BigInteger EFactor { get; set; }

        public BigInteger NFactor { get; set; }
    }
}