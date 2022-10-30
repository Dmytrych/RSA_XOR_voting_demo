namespace VotingApp.Server.Dto;

public class NetworkVotingPaper
{
    public byte[] RandCoef { get; set; }

    public byte[] NFactor { get; set; }

    public byte[] Data { get; set; }

    public string OriginalVote { get; set; }
}