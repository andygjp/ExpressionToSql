namespace ExpressionToSql.Dapper
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using ExpressionToSql;
    using global::Dapper;

    public static class ExpressionToSqlDapperExtensions
    {
        public static Task<IEnumerable<R>> QueryAsync<T, R>(this IDbConnection cnn, Select<T, R> sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return QueryAsync<R>(cnn, sql, param, transaction, commandTimeout, commandType);
        }

        public static Task<IEnumerable<R>> QueryAsync<T, R>(this IDbConnection cnn, Where<T, R> sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return QueryAsync<R>(cnn, sql, param, transaction, commandTimeout, commandType);
        }

        private static Task<IEnumerable<R>> QueryAsync<R>(IDbConnection cnn, Query sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType)
        {
            var query = sql.ToString();
            return cnn.QueryAsync<R>(query, param: param, transaction: transaction, commandTimeout: commandTimeout, commandType: commandType);
        }


        public static IEnumerable<R> Query<T, R>(this IDbConnection cnn, Select<T, R> sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Query<R>(cnn, sql, param, transaction, buffered, commandTimeout, commandType);
        }

        public static IEnumerable<R> Query<T, R>(this IDbConnection cnn, Where<T, R> sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Query<R>(cnn, sql, param, transaction, buffered, commandTimeout, commandType);
        }

        private static IEnumerable<R> Query<R>(IDbConnection cnn, Query sql, object param, IDbTransaction transaction, bool buffered, int? commandTimeout, CommandType? commandType)
        {
            var query = sql.ToString();
            return cnn.Query<R>(query, param: param, transaction: transaction, buffered: buffered, commandTimeout: commandTimeout, commandType: commandType);
        }
    }
}
