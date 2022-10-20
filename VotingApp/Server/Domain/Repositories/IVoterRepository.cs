using VotingApp.Server.Domain.Dto;

namespace VotingApp.Server.Domain.Repositories;

public interface IVoterRepository
{
    IReadOnlyCollection<Voter> GetVoters();

    Voter GetVoterById(int id);
}