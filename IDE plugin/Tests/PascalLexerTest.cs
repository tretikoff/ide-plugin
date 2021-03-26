using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IDE_plugin
{
    public class PascalLexerTest
    {
        private const string InnerComment1 = "{ Comment 1 (* comment 2 *) }";
        private const string InnerComment2 = "(* Comment 1 { comment 2 } *)";
        private const string InnerComment3 = "{ comment 1 // Comment 2 }";
        private const string InnerComment4 = "(* comment 1 // Comment 2 *)";
        private const string InnerComment5 = "// comment 1 (* comment 2 *)";
        private const string InnerComment6 = "// comment 1 { comment 2 }";
        private const string DelphiComment = "// This is a Delphi comment. All is ignored till the end of the line.";
        private const string TurboPascalComment = "{  This is a Turbo Pascal comment }";
        private const string OldStyleComment = "(* This is an old style comment *)";

        private static object Selector(Token i) => new {i.Type, i.Value};

        [Test]
        [TestCase(InnerComment1, " Comment 1 (* comment 2 *) ")]
        [TestCase(InnerComment2, " Comment 1 { comment 2 } ")]
        [TestCase(InnerComment3, " comment 1 // Comment 2 ")]
        [TestCase(InnerComment4, " comment 1 // Comment 2 ")]
        [TestCase(InnerComment5, " comment 1 (* comment 2 *)")]
        [TestCase(InnerComment6, " comment 1 { comment 2 }")]
        [TestCase(DelphiComment, " This is a Delphi comment. All is ignored till the end of the line.")]
        [TestCase(OldStyleComment, " This is an old style comment ")]
        [TestCase(TurboPascalComment, "  This is a Turbo Pascal comment ")]
        public void Comment(string expression, string value)
        {
            var actual = PascalLexer.Tokenize(expression);
            var expected = new List<Token> {new(TokenType.Comment, value)};
            Assert.That(actual.Select(Selector), Is.EquivalentTo(expected.Select(Selector)));
        }

        [TestCase("'The regular string'", "The regular string")]
        [TestCase("'This is a pascal string'", "This is a pascal string")]
        [TestCase("''", "")]
        [TestCase("'a'", "a")]
        [TestCase("'A tabulator character: '#9' is easy to embed'", "A tabulator character: '#9' is easy to embed")]
        public void String(string expression, string value)
        {
            var actual = PascalLexer.Tokenize(expression);
            var expected = new List<Token> {new(TokenType.CharacterString, value)};
            Assert.That(actual.Select(Selector), Is.EquivalentTo(expected.Select(Selector)));
        }

        [Test]
        public void Complex()
        {
            const string expression = @"Write('Enter a number:');
readln(no);";
            var actual = PascalLexer.Tokenize(expression);
            var expected = new List<Token>
            {
                new(TokenType.Identifier, "Write"),
                new(TokenType.Symbol, "("),
                new(TokenType.CharacterString, "Enter a number:"),
                new(TokenType.Symbol, ")"),
                new(TokenType.Symbol, ";"),
                new(TokenType.Identifier, "readln"),
                new(TokenType.Symbol, "("),
                new(TokenType.Identifier, "no"),
                new(TokenType.Symbol, ")"),
                new(TokenType.Symbol, ";")
            };
            Assert.That(actual.Select(Selector), Is.EquivalentTo(expected.Select(Selector)));
        }
        
        [Test]
        public void Complex2()
        {
            const string expression = @"program posneg;
uses crt;

var
  no: integer[length];
begin
  clrscr;
  Write(55+  8.9);
  Write('Enter a number:'); // User should enter the number";
            var actual = PascalLexer.Tokenize(expression);
            var expected = new List<Token>
            {
                new(TokenType.Identifier, "program"),
                new(TokenType.Identifier, "posneg"),
                new(TokenType.Symbol, ";"),
                new(TokenType.Identifier, "uses"),
                new(TokenType.Identifier, "crt"),
                new(TokenType.Symbol, ";"),
                new(TokenType.Identifier, "var"),
                new(TokenType.Identifier, "no"),
                new(TokenType.Symbol, ":"),
                new(TokenType.Identifier, "integer"),
                new(TokenType.Symbol, "["),
                new(TokenType.Identifier, "length"),
                new(TokenType.Symbol, "]"),
                new(TokenType.Symbol, ";"),
                new(TokenType.Identifier, "begin"),
                new(TokenType.Identifier, "clrscr"),
                new(TokenType.Symbol, ";"),
                new(TokenType.Identifier, "Write"),
                new(TokenType.Symbol, "("),
                new(TokenType.Number, "55"),
                new(TokenType.Symbol, "+"),
                new(TokenType.Number, "8.9"),
                new(TokenType.Symbol, ")"),
                new(TokenType.Symbol, ";"),
                new(TokenType.Identifier, "Write"),
                new(TokenType.Symbol, "("),
                new(TokenType.CharacterString, "Enter a number:"),
                new(TokenType.Symbol, ")"),
                new(TokenType.Symbol, ";"),
                new(TokenType.Comment, " User should enter the number")
            };
            Assert.That(actual.Select(Selector), Is.EquivalentTo(expected.Select(Selector)));
        }
    }
}
