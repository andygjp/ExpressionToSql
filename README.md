# ExpressionToSql

[![NuGet version](https://badge.fury.io/nu/expressiontosql.svg)](https://badge.fury.io/nu/expressiontosql)
[![](https://ci.appveyor.com/api/projects/status/github/andygjp/ExpressionToSql?branch=master&svg=true)](https://ci.appveyor.com/project/andygjp/expressiontosql)

You can write a strongly typed query and have it converted to a query that [Dapper](https://github.com/StackExchange/dapper-dot-net) understands. Like so:

    var postcode = "BL";
    var query = Sql.Select((Address x) => new { x.Id, x.Postcode }).Where(x => x.Id > 10 && x.Postcode != postcode).ToString();
    // "SELECT a.[Id], a.[Postcode] FROM [dbo].[Address] AS a WHERE a.[Id] > 10 AND a.[Postcode] <> @postcode"

Or:

    var query = Sql.Select((Address x) => 1, "TableName").Where(x => x.Id == 1001).ToString();
    // "SELECT 1 FROM [dbo].TableName AS a WHERE a.[Id] = 1001
    
# ExpressionToSql.Dapper

You can write the same strongly typed query and pass it directly to Dapper, like so:

    IEnumerable<Person> person = await _conn.QueryAsync(Sql.Select((Person x) => x).Where(x => x.Id == 2));
    // "SELECT a.[Id], a.[Name] FROM [dbo].[Person] AS a WHERE a.[Id] = 2"

## TODO
- Add support for joins
