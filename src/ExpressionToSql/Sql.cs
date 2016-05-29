namespace ExpressionToSql
{
    using System;
    using System.Linq.Expressions;

    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public static class Sql
    {
        public static Select<T, R> Select<T, R>(Expression<Func<T, R>> selector, string tableName = null)
        {
            return Create(selector, null, tableName);
        }

        public static Select<T1, T2, R> Select<T1, T2, R>(Expression<Func<T1, T2, R>> selector, Expression<Func<T1, T2, bool>> on, string tableName = null)
        {
            return new Select<T1, T2, R>(selector, on, null, new Table { Name = tableName });
        }

        public static Select<T, R> Top<T, R>(Expression<Func<T, R>> selector, int take, string tableName = null)
        {
            return Create(selector, take, tableName);
        }

        private static Select<T, R> Create<T, R>(Expression<Func<T, R>> selector, int? take, string tableName)
        {
            return new Select<T, R>(selector, take, new Table {Name = tableName});
        }

        public static Select<T, R> Select<T, R>(Expression<Func<T, R>> selector, Table table)
        {
            return Create(selector, null, table);
        }

        public static Select<T, R> Top<T, R>(Expression<Func<T, R>> selector, int take, Table table)
        {
            return Create(selector, take, table);
        }

        private static Select<T, R> Create<T, R>(Expression<Func<T, R>> selector, int? take, Table table)
        {
            return new Select<T, R>(selector, take, table ?? new Table());
        }
    }
}
