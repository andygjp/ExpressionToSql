namespace ExpressionToSql
{
    using System;
    using System.Linq.Expressions;
    using System.Text;

    public class Select<T, R> : Query
    {
        private readonly Expression<Func<T, R>> _select;
        private readonly int? _take;
        private readonly Table _table;

        internal Select(Expression<Func<T, R>> @select, int? take, Table table)
        {
            _select = @select;
            _take = take;
            _table = table;
        }

        public Where<T, R> Where(Expression<Func<T, bool>> predicate)
        {
            return new Where<T, R>(this, predicate);
        }

        public override StringBuilder ToSql(StringBuilder sb)
        {
            sb.Append("SELECT");

            if (_take.HasValue)
            {
                sb.Append(" TOP ");
                sb.Append(_take.Value);
            }

            const string identifier = "a";

            var body = _select.Body;
            var type = _select.Parameters[0].Type;
            if (body.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = (MemberExpression)body;
                if (memberExpression.Member.DeclaringType == type)
                {
                    sb.Append(" ");
                    sb.AppendMember(identifier, memberExpression);
                }
                else
                {
                    sb.Append(" @");
                    sb.Append(memberExpression.Member.Name);
                }
            }
            else if (body.NodeType == ExpressionType.Constant)
            {
                sb.Append(" ");
                sb.AppendConstant((ConstantExpression)body);
            }
            else if (body.NodeType == ExpressionType.New)
            {
                foreach (Expression argument in ((NewExpression)body).Arguments)
                {
                    if (argument.NodeType == ExpressionType.Constant)
                    {
                        sb.Append(" ");
                        sb.AppendConstant((ConstantExpression)argument);
                    }
                    else
                    {
                        var memberExpression = (MemberExpression)argument;
                        if (memberExpression.Member.DeclaringType == type)
                        {
                            sb.Append(" ");
                            sb.AppendMember(identifier, memberExpression);
                        }
                        else
                        {
                            sb.Append(" @");
                            sb.Append(memberExpression.Member.Name);
                        }
                    }
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
            if (string.IsNullOrWhiteSpace(_table.Name))
            {
                sb.AppendEscapedValue(type.Name);
            }
            else
            {
                sb.Append(_table.Name);
            }
            sb.Append(" AS ");
            sb.Append(identifier);

            return sb;
        }
    }

    public class Select<T1, T2, R>
    {
        internal Select(Expression<Func<T1, T2, R>> @select, Expression<Func<T1, T2, bool>> on, int? take, Table table)
        {
        }

        public override string ToString()
        {
            return "";
        }
    }
}