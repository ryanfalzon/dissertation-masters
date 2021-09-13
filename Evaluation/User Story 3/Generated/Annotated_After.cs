public class Profile
{
	public int Id { get; set; }

	public string Hash { get; set; }

	public string Username { get; set; }

	public string FirstName { get; set; }

	public string LastName { get; set; }

	public string Email { get; set; }

	public int Mobile { get; set; }

	public string Description { get; set; }
}

public class Post
{
	public int Id { get; set; }

	public string Hash { get; set; }

	public string Author { get; set; }

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

using UnifiedModel.Connectors;
using UnifiedModel.Connectors.Ethereum;

public class SocialNetwork
{
	public SocialNetwork (IPaymentPortal paymentPortal)
	{
		_paymentPortal = paymentPortal ?? throw new ArgumentNullException(nameof(paymentPortal));;
	}

	public void Register (Profile profile)
	{
		bool exists = IsUsernamePresentQuery<bool>.Execute(new {profile.Username});;
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

            var hash = profile.Hash();
            AddProfileHash(id, hash);
        };
	}

	public void AddProfileHash (int id, string hash)
	{
		XCall("Ethereum", "AddProfileHash", id, hash);
	}

	public Profile GetProfile (int id)
	{
		var profile = GetProfileQuery<Profile>.Execute(new {Id = id});;
		profile.Hash = GetProfileHash();;
		return profile;;
	}

	public string GetProfileHash (int id)
	{
		return XCall("Ethereum", "GetProfileHash", id);
	}

	public void EditProfile (Profile profile)
	{
		bool exists = IsUsernamePresentQuery<bool>.Execute(new {profile.Username});;
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

            var hash = profile.Hash();
            EditProfileHash(profile.Id, hash);
        };
	}

	public void EditProfileHash (int id, string hash)
	{
		XCall("Ethereum", "EditProfileHash", id, hash);
	}

	public void AddPost (Post post)
	{
		bool exists = IsUsernamePresentQuery<bool>.Execute(new {profile.Username});;
		if(exists)
        {
            var id = AddPostCommand.Execute(new
            {
                post.Author,
                post.Content,
                post.Timestamp
            });

            var hash = post.Hash();
            AddPostHash(id, hash);
        };
	}

	public void AddPostHash (int id, string hash)
	{
		XCall("Ethereum", "AddPostHash", id, hash);
	}

	public Post GetPost (int id)
	{
		var post = GetPostQuery<Post>.Execute(new {Id = id});;
		post.Hash = GetPostHash();;
		return profile;;
	}

	public string GetPostHash (int id)
	{
		return XCall("Ethereum", "GetPostHash", id);
	}

	public IEnumerable<string> GetUserPosts (address publicKey)
	{
		return GetUserPostsQuery<Post>.Execute(new {Id = id});;
	}

	public void EditPost (Post post)
	{
		bool authorIsCaller = IsPostAuthorMatch<bool>.Execute(new {post.Author});;
		if(authorIsCaller)
        {
            EditPostCommand.Execute(new {post.Content});

            var hash = post.Hash();
            EditPostHash(post.Id, hash);
        };
	}

	public void EditPostHash (int id, string hash)
	{
		XCall("Ethereum", "EditPostHash", id, hash);
	}

	public void DeletePost (int id)
	{
		bool authorIsCaller = IsPostAuthorMatch<bool>.Execute(new {post.Author});;
		if(authorIsCaller)
        {
            DeletePostCommand.Execute(new {Id = id});
        };
	}

	public void LikePost (int id)
	{
		LikePostCommand.Execute(new {Id = id});;
	}
	
	public void EditPrivacySettings (PrivacySettings privacySettings)
	{
		bool ownerIsCaller = IsProfileOwnerMatch<bool>.Execute(new {privacySettings.ProfileId});;
		if(ownerIsCaller)
        {
            EditPrivacySettingsCommand.Execute(new
            {
                privacySettings.ProfileId,
                privacySettings.IsProfileVisible,
                privacySettings.ArePostsVisible
            });
        };
	}
}