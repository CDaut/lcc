namespace Compiler.Parser.Nodes
{
    public enum OperatorType
    {
        //unary operators
        Negation,
        BitwiseComplement,
        LogicalNegation,
        
        //binary operators
        Addition,
        Subtraction,
        Multiplication,
        Division
    }
}