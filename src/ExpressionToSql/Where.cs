namespace ExpressionToSql
{
    using System;
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

            BuildWhere(new QueryBuilder(sb), (BinaryExpression)_where.Body, AndOrCondition.And);
            return sb;
        }

        private static void BuildWhere(QueryBuilder qb, BinaryExpression binaryExpression, AndOrCondition condition)
        {
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Equal:
                    BuildTest(qb, binaryExpression, Operand.Equal, condition);
                    break;
                case ExpressionType.NotEqual:
                    BuildTest(qb, binaryExpression, Operand.NotEqual, condition);
                    break;
                case ExpressionType.GreaterThan:
                    BuildTest(qb, binaryExpression, Operand.GreaterThan, condition);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    BuildTest(qb, binaryExpression, Operand.GreaterThanOrEqual, condition);
                    break;
                case ExpressionType.LessThan:
                    BuildTest(qb, binaryExpression, Operand.LessThan, condition);
                    break;
                case ExpressionType.LessThanOrEqual:
                    BuildTest(qb, binaryExpression, Operand.LessThanOrEqual, condition);
                    break;
                case ExpressionType.AndAlso:
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Left, condition);
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Right, AndOrCondition.And);
                    break;
                case ExpressionType.OrElse:
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Left, condition);
                    BuildWhere(qb, (BinaryExpression)binaryExpression.Right, AndOrCondition.Or);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void BuildTest(QueryBuilder qb, BinaryExpression binaryExpression, Operand operand, AndOrCondition condition)
        {
            var b = Arrange(binaryExpression);

            var m1 = (MemberExpression) b.Item1;
            if (b.Item2.NodeType == ExpressionType.Constant)
            {
                var c = (ConstantExpression) b.Item2;
                if (condition == AndOrCondition.And)
                {
                    qb.AddCondition(operand, m1.Member.Name, c.Value);
                }
                else
                {
                    qb.OrCondition(operand, m1.Member.Name, c.Value);
                }
            }
            else if (b.Item2.NodeType == ExpressionType.MemberAccess)
            {
                var m2 = (MemberExpression) b.Item2;
                if (condition == AndOrCondition.And)
                {
                    qb.AddCondition(operand, m1.Member.Name, m2.Member.Name);
                }
                else
                {
                    qb.OrCondition(operand, m1.Member.Name, m2.Member.Name);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

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

        private enum AndOrCondition
        {
            And,
            Or
        }
    }
}