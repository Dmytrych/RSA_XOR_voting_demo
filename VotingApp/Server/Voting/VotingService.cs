// using VotingApp.Common;
// using VotingApp.Server.Domain.Repositories;
//
// namespace VotingApp.Server.Voting;
//
// public class VotingService
// {
//     private readonly VoterRepository voterRepository;
//     private readonly RsaEncryption rsaEncryption;
//     private readonly CandidateRepository candidateRepository;
//
//     public VotingService(
//         VoterRepository voterRepository,
//         RsaEncryption rsaEncryption,
//         CandidateRepository candidateRepository)
//     {
//         this.voterRepository = voterRepository;
//         this.rsaEncryption = rsaEncryption;
//         this.candidateRepository = candidateRepository;
//     }
//
//     public string Vote(int voterId, string encryptedVotingResult, string signature)
//     {
//         var voter = voterRepository.GetVoterById(voterId);
//
//         if (voter == null)
//         {
//             return "The voter is not valid";
//         }
//
//         if (voter.Voted)
//         {
//             return "The voter already voted";
//         }
//
//         var decryptedData = XorEncryption.EncryptDecrypt(encryptedVotingResult);
//
//         if (!ValidateSignature(decryptedData, signature, voter.PublicKey))
//         {
//             return "The signature is invalid";
//         }
//
//         if (!int.TryParse(decryptedData, out int candidateId))
//         {
//             return "The vote was invalid";
//         }
//
//         var candidate = candidateRepository.GetCandidateById(candidateId);
//         if (candidate == null)
//         {
//             return "The candidate with the given Id does not exist";
//         }
//
//         candidate.Votes++;
//         voter.Voted = true;
//         return "Vote counted";
//     }
//
//     private bool ValidateSignature(string realData, string signature, string voterPrivateKey)
//     {
//         var dataHash = Md5Hash.GetHash(realData);
//
//         try
//         {
//             var result = rsaEncryption.Decrypt(signature, voterPrivateKey);
//
//             return result == dataHash;
//         }
//         catch
//         {
//             return false;
//         }
//     }
// }