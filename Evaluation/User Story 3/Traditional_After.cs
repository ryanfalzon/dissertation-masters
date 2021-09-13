public class Profile {

    public string Id { get; set; }

    public string Hash { get; set; }

    public string Username { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public long Mobile { get; set; }

    public string Description { get; set; }
}

public class Post {

    public int Id { get; set; }

    public string Hash { get; set; }

    public int Author { get; set; }

    public string Content { get; set; }
    
    public long Timestamp { get; set; }

    public int Likes { get; set; }

    public bool IsVisible { get; set; }
}

public class PrivacySettings
{
    public int ProfileId { get; set; }

    public bool IsProfileVisible { get; set; }

    public bool ArePostsVisible { get; set; }
}

public class CredentialManager
{
    public string PublicKey { get; set; }

    public string PrivateKey { get; set; }
}

public class SocialNetwork {

    public IPaymentPortal _paymentPortal { get; set; }

    public CredentialManager credentialManager { get; set; }
    
    public SocialNetwork(IPaymentPortal paymentPortal, CredentialManager credentialManager)
    {
        _paymentPortal = paymentPortal ?? throw new ArgumentNullException(nameof(paymentPortal));
        this.credentialManager = credentialManager ?? throw new ArgumentNullException(nameof(credentialManager));
    }

    public void Register(Profile profile)
    {
        bool exists = IsUsernamePresentQuery<bool>.Execute(new {profile.Username});
        if(!exists)
        {
            _paymentPortal.ProcessPayment(profile.Username);
            var id = InsertProfileCommand.Execute(new
            {
                profile.Username,
                profile.FirstName,
                profile.LastName,
                profile.Email,
                profile.Mobile,
                profile.Description
            });

            var hash = HashObject(profile).ToHexString();

            Connector connector = new Connector(credentialManager.PublicKey, credentialManager.PrivateKey);
            connector.Call("addProfileHash", id, hash);
        }
    }

    public Profile GetProfile(int id)
    {
        var profile = GetProfileQuery<Profile>.Execute(new {Id = id});
        
        Connector connector = new Connector(credentialManager.PublicKey, credentialManager.PrivateKey);
        profile.Hash = connector.Call("getProfileHash", id, hash);

        return profile;
    }

    public void EditProfile(Profile profile)
    {
        bool exists = IsUsernamePresentQuery<bool>.Execute(new {profile.Username});
        if(exists)
        {
            UpdateProfileCommand.Execute(new
            {
                profile.Id,
                profile.Username,
                profile.FirstName,
                profile.LastName,
                profile.Email,
                profile.Mobile,
                profile.Description
            });

            var hash = HashObject(profile).ToHexString();

            Connector connector = new Connector(credentialManager.PublicKey, credentialManager.PrivateKey);
            connector.Call("editProfileHash", profile.Id, hash);
        }
    }

    public void AddPost(Post post)
    {
        bool exists = IsUsernamePresentQuery<bool>.Execute(new {profile.Username});
        if(exists)
        {
            var id = AddPostCommand.Execute(new
            {
                post.Author,
                post.Content,
                post.Timestamp
            });

            var hash = HashObject(post).ToHexString();

            Connector connector = new Connector(credentialManager.PublicKey, credentialManager.PrivateKey);
            connector.Call("addPostHash", id, hash);
        }
    }

    public Post GetPost(int id)
    {
        var post = GetPostQuery<Post>.Execute(new {Id = id});

        Connector connector = new Connector(credentialManager.PublicKey, credentialManager.PrivateKey);
        post.Hash = connector.Call("getPostHash", id, hash);

        return profile;
    }

    public IEnumerable<Post> GetUserPosts(int id)
    {
        return GetUserPostsQuery<Post>.Execute(new {Id = id});
    }

    public void EditPost(Post post)
    {
        bool authorIsCaller = IsPostAuthorMatch<bool>.Execute(new {post.Author});
        if(authorIsCaller)
        {
            EditPostCommand.Execute(new {post.Content});

            var hash = HashObject(post).ToHexString();

            Connector connector = new Connector(credentialManager.PublicKey, credentialManager.PrivateKey);
            connector.Call("editPostHash", post.Id, hash);
        }
    }

    public void DeletePost(int id)
    {
        bool authorIsCaller = IsPostAuthorMatch<bool>.Execute(new {post.Author});
        if(authorIsCaller)
        {
            DeletePostCommand.Execute(new {Id = id});
        }
    }

    public void LikePost(int id)
    {
        LikePostCommand.Execute(new {Id = id});
    }

    public PrivacySettings GetPrivacySettings(int id)
    {
        return GetPrivacySettings<PrivacySettings>.Execute(new {Id = id});
    }

    public void EditPrivacySettings(PrivacySettings privacySettings)
    {
        bool ownerIsCaller = IsProfileOwnerMatch<bool>.Execute(new {privacySettings.ProfileId});
        if(ownerIsCaller)
        {
            EditPrivacySettingsCommand.Execute(new
            {
                privacySettings.ProfileId,
                privacySettings.IsProfileVisible,
                privacySettings.ArePostsVisible
            });
        }
    }

    private byte[] HashObject(object @object)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@object)));
        }
    }

    private string ToHexString(this byte[] data)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            stringBuilder.Append(data[i].ToString("x2"));
        }
        return stringBuilder.ToString();
    }
}

public class Connector
{
    private readonly Web3 web3;
    private readonly string publicKey;
    private readonly string abi;
    private readonly string contractAddress;

    public Connector(string publicKey, string privateKey)
    {
        this.publicKey = publicKey;
        web3 = new Web3(new Account(privateKey));
    }

    public TransactionReceipt Call(string functionName, params object[] functionInput)
    {
        var contract = web3.Eth.GetContract(abi, contractAddress);
        var function = contract.GetFunction(functionName);

        var gas = function.EstimateGasAsync(publicKey, null, null, functionInput).Result;
        var result = function.SendTransactionAndWaitForReceiptAsync(publicKey, gas, null, null, functionInput: functionInput).Result;

        return result;
    }
}