using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace IDE_plugin
{
    public class ExpressionCompilerVisitor : IExpressionVisitor
    {
        private readonly List<string> _variables = new List<string>();
        private ILGenerator _methIl;

        public void Visit(Literal expression)
        {
            _methIl?.Emit(OpCodes.Ldc_I4, int.Parse(expression.Value));
        }

        public void Visit(Variable expression)
        {
            if (!_variables.Contains(expression.Name))
            {
                _variables.Add(expression.Name);
            }

            _methIl?.Emit(OpCodes.Ldarg, _variables.IndexOf(expression.Name) + 1);
        }

        public void Visit(BinaryExpression expression)
        {
            expression.FirstOperand.Accept(this);
            expression.SecondOperand.Accept(this);
            switch (expression.Operator)
            {
                case "+":
                    _methIl?.Emit(OpCodes.Add);
                    break;
                case "-":
                    _methIl?.Emit(OpCodes.Sub);
                    break;
                case "*":
                    _methIl?.Emit(OpCodes.Mul);
                    break;
                case "/":
                    _methIl?.Emit(OpCodes.Div);
                    break;
            }
        }

        public void Visit(ParenExpression expression)
        {
            expression.Operand.Accept(this);
        }

        public Type Build(IExpression expression)
        {
            expression.Accept(this);
            var aName = new AssemblyName("Evaluator");
            var ab =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    aName,
                    AssemblyBuilderAccess.RunAndSave);

            var mb =
                ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
            var tb = mb.DefineType(
                "Evaluator",
                TypeAttributes.Public);
            var parameterTypes = _variables.Select(_ => typeof(int)).ToArray();
            var meth = tb.DefineMethod(
                "Evaluate",
                MethodAttributes.Public,
                typeof(int),
                parameterTypes);

            _methIl = meth.GetILGenerator();
            expression.Accept(this);
            _methIl.Emit(OpCodes.Ret);
            _methIl = null;

            var t = tb.CreateType();
            ab.Save(aName.Name + ".dll");
            return t;
        }
    }
}
