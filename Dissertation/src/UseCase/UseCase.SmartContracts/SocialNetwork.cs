using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;

namespace SmartContracts
{
    [ManifestExtra("Author", "Ryan Falzon")]
    [ManifestExtra("Email", "ryan.falzon.15@um.edu.mt")]
    [ManifestExtra("Description", "This is a smart contract that allows decentralized social networking")]
    [Features(ContractFeatures.HasStorage)]
    public class SocialNetwork : SmartContract
    {
        #region Contract Method

        [DisplayName("addTransaction")]
        public static bool AddTransaction(byte[] hash, string content, byte[] caller)
        {
            if (hash == null || content == null || caller == null)
            {
                throw new Exception("One or more parameters is null!");
            }

            // Ensure user calling the method is the user updating the post
            if (!Runtime.CheckWitness(caller)) throw new Exception();

            // Put transaction in storage
            StorageMap Transactions = Storage.CurrentContext.CreateMap(nameof(Transactions));
            Transactions.Put(hash, content);

            return true;
        }

        [DisplayName("getTransaction")]
        public static string GetTransaction(byte[] hash, byte[] caller)
        {
            // Validate input details
            if (hash == null || caller == null)
            {
                throw new Exception("One or more parameters is null!");
            }

            // Ensure user calling the method is the user updating the post
            if (!Runtime.CheckWitness(caller)) throw new Exception();

            StorageMap Transactions = Storage.CurrentContext.CreateMap(nameof(Transactions));
            string content = Transactions.Get(hash).ToByteString();

            return content;
        }

        [DisplayName("updatePrivacySettings")]
        public static bool AddPrivacySettings(string user, string privacySettings, byte[] caller)
        {
            // Validate input details
            if (user == null || privacySettings == null || caller == null)
            {
                throw new Exception("One or more parameters is null!");
            }

            // Ensure user calling the method is the user updating the post
            if (!Runtime.CheckWitness(caller)) throw new Exception();

            // Put privacy settings in storage
            StorageMap PrivacySettings = Storage.CurrentContext.CreateMap(nameof(PrivacySettings));
            PrivacySettings.Put(user, privacySettings);

            return true;
        }

        [DisplayName("getPrivacySettings")]
        public static string GetPrivacySettings(string user, byte[] caller)
        {
            // Validate input details
            if (user == null || caller == null)
            {
                throw new Exception("One or more parameters is null!");
            }

            // Ensure user calling the method is the user updating the post
            if (!Runtime.CheckWitness(caller)) throw new Exception();

            StorageMap PrivacySettings = Storage.CurrentContext.CreateMap(nameof(PrivacySettings));
            string privacySettings = PrivacySettings.Get(user).ToByteString();

            return privacySettings;
        }

        #endregion
    }
}