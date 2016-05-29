namespace ExpressionToSql.UnitTests
{
    using ExpressionToSql;
    using FluentAssertions;
    using Xunit;

    public class SqlTests
    {
        [Fact]
        public void Simple_case_should_produce_select()
        {
            string actual = Sql.Select((Address x) => x.Id).ToString();
            actual.Should().Be("SELECT Id FROM [Address]");
        }
    }
}
