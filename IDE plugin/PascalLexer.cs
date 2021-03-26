using System;
using System.Collections.Generic;
using System.Linq;

namespace IDE_plugin
{
    public static class PascalLexer
    {
        public static IEnumerable<Token> Tokenize(string program)
        {
            var tokens = new List<Token>();
            var remainingText = program;
            while (!string.IsNullOrWhiteSpace(remainingText))
            { 
                var match = FindMatch(remainingText);
                if (match.Value.Length == 0 && match.TokenType == TokenType.Symbol)
                {
                    throw new ArgumentException("Could not parse the argument");
                }
                tokens.Add(new Token(match.TokenType, match.Value));
                remainingText = match.RemainingText;
            }

            return tokens;
        }

        private static string WhiteSpaced(string val)
        {
            return "\\s*" + val + "\\s*";
        }

        private static readonly List<TokenDefinition> TokenDefinitions = new()
        {
            new TokenDefinition(TokenType.Comment, WhiteSpaced("^//(.*?)(\n|$)")),
            new TokenDefinition(TokenType.Comment, WhiteSpaced("^\\(\\*(.*?)\\*\\)")),
            new TokenDefinition(TokenType.Comment, WhiteSpaced("^{(.*?)}")),
            new TokenDefinition(TokenType.CharacterString, WhiteSpaced("^'(([^']|'#\\d+')*)'|''")),
            new TokenDefinition(TokenType.Number, WhiteSpaced("^([+-]?([0-9]*[.])?[0-9]+)")),
            new TokenDefinition(TokenType.Identifier, WhiteSpaced("^(\\w(\\w|\\d)*)")),
            new TokenDefinition(TokenType.Symbol, WhiteSpaced("^(\\;|'|\\+|-|\\*|\\/|=|<|>|\\[|\\]|\\.|,|\\(|\\)|:|^|@|{|}|$|#|&|%<<|>>|\\*\\*|<>|><|<=|>=|:=|\\+=|-=|\\*=|\\/=|\\(\\*|\\*\\)|\\(\\.|\\.\\)|\\)\\/\\/|\\w|[0-9])"))
        };

        private static TokenMatch FindMatch(string lqlText)
        {
            foreach (var match in TokenDefinitions.Select(tokenDefinition => tokenDefinition.Match(lqlText))
                .Where(match => match.IsMatch))
            {
                return match;
            }

            return new TokenMatch {IsMatch = false};
        }
    }
}
