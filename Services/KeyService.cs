using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Repository;
using System.IO;
using System.Security.Cryptography;

namespace Parmigiano.Services
{
    public class KeyService
    {
        private readonly IUserConfigRepository _userConfig = new UserConfigRepository();

        private readonly string _keysDir;

        public string PrivateKeyPath { get; private set; }
        public string PublicKeyPath { get; private set; }

        public KeyService()
        {
            this._keysDir = Path.Combine(Config.Current.APP_FOLDER_PATH, "keys");
            Directory.CreateDirectory(this._keysDir);

            PrivateKeyPath = Path.Combine(this._keysDir, "private.key");
            PublicKeyPath = Path.Combine(this._keysDir, "public.key");

            if (!File.Exists(PrivateKeyPath) || !File.Exists(PublicKeyPath))
            {
                this.GenerateKeys();

                this._userConfig.Set("rsa_private_key", PrivateKeyPath);
                this._userConfig.Set("rsa_public_key", PublicKeyPath);
            }
        }

        private void GenerateKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(4096))
            {
                var privateKey = rsa.ExportCspBlob(true);
                var publicKey = rsa.ExportCspBlob(false);

                File.WriteAllBytes(PrivateKeyPath, privateKey);
                File.WriteAllBytes(PublicKeyPath, publicKey);
            }
        }

        public RSA GetPrivateKey()
        {
            if (!File.Exists(PrivateKeyPath))
            {
                throw new FileNotFoundException("Файл приватного ключа не найден.", PrivateKeyPath);
            }

            var privateKey = File.ReadAllBytes(PrivateKeyPath);
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(privateKey);

            return rsa;
        }

        public RSA GetPublicKey()
        {
            if (!File.Exists(PublicKeyPath))
            {
                throw new FileNotFoundException("Файл публичного ключа не найден.", PublicKeyPath);
            }

            var publicKey = File.ReadAllBytes(PublicKeyPath);
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(publicKey);

            return rsa;
        }
    }
}
