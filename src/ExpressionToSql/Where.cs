namespace ExpressionToSql
{
    using System;
    using System.Linq.Expressions;

    public class Where<T, R> : Query
    {
        private readonly Select<T, R> _select;
        private readonly Expression<Func<T, bool>> _where;

        internal Where(Select<T, R> select, Expression<Func<T, bool>> where)
        {
            _select = select;
            _where = where;
        }

        internal override QueryBuilder ToSql(QueryBuilder qb)
        {
            _select.ToSql(qb);

            BuildWhere(qb, (BinaryExpression)_where.Body, Clause.And);
            return qb;
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
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Right, Clause.And);
                    break;
                case ExpressionType.OrElse:
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Left, clause);
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Right, Clause.Or);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private class Clause
        {
            private readonly BinaryExpression _binaryExpression;
            private readonly string _op;
            private readonly Func<string, string, object, string, QueryBuilder> _appendValue;
            private readonly Func<string, string, string, string, QueryBuilder> _appendParameter;

            private Clause(BinaryExpression binaryExpression, string op,
                Func<string, string, object, string, QueryBuilder> appendValue,
                Func<string, string, string, string, QueryBuilder> appendParameter)
            {
                _binaryExpression = binaryExpression;
                _op = op;
                _appendValue = appendValue;
                _appendParameter = appendParameter;
            }

            public static Clause And(QueryBuilder qb, BinaryExpression binaryExpression, string op)
            {
                return new Clause(binaryExpression, op, qb.AddCondition, qb.AddCondition);
            }

            public static Clause Or(QueryBuilder qb, BinaryExpression binaryExpression, string op)
            {
                return new Clause(binaryExpression, op, qb.OrCondition, qb.OrCondition);
            }

            public void Append()
            {
                var left = _binaryExpression.Left;
                var right = _binaryExpression.Right;
                if (left.NodeType == ExpressionType.Constant && right.NodeType == ExpressionType.MemberAccess)
                {
                    left = right;
                    right = left;
                }

                var attributeName = ((MemberExpression) left).Member.Name;

                switch (right.NodeType)
                {
                    case ExpressionType.Constant:
                        var c = (ConstantExpression)right;
                        _appendValue(_op, attributeName, c.Value, QueryBuilder.AliasName);
                        break;
                    case ExpressionType.MemberAccess:
                        var m2 = (MemberExpression)right;
                        _appendParameter(_op, attributeName, m2.Member.Name, QueryBuilder.AliasName);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}