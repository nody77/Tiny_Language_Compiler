using System;
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
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(match(Token_Class.Dot));
            MessageBox.Show("Success");
            return program;
        }
        
        // 1)Function Call ->  Identifier( idlist )
        Node FunctionCall()
        {
            Node functionCall = new Node("FunctionCall");
            functionCall.Children.Add(match(Token_Class.Idenifier));
            functionCall.Children.Add(match(Token_Class.LBrackets));
            functionCall.Children.Add(Idlist());
            functionCall.Children.Add(match(Token_Class.RBrackets));

            return functionCall;
        }

        //Idlist ->  identifier idenlist | ε
        Node Idlist()
        {
            Node idlist = new Node("Idlist");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                idlist.Children.Add(match(Token_Class.Idenifier));
                idlist.Children.Add(Idenlist());
            }
            return idlist;
        }

        //Idenlist -> , identifier idenlist | ε
        Node Idenlist()
        {
            Node idenlist = new Node("Idenlist");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                idenlist.Children.Add(match(Token_Class.Comma));
                idenlist.Children.Add(match(Token_Class.Idenifier));
                idenlist.Children.Add(Idenlist());
            }
            return idenlist;
        }

        //2)Term  Number | Identifier | Function Call    --> not done yet
        Node Term()
        {
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                term.Children.Add(match(Token_Class.Number));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                term.Children.Add(match(Token_Class.Idenifier));
            }
            else
            {
                term.Children.Add(FunctionCall());
            }
            return term;
        }

        Node Equation() // --> not done yet 
        {
            Node equation = new Node("Equation");
            return equation;
        }

        //4)Expression  String | Term | Equation --> not done yet 
        Node Expression()
        {
            Node expression = new Node("Expression");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                expression.Children.Add(match(Token_Class.Number));
            }
            //else if (InputPointer < TokenStream.Count && TokenStream[InputPointer]. == Token_Class)
            //{
            //    expression.Children.Add(match(Token_Class.Idenifier));
            //}
            //else
            //{
            //    expression.Children.Add(FunctionCall());
            //}
            return expression;
        }

        // 5)Assignment Statement  Identifier := Expression
        Node AssignmentStatment ()
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
            return declarationStatement;
        }

        //Declist -> Identifier Assignmentlist | AssignmentStatement assignmentlist  --> not done yet
        Node Declist()
        {
            Node declist = new Node("Declist");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                declist.Children.Add(match(Token_Class.Idenifier));
                declist.Children.Add(Assignmentlist());
            }
            else
            {
                declist.Children.Add(AssignmentStatment());
                declist.Children.Add(Assignmentlist());
            }
            return declist;
        }

        // Assignmentlist -> , AssList | ε
        Node Assignmentlist()
        {
            Node assignmentlist = new Node("Assignment list");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                assignmentlist.Children.Add(match(Token_Class.Comma));
                assignmentlist.Children.Add(AssList());
            }
            return assignmentlist;
        }

        //AssList -> AssignmentStatement Assignmentlist | Identifier Assignmentlist  --> not done yet
        Node AssList()
        {
            Node assList = new Node("Ass List");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                assList.Children.Add(match(Token_Class.Idenifier));
                assList.Children.Add(Assignmentlist());
            }
            else
            {
                assList.Children.Add(AssignmentStatment());
                assList.Children.Add(Assignmentlist());
            }
            return assList;
        }

        //Datatype -> int | float | string
        Node Datatype()
        {
            Node datatype = new Node("Data type");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Int)
            {
                datatype.Children.Add(match(Token_Class.Int));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                datatype.Children.Add(match(Token_Class.Float));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                datatype.Children.Add(match(Token_Class.String));
            }
            else
            {
                //Error List
            }
            return datatype;
        }

        // 7)Write Statement -> write WriteStmt;        
        Node WriteStatement()
        {
            Node writeStatement = new Node("Write Statement");
            writeStatement.Children.Add(match(Token_Class.Write));
            writeStatement.Children.Add(WriteStmt());
            return writeStatement;
        }

        //  WriteStmt -> Expression | endl          --> not done yet
        Node WriteStmt()
        {
            Node writeStmt = new Node("Write Stmt");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.endl)
            {
                writeStmt.Children.Add(match(Token_Class.endl));
            }
            else
            {
                writeStmt.Children.Add(Expression());
            }

            return writeStmt;
        }

        // 8)ReadStatement  read identifier ;
        Node ReadStatement()
        {
            Node readStatement = new Node("Read Statement");
            readStatement.Children.Add(match(Token_Class.Read));
            readStatement.Children.Add(match(Token_Class.Idenifier));
            return readStatement;
        }

        //9)ReturnStatement  return Expression;
        Node ReturnStatement()
        {
            Node returnStatement = new Node("Return Statement");
            returnStatement.Children.Add(match(Token_Class.Return));
            returnStatement.Children.Add(Expression());
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
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.EqualOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.NotEqualOp));
            }
            else
            {
                // ErrorList
            }
            return conditionalOperator;

        }
        //                               check                                         //

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
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Or || TokenStream[InputPointer].token_type == Token_Class.And))
            {
                booleanStatments.Children.Add(BooleanOperator());
                booleanStatments.Children.Add(Condition());
                booleanStatments.Children.Add(BooleanStatments());
            }
            return booleanStatments;
        }
        //BooleanOperator -> && | ||
        Node BooleanOperator()
        {
            Node booleanOperator = new Node("Boolean Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Or)
            {
                booleanOperator.Children.Add(match(Token_Class.Or));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.And)
            {
                booleanOperator.Children.Add(match(Token_Class.And));
            }
            else
            {
                // ErrorList
            }
            return booleanOperator;
        }

        // 12)Statement -> Write Statement | Read Statement | Assignment Statement | Declaration Statement | Repeat Statement

        //Statements -> Statement Statements | ε

        // 13)IFStatement -> if ConditionalStatement then Statements IFStmtList

        //IFStmtList -> ElseIfStatement ElseIfStmtList | ElseStatement | end

        //ElseIfStmtList -> ElseIfStatement ElseIfStmtList| ε

        //14)ElseIfStatement -> elseif ConditionStatement then Statements ElseIfFollowingList

        //ElseIfFollowingList -> ElseStatment | end

        //15)ElseStatement -> else Statements end

        //Repeat Statement -> repeat Statements until ConditionalStatement

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

        //Paramlist -> Parameter Parameters | ε

        //Parameters -> , Parameter parameters | ε

        //20)FunctionBody -> { Statements ReturnStatement }

        //21)FunctionStatment -> FunctionDeclaration FunctionBody

        //22)MainFunction -> Datatype main () FunctionBody

        //23)Programs -> funcstat MainFunction

        //Funcstat   FunctionStatment funcstat | ε 

       
 //================================================================================
         // Implement your logic here
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
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
