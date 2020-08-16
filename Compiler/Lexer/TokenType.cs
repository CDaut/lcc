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
        
        //special Token to represent invalid matches
        InvalidToken
        
    }
}