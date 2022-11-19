using System.Text;
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
        private readonly RsaEncryption rsaEncryption;

        public VotingApiController(
            IVotingService votingService,
            RsaEncryption rsaEncryption)
        {
            this.votingService = votingService;
            this.rsaEncryption = rsaEncryption;
        }

        [HttpPost("[action]")]
        public SignatureResponse VerifyData(VerificarionRequest request)
        {
            var results = votingService.VerifyData(request.VoterId, request.VotingPackages);
            var key = rsaEncryption.GetPublicKey();
            return new SignatureResponse
            {
                SignedData = results.Select(result => result.ToByteArray()).ToList(),
                ServerRsaKey = key
            };
        }

        [HttpPost("[action]")]
        public string Vote(SignedVotingPaper votingPaper)
        {
            return votingService.Vote(votingPaper);
        }

        [HttpGet("[action]")]
        public NetworkPublicKey GetKeys()
        {
            var key = votingService.GetPublicKey();
            return new NetworkPublicKey
            {
                EFactor = key.EFactor.ToByteArray(),
                NFactor = key.NFactor.ToByteArray()
            };
        }
    }
}