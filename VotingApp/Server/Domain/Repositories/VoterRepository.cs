

using VotingApp.Common;
using VotingApp.Server.Domain.Dto;

namespace VotingApp.Server.Domain.Repositories;

public class VoterRepository : IVoterRepository
{
    private static List<Voter> voters = new List<Voter>();
    
    public VoterRepository()
    {
        if (!voters.Any())
        {
            SeedVoters();
        }
    }

    public IReadOnlyCollection<Voter> GetVoters()
    {
        return voters;
    }
    
    public Voter GetVoterById(int id)
    {
        return voters.FirstOrDefault(voter => voter.Id == id);
    }

    private void SeedVoters()
    {
        for (int i = 0; i < 10; i++)
        {
            var rsa = new RsaEncryption();
            voters.Add(new Voter
            {
                Id = i,
                PublicKey = rsa.GetPrivateKey(),
                PrivateKey = rsa.GetPublicKey(),
                Voted = false
            });
        }
    }
}