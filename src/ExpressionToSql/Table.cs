namespace ExpressionToSql
{
    public class Table<T>
    {
        private string _name;

        public string Name
        {
            get { return _name ?? typeof(T).Name; }
            set { _name = value; }
        }

        public string Schema { get; set; } = "[dbo]";

        public static Table<T> WithSchema(string name)
        {
            return new Table<T> { Schema = name };
        }
    }
}