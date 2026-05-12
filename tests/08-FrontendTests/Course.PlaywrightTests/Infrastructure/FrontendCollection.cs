using Xunit;

namespace Course.PlaywrightTests.Infrastructure;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class FrontendCollection : ICollectionFixture<WebAppFixture>
{
    public const string Name = "Frontend";
}
