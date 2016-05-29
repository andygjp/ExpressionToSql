namespace ExpressionToSql
{
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendMember(this StringBuilder sb, string identifier, MemberExpression member)
        {
            return AppendMember(sb, identifier, member.Member);
        }

        public static StringBuilder AppendMember(this StringBuilder sb, string identifier, MemberInfo memberInfo)
        {
            if (!string.IsNullOrWhiteSpace(identifier))
            {
                sb.Append(identifier);
                sb.Append(".");
            }
            return AppendEscapedValue(sb, memberInfo.Name);
        }

        public static StringBuilder AppendEscapedValue(this StringBuilder sb, string value)
        {
            return sb.Append("[").Append(value).Append("]");
        }

        public static StringBuilder AppendConstant(this StringBuilder sb, ConstantExpression constant)
        {
            return sb.Append(constant.Value);
        }
    }
}