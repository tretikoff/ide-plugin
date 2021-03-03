using System;
using NUnit.Framework;

namespace IDE_plugin
{
    public class ExpressionCompilerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("5", 5)]
        [TestCase("1+1", 2)]
        [TestCase("1*2", 2)]
        [TestCase("2-1", 1)]
        [TestCase("5*(2+3)", 25)]
        [TestCase("6/(3-1)", 3)]
        public void VisitWithoutParams(string expression, int result)
        {
            var visitor = new ExpressionCompilerVisitor();
            var expr = SimpleParser.Parse(expression);
            dynamic instance = Activator.CreateInstance(visitor.Build(expr));
            Assert.AreEqual(result, (int) instance.Evaluate());
        }


        [Test]
        [TestCase("5+a", 5, 10)]
        [TestCase("9*(4+a)", 6, 90)]
        public void VisitOneParam(string expression, int param, int result)
        {
            var visitor = new ExpressionCompilerVisitor();
            var expr = SimpleParser.Parse(expression);
            dynamic instance = Activator.CreateInstance(visitor.Build(expr));
            Assert.AreEqual(result, (int) instance.Evaluate(param));
        }

        [Test]
        [TestCase("5+a*b+c", 5, 6, 7, 42)]
        [TestCase("9*(4+a)*b+c", 4, 1, 8, 80)]
        [TestCase("5*(4-a)*b/c", 1, 5, 3, 25)]
        public void VisitThreeParams(string expression, int fst, int snd, int third, int result)
        {
            var visitor = new ExpressionCompilerVisitor();
            var expr = SimpleParser.Parse(expression);
            dynamic instance = Activator.CreateInstance(visitor.Build(expr));
            Assert.AreEqual(result, (int) instance.Evaluate(fst, snd, third));
        }
    }
}
