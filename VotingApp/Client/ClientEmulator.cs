using VotingApp.Common;
using VotingApp.Server.Domain.Dto;

namespace VotingApp.Client;

public class ClientEmulator
{
    private readonly Random random;
    private readonly RsaEncryption rsaEncryption;
    
    public ClientEmulator(Random random, RsaEncryption rsaEncryption)
    {
        this.random = random;
        this.rsaEncryption = rsaEncryption;
    }
    
    public string GetSecureRandomVote(IReadOnlyCollection<Candidate> candidates, string publicKey)
    {
        var votedCandidateId = candidates.ToArray()[random.Next(candidates.Count)].Id;
        var rsaEncryptedText = rsaEncryption.Encrypt(votedCandidateId.ToString(), publicKey);
        return XorEncryption.EncryptDecrypt(rsaEncryptedText);
    }
}