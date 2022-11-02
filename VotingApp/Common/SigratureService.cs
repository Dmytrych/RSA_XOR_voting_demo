﻿using System;
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
        public RsaKey PublicRsaKey { get; set; }
        
        public SigratureService()
        {
            AsymmetricCipherKeyPair keyPair = GenerateKeyPair(1024);
            PrivateKey = (RsaKeyParameters)keyPair.Private;
            PublicKey = (RsaKeyParameters)keyPair.Public;
            randomBigInt = GenerateRandomRelativelyPrimeBigInteger();
            PublicRsaKey = new RsaKey(PublicKey);
        }
        
        public SigratureService(RsaKey rsaKey)
        {
            PublicKey = new RsaKeyParameters(false, rsaKey.NFactor, rsaKey.EFactor);
            PublicRsaKey = rsaKey;
            randomBigInt = GenerateRandomRelativelyPrimeBigInteger();
        }

        public RsaKeyParameters PublicKey { get; }

        public BigInteger GetRawMessage(string message)
        {
            return new BigInteger(Encoding.UTF8.GetBytes(message));
        }

        public BigInteger BlindMessage(string message)
        {
            return this.randomBigInt.ModPow(this.GetEFactor(), this.GetNFactor()).Multiply(this.GetRawMessage(message)).Mod(this.GetNFactor());
        }

        public BigInteger SignBlindedMessage(BigInteger blindedMessage)
        {
            return blindedMessage.ModPow(this.GetDFactor(), this.GetNFactor());
        }

        public BigInteger  UnblindMessage(BigInteger blindedString)
        {
            return this.randomBigInt.ModInverse(this.GetNFactor()).Multiply(blindedString).Mod(this.GetNFactor());
        }

        public BigInteger GetMsgFromSignedData(BigInteger signed)
        {
            return signed.ModPow(GetEFactor(), GetNFactor());
        }

        public bool VerifySignature(BigInteger unblinded, string dataToValidate)
        {
            //signature of m should = (m^d) mod n
            BigInteger sig_of_m = GetRawMessage(dataToValidate).ModPow(GetDFactor(), GetNFactor());
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

        public RsaKeyParameters PrivateKey { get; }
        
        public BigInteger GetDFactor()
        {
            return PrivateKey.Exponent;
        }
        
        public BigInteger GetNFactor()
        {
            return PublicKey.Modulus;
        }
        
        public BigInteger GetEFactor()
        {
            return PublicKey.Exponent;
        }


        private BigInteger GenerateRandomRelativelyPrimeBigInteger()
        {
            BigInteger tempRandomBigInt = new BigInteger("0");

            do
            {
                tempRandomBigInt = GenerateRandomBigInteger();
            }
            while (AreRelativelyPrime(tempRandomBigInt, GetNFactor()));

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