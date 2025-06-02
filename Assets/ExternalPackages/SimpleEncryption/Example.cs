using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleEncryption
{
    /// <summary>
    /// Usage example.
    /// </summary>
    public class Example : MonoBehaviour
    {
        public Text Text;

        /// <summary>
        /// Execute on start.
        /// </summary>
        public void Start()
        {
            const string sample = "Hello, world!";

	        string publicKey, privateKey;

			RSA.GenerateKeys(out publicKey, out privateKey);

	        var signature = RSA.SignData(sample, privateKey);

			Text.text = string.Format("Plain text: {0}\nMd5 hash: {1}\nBase64 encoding: {2}\nB64R encoding: {3}\nB64X encryption: {4}\nAES encryption: {5}\nRSA encryption: {6}\nRSA signature: {7}\nRSA signature check: {8}",
                sample,
                Md5.ComputeHash(sample),
                Base64.Encode(sample),
                B64R.Encode(sample),
                B64X.Encrypt(sample, "password"),
                AES.Encrypt(sample, "password"),
				RSA.EncryptText(sample, publicKey),
				signature,
				RSA.CheckSignature(sample, signature, publicKey) ? "valid" : "invalid");
        }
    }
}