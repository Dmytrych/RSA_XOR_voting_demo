using System.Text;
using System.Text.Json;
using Org.BouncyCastle.Math;
using VotingApp.Common;
using VotingApp.Server.Domain.Repositories;
using VotingApp.Server.Dto;

namespace VotingApp.Server.Services
{
    public class VotingService : IVotingService
    {
        private readonly SigratureService sigratureService;
        private readonly IVoterRepository voterRepository;
        private readonly ICandidateRepository candidateRepository;
        private readonly RsaEncryption rsaEncryption;

        public VotingService(
            SigratureService sigratureService,
            IVoterRepository voterRepository,
            ICandidateRepository candidateRepository,
            RsaEncryption rsaEncryption)
        {
            this.sigratureService = sigratureService;
            this.voterRepository = voterRepository;
            this.candidateRepository = candidateRepository;
            this.rsaEncryption = rsaEncryption;
        }

        public IReadOnlyCollection<BigInteger> VerifyData(int voterId, IReadOnlyCollection<VotingPackage> packages)
        {
            var voter = voterRepository.GetVoterById(voterId);

            if (voter == null || voter.Voted)
            {
                return new List<BigInteger>();
            }

            voter.Voted = true;

            var selectedPackage = GetRandomPackage(packages);

            var signatures = selectedPackage.Papers.Select(SignPaper).ToList();

            return signatures;
        }

        public string Vote(SignedVotingPaper paper)
        {
            Console.WriteLine(sigratureService.PublicRsaKey.EFactor.ToString());
            var decryptedData = rsaEncryption.Decrypt(paper.EncryptedData);

            if (!sigratureService.VerifySignature(new BigInteger(paper.Signature), decryptedData))
            {
                return "The signature was invalid";
            }

            var decryptedContent = JsonSerializer.Deserialize<VotingData>(decryptedData);

            if (decryptedContent == null || voterRepository.GetVoterById(decryptedContent.VoterId).Voted)
            {
                return "Such voter could not be found";
            }
            
            var voter = voterRepository.GetVoterById(decryptedContent.VoterId);
            if (voter.Voted)
            {
                return "You cannot vote twice";
            }

            if (!int.TryParse(decryptedContent?.Vote, out int candidateId))
            {
                return "The vote could not be counted";
            }

            var candidate = candidateRepository.GetCandidateById(candidateId);
            if (candidate == null)
            {
                return "The given candidate does not exist";
            }

            voter.Voted = true;
            candidate.Votes++;

            return "Your vote was successfully counted";
        }

        public RsaKey GetPublicKey()
        {
            return sigratureService.PublicRsaKey;
        }

        private BigInteger SignPaper(NetworkVotingPaper paper)
        {
            return sigratureService.SignBlindedMessage(new BigInteger(paper.Data));
        }

        private VotingPackage GetRandomPackage(IReadOnlyCollection<VotingPackage> packages)
        {
            var random = new Random();
            return packages.ToArray()[random.Next(packages.Count - 1)];
        }
    }
}