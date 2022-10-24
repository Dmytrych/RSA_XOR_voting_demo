using VotingApp.Client;
using VotingApp.Common;
using VotingApp.Server.Domain.Repositories;
using VotingApp.Server.Voting;

namespace VotingApp
{
    public static class Program
    {
        public static void Main()
        {
            var random = new Random();
            var voterRepository = new VoterRepository();
            var candidateRepository = new CandidateRepository();
            var voter = voterRepository.GetVoterById(1);

            var client = new ClientEmulator(random, new RsaEncryption());

            var votingService = new VotingService(voterRepository, new RsaEncryption(), candidateRepository);

            var clientVote = client.GetSecureRandomVote(candidateRepository.GetCandidates(), voter.PrivateKey);

            //At this point the data is transferred to the server via internet

            Console.WriteLine(votingService.Vote(voter.Id, clientVote.data, clientVote.signature));

            Console.WriteLine("\nIn case the voting data is corrupted or invalid:");
            Console.WriteLine(votingService.Vote(voter.Id, "sdasdasd", clientVote.signature));

            Console.WriteLine("\nIn case the voter is not existing or is not authorized to vote:");
            Console.WriteLine(votingService.Vote(int.MaxValue, clientVote.data, clientVote.signature));
        }
    }
}

