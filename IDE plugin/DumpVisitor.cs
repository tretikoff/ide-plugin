using System.Text;
using NUnit.Framework;

namespace IDE_plugin
{
    public interface IExpressionVisitor
    {
        void Visit(Literal expression);
        void Visit(Variable expression);
        void Visit(BinaryExpression expression);
        void Visit(ParenExpression expression);
    }
    
    public class DumpVisitor : IExpressionVisitor
    {
        private readonly StringBuilder _myBuilder;

        public DumpVisitor()
        {
            _myBuilder = new StringBuilder();
        }

        public void Visit(Literal expression)
        {
            _myBuilder.Append("Literal(" + expression.Value + ")");
        }

        public void Visit(Variable expression)
        {
            _myBuilder.Append("Variable(" + expression.Name + ")");
        }

        public void Visit(BinaryExpression expression)
        {
            _myBuilder.Append("Binary(");
            expression.FirstOperand.Accept(this);
            _myBuilder.Append(expression.Operator);
            expression.SecondOperand.Accept(this);
            _myBuilder.Append(")");
        }

        public void Visit(ParenExpression expression)
        {
            _myBuilder.Append("Paren(");
            expression.Operand.Accept(this);
            _myBuilder.Append(")");
        }

        public override string ToString()
        {
            return _myBuilder.ToString();
        }
    }
}
