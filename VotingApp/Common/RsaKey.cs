using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace VotingApp.Common
{
    public class RsaKey
    {
        public RsaKey(RsaKeyParameters keyParameters)
        {
            DorEFactor = keyParameters.Exponent;
            NFactor = keyParameters.Modulus;
        }

        public BigInteger DorEFactor { get; set; }

        public BigInteger NFactor { get; set; }
    }
}