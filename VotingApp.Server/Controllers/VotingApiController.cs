using Microsoft.AspNetCore.Mvc;
using VotingApp.Common;
using VotingApp.Server.Dto;
using VotingApp.Server.Services;

namespace VotingApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VotingApiController : ControllerBase
    {
        private readonly IVotingService votingService;
        private static readonly RsaEncryption rsaEncryption;

        public VotingApiController(IVotingService votingService, RsaEncryption rsaEncryption)
        {
            this.votingService = votingService;
            this.rsaEncryption = rsaEncryption;
        }

        [HttpGet("[action]")]
        public SignatureResponse VerifyData(int voterId, IReadOnlyCollection<VotingPackage> votingPackages)
        {
            var results = votingService.VerifyData(votingPackages);
            var key = rsaEncryption.GetPublicKey();
            return new SignatureResponse
            {
                SignedData = results,
                ServerRsaKey = key
            };
        }
    }
}