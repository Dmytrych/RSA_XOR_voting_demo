namespace VotingApp.Server.Domain.Dto;

public class Voter
{
    public int Id { get; set; }

    public string PublicKey { get; set; }

    public string PrivateKey { get; set; } // This must not be stored in a database, but is used for demo purposes

    public bool Voted { get; set; } = false;
}