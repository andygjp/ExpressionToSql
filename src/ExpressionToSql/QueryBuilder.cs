namespace ExpressionToSql
{
    using System;
    using System.Linq.Expressions;
    using System.Text;

    internal class QueryBuilder<T, R>
    {
        private readonly Expression<Func<T, R>> _select;
        private readonly Expression<Func<T, bool>> _where;
        private readonly int? _take;
        private readonly Table _table;

        public QueryBuilder(Expression<Func<T, R>> @select, int? take, Table table)
        {
            _select = @select;
            _take = take;
            _table = table;
        }

        public QueryBuilder(QueryBuilder<T, R> original, Expression<Func<T, bool>> @where) : this(original._select, original._take, original._table)
        {
            _where = @where;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            BuildSelect(sb);

            if (_where != null)
            {
                BuildWhere(sb);
            }

            return sb.ToString();
        }

        private void BuildSelect(StringBuilder sb)
        {
            sb.Append("SELECT");

            if (_take.HasValue)
            {
                sb.Append(" TOP ");
                sb.Append(_take.Value);
            }

            var body = _select.Body;
            if (body.NodeType == ExpressionType.MemberAccess)
            {
                sb.Append(" ");
                AppendMember(sb, (MemberExpression)body);
            }
            else if (body.NodeType == ExpressionType.Constant)
            {
                sb.Append(" ");
                AppendConstant(sb, (ConstantExpression)body);
            }
            else if (body.NodeType == ExpressionType.New)
            {
                foreach (var member in ((NewExpression)body).Members)
                {
                    sb.Append(" ");
                    sb.Append(member.Name);
                    sb.Append(",");
                }
                sb.Length -= 1; // Remove last comma
            }
            else
            {
                throw new NotImplementedException();
            }

            sb.Append(" FROM ");
            if (!string.IsNullOrWhiteSpace(_table.Schema))
            {
                sb.Append(_table.Schema);
                sb.Append(".");
            }
            sb.Append(_table.Name ?? $"[{_select.Parameters[0].Type.Name}]");
        }

        private void BuildWhere(StringBuilder sb)
        {
            sb.Append(" WHERE ");

            BuildWhere(sb, (BinaryExpression)_where.Body);
        }

        private static void BuildWhere(StringBuilder sb, BinaryExpression binaryExpression)
        {
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Equal:
                    BuildTest(sb, binaryExpression, "=");
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

            AppendMember(sb, (MemberExpression)left);
            sb.Append(" ");
            sb.Append(operand);

            if (right.NodeType == ExpressionType.Constant)
            {
                sb.Append(" ");
                AppendConstant(sb, (ConstantExpression)right);
            }
            else if (right.NodeType == ExpressionType.MemberAccess)
            {
                sb.Append(" @");
                AppendMember(sb, (MemberExpression)right);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void AppendMember(StringBuilder sb, MemberExpression member)
        {
            sb.Append(member.Member.Name);
        }

        private static void AppendConstant(StringBuilder sb, ConstantExpression constant)
        {
            sb.Append(constant.Value);
        }
    }
}