using PuzzleGames.Frontend.Models;

namespace PuzzleGames.Frontend.Clients;

public class UserClient(HttpClient httpClient)
{
    public async Task<User[]> GetUsersAsync() => await httpClient.GetFromJsonAsync<User[]>("users") ?? [];
    
    public async Task AddUserAsync(User user) => await httpClient.PostAsJsonAsync("users", user);

    public async Task<User> GetUserAsync(int id) => await httpClient.GetFromJsonAsync<User>($"users/{id}") ?? throw new Exception("Could not find user!"); 
    public async Task<User> GetUserByUsernameAsync(LoginUser user) => await httpClient.PostAsJsonAsync($"users/login", user).Result.Content.ReadFromJsonAsync<User>() ?? throw new Exception("Could not find user!"); 

    public async Task UpdateUserAsync(User updatedUser) => await httpClient.PutAsJsonAsync($"users/{updatedUser.Id}", updatedUser);

    public async Task DeleteUserAsync(int id) => await httpClient.DeleteAsync($"users/{id}");
}