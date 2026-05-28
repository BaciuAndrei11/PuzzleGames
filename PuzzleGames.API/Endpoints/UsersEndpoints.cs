using Microsoft.EntityFrameworkCore;
using PuzzleGames.API.Data;
using PuzzleGames.API.Dtos;
using PuzzleGames.API.Entities;
using PuzzleGames.API.Mapping;
using PuzzleGames.API.Services;

namespace PuzzleGames.API.Endpoints;

public static class UsersEndpoints
{
    const string GetUserEndpointName = "GetUser";

    public static RouteGroupBuilder MapUsersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("users")
            .WithParameterValidation();
        
        //GET /users
        group.MapGet("/", async (IUserService userService) => await userService.GetUsersAsync());

        //GET /users/1
        group.MapGet("/{id}", async (int id, IUserService userService) =>
        {
            var user = await userService.GetUserAsync(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        }).WithName(GetUserEndpointName);

        //POST /users
        group.MapPost("/", async (CreateUserDto newUser, IUserService userService) =>
        {
            var createdUser= await userService.CreateUserAsync(newUser);

            return Results.CreatedAtRoute(GetUserEndpointName, new { id = createdUser.Id }, createdUser);
        });

        //PUT /users/1
        group.MapPut("/{id}", async (int id, UpdateUserDto updatedUser,  IUserService userService) =>
        {
            await userService.UpdateUserAsync(id, updatedUser);
            return Results.NoContent();
        });

        //DELETE /users/1
        group.MapDelete("/{id}", async (int id,  IUserService userService) =>
        {
            await userService.DeleteUserAsync(id);
            return Results.NoContent();
        });
        
        return group;
    }
}