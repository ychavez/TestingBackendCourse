using Microsoft.AspNetCore.Mvc.Testing;

namespace Course.Api.IntegrationTest;

public sealed class CourseApiFactory : WebApplicationFactory<Program>
{
}

[CollectionDefinition(CourseApiCollection.Name)]
public sealed class CourseApiCollection : ICollectionFixture<CourseApiFactory>
{
    public const string Name = "CourseApi collection";
}
