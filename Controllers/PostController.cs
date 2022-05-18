using System.ComponentModel.DataAnnotations;
using DvorakEnd.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DvorakEnd.Controllers;

public static class PostControllerExtensions
{
    private static async Task<(Blog?, IResult?)> GetBlogAsync(BlogsContext db, string id)
    {
        if (!Guid.TryParse(id, out var gId))
        {
            return (null, Results.BadRequest($"{id} can't parse to Guid"));
        }
        var blog = await db.Blogs.FirstOrDefaultAsync(blog => blog.Id == gId);
        if (blog is null)
        {
            return (null, Results.NotFound("article not found"));
        }
        return (blog, null);
    }

    public static WebApplication UsePostController(this WebApplication web)
    {
        web.MapGet("/blogs", async ([FromQuery] string? s, BlogsContext db) =>
        {
            var blogs = string.IsNullOrWhiteSpace(s)
                ? await db.Blogs.ToListAsync()
                : await db.Blogs.Where(blog => blog.Title.Contains(s) || blog.Category.Contains(s)).ToListAsync();

            return Results.Ok(blogs);
        });

        web.MapGet("/blogs-categories", async (BlogsContext db) =>
        {
            var categories = await db.Blogs.GroupBy(blog => blog.Category)
                                           .Select(blog => blog.Key)
                                           .ToListAsync();

            return Results.Ok(categories);
        });

        web.MapGet("/blogs/{id}", async ([FromRoute] string id, BlogsContext db) =>
        {
            var (blog, result) = await GetBlogAsync(db, id);
            if (blog == null || result != null)
            {
                return result;
            }

            return Results.Ok(blog);
        });

        web.MapPost("/blogs", [Authorize("AuthorOnly")] async ([FromBody, Required] PostRequest input, BlogsContext db) =>
        {
            await db.Blogs.AddAsync(new()
            {
                Id = Guid.NewGuid(),
                Title = input.Title,
                Category = input.Category,
                Description = input.Description,
                Body = input.Body
            });
            await db.SaveChangesAsync();
        });

        web.MapPut("/blogs/{id}", [Authorize("AuthorOnly")] async ([FromRoute] string id, [FromBody, Required] PostRequest input, BlogsContext db) =>
        {
            var (blog, result) = await GetBlogAsync(db, id);
            if (blog == null || result != null)
            {
                return result;
            }

            blog.Title = input.Title;
            blog.Category = input.Category;
            blog.Description = input.Description;
            blog.Body = input.Body;
            blog.UpdateAt = DateTimeOffset.Now.ToString();

            await db.SaveChangesAsync();

            return Results.Ok(blog);
        });

        web.MapDelete("/blogs/{id}", [Authorize("AuthorOnly")] async ([FromRoute] string id, BlogsContext db) =>
        {
            var (blog, result) = await GetBlogAsync(db, id);
            if (blog == null || result != null)
            {
                return result;
            }

            db.Blogs.Remove(blog);
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        return web;
    }

    public class PostRequest : Blog
    { }
}
