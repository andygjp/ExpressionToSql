namespace ExpressionToSql.UnitTests
{
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
        public void Simple_ands_where_should_produce_select()
        {
            const int id = 10;
            var actual = Sql.Select((Address x) => x.Id).Where(x => x.Id > id && x.Id < 1000 && x.Address1 != "''").ToString();
            actual.Should().Be("SELECT a.[Id] FROM [dbo].[Address] AS a WHERE a.[Id] > 10 AND a.[Id] < 1000 AND a.[Address1] <> ''");
        }

        [Fact]
        public void Many_ands_where_should_produce_select()
        {
            const int id = 10;
            var actual = Sql.Select((Address x) => x.Id).Where(x => x.Id > id && x.Id < 1000 && x.Address1 != "''" && x.Postcode != "''").ToString();
            actual.Should().Be("SELECT a.[Id] FROM [dbo].[Address] AS a WHERE a.[Id] > 10 AND a.[Id] < 1000 AND a.[Address1] <> '' AND a.[Postcode] <> ''");
        }

        [Fact]
        public void Simple_ored_where_should_produce_select()
        {
            const int id = 10;
            var actual = Sql.Select((Address x) => new { x.Id, x.Postcode }).Where(x => x.Id > id || x.Postcode == "'M20'").ToString();
            actual.Should().Be("SELECT a.[Id], a.[Postcode] FROM [dbo].[Address] AS a WHERE a.[Id] > 10 OR a.[Postcode] = 'M20'");
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

        [Fact]
        public void Simple_select_with_specific_table_name_should_produce_select()
        {
            var actual = Sql.Select((Address x) => x.Address1, "MyTable").ToString();
            actual.Should().Be("SELECT a.[Address1] FROM [dbo].[MyTable] AS a");
        }

        [Fact]
        public void Simple_select_with_specific_table_name_and_schema_should_produce_select()
        {
            var actual = Sql.Select((Address x) => x.Address1, new Table {Name = "MyTable", Schema = "MySchema"}).ToString();
            actual.Should().Be("SELECT a.[Address1] FROM [MySchema].[MyTable] AS a");
        }

        [Fact]
        public void Simple_select_with_specific_schema_should_produce_select()
        {
            var actual = Sql.Select(x => x.Address1, Table<Address>.WithSchema("MySchema")).ToString();
            actual.Should().Be("SELECT a.[Address1] FROM [MySchema].[Address] AS a");
        }

        [Fact]
        public void Simple_select_with_hierarchial_table_should_produce_select()
        {
            var actual = Sql.Select((DeliveryAddress x) => x.DeliveryName, Table<Address>.WithDefaultSchema()).ToString();
            actual.Should().Be("SELECT a.[DeliveryName] FROM [dbo].[Address] AS a");
        }

        [Fact(Skip = "TODO")]
        public void Inner_join_2()
        {
            var actual = Sql.Select((Address x, Customer y) => new { x.Address1, y.Name }, (x, y) => x.Id == y.AddressId).ToString();
            actual.Should().Be("SELECT a.[Address1], b.[Name] FROM [dbo].[Address] AS a INNER JOIN dbo.[Customer] AS b ON a.[Id] = b.[AddressId]");
        }
    }
}
