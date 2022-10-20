namespace VotingApp.Server.Domain.Dto;

public class Candidate
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public int Votes { get; set; }
}