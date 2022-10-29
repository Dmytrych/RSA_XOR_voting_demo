using Org.BouncyCastle.Math;

namespace VotingApp.Server.Dto
{
    public class VotingPaper
    {
        public BigInteger RandCoef { get; set; }

        public BigInteger NFactor { get; set; }

        public BigInteger Data { get; set; }
    }
}