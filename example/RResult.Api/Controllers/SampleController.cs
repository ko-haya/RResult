namespace RResult.Api.Controllers;

using Microsoft.AspNetCore.Http.HttpResults;
using RResult.Api.DomainModels;

public readonly record struct SampleController
{
    public static async Task<Results<Ok<User>, NotFound<string>, UnprocessableEntity<string>, BadRequest<string>>> SampleGet(int id = 1) =>
        await User.Find(id)
              .Map(User.AppendMeta)
              .AndThen(User.Validate)
              .AndThenAsync(User.Update)
              .AndThenAsync(User.WriteMail)
              .InspectAsync(it => Console.WriteLine($"PutLog: {it.Text}"))
              .AndThenAsync(User.SendMail) switch
        {
            { IsOk: true, Unwrap: var ok } => TypedResults.Ok(ok),
            { IsOk: false, UnwrapErr: var err }
                when err.apiErr is ApiErr.NotFound => TypedResults.NotFound(err.desc),
            { IsOk: false, UnwrapErr: var err }
                when err.apiErr is ApiErr.Unknown => TypedResults.UnprocessableEntity(err.desc),
            { IsOk: false, UnwrapErr: var err } => TypedResults.BadRequest(err.desc),
        };
}
