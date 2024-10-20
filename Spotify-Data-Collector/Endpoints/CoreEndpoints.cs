using Carter;
using Microsoft.AspNetCore.Mvc;

public class CoreEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app){
        // Additional routes (make sure to add your other routes here)
        app.MapGet("/", () => Results.Ok("API is running!"))
        .WithName("Test Route")
        .WithSummary("This is a test route for the service.")
        .WithDescription("This route is used to test the service and ensure that it is running correctly.")
        .WithTags("Test");
    }
    
}