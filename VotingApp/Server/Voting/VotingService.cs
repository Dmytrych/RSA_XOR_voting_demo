using VotingApp.Common;
using VotingApp.Server.Domain.Repositories;

namespace VotingApp.Server.Voting;

public class VotingService
{
    private readonly VoterRepository voterRepository;
    private readonly RsaEncryption rsaEncryption;
    private readonly CandidateRepository candidateRepository;
    
    public VotingService(
        VoterRepository voterRepository,
        RsaEncryption rsaEncryption,
        CandidateRepository candidateRepository)
    {
        this.voterRepository = voterRepository;
        this.rsaEncryption = rsaEncryption;
        this.candidateRepository = candidateRepository;
    }
    
    public string Vote(int voterId, string encryptedVotingResult)
    {
        var voter = voterRepository.GetVoterById(voterId);

        if (voter == null)
        {
            return "The voter is not valid";
        }
        
        var votingResult = DecryptAll(encryptedVotingResult, voter.PrivateKey);

        if (!votingResult.isValid)
        {
            return "The vote was invalid";
        }

        if (!int.TryParse(votingResult.result, out int candidateId))
        {
            return "The vote was invalid";
        }
        
        var candidate = candidateRepository.GetCandidateById(candidateId);
        if (candidate == null)
        {
            return "The vote was invalid";
        }

        candidate.Votes++;
        return "Vote counted";
    }

    private (bool isValid, string result) DecryptAll(string encryptedVotingResult, string voterPrivateKey)
    {
        var xorDecryptedResult = XorEncryption.EncryptDecrypt(encryptedVotingResult);
        try
        {
            var result = rsaEncryption.Decrypt(xorDecryptedResult, voterPrivateKey);
            return (true, result);
        }
        catch
        {
            return (false, string.Empty);
        }
    }
}