namespace ExpressionToSql.Dapper
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using ExpressionToSql;
    using global::Dapper;

    public static class ExpressionToSqlDapperExtensions
    {
        public static Task<IEnumerable<R>> QueryAsync<T, R>(this IDbConnection cnn, Select<T, R> sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return QueryAsync<R>(cnn, sql, transaction, commandTimeout, commandType);
        }

        public static Task<IEnumerable<R>> QueryAsync<T, R>(this IDbConnection cnn, Where<T, R> sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return QueryAsync<R>(cnn, sql, transaction, commandTimeout, commandType);
        }

        private static Task<IEnumerable<R>> QueryAsync<R>(IDbConnection cnn, Query sql, IDbTransaction transaction, int? commandTimeout, CommandType? commandType)
        {
            var query = sql.ToString();
            return cnn.QueryAsync<R>(query, transaction: transaction, commandTimeout: commandTimeout, commandType: commandType);
        }


        public static IEnumerable<R> Query<T, R>(this IDbConnection cnn, Select<T, R> sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Query<R>(cnn, sql, transaction, buffered, commandTimeout, commandType);
        }

        public static IEnumerable<R> Query<T, R>(this IDbConnection cnn, Where<T, R> sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Query<R>(cnn, sql, transaction, buffered, commandTimeout, commandType);
        }

        private static IEnumerable<R> Query<R>(IDbConnection cnn, Query sql, IDbTransaction transaction, bool buffered, int? commandTimeout, CommandType? commandType)
        {
            var query = sql.ToString();
            return cnn.Query<R>(query, transaction: transaction, buffered: buffered, commandTimeout: commandTimeout, commandType: commandType);
        }
    }
}
