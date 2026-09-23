using FluentAssertions;
using IdeaForge.Infrastructure;

namespace IdeaForge.Tests.Infrastructure;

public class PostgresConnectionStringTests
{
    [Fact]
    public void Normalize_NeonStyleUrl_ReturnsNpgsqlStringWithSslRequire()
    {
        var result = PostgresConnectionString.Normalize(
            "postgresql://alex:mypass123@ep-cool-123.us-east-2.aws.neon.tech/ideaforge?sslmode=require");

        result.Should().Be(
            "Host=ep-cool-123.us-east-2.aws.neon.tech;Port=5432;Database=ideaforge;Username=alex;Password=mypass123;SSL Mode=Require");
    }

    [Fact]
    public void Normalize_KeyValueInput_ReturnsUnchanged()
    {
        const string keyValue = "Host=localhost;Port=5432;Database=ideaforge;Username=postgres;Password=postgres";

        PostgresConnectionString.Normalize(keyValue).Should().Be(keyValue);
    }

    [Fact]
    public void Normalize_UrlWithoutSslMode_DefaultsToPrefer()
    {
        var result = PostgresConnectionString.Normalize(
            "postgres://bob:secret@db.example.com:5433/appdb");

        result.Should().Be(
            "Host=db.example.com;Port=5433;Database=appdb;Username=bob;Password=secret;SSL Mode=Prefer");
    }

    [Fact]
    public void Normalize_UrlEncodedPassword_DecodesIt()
    {
        var result = PostgresConnectionString.Normalize(
            "postgresql://u:p%40ss%3Aw%21@host/db?sslmode=require");

        result.Should().Contain("Password=p@ss:w!");
    }
}
