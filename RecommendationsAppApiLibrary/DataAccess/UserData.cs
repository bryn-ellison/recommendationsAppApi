using RecommendationsAppApiLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationsAppApiLibrary.DataAccess;

public class UserData : IUserData
{
    private readonly ISqliteDataAccess _db;
    private readonly string connectionStringName = "Default";
    public UserData(ISqliteDataAccess db)
    {
        _db = db;
    }

    public Task<List<UserModel>> LoadUsers()
    {
        string sqlQuery = "select * from Users;";

        return _db.LoadData<UserModel, dynamic>(sqlQuery, "", connectionStringName);
    }

    public Task LoadUserById(string userId)
    {
        string sqlQuery = @"select * from Users where Id = @UserId;";

        return _db.LoadData<UserModel, dynamic>(sqlQuery, new { UserId = userId }, connectionStringName);
    }

    public Task CreateUser(string userId, string username, string role)
    {
        string sqlQuery = @"insert into Users (Id, Username, Role) values (@Id, @UserName, @Role);";

        return _db.SaveData<dynamic>(sqlQuery, new { Id = userId, Username = username, Role = role }, connectionStringName);
    }
}
