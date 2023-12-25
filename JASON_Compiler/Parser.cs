using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStreaminParser;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            InputPointer = 0;
            TokenStreaminParser = TokenStream.ToList();
            RemoveComments();
            root = new Node("Program");
            root.Children.Add(Programs());
            return root;
        }
        void RemoveComments()
        {
            foreach (Token token in TokenStreaminParser.ToList())
            {
                if(token.token_type == Token_Class.Comment)
                {
                    TokenStreaminParser.Remove(token);
                }
            } 
        }
        //23)Programs -> funcstat MainFunction
        Node Programs()
        {
            Node programs = new Node("Program");
            if (InputPointer < TokenStreaminParser.Count && (TokenStreaminParser[InputPointer].token_type == Token_Class.Int_Datatype || TokenStreaminParser[InputPointer].token_type == Token_Class.String_Datatype || TokenStreaminParser[InputPointer].token_type == Token_Class.Float_Datatype))
            {
                if (InputPointer +1  < TokenStreaminParser.Count && TokenStreaminParser[InputPointer +1 ].token_type == Token_Class.Main)
                {
                    programs.Children.Add(MainFunction());
                }
                else
                {
                    programs.Children.Add(Funcstat());
                    programs.Children.Add(MainFunction());
                }
            }
            //MessageBox.Show("Success");
            return programs;
        }

        // 1)FunctionCall ->  Identifier( idlist )
        Node FunctionCall()
        {
            Node functionCall = new Node("FunctionCall");
            functionCall.Children.Add(match(Token_Class.Idenifier));
            functionCall.Children.Add(match(Token_Class.LBrackets));
            functionCall.Children.Add(Idlist());
            functionCall.Children.Add(match(Token_Class.RBrackets));

            return functionCall;
        }

        //Idlist ->  identifier idenlist | number idenlist |ε
        Node Idlist()
        {
            Node idlist = new Node("Idlist");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                idlist.Children.Add(match(Token_Class.Idenifier));
                idlist.Children.Add(Idenlist());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Number)
            {
                idlist.Children.Add(match(Token_Class.Number));
                idlist.Children.Add(Idenlist());
            }
            else
            {
                return null;
            }
            return idlist;
        }

        //Idenlist -> , Ide idenlist | ε
        Node Idenlist()
        {
            Node idenlist = new Node("Idenlist");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Comma)
            {
                idenlist.Children.Add(match(Token_Class.Comma));
                idenlist.Children.Add(Ide());
                idenlist.Children.Add(Idenlist());
            }
            else
            {
                return null;
            }
            return idenlist;
        }
        //Ide -> identifier | number
        Node Ide()
        {
            Node ide = new Node("Ide");
            if(InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Number)
            {
                ide.Children.Add(match(Token_Class.Number));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                ide.Children.Add(match(Token_Class.Idenifier));
            }
            else
            {
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected identifier or number and " +
                           TokenStreaminParser[InputPointer].token_type.ToString() +
                           "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected identifier or number \r\n");
                    InputPointer++;
                }
                return null;
            }
            return ide; 
        }
        //2)Term -> Number | Identifier | FunctionCall 
        Node Term()
        {
            Node term = new Node("Term");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Number)
            {
                term.Children.Add(match(Token_Class.Number));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {

                if (InputPointer + 1 < TokenStreaminParser.Count && TokenStreaminParser[InputPointer + 1].token_type == Token_Class.LBrackets)
                {
                    term.Children.Add(FunctionCall());
                }
                else
                {
                    term.Children.Add(match(Token_Class.Idenifier));
                }
            }
            else
            {
                //ErroList
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Term and " +
                           TokenStreaminParser[InputPointer].token_type.ToString() +
                           "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Term and \r\n");
                    InputPointer++;
                }
                return null;
            }
            return term;
        }

        //3)Equation -> TermList  |( Eq ) EquationList
        Node Equation()
        {
            Node equation = new Node("Equation");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.LBrackets)
            {
                equation.Children.Add(match(Token_Class.LBrackets));
                equation.Children.Add(Eq());
                equation.Children.Add(match(Token_Class.RBrackets));
                equation.Children.Add(EquationList());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Number)
            {
                equation.Children.Add(TermList());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                equation.Children.Add(TermList());
            }
            else
            {
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Equation and " +
                          TokenStreaminParser[InputPointer].token_type.ToString() +
                          "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Equation \r\n");
                    InputPointer++;
                }
                return null;
            }
            return equation;
        }
        //TermList -> Term ArithmaticOperator EqList EquationList 
        Node TermList()
        {
            Node termList = new Node("Term List");
            termList.Children.Add(Term());
            termList.Children.Add(ArithmaticOperator());
            termList.Children.Add(EqList());
            termList.Children.Add(EquationList());
            return termList;
        }
        //Eq -> TermList  |( TermList ) EquationList 
        Node Eq()
        {
            Node eq = new Node("Eq");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.LBrackets)
            {
                eq.Children.Add(match(Token_Class.LBrackets));
                eq.Children.Add(TermList());
                eq.Children.Add(match(Token_Class.RBrackets));
                eq.Children.Add(EquationList());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Number)
            {
                eq.Children.Add(TermList());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                eq.Children.Add(TermList());
            }
            else
            {
                if (InputPointer == TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Equation and " +
                         TokenStreaminParser[InputPointer].token_type.ToString() +
                         "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Equation \r\n");
                    InputPointer++;
                }
                return null;
            }
            return eq;
        }
        //EquationList ->  ArithmaticOperator EqList  | ε
        Node EquationList()
        {
            Node equationList = new Node("Equation List");
            if (InputPointer < TokenStreaminParser.Count && (TokenStreaminParser[InputPointer].token_type == Token_Class.PlusOp || TokenStreaminParser[InputPointer].token_type == Token_Class.MinusOp || TokenStreaminParser[InputPointer].token_type == Token_Class.MultiplyOp || TokenStreaminParser[InputPointer].token_type == Token_Class.DivideOp))
            {
                equationList.Children.Add(ArithmaticOperator());
                equationList.Children.Add(EqList());
            }
            else
            {
                return null;
            }
            return equationList;
        }
        // EqList -> Term EquationList |(TermList ) EquationList
        Node EqList()
        {
            Node eqList = new Node(" Eq List");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                eqList.Children.Add(Term());
                eqList.Children.Add(EquationList());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Number)
            {
                eqList.Children.Add(Term());
                eqList.Children.Add(EquationList());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.LBrackets)
            {
                eqList.Children.Add(match(Token_Class.LBrackets));
                eqList.Children.Add(TermList());
                eqList.Children.Add(match(Token_Class.RBrackets));
                eqList.Children.Add(EquationList());
            }
            else
            {
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected EqList and " +
                         TokenStreaminParser[InputPointer].token_type.ToString() +
                         "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected EqList \r\n");
                    InputPointer++;
                }
                return null;
            }
            return eqList;
        }
        // ArithmaticOperator -> + | - | * | /
        Node ArithmaticOperator()
        {
            Node arithmaticOperator = new Node("Arithmatic Operator");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.PlusOp)
            {
                arithmaticOperator.Children.Add(match(Token_Class.PlusOp));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.MinusOp)
            {
                arithmaticOperator.Children.Add(match(Token_Class.MinusOp));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                arithmaticOperator.Children.Add(match(Token_Class.MultiplyOp));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.DivideOp)
            {
                arithmaticOperator.Children.Add(match(Token_Class.DivideOp));
            }
            else
            {
                // ErrorList
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Arithmatic Operator and " +
                         TokenStreaminParser[InputPointer].token_type.ToString() +
                         "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Arithmatic Operator\r\n");
                    InputPointer++;
                }
                return null;
            }
            return arithmaticOperator;

        }
        //4)Expression -> String | Term | Equation
        Node Expression()
        {
            Node expression = new Node("Expression");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.String)
            {
                expression.Children.Add(match(Token_Class.String));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Number)
            {
                if (InputPointer + 1 < TokenStreaminParser.Count && (TokenStreaminParser[InputPointer + 1].token_type == Token_Class.PlusOp || TokenStreaminParser[InputPointer + 1].token_type == Token_Class.MinusOp || TokenStreaminParser[InputPointer + 1].token_type == Token_Class.MultiplyOp || TokenStreaminParser[InputPointer + 1].token_type == Token_Class.DivideOp))
                {
                    expression.Children.Add(Equation());
                }
                else
                {
                    expression.Children.Add(Term());
                }

            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (InputPointer + 1 < TokenStreaminParser.Count && (TokenStreaminParser[InputPointer + 1].token_type == Token_Class.PlusOp || TokenStreaminParser[InputPointer + 1].token_type == Token_Class.MinusOp || TokenStreaminParser[InputPointer + 1].token_type == Token_Class.MultiplyOp || TokenStreaminParser[InputPointer + 1].token_type == Token_Class.DivideOp))
                {
                    expression.Children.Add(Equation());
                }
                else
                {
                    expression.Children.Add(Term());
                }

            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.LBrackets)
            {
                expression.Children.Add(Equation());
            }
            else
            {
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Expression and " +
                      TokenStreaminParser[InputPointer].token_type.ToString() +
                      "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Expression \r\n");
                    InputPointer++;
                }
                return null;
            }
            return expression;
        }

        // 5)Assignment Statement  Identifier := Expression
        Node AssignmentStatment()
        {
            Node assignmentStatment = new Node("Assignment Statment");
            assignmentStatment.Children.Add(match(Token_Class.Idenifier));
            assignmentStatment.Children.Add(match(Token_Class.Assignment));
            assignmentStatment.Children.Add(Expression());
            return assignmentStatment;
        }

        //6)Declaration Statement -> Datatype Declist;
        Node DeclarationStatement()
        {
            Node declarationStatement = new Node("Declaration Statement");
            declarationStatement.Children.Add(Datatype());  //modified --> Datatype()
            declarationStatement.Children.Add(Declist());
            declarationStatement.Children.Add(match(Token_Class.Semicolon));
            return declarationStatement;
        }

        //Declist -> Identifier Assignmentlist | AssignmentStatement assignmentlist
        Node Declist()
        {
            Node declist = new Node("Declist");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (InputPointer + 1 < TokenStreaminParser.Count && TokenStreaminParser[InputPointer + 1].token_type == Token_Class.Assignment)
                {
                    declist.Children.Add(AssignmentStatment());
                    declist.Children.Add(Assignmentlist());
                }
                else
                {
                    declist.Children.Add(match(Token_Class.Idenifier));
                    declist.Children.Add(Assignmentlist());
                }
            }
            else
            {
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Dec list and " +
                          TokenStreaminParser[InputPointer].token_type.ToString() +
                          "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Dec list\r\n");
                    InputPointer++;
                }
                return null;
            }
            return declist;
        }

        // Assignmentlist -> , AssList | ε
        Node Assignmentlist()
        {
            Node assignmentlist = new Node("Assignment list");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Comma)
            {
                assignmentlist.Children.Add(match(Token_Class.Comma));
                assignmentlist.Children.Add(AssList());
            }
            else
            {
                return null;
            }
            return assignmentlist;
        }

        //AssList -> AssignmentStatement Assignmentlist | Identifier Assignmentlist 
        Node AssList()
        {
            Node assList = new Node("Ass List");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (InputPointer + 1 < TokenStreaminParser.Count && TokenStreaminParser[InputPointer + 1].token_type == Token_Class.Assignment)
                {
                    assList.Children.Add(AssignmentStatment());
                    assList.Children.Add(Assignmentlist());
                }
                else
                {
                    assList.Children.Add(match(Token_Class.Idenifier));
                    assList.Children.Add(Assignmentlist());
                }
            }
            else
            {
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Ass List and " +
                          TokenStreaminParser[InputPointer].token_type.ToString() +
                          "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Ass List \r\n");
                    InputPointer++;
                }
                return null;
            }
            return assList;
        }

        //Datatype -> int | float | string
        Node Datatype()
        {
            Node datatype = new Node("Data type");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Float_Datatype)
            {
                datatype.Children.Add(match(Token_Class.Float_Datatype));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Int_Datatype)
            {
                datatype.Children.Add(match(Token_Class.Int_Datatype));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.String_Datatype)
            {
                datatype.Children.Add(match(Token_Class.String_Datatype));
            }
            else
            {
                //Error List
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Datatype and " +
                          TokenStreaminParser[InputPointer].token_type.ToString() +
                          "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Datatype\r\n");
                    InputPointer++;
                }
                return null;
            }
            return datatype;
        }

        // 7)Write Statement -> write WriteStmt;        
        Node WriteStatement()
        {
            Node writeStatement = new Node("Write Statement");
            writeStatement.Children.Add(match(Token_Class.Write));
            writeStatement.Children.Add(WriteStmt());
            writeStatement.Children.Add(match(Token_Class.Semicolon));
            return writeStatement;
        }

        //  WriteStmt -> Expression | endl    
        Node WriteStmt()
        {
            Node writeStmt = new Node("Write Stmt");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.endl)
            {
                writeStmt.Children.Add(match(Token_Class.endl));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.String)
            {
                writeStmt.Children.Add(Expression());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Number)
            {
                writeStmt.Children.Add(Expression());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                writeStmt.Children.Add(Expression());
            }
            else
            {
                //Error List
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Write Stmt and " +
                          TokenStreaminParser[InputPointer].token_type.ToString() +
                          "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Write Stmt \r\n");
                    InputPointer++;
                }
                return null;
            }
            return writeStmt;
        }

        // 8)ReadStatement  read identifier ;
        Node ReadStatement()
        {
            Node readStatement = new Node("Read Statement");
            readStatement.Children.Add(match(Token_Class.Read));
            readStatement.Children.Add(match(Token_Class.Idenifier));
            readStatement.Children.Add(match(Token_Class.Semicolon));
            return readStatement;
        }

        //9)ReturnStatement  return Expression;
        Node ReturnStatement()
        {
            Node returnStatement = new Node("Return Statement");
            returnStatement.Children.Add(match(Token_Class.Return));
            returnStatement.Children.Add(Expression());
            returnStatement.Children.Add(match(Token_Class.Semicolon));
            return returnStatement;
        }

        //Condition -->  identifier ConditionalOperator Term
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(ConditionalOperator());
            condition.Children.Add(Term());
            return condition;

        }
        //ConditionalOperator  < | > | = | <>
        Node ConditionalOperator()
        {
            Node conditionalOperator = new Node("Conditional Operator");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.LessThanOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.EqualOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.EqualOp));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.NotEqualOp));
            }
            else
            {
                // ErrorList
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Conditional Operator and " +
                          TokenStreaminParser[InputPointer].token_type.ToString() +
                          "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Conditional Operator \r\n");
                    InputPointer++;
                }
                return null;
            }
            return conditionalOperator;

        }

        // 11)ConditionalStatement -> Condition BooleanStatments
        Node ConditionalStatement()
        {
            Node conditionalStatement = new Node("Conditional Statement");
            conditionalStatement.Children.Add(Condition());
            conditionalStatement.Children.Add(BooleanStatments());
            return conditionalStatement;
        }
        //BooleanStatments  BooleanOperator Condition BooleanStatments | ε
        Node BooleanStatments()
        {
            Node booleanStatments = new Node("Boolean Statments");
            if (InputPointer < TokenStreaminParser.Count && (TokenStreaminParser[InputPointer].token_type == Token_Class.Or || TokenStreaminParser[InputPointer].token_type == Token_Class.And))
            {
                booleanStatments.Children.Add(BooleanOperator());
                booleanStatments.Children.Add(Condition());
                booleanStatments.Children.Add(BooleanStatments());
            }
            else
            {
                return null;
            }
            return booleanStatments;
        }
        //BooleanOperator -> && | ||
        Node BooleanOperator()
        {
            Node booleanOperator = new Node("Boolean Operator");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Or)
            {
                booleanOperator.Children.Add(match(Token_Class.Or));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.And)
            {
                booleanOperator.Children.Add(match(Token_Class.And));
            }
            else
            {
                // ErrorList
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Boolean Operator and " +
                          TokenStreaminParser[InputPointer].token_type.ToString() +
                          "  found\r\n");
                    InputPointer++;
                }
                else 
                {
                    Errors.Error_List.Add("Parsing Error: Expected Boolean Operator \r\n");
                    InputPointer++;
                }
                return null;
            }
            return booleanOperator;
        }

        // 12)Statement -> Write Statement | Read Statement | Assignment Statement | Declaration Statement | Repeat Statement | IF Statement | ε
        Node Statement()
        {
            Node statement = new Node("Statement");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Write)
            {
                statement.Children.Add(WriteStatement());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Read)
            {
                statement.Children.Add(ReadStatement());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (InputPointer + 1 < TokenStreaminParser.Count && TokenStreaminParser[InputPointer + 1].token_type == Token_Class.Assignment)
                {
                    statement.Children.Add(AssignmentStatment());
                    statement.Children.Add(match(Token_Class.Semicolon));
                }
                else
                {
                    
                        Errors.Error_List.Add("Parsing Error: Expected assignment operator and " +
                               TokenStreaminParser[InputPointer + 1].token_type.ToString() +
                               "  found\r\n");
                        InputPointer++;
                    
                    return null;
                }
            }
            else if (InputPointer < TokenStreaminParser.Count && (TokenStreaminParser[InputPointer].token_type == Token_Class.Float_Datatype|| TokenStreaminParser[InputPointer].token_type == Token_Class.String_Datatype || TokenStreaminParser[InputPointer].token_type == Token_Class.Int_Datatype))
            {
                statement.Children.Add(DeclarationStatement());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.repeat)
            {
                statement.Children.Add(RepeatStatment());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.If)
            {
                statement.Children.Add(IFStatement());
            }
            else
            {
                return null;
            }
            return statement;
        }
        //Statements -> Statement State
        Node Statements()
        {
            Node statements = new Node("Statements");
            statements.Children.Add(Statement());
            statements.Children.Add(State());
            
            return statements;
        }
        //State --> Statment State | ε
        Node State ()
        {
            Node states = new Node("State");
            if(InputPointer < TokenStreaminParser.Count && (TokenStreaminParser[InputPointer].token_type == Token_Class.Write || TokenStreaminParser[InputPointer].token_type == Token_Class.Read || TokenStreaminParser[InputPointer].token_type == Token_Class.Idenifier || TokenStreaminParser[InputPointer].token_type == Token_Class.Int_Datatype || TokenStreaminParser[InputPointer].token_type == Token_Class.String_Datatype || TokenStreaminParser[InputPointer].token_type == Token_Class.Float_Datatype || TokenStreaminParser[InputPointer].token_type == Token_Class.repeat || TokenStreaminParser[InputPointer].token_type == Token_Class.If))
            {
                states.Children.Add(Statement());
                states.Children.Add(State());
            }
            else
            {
                return null;
            }
            return states;
        }
        // 13)IFStatement -> if ConditionalStatement then Statements IFStmtList
        Node IFStatement()
        {
            Node iFStatement = new Node("IF Statement");

            iFStatement.Children.Add(match(Token_Class.If));
            iFStatement.Children.Add(ConditionalStatement());
            iFStatement.Children.Add(match(Token_Class.Then));
            iFStatement.Children.Add(Statements());
            iFStatement.Children.Add(IFStmtList());

            return iFStatement;
        }
        //IFStmtList -> ElseIfStatement ElseIfStmtList | ElseStatement | end
        Node IFStmtList()
        {
            Node iFStmtList = new Node("IF Stmt List");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Elseif)
            {
                iFStmtList.Children.Add(ElseIfStatement());
                iFStmtList.Children.Add(ElseIfStmtList());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Else)
            {
                iFStmtList.Children.Add(ElseStatement());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.End)
            {
                iFStmtList.Children.Add(match(Token_Class.End));
            }
            else
            {
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected end or else statement or else if statement and " +
                          TokenStreaminParser[InputPointer].token_type.ToString() +
                          "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected end or else statement or else if statement \r\n");
                    InputPointer++;
                }
                return null;
            }
            return iFStmtList;
        }
        //ElseIfStmtList -> ElseIfStatement ElseIfStmtList| ε
        Node ElseIfStmtList()
        {
            Node elseIfStmtList = new Node("Else If Stmt List");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Elseif)
            {
                elseIfStmtList.Children.Add(ElseIfStatement());
                elseIfStmtList.Children.Add(ElseIfStmtList());
            }
            else
            {
                return null;
            }
            return elseIfStmtList;
        }
        //14)ElseIfStatement -> elseif ConditionStatement then Statements ElseIfFollowingList
        Node ElseIfStatement()
        {
            Node elseIfStatement = new Node("Else If Statement");
            elseIfStatement.Children.Add(match(Token_Class.Elseif));
            elseIfStatement.Children.Add(ConditionalStatement());
            elseIfStatement.Children.Add(match(Token_Class.Then));
            elseIfStatement.Children.Add(Statements());
            elseIfStatement.Children.Add(ElseIfFollowingList());
            return elseIfStatement;
        }
        //ElseIfFollowingList -> ElseStatment | end | ElseIfStmtList
        Node ElseIfFollowingList()
        {
            Node elseIfFollowingList = new Node("Else If Following List");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.End)
            {
                elseIfFollowingList.Children.Add(match(Token_Class.End));
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Else)
            {
                elseIfFollowingList.Children.Add(ElseStatement());
            }
            else if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Elseif)
            {
                elseIfFollowingList.Children.Add(ElseIfStmtList());
            }
            else
            {
                if (InputPointer < TokenStreaminParser.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected end or else statement and " +
                          TokenStreaminParser[InputPointer].token_type.ToString() +
                          "  found\r\n");
                    InputPointer++;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected end or else statement \r\n");
                    InputPointer++;
                }
                return null;
            }
            return elseIfFollowingList;
        }
        //15)ElseStatement -> else Statements end
        Node ElseStatement()
        {
            Node elseStatement = new Node("Else Statement");
            elseStatement.Children.Add(match(Token_Class.Else));
            elseStatement.Children.Add(Statements());
            elseStatement.Children.Add(match(Token_Class.End));
            return elseStatement;
        }
        //Repeat Statement -> repeat Statements until ConditionalStatement
        Node RepeatStatment()
        {
            Node repeatStatement = new Node("Repeat Statement");
            repeatStatement.Children.Add(match(Token_Class.repeat));
            repeatStatement.Children.Add(Statements());
            repeatStatement.Children.Add(match(Token_Class.Until));
            repeatStatement.Children.Add(ConditionalStatement());
            return repeatStatement;
        }
        //17)FunctionName ->  identifier
        Node FunctionName()
        {
            Node functionName = new Node("Function Name");
            functionName.Children.Add(match(Token_Class.Idenifier));
            return functionName;
        }

        //18)Parameter -> Datatype identifier
        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Idenifier));
            return parameter;
        }
        //19)FunctionDeclaration -> Datatype FunctionName (Paramlist)
        Node FunctionDeclaration()
        {
            Node functionDeclaration = new Node("Function Declaration");
            functionDeclaration.Children.Add(Datatype());
            functionDeclaration.Children.Add(FunctionName());
            functionDeclaration.Children.Add(match(Token_Class.LBrackets));
            functionDeclaration.Children.Add(Paramlist());
            functionDeclaration.Children.Add(match(Token_Class.RBrackets));
            return functionDeclaration;
        }
        //Paramlist -> Parameter Parameters | ε
        Node Paramlist()
        {
            Node paramlist = new Node("Param list");
            if (InputPointer < TokenStreaminParser.Count && (TokenStreaminParser[InputPointer].token_type == Token_Class.Float_Datatype|| TokenStreaminParser[InputPointer].token_type == Token_Class.Int_Datatype|| TokenStreaminParser[InputPointer].token_type == Token_Class.String_Datatype))
            {
                paramlist.Children.Add(Parameter());
                paramlist.Children.Add(Parameters());
            }
            else
            {
                return null;
            }
            return paramlist;

        }
        //Parameters -> , Parameter parameters | ε
        Node Parameters()
        {
            Node parameters = new Node("Parameters");
            if (InputPointer < TokenStreaminParser.Count && TokenStreaminParser[InputPointer].token_type == Token_Class.Comma)
            {
                parameters.Children.Add(match(Token_Class.Comma));
                parameters.Children.Add(Parameter());
                parameters.Children.Add(Parameters());
            }
            else
            {
                return null;
            }
            return parameters;
        }
        //20)FunctionBody -> { Statements ReturnStatement }
        Node FunctionBody()
        {
            Node functionBody = new Node("Function Body");
            functionBody.Children.Add(match(Token_Class.LParanthesis));
            functionBody.Children.Add(Statements());
            functionBody.Children.Add(ReturnStatement());
            functionBody.Children.Add(match(Token_Class.RParanthesis));
            return functionBody;
        }
        //21)FunctionStatment -> FunctionDeclaration FunctionBody
        Node FunctionStatment()
        {
            Node functionStatment = new Node("Function Statment");
            functionStatment.Children.Add(FunctionDeclaration());
            functionStatment.Children.Add(FunctionBody());
            return functionStatment;
        }
        //22)MainFunction -> Datatype main () FunctionBody
        Node MainFunction()
        {
            Node mainFunction = new Node("Main Function");
            mainFunction.Children.Add(Datatype());
            mainFunction.Children.Add(match(Token_Class.Main));
            mainFunction.Children.Add(match(Token_Class.LBrackets));
            mainFunction.Children.Add(match(Token_Class.RBrackets));
            mainFunction.Children.Add(FunctionBody());
            return mainFunction;
        }
        
        //Funcstat ->  FunctionStatment funcstat | ε 
        Node Funcstat()
        {
            Node funcstat = new Node("Func stat");
            if (InputPointer < TokenStreaminParser.Count && (TokenStreaminParser[InputPointer].token_type == Token_Class.Float_Datatype|| TokenStreaminParser[InputPointer].token_type == Token_Class.Int_Datatype|| TokenStreaminParser[InputPointer].token_type == Token_Class.String_Datatype))
            {
                if (InputPointer+1 < TokenStreaminParser.Count && TokenStreaminParser[InputPointer+1].token_type == Token_Class.Main)
                {
                    return null;
                }
                funcstat.Children.Add(FunctionStatment());
                funcstat.Children.Add(Funcstat());
            }
            else
            {
                return null;
            }
            return funcstat;
        }

        //================================================================================
        // Implement your logic here
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStreaminParser.Count)
            {
                if (ExpectedToken == TokenStreaminParser[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStreaminParser[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
