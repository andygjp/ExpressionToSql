namespace ExpressionToSql.Dapper.UnitTests
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using FluentAssertions;
    using global::Dapper;
    using global::Dapper.Contrib.Extensions;
    using Xunit;

    public class When_getting_all_persons : IClassFixture<Fixture>
    {
        private readonly SqlConnection _conn;

        public When_getting_all_persons(Fixture fixture)
        {
            _conn = fixture.GetConnection();
        }

        [Fact]
        public async Task It_should_return_all_person()
        {
            IEnumerable<string> names = await _conn.QueryAsync(Sql.Select((Person x) => x.Name));
            names.Should().HaveCount(2);
        }
    }

    public class When_getting_specific_person : IClassFixture<Fixture>
    {
        private readonly SqlConnection _conn;

        public When_getting_specific_person(Fixture fixture)
        {
            _conn = fixture.GetConnection();
        }

        [Fact]
        public async Task It_should_return_every_member()
        {
            var o = new {id = 2};

            var persons = await _conn.QueryAsync(Sql.Select((Person x) => x).Where(x => x.Id == o.id), o);
            persons.Single().ShouldBeEquivalentTo(new Person {Id = 2, Name = "John Johnson"});
        }
    }

    public class When_checking_entity_exists : IClassFixture<Fixture>
    {
        private readonly SqlConnection _conn;

        public When_checking_entity_exists(Fixture fixture)
        {
            _conn = fixture.GetConnection();
        }

        [Fact]
        public async Task It_should_return_every_member()
        {
            var o = new { id = 2 };

            var @where = Sql.Select((Person x) => 1).Where(x => x.Id == o.id).ToString();

            var exists = await _conn.QuerySingleOrDefaultAsync<bool>(@where, o);

            exists.Should().BeTrue();
        }
    }

    public class When_checking_for_entity_that_does_not_exist : IClassFixture<Fixture>
    {
        private readonly SqlConnection _conn;

        public When_checking_for_entity_that_does_not_exist(Fixture fixture)
        {
            _conn = fixture.GetConnection();
        }

        [Fact]
        public async Task It_should_return_every_member()
        {
            var @where = Sql.Select((Person x) => 1).Where(x => x.Id == -1).ToString();

            var exists = await _conn.QuerySingleOrDefaultAsync<bool>(@where);

            exists.Should().BeFalse();
        }
    }

    public class Fixture : IAsyncLifetime
    {
        private TestDatabase _testDatabase;

        public async Task InitializeAsync()
        {
            CreateTestDatabase();
            var conn = GetConnection();
            await conn.InsertAsync(new Person { Name = "John Smith" });
            await conn.InsertAsync(new Person { Name = "John Johnson" });
        }

        private void CreateTestDatabase([CallerFilePath] string codeFilePath = null)
        {
            var dir = Path.GetDirectoryName(codeFilePath);
            var file = Path.Combine(dir, "TestDatabase.mdf");
            _testDatabase = TestDatabase.Create(file);
        }

        public Task DisposeAsync()
        {
            _testDatabase.Dispose();
            return Task.CompletedTask;
        }

        public SqlConnection GetConnection()
        {
            return _testDatabase.GetSqlConnection();
        }
    }
}
