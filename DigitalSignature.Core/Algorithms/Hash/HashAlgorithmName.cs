namespace DigitalSignature.Core.Algorithms.Hash
{
    /// <summary>
    ///     Funkcija za izračunavanje sažetka poruke (hash funkcija):
    ///     SHA-1, SHA-2 i SHA-3.
    ///     Omogućiti izbor svih raspoloživih inačica algoritama,
    ///     npr. SHA3-256, SHA3-512, itd.
    /// </summary>
    public enum HashAlgorithmName
    {
        SHA1 = 1,
        SHA256 = 256,
        SHA384 = 384,
        SHA512 = 512
    }
}