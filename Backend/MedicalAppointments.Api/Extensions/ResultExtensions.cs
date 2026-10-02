using MedicalAppointments.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return ToProblemResult(controller, result.Error!);
    }

    public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return ToProblemResult<T>(controller, result.Error!);
    }

    public static ActionResult<T> ToCreatedActionResult<T>(
        this Result<T> result,
        ControllerBase controller,
        Func<T, string> locationBuilder)
    {
        if (result.IsSuccess)
        {
            return controller.Created(locationBuilder(result.Value!), result.Value);
        }

        return ToProblemResult<T>(controller, result.Error!);
    }

    private static ActionResult ToProblemResult(ControllerBase controller, Error error) =>
        controller.Problem(
            title: GetTitle(error.Code),
            detail: error.Message,
            statusCode: MapStatusCode(error.Code),
            instance: controller.HttpContext.Request.Path);

    private static ActionResult<T> ToProblemResult<T>(ControllerBase controller, Error error) =>
        controller.Problem(
            title: GetTitle(error.Code),
            detail: error.Message,
            statusCode: MapStatusCode(error.Code),
            instance: controller.HttpContext.Request.Path);

    private static int MapStatusCode(string code) =>
        code switch
        {
            "Validation" => StatusCodes.Status400BadRequest,
            "Unauthorized" => StatusCodes.Status401Unauthorized,
            "Forbidden" => StatusCodes.Status403Forbidden,
            "NotFound" => StatusCodes.Status404NotFound,
            "Conflict" => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(string code) =>
        code switch
        {
            "Validation" => "Validation error",
            "Unauthorized" => "Unauthorized",
            "Forbidden" => "Forbidden",
            "NotFound" => "Resource not found",
            "Conflict" => "Conflict",
            _ => "Unexpected error"
        };
}
