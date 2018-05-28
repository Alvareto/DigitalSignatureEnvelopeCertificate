namespace DigitalSignature.Web.Controllers
{
    public static class Constants
    {
        public static string START = "---BEGIN OS 2 CRYPTO DATA---";
        public static string END = "---END OS2 CRYPTO DATA---";

        public static string DESCRIPTION = "Description:";
        public static string METHOD = "Method:";
        public static string SIGNATURE = "Signature:";
        public static string KEY_LENGTH = "Key length:";
        public static string MODULUS = "Modulus:";
        public static string PUBLIC_EXPONENT = "Public exponent:";
        public static string PRIVATE_EXPONENT = "Private exponent:";
        public static string SECRET_KEY = "Secret key:";
        public static string FILE_NAME = "File name:";
        public static string DATA = "Data:";
        public static string INIT_VECTOR = "Initialization vector:";
        public static string ENVELOPE_DATA = "Envelope data:";
        public static string ENVELOPE_KEY = "Envelope crypt key:";

        public static string TAB = "    ";

        public static string DESCRIPTION_SECRET_KEY = "Secret key";
        public static string DESCRIPTION_PUBLIC_KEY = "Public key";
        public static string DESCRIPTION_PRIVATE_KEY = "Private key";
        public static string DESCRIPTION_SIGNATURE = "Signature";
        public static string DESCRIPTION_ENVELOPE = "Envelope";
        public static string DESCRIPTION_CRYPTED_FILE = "Crypted file";

        public static int ROW__CHARACTER_COUNT = 60;

        public static class File
        {
            public static class Path
            {
                public static string ENVELOPE = @"\Envelope\";
                public static string SIGNATURE = @"\Signature\";
                public static string CERTIFICATE = @"\Certificate\";

                public static string INPUT = @"\Input\";
                public static string CYPHERTEXT = @"\Cyphertext\";

                public static string SECRET_KEY = @"\Key\";
                public static string PUBLIC_KEY = @"\Key\";
                public static string PRIVATE_KEY = @"\Key\";
            }

            public static class Name
            {
                public static string ENVELOPE = @"Envelope.txt";
                public static string SIGNATURE = @"Signature.txt";
                public static string CERTIFICATE = @"Certificate.txt";

                public static string INPUT = @"Input.txt";
                public static string CYPHERTEXT = @"Cyphertext.txt";

                public static string SECRET_KEY = @"SecretKey.txt";
                public static string PUBLIC_KEY = @"PublicKey.txt";
                public static string PRIVATE_KEY = @"PrivateKey.txt";
            }
        }

    }
}
