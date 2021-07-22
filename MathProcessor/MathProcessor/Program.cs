using System;

namespace MathProcessor
{
    /*
     * Вычисление арифметических операций над числами
     */
    namespace V1
    {
        public enum TokenName { BAD, EXPRESSION, UNEXPRESSION, NUMBER, ADD, SUB, MLT, DIV, POW }

        public static class Token
        {
            static char[] TokenChar = new char[] { '~', '(', ')', 'n', '+', '-', '*', '/', '^' };

            public static char GetChar(TokenName name)
            {
                return TokenChar[(Int32)name];
            }

            public static TokenName GetName(char ch)
            {
                TokenName t = TokenName.BAD;
                for (Int32 i = 1; i < TokenChar.Length; ++i)
                    if (ch == TokenChar[i])
                    {
                        t = (TokenName)i;
                        break;
                    }
                return t;
            }
        }

        public static class MathProcessor
        {
            static TokenName token;
            static double number;

            static Int32 index;
            static char[] buffer;
            public static TokenName Scan(char[] exp)
            {
                if (exp != null)
                {
                    buffer = exp;
                    index = 0;
                }
                else if (index == buffer.Length) return TokenName.BAD;

                while (char.IsWhiteSpace(buffer[index]))
                    index++;
                if (char.IsDigit(buffer[index]) || buffer[index] == '.')
                {
                    token = TokenName.NUMBER;
                    number = NumberScaner.ScanDouble(buffer, index, out index);
                    return token;
                }

                return token = Token.GetName(buffer[index++]);
            }

            static IOperation Factor()
            {
                IOperation result = null;
                switch (token)
                {
                    default:
                        Console.WriteLine("bad factor {0}", token);
                        Environment.Exit(1);
                        break;

                    case TokenName.ADD:
                        Scan(null);
                        return Factor();

                    case TokenName.SUB:
                        Scan(null);
                        return new UnOpNeg(Factor());

                    case TokenName.NUMBER:
                        result = new ValOp(number);
                        break;

                    case TokenName.EXPRESSION:
                        Scan(null);
                        result = Sum();
                        if (token != TokenName.UNEXPRESSION)
                        {
                            Console.WriteLine("expecting )");
                            Environment.Exit(1);
                        }
                        break;
                }
                Scan(null);
                return result;
            }

            static IOperation Product()
            {
                IOperation result = Factor();
                for (; ; )
                {
                    switch (token)
                    {
                        default: return result;

                        case TokenName.MLT:
                            Scan(null);
                            result = new BinOpMlt(result, Factor());
                            break;

                        case TokenName.DIV:
                            Scan(null);
                            result = new BinOpDiv(result, Factor());
                            break;

                        case TokenName.POW:
                            Scan(null);
                            result = new BinOpPow(result, Factor());
                            break;
                    }
                }
            }

            public static IOperation Sum()
            {
                IOperation result = Product();
                for (; ; )
                {
                    switch (token)
                    {
                        default: return result;

                        case TokenName.ADD:
                            Scan(null);
                            result = new BinOpAdd(result, Product());
                            break;

                        case TokenName.SUB:
                            Scan(null);
                            result = new BinOpSub(result, Product());
                            break;
                    }
                }
            }

            public static double Process(string expression)
            {
                Scan(expression.ToCharArray());
                return (Sum()).Calculate();
            }
        }

        public static class NumberScaner
        {
            public static Int32 PowerOfTen(Int32 power)
            {
                Int32 number = 1;
                for (Int32 i = 0; i < power; ++i)
                    number *= 10;
                return number;
            }

            public static Int32 ScanInt(char[] s, Int32 startIndex, out Int32 endIndex)
            {
                endIndex = startIndex;

                Int32 i = startIndex;
                while (char.IsDigit(s[i]))
                    i++;
                endIndex = i;

                Int32 number = 0;
                for (Int32 j = 0; j < endIndex - startIndex; ++j)
                {
                    number = (Int32)s[i - j - 1] * PowerOfTen(j);
                }
                return number;
            }

            public static double ScanDouble(char[] s, Int32 startIndex, out Int32 endIndex)
            {
                endIndex = startIndex;

                Int32 i = startIndex;
                while (i < s.Length && char.IsDigit(s[i]))
                    i++;
                endIndex = i;

                double number = 0;

                if (i == s.Length || s[i] != '.')
                    for (Int32 j = 0; j < endIndex - startIndex; ++j)
                    {
                        number += ((Int32)s[i - j - 1] - 48) * PowerOfTen(j);
                    }

                if (i < s.Length && s[i] == '.')
                {
                    startIndex = ++i;
                    while (i < s.Length && char.IsDigit(s[i]))
                        i++;
                    endIndex = i;

                    for (Int32 j = 0; j < endIndex - startIndex; ++j)
                    {
                        number += ((Int32)s[startIndex + j] - 48) * 1.0 / PowerOfTen(j + 1);
                    }
                    return number;
                }
                else return number;
            }
        }
    }

    /*
     * Изменена структура:
     * 1) Processor. 
     *      Отвечает за интерпритацию выражения. 
     *      Строит дерево выражения.  
     *      
     * 2) Processor.Scaner 
     *      Отвечат за разбор строки выражения. 
     *      
     * 3) Вар
     * 
     * Добавлено вычисление функций одного переменного
     */
    namespace V2
    {
        using MathProcessor.V2.Collections;

        public static class Processor
        {
            static class Scaner
            {
                public enum TokenName
                {
                    BAD,
                    EXPRESSION, UNEXPRESSION,
                    FUNCTION, NUMBER,
                    ADD, SUB, MLT, DIV, POW,
                }

                public static class Token
                {
                    static char[] TokenChar = new char[] { '~', '(', ')', 'f', 'n', '+', '-', '*', '/', '^' };

                    public static char GetChar(TokenName name)
                    {
                        return TokenChar[(Int32)name];
                    }

                    public static TokenName GetName(char ch)
                    {
                        for (Int32 i = 1; i < TokenChar.Length; ++i)
                            if (ch == TokenChar[i])
                                return (TokenName)i;

                        return TokenName.BAD;
                    }
                }

                public static TokenName token;
                public static double number;
                public static string symbol;

                public static Int32 index;
                public static char[] buffer;

                public static TokenName Scan(char[] exp)
                {
                    // "buffer" initialization
                    if (exp != null)
                    {
                        buffer = exp;
                        index = 0;
                    }

                    // It skips spaces
                    while (index < buffer.Length && char.IsWhiteSpace(buffer[index]))
                        index++;
                    if (index == buffer.Length) return TokenName.BAD;

                    // Number
                    if (char.IsDigit(buffer[index]) || buffer[index] == '.')
                    {
                        token = TokenName.NUMBER;
                        number = ScanDouble(buffer, index, out index);
                        return token;
                    }

                    // Symbol
                    if (char.IsLetter(buffer[index]) || buffer[index] == '_')
                    {
                        token = TokenName.FUNCTION;
                        symbol = ScanSymbol(buffer, index, out index);
                        return token;
                    }

                    // Arithmetic operations and brackets
                    return token = Token.GetName(buffer[index++]);
                }

                static Int32 PowerOfTen(Int32 power)
                {
                    Int32 number = 1;
                    for (Int32 i = 0; i < power; ++i)
                        number *= 10;
                    return number;
                }

                static Int32 ScanInt(char[] s, Int32 startIndex, out Int32 endIndex)
                {
                    endIndex = startIndex;

                    Int32 i = startIndex;
                    while (char.IsDigit(s[i]))
                        i++;
                    endIndex = i;

                    Int32 number = 0;
                    for (Int32 j = 0; j < endIndex - startIndex; ++j)
                    {
                        number = (Int32)s[i - j - 1] * PowerOfTen(j);
                    }
                    return number;
                }

                static double ScanDouble(char[] s, Int32 startIndex, out Int32 endIndex)
                {
                    endIndex = startIndex;

                    Int32 i = startIndex;
                    while (i < s.Length && char.IsDigit(s[i]))
                        i++;
                    endIndex = i;

                    double number = 0;

                    if (i == s.Length || s[i] != '.')
                        for (Int32 j = 0; j < endIndex - startIndex; ++j)
                        {
                            number += ((Int32)s[i - j - 1] - 48) * PowerOfTen(j);
                        }

                    if (i < s.Length && s[i] == '.')
                    {
                        startIndex = ++i;
                        while (i < s.Length && char.IsDigit(s[i]))
                            i++;
                        endIndex = i;

                        for (Int32 j = 0; j < endIndex - startIndex; ++j)
                        {
                            number += ((Int32)s[startIndex + j] - 48) * 1.0 / PowerOfTen(j + 1);
                        }
                        return number;
                    }
                    else return number;
                }

                static string ScanSymbol(char[] s, Int32 startINdex, out Int32 endIndex)
                {
                    endIndex = startINdex;

                    Int32 i = startINdex;
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_'))
                        i++;
                    endIndex = i;

                    return new String(s, startINdex, endIndex - startINdex);
                }
            }

            static IOperation Factor()
            {
                switch (Scaner.token)
                {
                    default:
                        Console.WriteLine();
                        throw new FormatException($"bad factor { Scaner.token }");

                    case Scaner.TokenName.FUNCTION:
                        Scaner.Scan(null);
                        return new UnOp(UnSymbolTable.Find(Scaner.symbol), Factor());

                    case Scaner.TokenName.ADD:
                        Scaner.Scan(null);
                        return Factor();

                    case Scaner.TokenName.SUB:
                        Scaner.Scan(null);
                        return new UnOp(UnOpEssence.Neg, Factor());

                    case Scaner.TokenName.NUMBER:
                        Scaner.Scan(null);
                        return new ValOp(Scaner.number);

                    case Scaner.TokenName.EXPRESSION:
                        Scaner.Scan(null);
                        IOperation result = Sum();

                        if (Scaner.token != Scaner.TokenName.UNEXPRESSION)
                            throw new FormatException($"Expecting ')'");

                        Scaner.Scan(null);
                        return result;
                }
            }

            static IOperation Product()
            {
                IOperation result = Factor();
                BinOpEssence.OpEssence opEssence;
                for (; ; )
                {
                    switch (Scaner.token)
                    {
                        default: return result;

                        case Scaner.TokenName.MLT:
                            opEssence = BinOpEssence.Mlt;
                            break;

                        case Scaner.TokenName.DIV:
                            opEssence = BinOpEssence.Div;
                            break;

                        case Scaner.TokenName.POW:
                            opEssence = BinOpEssence.Pow;
                            break;
                    }
                    Scaner.Scan(null);
                    result = new BinOp(opEssence, result, Factor());
                }
            }

            public static IOperation Sum()
            {
                IOperation result = Product();
                BinOpEssence.OpEssence opEssence;
                for (; ; )
                {
                    switch (Scaner.token)
                    {
                        default: return result;

                        case Scaner.TokenName.ADD:
                            opEssence = BinOpEssence.Add;
                            break;

                        case Scaner.TokenName.SUB:
                            opEssence = BinOpEssence.Sub;
                            break;
                    }
                    Scaner.Scan(null);
                    result = new BinOp(opEssence, result, Product());
                }
            }

            public static double Process(string expression)
            {
                Scaner.Scan(expression.ToCharArray());
                return Sum().Calculate();
            }
        }
    }

    /*
     *  Добавлено: определение переменных и пользовательских функций
     *  Добавлено: вычисление выражений содержащих переменные и пользовательские функции
     *  Добавлено: тип Processor.Validator
     *      Отвечает за валидацию выражения декларации
     * 
     *  расширено: 
     *      Тип. Processor.Scaner.TokenName:
     *      1) добавлен терм LET
     *      
     *      Тип. Processor.Scaner.Token
     *      1)
 
     *      Тип. Processor
     *      1) Processor.Factor
     */
    namespace V3
    {
        using MathProcessor.V3.Collections;

        public static class Processor
        {
            static class Scaner
            {
                public enum TokenName
                {
                    BAD, LET, ENDLET,
                    EXPRESSION, UNEXPRESSION,
                    FUNCTION, NUMBER, DECLARATION,
                    ADD, SUB, MLT, DIV, POW,
                }

                public static class Token
                {
                    static char[] TokenChar = new char[] { '~', 'l', 'e', '(', ')', 'f', 'n', 'd', '+', '-', '*', '/', '^' };

                    public static char GetChar(TokenName name)
                    {
                        return TokenChar[(Int32)name];
                    }

                    public static TokenName GetName(char ch)
                    {
                        for (Int32 i = 1; i < TokenChar.Length; ++i)
                            if (ch == TokenChar[i])
                                return (TokenName)i;

                        return TokenName.BAD;
                    }
                }

                public static TokenName token;
                public static double number;
                public static string symbol;
                public static Tuple<Int32, string[]> signature;

                public static Int32 index;
                public static char[] buffer;

                public static TokenName Scan(char[] exp)
                {
                    // "buffer" initialization
                    if (exp != null)
                    {
                        buffer = exp;
                        index = 0;
                    }

                    // It skips spaces
                    while (index < buffer.Length && char.IsWhiteSpace(buffer[index]))
                        index++;
                    if (index == buffer.Length) return TokenName.BAD;

                    // Number
                    if (char.IsDigit(buffer[index]) || buffer[index] == '.')
                    {
                        token = TokenName.NUMBER;
                        number = ScanDouble(buffer, index, out index);
                        return token;
                    }

                    // Declaration
                    if (char.IsUpper(buffer[index]))
                    {
                        symbol = ScanSymbol(buffer, index, out index);

                        if (string.Equals(symbol, "LET"))
                            token = TokenName.LET;

                        else
                        if (string.Equals(symbol, "FUNC"))
                        {

                        }
                        else
                            token = TokenName.DECLARATION;
                        return token;
                    }

                    if (buffer[index] == ';')
                    {
                        index++;
                        token = TokenName.ENDLET;
                        return token;
                    }

                    // Symbol
                    if (char.IsLetter(buffer[index]) || buffer[index] == '_')
                    {
                        token = TokenName.FUNCTION;
                        symbol = ScanSymbol(buffer, index, out index);
                        return token;
                    }

                    // Arithmetic operations and brackets
                    return token = Token.GetName(buffer[index++]);
                }

                static Int32 PowerOfTen(Int32 power)
                {
                    Int32 number = 1;
                    for (Int32 i = 0; i < power; ++i)
                        number *= 10;
                    return number;
                }

                static Int32 ScanInt(char[] s, Int32 startIndex, out Int32 endIndex)
                {
                    endIndex = startIndex;

                    Int32 i = startIndex;
                    while (char.IsDigit(s[i]))
                        i++;
                    endIndex = i;

                    Int32 number = 0;
                    for (Int32 j = 0; j < endIndex - startIndex; ++j)
                    {
                        number = (Int32)s[i - j - 1] * PowerOfTen(j);
                    }
                    return number;
                }

                static double ScanDouble(char[] s, Int32 startIndex, out Int32 endIndex)
                {
                    endIndex = startIndex;

                    Int32 i = startIndex;
                    while (i < s.Length && char.IsDigit(s[i]))
                        i++;
                    endIndex = i;

                    double number = 0;

                    if (i == s.Length || s[i] != '.')
                        for (Int32 j = 0; j < endIndex - startIndex; ++j)
                        {
                            number += ((Int32)s[i - j - 1] - 48) * PowerOfTen(j);
                        }

                    if (i < s.Length && s[i] == '.')
                    {
                        startIndex = ++i;
                        while (i < s.Length && char.IsDigit(s[i]))
                            i++;
                        endIndex = i;

                        for (Int32 j = 0; j < endIndex - startIndex; ++j)
                        {
                            number += ((Int32)s[startIndex + j] - 48) * 1.0 / PowerOfTen(j + 1);
                        }
                        return number;
                    }
                    else return number;
                }

                static string ScanSymbol(char[] s, Int32 startIndex, out Int32 endIndex)
                {
                    endIndex = startIndex;

                    Int32 i = startIndex;
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_'))
                        i++;
                    endIndex = i;

                    return new String(s, startIndex, endIndex - startIndex);
                }

                static Tuple<Int32, string[]> ScanSignature(char[] s, Int32 startINdex, out Int32 endIndex)
                {
                    endIndex = startINdex;

                    Int32 i = startINdex;
                    if (s[i++] == '(')
                    {
                        return null;
                    }
                    else return null;
                }
            }

            static class Validator
            {
                public static bool IsLetStatementNameValid(string name)
                {
                    return true;
                }
            }

            static void Let()
            {
                Scaner.Scan(null);

                if (Validator.IsLetStatementNameValid(Scaner.symbol))
                {
                    string name = Scaner.symbol;
                    IOperation declaration;

                    // read IS keyword
                    Scaner.Scan(null);

                    Scaner.Scan(null);
                    switch (Scaner.token)
                    {
                        default:
                            break;

                        case Scaner.TokenName.NUMBER:
                            DeclatationTable.table.Add(
                                new Tuple<string, IOperation>(name, new ValOp(Scaner.number)));
                            break;

                        case Scaner.TokenName.FUNCTION:
                            declaration = Sum();
                            DeclatationTable.table.Add(
                                new Tuple<string, IOperation>(name, declaration));
                            break;
                    }
                    Scaner.Scan(null);
                    if (Scaner.token != Scaner.TokenName.ENDLET)
                        throw new FormatException("Exprected ';' in the end of LET statement");
                }
                else throw new FormatException("Bad LET statement name");
            }

            static IOperation Factor()
            {
                switch (Scaner.token)
                {
                    default:
                        Console.WriteLine();
                        throw new FormatException($"bad factor { Scaner.token }");

                    case Scaner.TokenName.LET:
                        Let();
                        Scaner.Scan(null);
                        return Factor();

                    case Scaner.TokenName.EXPRESSION:
                        Scaner.Scan(null);
                        IOperation result = Sum();

                        if (Scaner.token != Scaner.TokenName.UNEXPRESSION)
                            throw new FormatException($"Expecting ')'");

                        Scaner.Scan(null);
                        return result;

                    case Scaner.TokenName.FUNCTION:
                        string fname = Scaner.symbol;
                        Scaner.Scan(null);
                        return new UnOp(UnSymbolTable.Find(fname), Factor());

                    case Scaner.TokenName.NUMBER:
                        Scaner.Scan(null);
                        return new ValOp(Scaner.number);

                    case Scaner.TokenName.DECLARATION:
                        Scaner.Scan(null);
                        return DeclatationTable.Find(Scaner.symbol);

                    case Scaner.TokenName.ADD:
                        Scaner.Scan(null);
                        return Factor();

                    case Scaner.TokenName.SUB:
                        Scaner.Scan(null);
                        return new UnOp(UnOpEssence.Neg, Factor());
                }
            }

            static IOperation Product()
            {
                IOperation result = Factor();
                BinOpEssence.OpEssence opEssence;
                for (; ; )
                {
                    switch (Scaner.token)
                    {
                        default: return result;

                        case Scaner.TokenName.MLT:
                            opEssence = BinOpEssence.Mlt;
                            break;

                        case Scaner.TokenName.DIV:
                            opEssence = BinOpEssence.Div;
                            break;

                        case Scaner.TokenName.POW:
                            opEssence = BinOpEssence.Pow;
                            break;
                    }
                    Scaner.Scan(null);
                    result = new BinOp(opEssence, result, Factor());
                }
            }

            public static IOperation Sum()
            {
                IOperation result = Product();
                BinOpEssence.OpEssence opEssence;
                for (; ; )
                {
                    switch (Scaner.token)
                    {
                        default: return result;

                        case Scaner.TokenName.ADD:
                            opEssence = BinOpEssence.Add;
                            break;

                        case Scaner.TokenName.SUB:
                            opEssence = BinOpEssence.Sub;
                            break;
                    }
                    Scaner.Scan(null);
                    result = new BinOp(opEssence, result, Product());
                }
            }

            public static double Process(string expression)
            {
                Scaner.Scan(expression.ToCharArray());
                return Sum().Calculate();
            }
        }
    }

    /* Поиск в глубину и в ширину*/
    namespace Search 
    {
        
    }
    public static class Program
    {
        static Program()
        {
            Console.WriteLine();
        }

        static void Test__MathProcessorV1()
        {
            string s = "(2 + 4) * (3 - 1) * 2";

            double result1 = V1.MathProcessor.Process(s);

            Console.WriteLine(s);
            Console.WriteLine("Result (version 1) = " + result1.ToString());
            Console.ReadKey();
        }

        static void Test__MathProcessorV2()
        {
            string s = "(2 + 4) * (3 - 1)";

            double result2 = V2.Processor.Process(s);

            Console.WriteLine(s);
            Console.WriteLine("Result (versoin 2) = " + result2.ToString());
            Console.ReadKey();
        }

        static void Test__MathProcessorV2__ItselfTypes__Operations()
        {
            V2.IOperation AddTenAndFive =
                new V2.BinOp(V2.BinOpEssence.Add, new V2.ValOp(10), new V2.ValOp(5));

            Console.WriteLine("10 + 5");
            Console.WriteLine((AddTenAndFive.Calculate()).ToString());
            Console.ReadKey();
        }

        static void Test__MathProcessorV2__Features__Function()
        {
            string s = "sqrt(sqr 3 + sqr 4)";

            double result2 = V2.Processor.Process(s);

            Console.WriteLine("Test. MathProcessor (version 2)'s fetures: functions.\n");
            Console.WriteLine("Expression: " + s);
            Console.WriteLine("Result      = " + result2.ToString());
            Console.ReadKey();
        }

        static void Test__MathPRocessorV3__Feutures__Declaration()
        {
            string s =
                "LET A = 5;\n" +
                "LET B = 1;\n" +
                "LET D = 9;\n" +
                "A + D + B";

            double result = 0;
            try
            {
                result = V3.Processor.Process(s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Test. MathProcessor (version 3)'s fetures: Declaration.\n");
            Console.WriteLine("Expression:\n" + s);
            Console.WriteLine();
            Console.WriteLine("Result\n\t= " + result.ToString());
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            Test__MathPRocessorV3__Feutures__Declaration();
        }
    }
}
