pragma solidity >=0.4.22 <0.7.0;
contract SocialNetwork{
	struct Profile{
		uint128 Id;
		string Hash;
	}
	struct Post{
		uint128 Id;
		string Hash;
	}
	struct PrivacySettings{
		uint128 ProfileId;
		bool IsProfileVisible;
		bool ArePostsVisible;
	}

	mapping(uint128 => Profile) public profiles;
	mapping(uint128 => Post) public posts;
	mapping(uint128 => PrivacySettings) public privacySettings;

	function AddProfileHash(uint128 id, string memory hash) public{
		Profile memory profile = Profile(id, hash);
		profiles[id] = profile;
	}
	function GetProfileHash(uint128 id) public returns (string memory) {
		Profile memory profile = profiles[id];
		return(profile.Hash);
	}
	function EditProfileHash(uint128 id, string memory hash) public{
		Profile memory profile = profiles[id];
		profile.Hash = hash;
		profiles[id] = profile;
	}
	function AddPostHash(uint128 id, string memory hash) public{
		Post memory post = Post(id, hash);
		posts[id] = post;
	}
	function GetPostHash(uint128 id) public returns (string memory) {
		Post memory post = posts[id];
		return(post.Hash);
	}
	function EditPostHash(uint128 id, string memory hash) public{
		Post memory post = posts[id];
		post.Hash = hash;
		posts[id] = post;
	}
	function EditPrivacySettings(uint128 ProfileId, bool IsProfileVisible, bool ArePostsVisible) public{
		PrivacySettings memory privacySetting = PrivacySettings(ProfileId, IsProfileVisible, ArePostsVisible);
		privacySettings[ProfileId] = privacySetting;
	}
}