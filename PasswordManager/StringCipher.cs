using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace PasswordManager
{
    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 20 MARCH 2019
    // PURPOSE     : CipherText class for iD Password Manager, encrypt method takes plain text, password and encrypts
    //              decrypt method takes ciphered text, password and decrypts
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    // 
    //
    //==================================
    public static class StringCipher
    {
        //using 256 bit key size
        private const int Keysize = 256;

        //The number of iteration the password derieve bytes will go through the encryption
        private const int iterations = 1000;

        public static string Encrypt(string plainText, string passWord)
        {
            // Salt will be randomly generated each time and will be appended to the end of the encrypted cipher text
            // the IV will be randomly generated every single time and will be added to the end of encrypted text.

            //Salt and IV is created using 256 bits of random Entropy
            byte[] randSaltBytes = new byte[32]; 
            byte[] randIvBytes = new byte[32]; 

            //Gets random bytes using the RNG CryptoServie Provider for Salt and IV
            using(RNGCryptoServiceProvider randNumGenCSP = new RNGCryptoServiceProvider())
            {
                //fills array with cryptographically secured random bytes
                randNumGenCSP.GetBytes(randSaltBytes);
                randNumGenCSP.GetBytes(randIvBytes);
            }
            //Converts plain text to UTF8 encoding
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            //implements password based key derivation functionality PBKDF2 
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passWord, randSaltBytes, iterations))
            {
                //gets keyBytes of 32 bytes which is: 256bit/8 = 32 bytes
                byte[] keyBytes = password.GetBytes(Keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    //key size is 32 byte
                    //key mode is Cypher Block Chaining
                    //Padding mode is PKCS7 which is equal to total number of padding bytes added
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, randIvBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            //CryptoStream used in cryptographic transformation
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                //writes sequence of cyprographic bytes to cryptoStream
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                //updates the buffer
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                byte[] cipherTextBytes = randSaltBytes;
                                //concatinates cipherTextBytes from array
                                cipherTextBytes = cipherTextBytes.Concat(randIvBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                //close memory stream and cryptostream
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        //Method Decrypt, takes cipherText and password and decrypts
        public static string Decrypt(string cipherText, string passWord)
        {
            // gets Stream of bytes which represents
            // 32 bytes of Salt + 32 bytes of IV and n bytes of CipherText
            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // saltbyte by extracting the first 32 bytes from the supplied cipherText bytes.
            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // gets the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8 * 2).
                Take(cipherTextBytesWithSaltAndIv.Length - (Keysize / 8 * 2)).ToArray();

            //uses password based key derivation function passing cipherText, salt and number of iterations
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passWord, saltStringBytes, iterations))
            {
                byte[] keyBytes = password.GetBytes(Keysize / 8);
                //uses managed verison of Rijndael algorithm
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    //key size is 32 byte
                    //key mode is Cypher Block Chaining
                    //Padding mode is PKCS7 which is equal to total number of padding bytes added
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    //performs cyrptographic transformation taking key and iv
                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        //creates stream of memory passing cipherTextBytes
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            //creates crypto stream by taking value from memorystream, decryptor, and mode
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                //instantiates plaintextBytes equal to the cipherTextBytes
                                //decryptedByteCount reads from cryptoStream
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                //Converts to UTF8 and returns it
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }


    }
}
