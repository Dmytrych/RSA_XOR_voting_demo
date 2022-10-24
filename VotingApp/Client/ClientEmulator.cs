using System.Security.Cryptography;
using System.Text;
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

    public (string data, string signature) GetSecureRandomVote(IReadOnlyCollection<Candidate> candidates, string publicKey)
    {
        var votedCandidateId = candidates.ToArray()[random.Next(candidates.Count)].Id.ToString();
        var signature = GetSignature(votedCandidateId, publicKey);
        return (XorEncryption.EncryptDecrypt(votedCandidateId), signature);
    }

    private string GetSignature(string text, string publicKey)
    {
        var hash = Md5Hash.GetHash(text);
        return rsaEncryption.Encrypt(hash, publicKey);
    }
}