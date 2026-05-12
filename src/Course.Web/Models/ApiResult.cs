using System.Net;

namespace Course.Web.Models;

public sealed record ApiResult<T>(bool Success, T? Value, string? ErrorMessage, HttpStatusCode? StatusCode)
{
    public static ApiResult<T> Ok(T value, HttpStatusCode statusCode)
    {
        return new ApiResult<T>(true, value, null, statusCode);
    }

    public static ApiResult<T> Failure(string errorMessage, HttpStatusCode? statusCode = null)
    {
        return new ApiResult<T>(false, default, errorMessage, statusCode);
    }
}
