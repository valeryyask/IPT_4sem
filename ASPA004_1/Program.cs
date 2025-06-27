﻿using DAL004;
using Microsoft.AspNetCore.Diagnostics;
internal partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        Repository.JSONFileName = "Celebrities.json";
        using (Repository repository = Repository.Create("Celebrities"))
        {
            app.UseExceptionHandler("/Celebrities/Error");
            app.MapGet("/Celebrities", () => repository.GetAllCelebrities());
            app.MapGet("/Celebrities/{id:int}", (int id) =>
            {
                Celebrity? celebrity = repository.GetCelebrityById(id);
                if (celebrity == null) throw new FoundByIdException($"Celebrity Id = {id}");
                return celebrity;
            });

            app.MapPost("/Celebrities", (Celebrity celebrity) =>
            {
                int? id = repository.addCelebrity(celebrity);
                if (id == null)
                    throw new AddCelebrityException("/Celebrities error, id null");
                if (repository.saveChanges() <= 0) throw new SaveException("/Celebrities error, SaveChanges() <= 0");
                return new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
            });
            app.MapFallback((HttpContext ctx) => Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" }));


            app.Map("/Celebrities/Error", (HttpContext ctx) =>
            {
                Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
                IResult rc = Results.Problem(detail: "Panic", instance: app.Environment.EnvironmentName, title: "ASPA004", statusCode: 500);
                if (ex != null)
                {
                    if (ex is FoundByIdException) rc = Results.NotFound(ex.Message);
                    if (ex is BadHttpRequestException) rc = Results.BadRequest(ex.Message);
                    if (ex is SaveException) rc = Results.Problem(title: "ASPA001/SaveChanges", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                    if (ex is AddCelebrityException) rc = Results.Problem(title: "ASPA004/addCelebrity", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                    if (ex is FileNotFoundException) rc = Results.Problem(title: "ASPA00", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                }
                return rc;
            });

            app.Run();
        }
    }
}
public class FoundByIdException : Exception { public FoundByIdException(string message) : base($"Found by Id: {message}") { } }
public class SaveException : Exception { public SaveException(string message) : base($"SaveChanges error:{message}") { } }
public class AddCelebrityException : Exception { public AddCelebrityException(string message) : base($"AddCelebrityException error:{message}") { } };


