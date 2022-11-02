using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Org.BouncyCastle.Math;
using VotingApp.Common;
using VotingApp.Server.Domain.Repositories;
using VotingApp.Server.Dto;

namespace VotingApp.Client
{
    public static class Program
    {
        private const string serverUrl = "http://localhost:5288/VotingApi";
        private static SigratureService sigratureService = new SigratureService();
        
        public static async Task Main()
        {
            //Console.WriteLine(Encoding.ASCII.GetString(sigratureService.UnblindMessage().ToByteArray()));
            var candidateRepository = new CandidateRepository();
            var voterId = 1;
            var candidateVotes = new List<string>();

            sigratureService = new SigratureService(await SendKeyRequest());

            foreach (var candidate in candidateRepository.GetCandidates())
            {
                candidateVotes.Add(candidate.Id.ToString());
            }

            var package = FormVotingPackage(candidateVotes);
            var request = new List<VotingPackage>();

            for (int i = 0; i < 10; i++)
            {
                request.Add(package);
            }

            var signedData = await SendValidateRequest(voterId, request);

            if (signedData == null)
            {
                Console.WriteLine("Recieved invalid data from server");
                return;
            }

            var votingPaperSignatures = signedData.SignedData.Select(data => new BigInteger(data)).ToList();

            var signatures = votingPaperSignatures.Select(signature => sigratureService.UnblindMessage(signature));

            var votedCandidateBytes = sigratureService.UnblindMessage(new BigInteger(package.Papers.First().Data));

            var vote = package.Papers.First().OriginalVote;
            var unblindedSignature = signatures.First();

            await SendValidateRequest(vote, signedData.ServerRsaKey, unblindedSignature);
        }

        public static async Task<RsaKey> SendKeyRequest()
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(serverUrl + "/GetKeys");

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<RsaKey>();
            }

            return null;
        }

        public static async Task SendValidateRequest(string vote, string serverKey, BigInteger signature)
        {
            var httpClient = new HttpClient();
            var data = JsonSerializer.Serialize(new SignedVotingPaper
            {
                Signature = Encoding.UTF8.GetString(signature.ToByteArray()),
                EncryptedData = RsaEncryption.Encrypt(vote, serverKey)
            });
            var result = await httpClient.PostAsync(
                serverUrl + "/Vote",
                new StringContent(data, Encoding.UTF8, "application/json"));

            Console.WriteLine(new StreamReader(result.Content.ReadAsStream()).ReadToEnd());
        }

        public static async Task<SignatureResponse> SendValidateRequest(int voterId, List<VotingPackage> votingPackages)
        {
            var httpClient = new HttpClient();
            var data = JsonSerializer.Serialize(new VerificarionRequest{ VoterId = voterId, VotingPackages = votingPackages });
            var result = await httpClient.PostAsync(
                serverUrl + "/VerifyData",
                new StringContent(data, Encoding.UTF8, "application/json"));

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<SignatureResponse>();
            }

            return null;
        }

        private static VotingPackage FormVotingPackage(IReadOnlyCollection<string> votes)
            => new()
            {
                Papers = votes.Select(vote => new NetworkVotingPaper()
                {
                    Data = sigratureService.BlindMessage(vote).ToByteArray(),
                    NFactor = sigratureService.GetNFactor().ToByteArray(),
                    RandCoef = sigratureService.randomBigInt.ToByteArray(),
                    OriginalVote = vote
                }).ToList()
            };
    }
}