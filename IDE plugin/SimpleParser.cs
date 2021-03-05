using System.Collections.Generic;


namespace IDE_plugin
{
    public static class SimpleParser
    {
        public static IExpression Parse(string text)
        {
            var expressions = new Stack<IExpression>();
            var operations = new Stack<char>();

            var operationPriorities = new Dictionary<char, int> {{'+', 1}, {'-', 1}, {'*', 2}, {'/', 2}};

            void Combine(char prev)
            {
                var op2 = expressions.Pop();
                var op1 = expressions.Pop();
                expressions.Push(new BinaryExpression(op1, op2, prev.ToString()));
            }

            foreach (var ch in text)
            {
                switch (ch)
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    {
                        while (operations.Count > 0)
                        {
                            var prev = operations.Pop();
                            if (!operationPriorities.ContainsKey(prev) ||
                                operationPriorities[prev] < operationPriorities[ch])
                            {
                                operations.Push(prev);
                                break;
                            }

                            Combine(prev);
                        }

                        operations.Push(ch);
                        break;
                    }
                    case '(':
                    {
                        operations.Push('(');
                        break;
                    }
                    case ')':
                    {
                        while (true)
                        {
                            var prev = operations.Pop();
                            if (prev == '(')
                            {
                                expressions.Push(new ParenExpression(expressions.Pop()));
                                break;
                            }

                            Combine(prev);
                        }

                        break;
                    }
                    default:
                    {
                        if (char.IsDigit(ch))
                        {
                            expressions.Push(new Literal(ch.ToString()));
                        }
                        else if (char.IsLetter(ch))
                        {
                            expressions.Push(new Variable(ch.ToString()));
                        }

                        break;
                    }
                }
            }

            while (operations.Count > 0)
            {
                var prev = operations.Pop();
                Combine(prev);
            }

            return expressions.Pop();
        }
    }
}
