using Org.BouncyCastle.Math;

namespace VotingApp.Server.Dto
{
    public class SignedVotingPaper
    {
        public BigInteger RandCoef { get; set; }

        public BigInteger NFactor { get; set; }

        public BigInteger Signature { get; set; }

        public string EncryptedData { get; set; }
    }
}