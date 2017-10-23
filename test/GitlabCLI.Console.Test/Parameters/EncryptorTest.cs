using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using GitLabCLI.Console.Parameters;
using Xunit;

namespace GitLabCLI.Console.Test.Parameters
{
    public class EncryptorTest
    {
        [Fact]
        public void EncryptedValueCanBeDecrypted()
        {
            Enumerable.Range(1, 100).ToList().ForEach(n =>
            {
                string uniqueText = GetUniqueKey(n);
                AssertEncryptedValueCanBeDecrypted(uniqueText);
            });
        }

        [Fact]
        public void EncryptedSpecialValuesCanBeDecrypted()
        {
            string specialSymbols = "//" + "#" + "." + ";" + "@" + "}" + "[" + "+" + " " + "-";
            Enumerable.Range(1, 100).ToList().ForEach(n =>
            {
                string uniqueText = GetUniqueKey(n) + specialSymbols;
                AssertEncryptedValueCanBeDecrypted(uniqueText);
            });
        }

        [Fact]
        public void EmptyStringCanBeEncryptedAndDecrypted()
        {
            AssertEncryptedValueCanBeDecrypted("");
        }

        private static void AssertEncryptedValueCanBeDecrypted(string text)
        {
            var sut = new Encryptor();
            sut.Decrypt(sut.Encrypt(text)).Should().Be(text);
        }

        private static string GetUniqueKey(int maxSize)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];

            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}
