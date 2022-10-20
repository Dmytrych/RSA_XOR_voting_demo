using VotingApp.Server.Domain.Dto;

namespace VotingApp.Server.Domain.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly List<Candidate> Candidates = new List<Candidate>
    {
        new () { Id = 1, Name = "Петренко Іван Олексійович", Votes = 0 },
        new () { Id = 2, Name = "Дмитрук Олексій Іванович", Votes = 0 }
    };

    public IReadOnlyCollection<Candidate> GetCandidates()
    {
        return Candidates;
    }

    public Candidate GetCandidateById(int id)
    {
        return Candidates.FirstOrDefault(candidate => candidate.Id == id);
    }
}