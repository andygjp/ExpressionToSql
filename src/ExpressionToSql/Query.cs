namespace ExpressionToSql
{
    using System.Text;

    public abstract class Query
    {
        public override string ToString()
        {
            return ToSql(new StringBuilder()).ToString();
        }

        public abstract StringBuilder ToSql(StringBuilder sb);
    }
}