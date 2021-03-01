using System.Collections.Generic;

namespace IDE_plugin
{
    public static class SimpleParser
    {
        //TODO parentheses
        //TODO -, /
        public static IExpression Parse(string text)
        {
            var s1 = new Stack<IExpression>();
            var operations = new Stack<char>();

            foreach (var ch in text)
            {
                switch (ch)
                {
                    case '+':
                    {
                        while (operations.Count > 0)
                        {
                            var prev = operations.Pop();
                            var op2 = s1.Pop();
                            var op1 = s1.Pop();
                            s1.Push(new BinaryExpression(op1, op2, prev.ToString()));
                        }

                        operations.Push('+');
                        break;
                    }
                    case '*':
                        while (operations.Count > 0)
                        {
                            var prev = operations.Pop();
                            if (prev == '+')
                            {
                                operations.Push(prev);
                                break;
                            }

                            var op2 = s1.Pop();
                            var op1 = s1.Pop();
                            s1.Push(new BinaryExpression(op1, op2, prev.ToString()));
                        }

                        operations.Push('*');
                        break;
                    default:
                    {
                        if (char.IsDigit(ch))
                        {
                            s1.Push(new Literal(ch.ToString()));
                        }
                        else if (char.IsLetter(ch))
                        {
                            s1.Push(new Variable(ch.ToString()));
                        }

                        break;
                    }
                }
            }

            while (operations.Count > 0)
            {
                var prev = operations.Pop();
                var op2 = s1.Pop();
                var op1 = s1.Pop();
                s1.Push(new BinaryExpression(op1, op2, prev.ToString()));
            }

            return s1.Pop();
        }
    }
}
