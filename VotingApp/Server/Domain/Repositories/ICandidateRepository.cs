using VotingApp.Server.Domain.Dto;

namespace VotingApp.Server.Domain.Repositories;

public interface ICandidateRepository
{
    IReadOnlyCollection<Candidate> GetCandidates();

    Candidate GetCandidateById(int id);
}