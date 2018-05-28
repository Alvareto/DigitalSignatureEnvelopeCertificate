//using Org.BouncyCastle.Crypto.Digests;

//namespace DigitalSignature.Core.Algorithms.Hash.SHA2
//{
//    public class SHA3 : IHashAlgorithm
//    {
//        protected readonly Sha3Digest Algorithm;
//        public HashAlgorithmName AlgorithmName => HashAlgorithmName.SHA256;

//        public SHA3(int hashStrength)
//        {
//            Algorithm = new Sha3Digest(hashStrength);
//        }

//        public byte[] Calculate(byte[] input)
//        {
//            Algorithm.Update(input);
//            byte[] result = new byte[Algorithm.GetDigestSize()];
//            Algorithm.DoFinal(result, 0);
//            return result;

//        }
//    }
//}

