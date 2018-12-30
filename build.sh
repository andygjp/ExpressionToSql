dotnet --version
dotnet restore
dotnet test tests/ExpressionToSql.UnitTests
dotnet pack src/ExpressionToSql --output ../../output/ExpressionToSql --configuration Release
dotnet pack src/ExpressionToSql.Dapper --output ../../output/ExpressionToSql_Dapper --configuration Release
