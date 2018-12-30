namespace ExpressionToSql
{
    using System.Text;

    public abstract class Query
    {
        public override string ToString()
        {
            return ToSql();
        }
        
        public string ToSql()
        {
            return ToSql(new StringBuilder()).ToString();
        }

        public StringBuilder ToSql(StringBuilder sb)
        {
            ToSql(new QueryBuilder(sb));
            return sb;
        }

        internal abstract QueryBuilder ToSql(QueryBuilder qb);
    }
}