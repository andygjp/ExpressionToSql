namespace ExpressionToSql
{
    using System;

    internal static class OperandExtensions
    {
        public static string ToSqlOperand(this Operand operand)
        {
            switch (operand)
            {
                case Operand.Equal:
                    return "=";
                case Operand.NotEqual:
                    return "<>";
                case Operand.GreaterThan:
                    return ">";
                case Operand.GreaterThanOrEqual:
                    return ">=";
                case Operand.LessThan:
                    return "<";
                case Operand.LessThanOrEqual:
                    return "<=";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}