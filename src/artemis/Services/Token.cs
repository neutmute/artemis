/********************************************************
 *	Author: Andrew Deren
 *	Date: July, 2004
 *	http://www.adersoftware.com
 * 
 *	StringTokenizer class. You can use this class in any way you want
 * as long as this header remains in this file.
 * 
 **********************************************************/

using System;

namespace Artemis
{
    [Flags]
    public enum TokenKind
    {
        Illegal = 0,
        Unknown = 256,
        Word = 2,
        Number = 4,
        QuotedString = 8,
        WhiteSpace = 16,
        Symbol = 32,
        EOL = 64,
        EOF = 128,
        NotOperator = 512,
        KeyValueOperator = 1028,
        AttributeOperator = 2048,
        // Searchable = Word | Number | QuotedString
    }

    public class Token
    {
        int line;
        int column;
        string value;
        TokenKind kind;

        public Token(TokenKind kind, string value, int line, int column)
        {
            this.kind = kind;
            this.value = value;
            this.line = line;
            this.column = column;
        }

        public int Column
        {
            get { return this.column; }
        }

        public TokenKind Kind
        {
            get { return this.kind; }
        }

        public int Line
        {
            get { return this.line; }
        }

        public string Value
        {
            get
            {
                if (Kind == TokenKind.QuotedString)
                {
                    return value.Substring(1, value.Length - 2);
                }
                else
                {
                    return this.value;
                }
            }
        }
    }

}
