using Microsoft.AspNetCore.Mvc;
using RAYS.Services;

public class FriendsListViewComponent : ViewComponent
{
    private readonly FriendService _friendService;
    private readonly UserService _userService;

    public FriendsListViewComponent(FriendService friendService, UserService userService)
    {
        _friendService = friendService;
        _userService = userService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int userId)
    {
        var friends = await GetFriends(userId);
        return View(friends); // Render the ViewComponent view with the friends list
    }

    private async Task<List<dynamic>> GetFriends(int userId)
    {
        var friends = await _friendService.GetFriendsAsync(userId);
        var friendViewModels = new List<dynamic>(); // Use dynamic to avoid specific model

        foreach (var friend in friends)
        {
            var friendId = friend.ReceiverId == userId ? friend.SenderId : friend.ReceiverId;
            if (friendId != userId)
            {
                var user = await _userService.GetUserById(friendId);
                if (user != null)
                {
                    friendViewModels.Add(new
                    {
                        UserId = user.Id,
                        Username = user.Username
                    });
                }
            }
        }
        return friendViewModels;
    }
}
