using Org.BouncyCastle.Math;
using VotingApp.Common;
using VotingApp.Server.Dto;

namespace VotingApp.Server.Services
{
    public interface IVotingService
    {
        IReadOnlyCollection<BigInteger> VerifyData(int voterId, IReadOnlyCollection<VotingPackage> packages);

        string Vote(SignedVotingPaper paper);

        RsaKey GetPublicKey();
    }
}