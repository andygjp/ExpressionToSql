namespace ExpressionToSql
{
    using System;
    using System.Linq.Expressions;

    public class Where<T, R>
    {
        private readonly QueryBuilder<T, R> _queryBuilder;

        internal Where(QueryBuilder<T, R> queryBuilder, Expression<Func<T, bool>> predicate)
        {
            _queryBuilder = new QueryBuilder<T, R>(queryBuilder, predicate);
        }

        public override string ToString()
        {
            return _queryBuilder.ToString();
        }
    }
}