﻿using Org.BouncyCastle.Math;
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

        public VotingService(
            SigratureService sigratureService,
            IVoterRepository voterRepository,
            ICandidateRepository candidateRepository)
        {
            this.sigratureService = sigratureService;
            this.voterRepository = voterRepository;
            this.candidateRepository = candidateRepository;
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

        public string Vote(SignedVotingPaper paper, RsaEncryption rsaEncryption)
        {
            var decryptedData = rsaEncryption.Decrypt(paper.EncryptedData);

            if (!sigratureService.VerifySignature(paper.Signature, decryptedData))
            {
                return "The signature was invalid";
            }

            if (!int.TryParse(decryptedData, out int candidateId))
            {
                return "The vote could not be counted";
            }

            var candidate = candidateRepository.GetCandidateById(candidateId);
            if (candidate == null)
            {
                return "The given candidate does not exist";
            }

            candidate.Votes++;

            return "Your vote was successfully counted";
        }

        private BigInteger SignPaper(VotingPaper paper)
        {
            return sigratureService.SignBlindedMessage(paper.Data);
        }

        private VotingPackage GetRandomPackage(IReadOnlyCollection<VotingPackage> packages)
        {
            var random = new Random();
            return packages.ToArray()[random.Next(packages.Count - 1)];
        }
    }
}