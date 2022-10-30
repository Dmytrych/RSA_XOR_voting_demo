namespace VotingApp.Server.Dto;

public class VerificarionRequest
{
    public int VoterId { get; set; }
    
    public IReadOnlyCollection<VotingPackage> VotingPackages { get; set; }
}