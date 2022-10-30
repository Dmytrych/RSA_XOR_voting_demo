using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;

namespace VotingApp.Common
{
    public class SigratureService
    {
        public SigratureService()
        {
            AsymmetricCipherKeyPair keyPair = GenerateKeyPair(1024);
            PrivateKey = new RsaKey((RsaKeyParameters)keyPair.Private);
            PublicKey = new RsaKey((RsaKeyParameters)keyPair.Public);
            randomBigInt = GenerateRandomRelativelyPrimeBigInteger();
        }

        public RsaKey PublicKey { get; }

        public BigInteger GetRawMessage(string message)
        {
            return new BigInteger(Encoding.UTF8.GetBytes(message));
        }

        public BigInteger BlindMessage(string message)
        {
            return randomBigInt.ModPow(PublicKey.DorEFactor, PublicKey.NFactor).Multiply(GetRawMessage(message)).Mod(PublicKey.NFactor);
        }
        
        public BigInteger BlindMessage(BigInteger message)
        {
            return randomBigInt.ModPow(PublicKey.DorEFactor, PublicKey.NFactor).Multiply(message).Mod(PublicKey.NFactor);
        }

        public BigInteger SignBlindedMessage(BigInteger blindedMessage)
        {
            return blindedMessage.ModPow(PrivateKey.DorEFactor, PrivateKey.NFactor);
        }

        public BigInteger  UnblindMessage(BigInteger blindedString)
        {
            return randomBigInt.ModInverse(PublicKey.NFactor).Multiply(blindedString).Mod(PublicKey.NFactor);
        }

        public BigInteger UnblindMessage(BigInteger blindedString, BigInteger nFactor, BigInteger randCoef)
        {
            return randCoef.ModInverse(nFactor).Multiply(blindedString).Mod(nFactor);
        }

        public BigInteger GetMsgFromSignedData(BigInteger signed)
        {
            return signed.ModPow(PublicKey.DorEFactor, PublicKey.NFactor);
        }

        public bool VerifySignature(BigInteger unblinded, string dataToValidate)
        {
            //signature of m should = (m^d) mod n
            BigInteger sig_of_m = GetRawMessage(dataToValidate).ModPow(PrivateKey.DorEFactor, PrivateKey.NFactor);
            return unblinded.Equals(sig_of_m);
        }

        private AsymmetricCipherKeyPair GenerateKeyPair(int bitStrength)
        {
            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), bitStrength);
            RsaKeyPairGenerator keyGen = new RsaKeyPairGenerator();

            keyGen.Init(keyGenerationParameters);
            return keyGen.GenerateKeyPair();
        }

        public BigInteger randomBigInt;

        public RsaKey PrivateKey { get; }
        //
        // public BigInteger GetDFactor()
        // {
        //     return PrivateKey.Exponent;
        // }
        //
        // public BigInteger GetNFactor()
        // {
        //     return PublicKey.Modulus;
        // }
        //
        // public BigInteger GetEFactor()
        // {
        //     return PublicKey.Exponent;
        // }


        private BigInteger GenerateRandomRelativelyPrimeBigInteger()
        {
            BigInteger tempRandomBigInt = new BigInteger("0");

            do
            {
                tempRandomBigInt = GenerateRandomBigInteger();
            }
            while (AreRelativelyPrime(tempRandomBigInt, PrivateKey.NFactor));

            return tempRandomBigInt;
        }

        private BigInteger GenerateRandomBigInteger()
        {
            byte[] randomBytes = new byte[20];
            SecureRandom random = new SecureRandom();

            random.NextBytes(randomBytes);
            return new BigInteger(1, randomBytes);
        }

        private bool AreRelativelyPrime(BigInteger first, BigInteger second)
        {
            BigInteger one = new BigInteger("1");
            BigInteger gcd = first.Gcd(second);
            return !gcd.Equals(one) || first.CompareTo(second) >= 0 || first.CompareTo(one) <= 0;
        }
    }
}