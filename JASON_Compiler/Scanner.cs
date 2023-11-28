using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Else, If,
    Read, Then, Until, Write, Number, String, Comment,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,Main,
    Idenifier, Constant, DataTypes, repeat, Elseif, Return, endl, And, Or, Assignment, LBrackets, RBrackets
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
            ReservedWords.Add("main", Token_Class.Main);
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
            Operators.Add("{", Token_Class.LParanthesis);
            Operators.Add("}", Token_Class.RParanthesis);
            Operators.Add("(", Token_Class.LBrackets);
            Operators.Add(")", Token_Class.RBrackets);


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

            Operators.Add(":=", Token_Class.Assignment);

            DataTypes.Add("int", Token_Class.DataTypes);
            DataTypes.Add("float", Token_Class.DataTypes);
            DataTypes.Add("string", Token_Class.DataTypes);


        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                //string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    // Identifier // Reserved Word  // String // Data Type //
                    j += 1;
                    if (j < SourceCode.Length)
                    {

                        string CurrentLexeme = CurrentChar.ToString();
                        CurrentChar = SourceCode[j];
                        while (CurrentChar >= 'A' && CurrentChar <= 'z' || CurrentChar >= '0' && CurrentChar <= '9')
                        {
                            CurrentLexeme += CurrentChar;
                            j += 1;
                            if (j < SourceCode.Length)
                            {
                                CurrentChar = SourceCode[j];
                            }
                            else
                            {
                                break;
                            }
                        }
                        FindTokenClass(CurrentLexeme);
                        i = j - 1;
                    }
                    else
                    {
                        string CurrentLexeme = CurrentChar.ToString();
                        FindTokenClass(CurrentLexeme);
                    }
                }
                // Number // Constent //
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    if (j + 1 < SourceCode.Length)
                    {
                        j += 1;
                        string CurrentLexeme = CurrentChar.ToString();
                        CurrentChar = SourceCode[j];
                        while ((CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '.' || (CurrentChar >= 'A' && CurrentChar <= 'z'))
                        {

                            CurrentLexeme += CurrentChar;
                            j += 1;
                            if (j < SourceCode.Length)
                            {
                                CurrentChar = SourceCode[j];
                            }
                            else
                            {
                                break;
                            }

                        }
                        FindTokenClass(CurrentLexeme);
                        i = j - 1;
                    }
                    else
                    {
                        string CurrentLexeme = CurrentChar.ToString();
                        FindTokenClass(CurrentLexeme);
                    }
                }
                else if (CurrentChar == '\'')
                {
                    string CurrentLexeme = CurrentChar.ToString();
                    while (true)
                    {
                        j++;
                        if (j < SourceCode.Length)
                        {
                            CurrentChar = SourceCode[j];
                            if (CurrentChar == '\'')
                            {
                                CurrentLexeme += CurrentChar;
                                break;
                            }
                            else if (CurrentChar == '\n')
                            {
                                break;
                            }
                            else
                            {
                                CurrentLexeme += CurrentChar;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                //String Content
                else if (CurrentChar == '"')
                {
                    string CurrentLexeme = CurrentChar.ToString();
                    while (true)
                    {
                        j++;
                        if (j < SourceCode.Length)
                        {

                            CurrentChar = SourceCode[j];
                            if (CurrentChar == '"')
                            {
                                CurrentLexeme += CurrentChar;
                                break;
                            }
                            else if (CurrentChar == '\n')
                            {
                                break;
                            }
                            else
                            {
                                CurrentLexeme += CurrentChar;
                            }
                            
                        }
                        else
                        {
                            break;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                else if (CurrentChar == '(' || CurrentChar == ')' || CurrentChar == '{' || CurrentChar == '}')
                {
                    string CurrentLexeme = CurrentChar.ToString();
                    FindTokenClass(CurrentLexeme);
                }

                // Semicolon
                else if (CurrentChar == ';')
                {
                    string CurrentLexeme = CurrentChar.ToString();
                    CurrentChar = SourceCode[j];
                    FindTokenClass(CurrentLexeme);
                }
                // Operators
                else if ((CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*'))
                {
                    if (CurrentChar == '-')
                    {
                        j++;
                        if (j < SourceCode.Length) 
                        {
                            string CurrentLexeme = CurrentChar.ToString();
                            CurrentChar = SourceCode[j];
                            while (CurrentChar >= '0' && CurrentChar <= '9' || CurrentChar == '.')
                            {

                                CurrentLexeme += CurrentChar;
                                j += 1;
                                if (j < SourceCode.Length)
                                {
                                    CurrentChar = SourceCode[j];
                                }
                                else
                                {
                                    break;
                                }

                            }
                            FindTokenClass(CurrentLexeme);
                            i = j - 1;
                        }
                    }
                    else 
                    { 
                        string CurrentLexeme = CurrentChar.ToString();
                        CurrentChar = SourceCode[j];
                        FindTokenClass(CurrentLexeme);
                    }
                }
                // Boolean Operators || 
                else if (CurrentChar == '|')
                {
                    j++;
                    string CurrentLexeme = CurrentChar.ToString();
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '|')
                        {
                            CurrentLexeme += CurrentChar;
                            FindTokenClass(CurrentLexeme);
                            i = j - 1;
                        }
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                }
                else if (CurrentChar == '&')
                {
                    j++;
                    string CurrentLexeme = CurrentChar.ToString();
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '&')
                        {
                            CurrentLexeme += CurrentChar;
                            FindTokenClass(CurrentLexeme);
                            i = j - 1;
                        }
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                }
                // Comparetor operator
                else if (CurrentChar == '>' || CurrentChar == '<' || CurrentChar == '=')
                {
                    if (CurrentChar == '<')
                    {
                        j++;
                        string CurrentLexeme = CurrentChar.ToString();
                        if (j < SourceCode.Length)
                        {
                            CurrentChar = SourceCode[j];
                            if (CurrentChar == '>')
                            {
                                CurrentLexeme += CurrentChar;
                                FindTokenClass(CurrentLexeme);
                                i = j;
                            }
                            else
                            {
                                FindTokenClass(CurrentLexeme);
                            }
                        }
                        else { FindTokenClass(CurrentLexeme); }

                    }
                    else
                    {
                        string CurrentLexeme = CurrentChar.ToString();
                        CurrentChar = SourceCode[j];
                        FindTokenClass(CurrentLexeme);
                    }

                }
                // Assignment Operator
                else if ((CurrentChar == ':'))
                {
                    j++;
                    string CurrentLexeme = CurrentChar.ToString();
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '=')
                        {
                            CurrentLexeme += CurrentChar;
                            FindTokenClass(CurrentLexeme);
                            i = j + 1;
                        }
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }

                }
                // Comment  OR divide//
                else if (CurrentChar == '/')
                {
                    string CurrentLexeme = CurrentChar.ToString();
                    j++;
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '*')
                        {
                            CurrentLexeme += CurrentChar;
                            while (true)
                            {
                                j++;
                                if (j < SourceCode.Length)
                                {
                                    CurrentChar = SourceCode[j];
                                    if (CurrentChar == '*' || CurrentChar == '\n' || CurrentChar=='\r'|| CurrentChar == '\t')
                                    {
                                        if(CurrentChar == '\n' || CurrentChar == '\r' || CurrentChar == '\t')
                                        {
                                            break;
                                        }
                                        CurrentLexeme += CurrentChar;
                                        j++;
                                        if (j < SourceCode.Length)
                                        {
                                            CurrentChar = SourceCode[j];
                                            if (CurrentChar == '/')
                                            {
                                                CurrentLexeme += CurrentChar;
                                                //break;
                                            }
                                            else
                                            {
                                                CurrentLexeme += CurrentChar;
                                                continue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CurrentLexeme += CurrentChar;
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                            FindTokenClass(CurrentLexeme);
                            i = j;
                        }
                        else
                        {
                            FindTokenClass(CurrentLexeme);
                        }
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                }
            }

            JASON_COMPILER.JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
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
            else if (isBracket(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it a comment?
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
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
            //Is it an Assignment Operator/
            else if (isAssingmentOperator(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it a semicolon?
            else if (isSemiColon(Lex))
            {
                Tok.token_type = Token_Class.Semicolon;
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }
            else
            {
                Errors.Error_List.Add(Lex);
            }


        }
        bool isNumber(string lex)
        {

            var rx = new Regex(@"^[+-]?[0-9]+((\.)?([0-9])+)?$");
            
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
            if (System.Text.RegularExpressions.Regex.IsMatch(lex, "^\".*\"$"))
            {
                isValid = true;
            }

            return isValid;
        }
        bool isReserved(string lex)
        {
            if (ReservedWords.ContainsKey(lex))
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
            // Check if the lex is an identifier or not.
            var rx = new Regex("^[a-zA-Z][a-zA-Z0-9]*");
            if (rx.IsMatch(lex))
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
            var rx = new Regex(@"^/\*.*\*/$");
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


        bool isAssingmentOperator(string lex)
        {
            bool isValid = false;
            if (Operators.ContainsKey(lex) && (Operators[lex] == Token_Class.Assignment))
            {
                isValid = true;
            }

            return isValid;
        }

        bool isSemiColon(string lex)
        {
            string v = ";";
            if (lex == v)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isBracket(string lex) 
        {
            bool isValid = false;
            if (Operators.ContainsKey(lex) && (Operators[lex] == Token_Class.LParanthesis || Operators[lex] == Token_Class.RParanthesis || Operators[lex] == Token_Class.RBrackets || Operators[lex] == Token_Class.LBrackets))
            {
                isValid = true;
            }

            return isValid;
        }
    }
}