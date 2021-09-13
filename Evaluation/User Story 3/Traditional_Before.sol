
pragma solidity >=0.4.22 <0.7.0;

contract SocialNetwork {
    
    struct Profile {
        uint256 id;
        string hash;
    }
    
    struct Post {
        uint256 id;
        string hash;
    }

    mapping(uint256 => Profile) public profiles;
    mapping(uint256 => Post) public posts;
    
    function addProfileHash(uint256 id, string memory hash) public {        
        Profile memory profile = Profile(id, hash);
        profiles[id] = profile;
    }
    
    function getProfileHash(uint256 id) public view returns(string memory) {
        Profile memory profile = profiles[id];
        return(profile.hash);
    }
    
    function editProfileHash(uint256 id, string memory hash) public {
        Profile memory profile = profiles[id];
        profile.hash = hash;
        profiles[id] = profile;
    }
    
    function addPostHash(uint256 id, string memory hash) public {
        Post memory post = Post(id, hash);
        posts[id] = post;
    }
    
    function getPostHash(uint256 id) public view returns(string memory) {
        Post memory post = posts[id];
        return(post.hash);
    }
    
    function editPostHash(uint256 id, string memory hash) public {
        Post memory post = posts[id];
        post.hash = hash;
        posts[id] = post;
    }
}