﻿using System;
using System.Security.Cryptography;

namespace e_me.Core.Communication
{
    public class EcdhKeyStore : IDisposable
    {
        public ECDiffieHellmanPublicKey PublicKey { get; set; }

        public ECDiffieHellmanPublicKey OtherPartyPublicKey { get; private set; }

        public byte[] AesKey { get; private set; }

        public byte[] HmacKey { get; private set; }

        public byte[] DerivedHmacKey { get; private set; }

        public ECDiffieHellmanCng DiffieHellmanCng { get; }

        public EcdhKeyStore()
        {
            DiffieHellmanCng = new ECDiffieHellmanCng
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hmac,
                HashAlgorithm = CngAlgorithm.Sha256,
            };
            PublicKey = DiffieHellmanCng.PublicKey;
        }

        public EcdhKeyStore(byte[] publicKey) : this()
        {
            var ecdhKey = new ApplicationEcDiffieHellmanPublicKey(publicKey);
            SetOtherPartyPublicKey(ecdhKey);
        }

        public void SetOtherPartyPublicKey(ECDiffieHellmanPublicKey publicKey)
        {
            OtherPartyPublicKey = publicKey;
            AesKey = DiffieHellmanCng.DeriveKeyMaterial(publicKey);
            HmacKey = AesKey;
            DiffieHellmanCng.HmacKey = AesKey;
            DerivedHmacKey = DiffieHellmanCng.DeriveKeyFromHmac(OtherPartyPublicKey, HashAlgorithmName.SHA256, HmacKey);
        }

        public void Dispose()
        {
            DiffieHellmanCng.Dispose();
        }
    }
}