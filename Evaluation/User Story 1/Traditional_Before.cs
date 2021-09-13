public class Profile
{
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

    public int Author { get; set; }

    public string Content { get; set; }
    
    public long Timestamp { get; set; }

    public int Likes { get; set; }
}

public class SocialNetwork
{
    public IPaymentPortal _paymentPortal { get; set; }

    public SocialNetwork(IPaymentPortal paymentPortal)
    {
        _paymentPortal = paymentPortal ?? throw new ArgumentNullException(nameof(paymentPortal));
    }

    public void Register(Profile profile)
    {
        bool exists = IsUsernamePresentQuery<bool>.Execute(new {profile.Username});
        if(!exists)
        {
            _paymentPortal.ProcessPayment(profile.Username);
            InsertProfileCommand.Execute(new
            {
                profile.Username,
                profile.FirstName,
                profile.LastName,
                profile.Email,
                profile.Mobile,
                profile.Description
            });
        }
    }

    public Profile GetProfile(int id)
    {
        return GetProfileQuery<Profile>.Execute(new {Id = id});
    }

    public void EditProfile(Profile profile)
    {
        bool exists = IsUsernamePresentQuery<bool>.Execute(new {profile.Username});
        if(exists)
        {
            UpdateProfileCommand.Execute(new
            {
                profile.Username,
                profile.FirstName,
                profile.LastName,
                profile.Email,
                profile.Mobile,
                profile.Description
            });
        }
    }

    public void AddPost(Post post)
    {
        bool exists = IsUsernamePresentQuery<bool>.Execute(new {profile.Username});
        if(exists)
        {
            AddPostCommand.Execute(new
            {
                post.Author,
                post.Content,
                post.Timestamp
            });
        }
    }

    public Post GetPost(int id)
    {
        return GetPostQuery<Post>.Execute(new {Id = id});
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
}