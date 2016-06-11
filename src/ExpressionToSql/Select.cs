namespace ExpressionToSql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    public class Select<T, R> : Query
    {
        private readonly Expression<Func<T, R>> _select;
        private readonly int? _take;
        private readonly Table<T> _table;

        internal Select(Expression<Func<T, R>> select, int? take, Table<T> table)
        {
            _select = select;
            _take = take;
            _table = table;
        }

        public Where<T, R> Where(Expression<Func<T, bool>> predicate)
        {
            return new Where<T, R>(this, predicate);
        }

        public override StringBuilder ToSql(StringBuilder sb)
        {
            var qb = new QueryBuilder(sb);

            if (_take.HasValue)
            {
                qb.Take(_take.Value);
            }

            var type = _select.Parameters[0].Type;

            var expressions = GetExpressions(type);

            AddExpressions(expressions, qb, type);

            qb.AddTable(_table);

            return sb;
        }

        private IEnumerable<Expression> GetExpressions(Type type)
        {
            var body = _select.Body;

            if (body.NodeType == ExpressionType.New)
            {
                var n = (NewExpression) body;
                return n.Arguments;
            }
            if (body.NodeType == ExpressionType.Parameter)
            {
                var propertyInfos = new SimpleTypeBinder().GetProperties(type);
                return propertyInfos.Values.Select(pi => Expression.Property(body, pi));
            }

            return new[] {body};
        }

        private static void AddExpressions(IEnumerable<Expression> expressions, QueryBuilder qb, Type type)
        {
            foreach (var argument in expressions)
            {
                switch (argument.NodeType)
                {
                    case ExpressionType.Constant:
                        var c = (ConstantExpression) argument;
                        qb.AddValue(c.Value);
                        break;
                    case ExpressionType.MemberAccess:
                        var m = (MemberExpression) argument;
                        AddExpression(m, type, qb);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                qb.AddSeparator();
            }
            qb.Remove(); // Remove last comma
        }

        private static void AddExpression(MemberExpression m, Type type, QueryBuilder qb)
        {
            if (m.Member.DeclaringType == type)
            {
                qb.AddAttribute(m.Member.Name);
            }
            else
            {
                qb.AddParameter(m.Member.Name);
            }
        }
    }

    public class Select<T1, T2, R>
    {
        internal Select(Expression<Func<T1, T2, R>> select, Expression<Func<T1, T2, bool>> on, int? take, Table<T1> table)
        {
        }

        public override string ToString()
        {
            return "";
        }
    }
}