﻿using NUnit.Framework;

namespace IDE_plugin
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void VisitBinaryExpression()
        {
            var dumpVisitor = new DumpVisitor();
            new BinaryExpression(new Literal("1"), new Literal("2"), "+").Accept(dumpVisitor);
            Assert.AreEqual("Binary(Literal(1)+Literal(2))", dumpVisitor.ToString());

            Assert.Pass();
        }

        [Test]
        public void ParseExpression()
        {
            const string expr = "a+b*c+d";
            var dumpVisitorActual = new DumpVisitor();
            var dumpVisitorExpected = new DumpVisitor();
            SimpleParser.Parse(expr).Accept(dumpVisitorActual);
            var expected = new BinaryExpression(new BinaryExpression(
                    new Variable("a"),
                    new BinaryExpression(
                        new Variable("b"),
                        new Variable("c"),
                        "*"),
                    "+"),
                new Variable("d"),
                "+"
            );
            expected.Accept(dumpVisitorExpected);
            Assert.AreEqual(dumpVisitorExpected.ToString(), dumpVisitorActual.ToString());
        }

        [Test]
        public void ParenExpression()
        {
            const string expr = "a+b/(c+d)";
            var dumpVisitorActual = new DumpVisitor();
            var dumpVisitorExpected = new DumpVisitor();
            SimpleParser.Parse(expr).Accept(dumpVisitorActual);
            var expected = new BinaryExpression(
                new Variable("a"),
                new BinaryExpression(
                    new Variable("b"),
                    new ParenExpression(
                        new BinaryExpression(
                            new Variable("c"),
                            new Variable("d"),
                            "+")
                    ), "/"
                ), "+");
            expected.Accept(dumpVisitorExpected);
            Assert.AreEqual(dumpVisitorExpected.ToString(), dumpVisitorActual.ToString());
        }
        
        
        [Test]
        public void ParenExpressionDivMinus()
        {
            const string expr = "5*(4-a)*b/c";
            var dumpVisitorActual = new DumpVisitor();
            var dumpVisitorExpected = new DumpVisitor();
            SimpleParser.Parse(expr).Accept(dumpVisitorActual);
            var expected = new BinaryExpression(new BinaryExpression(
                    new BinaryExpression(
                        new Literal("5"),
                        new ParenExpression(
                            new BinaryExpression(
                                new Literal("4"),
                                new Variable("a"),
                                "-")), "*"),
                    new Variable("b"),
                    "*"),
                new Variable("c"),
                "/");
            expected.Accept(dumpVisitorExpected);
            Assert.AreEqual(dumpVisitorExpected.ToString(), dumpVisitorActual.ToString());
        }
    }
}
