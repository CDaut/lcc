namespace Compiler.Lexer
{
    public enum TokenType
    {
        OpenParenthesisToken,
        CloseParenthesisToken,
        IntToken,
        OpenBraceToken,
        CloseBraceToken,
        ReturnToken,
        SemicolonToken,
        IdentifierToken,
        IntegerLiteralToken,
        
        //unary operator tokens
        NegationToken,
        BitwiseComplementToken,
        LogicalNegationToken,

        //special Token to represent invalid matches
        InvalidToken,
    }
}