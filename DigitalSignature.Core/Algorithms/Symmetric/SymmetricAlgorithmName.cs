namespace DigitalSignature.Core.Algorithms.Symmetric
{
    /// <summary>
    /// Simetrični kriptosustav: AES i 3-DES. 
    /// Ponuditi na izbor sve moguće veličine ključeva za svaki algoritam 
    /// te nekoliko načina kriptiranja (ECB, CTR, ...).
    /// </summary>
    public enum SymmetricAlgorithmName
    {
        AES = 0,
        TripleDES = 3
    }
}