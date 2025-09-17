using System;

using AlonBuyumEx.Models;
using AlonBuyumEx.Services;

using Microsoft.AspNetCore.Mvc;

namespace AlonBuyumEx.Domains.DataDomain
{
    public static class DataEndpoints
    {
        private const string baseUrl = "/api/Data";
        public static void MapDataEndPoints(WebApplication app)
        {
            app.MapGet($"{baseUrl}/GetDataById/{{id}}", async ([FromServices] IDataService service, int id ) =>
            {
                var result = await service.GetDataAsync(id);
                return result is null ? Results.NotFound() : Results.Ok(result);
            }).RequireAuthorization("UserPolicy");

            app.MapPost($"{baseUrl}/AddData", async ([FromServices] IDataService service, [FromBody] DataModel dataModel) =>
            {
                var result = await service.AddDataAsync(dataModel);
                return result is not null ? Results.Created($"{baseUrl}/AddData", result) : Results.Problem("Data was not added.");
            }).RequireAuthorization("AdminPolicy");

            app.MapPut($"{baseUrl}/UpdateData", async ([FromServices] IDataService service, [FromBody] DataModel dataModel) =>
            {
                var result = await service.UpdateDataAsync(dataModel);
                return result is not null ? Results.Ok(result) : Results.Problem("Data was not updated");
            }).RequireAuthorization("AdminPolicy");
        }
    }
}
