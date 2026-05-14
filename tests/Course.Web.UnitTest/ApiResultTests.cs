using System.Net;
using Course.Web.Models;
using FluentAssertions;

namespace Course.Web.UnitTest;

public class ApiResultTests
{
    [Fact]
    public void Ok_ShouldReturnSuccessResult()
    {
        var result = ApiResult<string>.Ok("data", HttpStatusCode.OK);

        result.Success.Should().BeTrue();
        result.Value.Should().Be("data");
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void Failure_ShouldReturnFailureResult()
    {
        var result = ApiResult<string>.Failure("error", HttpStatusCode.BadRequest);

        result.Success.Should().BeFalse();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Be("error");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void Failure_WithoutStatusCode_ShouldSetNull()
    {
        var result = ApiResult<string>.Failure("error");

        result.Success.Should().BeFalse();
        result.StatusCode.Should().BeNull();
    }
}
