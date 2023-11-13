using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Else, If,
    Read, Then, Until, Write,Number,String,Comment,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant , DataTypes ,  repeat, Elseif ,  Return , endl , And , Or
}
namespace JASON_Compiler
{
    

    public class Token
    {
       public string lex;
       public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> DataTypes = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.If);
           
            ReservedWords.Add("else", Token_Class.Else);
            
            ReservedWords.Add("read", Token_Class.Read);
          
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("write", Token_Class.Write);

            ReservedWords.Add("repeat", Token_Class.repeat);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("return", Token_Class.Return);

            ReservedWords.Add("endl", Token_Class.endl);
      

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);

            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);

            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);

            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);

            DataTypes.Add("int", Token_Class.DataTypes);
            DataTypes.Add("float", Token_Class.DataTypes);
            DataTypes.Add("string", Token_Class.DataTypes);


        }

    public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                //string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    // Identifier // Reserved Word  
                    j += 1;
                    string CurrentLexeme = CurrentChar.ToString();
                    CurrentChar = SourceCode[j];
                    while (CurrentChar >= 'A' && CurrentChar <='z' || CurrentChar >= '1' &&   CurrentChar <= '9')
                    {
                        CurrentLexeme += CurrentChar;
                        j += 1;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;

                }

                else if(CurrentChar >= '0' && CurrentChar <= '9')
                {
                   
                }
                else if(CurrentChar == '{')
                {
                   
                }
                else
                {
                   
                }
            }

            JASON_COMPILER.JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (isReserved(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it Data Type?
            else if (isDataType(Lex))
            {
                Tok.token_type = DataTypes[Lex];
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }
            //Is it a Number?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }
            //Is it a string?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            //Is it a comment?
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if (isOperator(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it a Conditional Operator?
            else if (isconditinalOperator(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it a Boolean Operator?
            else if (isBooleanOperator(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
          

        }
        bool isNumber(string lex) {

            var rx = new Regex(@"[+-]?[0-9]+(\.[0-9]+)?([Ee][+-]?[0-9]+)?");

            if (rx.IsMatch(lex))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        bool isString(string lex)
        {
            bool isValid = false;

            if (System.Text.RegularExpressions.Regex.IsMatch(lex, "^\"[a-zA-Z0-9\\s]*\"$"))
            {
                isValid = true;
            }

            return isValid;
        }
        bool isReserved(string lex)
        {
            if(ReservedWords.ContainsKey(lex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        

        bool isIdentifier(string lex)
        {
            //bool isValid=true;
            // Check if the lex is an identifier or not.
            var rx = new Regex("[a-zA-Z][a-zA-Z0-9]*");
            if(rx.IsMatch(lex))
            {
                return true;
            }
            else
            {
                return false;
            }
           // return isValid;
        }
        bool isComment(string lex)
        {
            var rx = new Regex("\\/\\*.*?\\*\\/\r\n");
            if (rx.IsMatch(lex))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        bool isDataType(string lex)
        {
            bool isValid = false;
            if (DataTypes.ContainsKey(lex))
            {
                isValid = true;
            }

            return isValid;
        }
        bool isOperator(string lex)
        {
            bool isValid = false;
            if (Operators.ContainsKey(lex) && (Operators[lex] == Token_Class.PlusOp || Operators[lex] == Token_Class.MinusOp || Operators[lex] == Token_Class.MultiplyOp || Operators[lex] == Token_Class.DivideOp))
            {
                isValid = true;
            }

            return isValid;
        }
        bool isconditinalOperator(string lex)
        {
            bool isValid = false;
            if (Operators.ContainsKey(lex) && (Operators[lex] == Token_Class.EqualOp || Operators[lex] == Token_Class.LessThanOp || Operators[lex] == Token_Class.GreaterThanOp || Operators[lex] == Token_Class.NotEqualOp))
            {
                isValid = true;
            }

            return isValid;
        }

        bool isBooleanOperator(string lex)
        {
            bool isValid = false;
            if (Operators.ContainsKey(lex) && (Operators[lex] == Token_Class.And || Operators[lex] == Token_Class.Or))
            {
                isValid = true;
            }

            return isValid;
        }


    }
}
