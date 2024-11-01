using RecommendationsAppApiLibrary.Models;

namespace RecommendationsAppApiLibrary.DataAccess
{
    public interface IUserData
    {
        Task CreateUser(string userId, string username, string role);
        Task LoadUserById(string userId);
        Task<List<UserModel>> LoadUsers();
    }
}