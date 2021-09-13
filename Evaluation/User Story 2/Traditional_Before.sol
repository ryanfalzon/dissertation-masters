
pragma solidity >=0.4.22 <0.7.0;

contract SocialNetwork {
    
    struct Profile {
        bytes32 id;
        address publicKey;
        string firstName;
        string lastName;
        string email;
        uint64 mobile;
        string description;
    }
    
    struct Post {
        bytes32 id;
        address author;
        string content;
        uint256 timestamp;
        uint8 likes;
        bool isVisible;
    }
    
    mapping(address => bool) public registeredUsers;
    mapping(address => Profile) public userProfiles;
    mapping(address => bytes32[]) public userPosts;
    mapping(bytes32 => Post) public posts;
    
    function register(string memory firstName, string memory lastName, string memory email, uint64 mobile, string memory description) public {
        assert(!registeredUsers[msg.sender]);
        assert(address(msg.sender).balance >= 1);
        
        bytes32 id = keccak256(abi.encodePacked(msg.sender, firstName, lastName, email, mobile, description, now));
        Profile memory profile = Profile(id, msg.sender, firstName, lastName, email, mobile, description);
        registeredUsers[msg.sender] = true;
        userProfiles[msg.sender] = profile;
    }
    
    function getProfile(address publicKey) public view returns(bytes32, string memory, string memory, string memory, uint64, string memory) {
        assert(registeredUsers[publicKey]);
        
        Profile memory profile = userProfiles[publicKey];
        return(profile.id, profile.firstName, profile.lastName, profile.email, profile.mobile, profile.description);
    }
    
    function editProfile(string memory firstName, string memory lastName, string memory email, uint64 mobile, string memory description) public {
        assert(registeredUsers[msg.sender]);
        
        bytes32 newId = keccak256(abi.encodePacked(msg.sender, firstName, lastName, email, mobile, description, now));
        
        Profile memory profile = userProfiles[msg.sender];
        profile.id = newId;
        profile.firstName = firstName;
        profile.lastName = lastName;
        profile.email = email;
        profile.mobile = mobile;
        profile.description = description;
        
        userProfiles[msg.sender] = profile;
    }
    
    function addPost(string memory content) public {
        assert(registeredUsers[msg.sender]);
        
        uint256 timestamp = now;
        bytes32 id = keccak256(abi.encodePacked(msg.sender, content, timestamp));
        Post memory post = Post(id, msg.sender, content, timestamp, 0, true);
        
        userPosts[msg.sender].push(id);
        posts[id] = post;
    }
    
    function getPost(bytes32 id) public view returns(address, string memory, uint8){
        Post memory post = posts[id];
        assert(post.isVisible);
        return(post.author, post.content, post.likes);
    }
    
    function getUserPosts() public view returns(bytes32[] memory){
        return userPosts[msg.sender];
    }
    
    function editPost(bytes32 id, string memory content) public {
        Post memory post = posts[id];
        assert(post.author == msg.sender);
        post.content = content;
        
        bytes32 newId = keccak256(abi.encodePacked(post.author, post.content, post.timestamp, post.likes));
        post.id = newId;
        
        posts[newId].isVisible = false;
        userPosts[msg.sender].push(newId);
        posts[newId] = post;
    }
    
    function deletePost(bytes32 id) public {
        assert(posts[id].author == msg.sender);
        assert(posts[id].isVisible);
        posts[id].isVisible = false;
    }
    
    function likePost(bytes32 id) public {
        assert(posts[id].isVisible);
        posts[id].likes++;
    }
}