# ExpressionToSql

You can write a strongly typed query and have it converted to a query that [Dapper](https://github.com/StackExchange/dapper-dot-net) understands. Like so:

    var postcode = "BL";
    var query = Sql.Select((Address x) => new { x.Id, x.Postcode }).Where(x => x.Id > 10 && x.Postcode != postcode).ToString();
    // "SELECT a.[Id], a.[Postcode] FROM [dbo].[Address] AS a WHERE a.[Id] > 10 AND a.[Postcode] <> @postcode"

Or:

    var query = Sql.Select((Address x) => 1, "TableName").Where(x => x.Id == 1001).ToString();
    // "SELECT 1 FROM [dbo].[Address] AS a WHERE a.[Id] = 1001

## TODO
- Add support for joins
