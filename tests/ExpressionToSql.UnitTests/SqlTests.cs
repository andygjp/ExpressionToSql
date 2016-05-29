namespace ExpressionToSql.UnitTests
{
    using System.IO.Compression;
    using ExpressionToSql;
    using FluentAssertions;
    using Xunit;

    public class SqlTests
    {
        [Fact]
        public void Simple_select_with_no_attributes_should_produce_select()
        {
            var actual = Sql.Select((Address x) => 1).ToString();
            actual.Should().Be("SELECT 1 FROM [dbo].[Address] AS a");
        }

        [Fact]
        public void Simple_select_should_produce_select()
        {
            var actual = Sql.Select((Address x) => x.Id).ToString();
            actual.Should().Be("SELECT a.[Id] FROM [dbo].[Address] AS a");
        }

        [Fact]
        public void Simple_select_with_many_attributes_should_produce_select()
        {
            var actual = Sql.Select((Address x) => new { x.Id, x.Address1 }).ToString();
            actual.Should().Be("SELECT a.[Id], a.[Address1] FROM [dbo].[Address] AS a");
        }

        [Fact]
        public void Simple_where_should_produce_select()
        {
            var actual = Sql.Select((Address x) => x.Id).Where(x => x.Id == 1).ToString();
            actual.Should().Be("SELECT a.[Id] FROM [dbo].[Address] AS a WHERE a.[Id] = 1");
        }

        [Fact]
        public void Simple_negative_where_should_produce_select()
        {
            var actual = Sql.Select((Address x) => 1).Where(x => x.Id != 1).ToString();
            actual.Should().Be("SELECT 1 FROM [dbo].[Address] AS a WHERE a.[Id] <> 1");
        }

        [Fact]
        public void Simple_anded_where_should_produce_select()
        {
            const int id = 10;
            var postcode = "BL";
            var actual = Sql.Select((Address x) => new { x.Id, x.Postcode}).Where(x => x.Id > id && x.Postcode != postcode).ToString();
            actual.Should().Be("SELECT a.[Id], a.[Postcode] FROM [dbo].[Address] AS a WHERE a.[Id] > 10 AND a.[Postcode] <> @postcode");
        }

        [Fact]
        public void Queries_with_parameters_should_produce_select()
        {
            const int a = 1;
            int b = 2;
            var actual = Sql.Select((Address x) => new {a, b}).ToString();
            actual.Should().Be("SELECT 1, @b FROM [dbo].[Address] AS a");
        }

        [Fact]
        public void Queries_with_mixed_parameters_should_produce_select()
        {
            int a = 1;
            var actual = Sql.Select((Address x) => new { a, x.Id }).ToString();
            actual.Should().Be("SELECT @a, a.[Id] FROM [dbo].[Address] AS a");
        }

        [Fact]
        public void Queries_with_select_parameter_should_produce_select()
        {
            int a = 1;
            var actual = Sql.Select((Address x) => a).ToString();
            actual.Should().Be("SELECT @a FROM [dbo].[Address] AS a");
        }

        [Fact]
        public void Queries_with_constant_should_produce_select()
        {
            const int a = 1;
            var actual = Sql.Select((Address x) => a).ToString();
            actual.Should().Be("SELECT 1 FROM [dbo].[Address] AS a");
        }
    }
}
