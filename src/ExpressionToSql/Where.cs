namespace ExpressionToSql
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    public class Where<T, R> : Query
    {
        private readonly Select<T, R> _select;
        private readonly Expression<Func<T, bool>> _where;

        internal Where(Select<T, R> select, Expression<Func<T, bool>> where)
        {
            _select = select;
            _where = where;
        }

        public override StringBuilder ToSql(StringBuilder sb)
        {
            _select.ToSql(sb);

            BuildWhere(new QueryBuilder(sb), (BinaryExpression)_where.Body, And);
            return sb;
        }

        private static Clause And(QueryBuilder qb, BinaryExpression binaryExpression, string op)
        {
            return new AndClause(qb, binaryExpression, op);
        }

        private static Clause Or(QueryBuilder qb, BinaryExpression binaryExpression, string op)
        {
            return new OrClause(qb, binaryExpression, op);
        }

        private static void BuildWhere(QueryBuilder qb, BinaryExpression binaryExpression, Func<QueryBuilder, BinaryExpression, string, Clause> clause)
        {
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Equal:
                    clause(qb, binaryExpression, "=").Append();
                    break;
                case ExpressionType.NotEqual:
                    clause(qb, binaryExpression, "<>").Append();
                    break;
                case ExpressionType.GreaterThan:
                    clause(qb, binaryExpression, ">").Append();
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    clause(qb, binaryExpression, ">=").Append();
                    break;
                case ExpressionType.LessThan:
                    clause(qb, binaryExpression, "<").Append();
                    break;
                case ExpressionType.LessThanOrEqual:
                    clause(qb, binaryExpression, "<=").Append();
                    break;
                case ExpressionType.AndAlso:
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Left, clause);
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Right, And);
                    break;
                case ExpressionType.OrElse:
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Left, clause);
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Right, Or);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private abstract class Clause
        {
            private readonly BinaryExpression _binaryExpression;
            private readonly string _op;

            protected Clause(QueryBuilder qb, BinaryExpression binaryExpression, string op)
            {
                QueryBuilder = qb;
                _binaryExpression = binaryExpression;
                _op = op;
            }

            protected QueryBuilder QueryBuilder { get; }

            public void Append()
            {
                var b = Arrange(_binaryExpression);

                var m1 = (MemberExpression) b.Item1;

                switch (b.Item2.NodeType)
                {
                    case ExpressionType.Constant:
                        var c = (ConstantExpression)b.Item2;
                        AddValue()(_op, m1.Member.Name, c.Value, "a");
                        break;
                    case ExpressionType.MemberAccess:
                        var m2 = (MemberExpression)b.Item2;
                        AddParameter()(_op, m1.Member.Name, m2.Member.Name, "a");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            protected abstract Func<string, string, object, string, QueryBuilder> AddValue();

            protected abstract Func<string, string, string, string, QueryBuilder> AddParameter();

            private static Tuple<Expression, Expression> Arrange(BinaryExpression binaryExpression)
            {
                var left = binaryExpression.Left;
                var right = binaryExpression.Right;
                if (left.NodeType == ExpressionType.Constant && right.NodeType == ExpressionType.MemberAccess)
                {
                    return Tuple.Create(right, left);
                }
                return Tuple.Create(left, right);
            }
        }

        private class AndClause : Clause
        {
            public AndClause(QueryBuilder qb, BinaryExpression binaryExpression, string op) : base(qb, binaryExpression, op)
            {
            }

            protected override Func<string, string, object, string, QueryBuilder> AddValue() => QueryBuilder.AddCondition;

            protected override Func<string, string, string, string, QueryBuilder> AddParameter() => QueryBuilder.AddCondition;
        }

        private class OrClause : Clause
        {
            public OrClause(QueryBuilder qb, BinaryExpression binaryExpression, string op) : base(qb, binaryExpression, op)
            {
            }

            protected override Func<string, string, object, string, QueryBuilder> AddValue() => QueryBuilder.OrCondition;

            protected override Func<string, string, string, string, QueryBuilder> AddParameter() => QueryBuilder.OrCondition;
        }
    }
}