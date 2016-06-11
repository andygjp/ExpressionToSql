namespace ExpressionToSql.Dapper.UnitTests
{
    using global::Dapper.Contrib.Extensions;

    [Table("Person")]
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}