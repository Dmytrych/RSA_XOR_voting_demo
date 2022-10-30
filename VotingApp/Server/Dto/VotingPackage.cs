using System.Text.Json.Serialization;

namespace VotingApp.Server.Dto
{
    public class VotingPackage
    {
        public List<NetworkVotingPaper> Papers { get; set; }
    }
}