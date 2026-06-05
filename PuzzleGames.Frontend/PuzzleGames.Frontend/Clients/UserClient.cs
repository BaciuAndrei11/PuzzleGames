using PuzzleGames.Frontend.Models;

namespace PuzzleGames.Frontend.Clients;

public class UserClient(HttpClient httpClient)
{
    public async Task AddUserAsync(User user) => await httpClient.PostAsJsonAsync("users", user);

     public async Task<User> GetUserByUsernameAsync(LoginUser user) => await httpClient.PostAsJsonAsync($"users/login", user).Result.Content.ReadFromJsonAsync<User>() ?? throw new Exception("Could not find user!"); 

     public async Task UpdateGameProgressAsync(int userId, UpdateProgress progressDto)
     {
         var response = await httpClient.PutAsJsonAsync($"gamesProgress/{userId}", progressDto);
         response.EnsureSuccessStatusCode();
     }
}