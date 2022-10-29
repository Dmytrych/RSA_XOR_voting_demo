using Org.BouncyCastle.Math;
using VotingApp.Server.Dto;

namespace VotingApp.Server.Services
{
    public interface IVotingService
    {
        IReadOnlyCollection<BigInteger> VerifyData(int voterId, IReadOnlyCollection<VotingPackage> packages);
    }
}