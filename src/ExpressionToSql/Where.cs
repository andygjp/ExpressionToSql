namespace ExpressionToSql
{
    using System;
    using System.Linq.Expressions;
    using System.Text;

    public class Where<T, R> : Query
    {
        private readonly Select<T, R> _select;
        private readonly Expression<Func<T, bool>> _where;

        internal Where(Select<T, R> select, Expression<Func<T, bool>> @where)
        {
            _select = @select;
            _where = @where;
        }

        public override StringBuilder ToSql(StringBuilder sb)
        {
            _select.ToSql(sb);

            sb.Append(" WHERE ");
            BuildWhere(sb, (BinaryExpression)_where.Body);

            return sb;
        }

        private static void BuildWhere(StringBuilder sb, BinaryExpression binaryExpression)
        {
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Equal:
                    BuildTest(sb, binaryExpression, "=");
                    break;
                case ExpressionType.NotEqual:
                    BuildTest(sb, binaryExpression, "<>");
                    break;
                case ExpressionType.GreaterThan:
                    BuildTest(sb, binaryExpression, ">");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    BuildTest(sb, binaryExpression, ">=");
                    break;
                case ExpressionType.LessThan:
                    BuildTest(sb, binaryExpression, "<");
                    break;
                case ExpressionType.LessThanOrEqual:
                    BuildTest(sb, binaryExpression, "<=");
                    break;
                case ExpressionType.AndAlso:
                    BuildWhere(sb, (BinaryExpression)binaryExpression.Left);
                    sb.Append(" AND ");
                    BuildWhere(sb, (BinaryExpression)binaryExpression.Right);
                    break;
                case ExpressionType.OrElse:
                    BuildWhere(sb, (BinaryExpression)binaryExpression.Left);
                    sb.Append(" OR ");
                    BuildWhere(sb, (BinaryExpression)binaryExpression.Right);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void BuildTest(StringBuilder sb, BinaryExpression binaryExpression, string operand)
        {
            var left = binaryExpression.Left;
            var right = binaryExpression.Right;
            if (left.NodeType == ExpressionType.Constant && right.NodeType == ExpressionType.MemberAccess)
            {
                left = binaryExpression.Right;
                right = binaryExpression.Left;
            }

            sb.AppendMember("a", (MemberExpression)left);
            sb.Append(" ");
            sb.Append(operand);

            if (right.NodeType == ExpressionType.Constant)
            {
                sb.Append(" ");
                sb.AppendConstant((ConstantExpression)right);
            }
            else if (right.NodeType == ExpressionType.MemberAccess)
            {
                sb.Append(" @");
                sb.Append(((MemberExpression)right).Member.Name);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}