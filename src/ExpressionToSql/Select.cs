namespace ExpressionToSql
{
    using System;
    using System.Linq.Expressions;

    public class Select<T, R>
    {
        private readonly QueryBuilder<T, R> _queryBuilder;

        internal Select(Expression<Func<T, R>> @select, int? take, Table table)
        {
            _queryBuilder = new QueryBuilder<T, R>(@select, take, table);
        }

        public Where<T, R> Where(Expression<Func<T, bool>> predicate)
        {
            return new Where<T, R>(_queryBuilder, predicate);
        }

        public override string ToString()
        {
            return _queryBuilder.ToString();
        }
    }
}