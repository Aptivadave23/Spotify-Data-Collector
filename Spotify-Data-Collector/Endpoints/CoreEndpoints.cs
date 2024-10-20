using Carter;
using Microsoft.AspNetCore.Mvc;
using ResponseMessages;

public class CoreEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app){
        // Additional routes (make sure to add your other routes here)
        app.MapGet("/", () => Results.Ok("API is running!"))
        .Produces<OkResult>(200)
        .Produces<ErrorResponse>(400, "application/json")
        .Produces<ErrorResponse>(404, "application/json")
        .WithDisplayName("Test Route")
        .WithName("Test Route")
        .WithSummary("This is a test route for the service.")
        .WithDescription("This route is used to test the service and ensure that it is running correctly.")
        .WithTags("Test");
    }
    
}