using Org.BouncyCastle.Math;
using VotingApp.Common;

namespace VotingApp.Server.Dto;

public class NetworkPublicKey
{
    public byte[] NFactor { get; set; }

    public byte[] EFactor { get; set; }

    public RsaKey ToRsaKey()
    {
        return new RsaKey(new BigInteger(EFactor), new BigInteger(NFactor));
    }
}