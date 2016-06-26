namespace ExpressionToSql
{
    public class Table<T> : Table
    {
        private string _name;

        public override string Name
        {
            get { return _name ?? typeof(T).Name; }
            set { _name = value; }
        }

        public static Table<T> WithSchema(string name)
        {
            return new Table<T> { Schema = name };
        }

        public static Table<T> WithDefaultSchema()
        {
            return new Table<T>();
        }
    }

    public class Table
    {
        public virtual string Name { get; set; }

        public string Schema { get; set; } = "[dbo]";
    }
}