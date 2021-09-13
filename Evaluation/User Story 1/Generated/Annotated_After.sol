pragma solidity >=0.4.22 <0.7.0;
contract SocialNetwork{
	struct Profile{
		bytes32 Id;
		address PublicKey;
		string FirstName;
		string LastName;
		string Email;
		uint64 Mobile;
		string Description;
	}
	struct Post{
		bytes32 Id;
		address Author;
		string Content;
		uint256 Timestamp;
		uint8 Likes;
		bool IsVisible;
	}

	mapping(address => bool) public registeredUsers;
	mapping(address => Profile) public userProfiles;
	mapping(address => bytes32[]) public userPosts;
	mapping(bytes32 => Post) public posts;

	function Register(string memory FirstName, string memory LastName, string memory Email, uint64 Mobile, string memory Description) public{
		assert(!registeredUsers[msg.sender]);
		assert(address(msg.sender).balance >= 1);
		bytes32 id = keccak256(abi.encodePacked(msg.sender, FirstName, LastName, Email, Mobile, Description, now));
		Profile memory profile = Profile(id, msg.sender, FirstName, LastName, Email, Mobile, Description);
		registeredUsers[msg.sender] = true;
		userProfiles[msg.sender] = profile;
	}
	function GetProfile(address publicKey) public returns (bytes32, address, string memory, string memory, string memory, uint64, string memory) {
		assert(registeredUsers[publicKey]);
		Profile memory profile = userProfiles[publicKey];
		return(profile.Id, profile.PublicKey, profile.FirstName, profile.LastName, profile.Email, profile.Mobile, profile.Description);
	}
	function EditProfile(string memory FirstName, string memory LastName, string memory Email, uint64 Mobile, string memory Description) public{
		assert(registeredUsers[msg.sender]);
		bytes32 newId = keccak256(abi.encodePacked(msg.sender, FirstName, LastName, Email, Mobile, Description, now));
		Profile memory profile = userProfiles[msg.sender];
		profile.Id = newId;
		profile.FirstName = profile.FirstName;
		profile.LastName = profile.LastName;
		profile.Email = profile.Email;
		profile.Mobile = profile.Mobile;
		profile.Description = profile.Description;
		userProfiles[msg.sender] = profile;
	}
	function AddPost(string memory Content, uint256 Timestamp, uint8 Likes, bool IsVisible) public{
		assert(registeredUsers[msg.sender]);
		uint256 timestamp = now;
		bytes32 id = keccak256(abi.encodePacked(msg.sender, Content, Timestamp));
		Post memory post = Post(id, msg.sender, Content, Timestamp, 0, true);
		userPosts[msg.sender].push(id);
		posts[id] = post;
	}
	function GetPost(bytes32 id) public returns (bytes32, address, string memory, uint256, uint8, bool) {
		Post memory post = posts[id];
		assert(post.IsVisible);
		return(post.Id, post.Author, post.Content, post.Timestamp, post.Likes, post.IsVisible);
	}
	function GetUserPosts() public returns (bytes32[] memory) {
		return userPosts[msg.sender];
	}
	function EditPost(bytes32 Id, string memory Content, uint256 Timestamp, uint8 Likes, bool IsVisible) public{
		Post memory post = posts[Id];
		assert(post.Author == msg.sender);
		post.Content = Content;
		bytes32 newId = keccak256(abi.encodePacked(msg.sender, Content, Timestamp, Likes));
		post.Id = newId;
		posts[newId].IsVisible = false;
		userPosts[msg.sender].push(newId);
		posts[newId] = post;
	}
	function DeletePost(bytes32 id) public{
		assert(posts[id].Author == msg.sender);
		assert(posts[id].IsVisible);
		posts[id].IsVisible = false;
	}
	function LikePost(bytes32 id) public{
		assert(posts[id].IsVisible);
		posts[id].Likes++;
	}
}