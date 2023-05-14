using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

class Program
{
    static void Main()
    {
        string cCode = File.ReadAllText("input2.c");
        string csCode = ConvertCToCS(cCode);
        File.WriteAllText("output.cs", csCode);
    }

    static string ConvertCToCS(string cCode)
    {
        string csCode = cCode;

        csCode = csCode.Replace("#include <stdio.h>", "using System; \n class Program { ");
        csCode = csCode.Replace("return 0;", "}");

        csCode = ProcessInputOutput(csCode);
        csCode = ProcessFunctionDeclarations(csCode);

        return csCode;
    }


    public static string ReplaceFirstNOccurrences(string input, string oldValue, string newValue, int n)
    {
        int count = 0;
        int place = input.IndexOf(oldValue, StringComparison.Ordinal);

        while (place >= 0 && count < n)
        {
            input = input.Remove(place, oldValue.Length).Insert(place, newValue);
            place = input.IndexOf(oldValue, StringComparison.Ordinal);
            count++;
        }

        return input;
    }

    static string ProcessInputOutput(string csCode)
    {
        // Matches printf and scanf function calls
        var printfScanfPattern = @"(printf|scanf)\(([^);]*)\);\s*";
        var printfScanfMatches = Regex.Matches(csCode, printfScanfPattern);

        foreach (Match match in printfScanfMatches)
        {
            var functionName = match.Groups[1].Value;
            var parameters = match.Groups[2].Value.Split(',');

            if (functionName == "printf")
            {
                var formatString = parameters[0].Trim();
                // Replace C-style format specifiers with C#-style
                var formatMatches = Regex.Matches(formatString, @"%.\d*[l]?[d|f]");
                for (int i = 0; i < formatMatches.Count; i++)
                {
                    formatString = formatString.Replace(formatMatches[i].Value, $"{{{i}}}");
                }
                csCode = csCode.Replace(match.Value, $"Console.WriteLine({formatString}{((parameters.Length > 1) ? "," + string.Join(",", parameters.Skip(1)) : "")});");
            }
            else if (functionName == "scanf")
            {
                var formatSpecifierMatches = Regex.Matches(parameters[0], @"%.\d*[l]?[d|f]");
                var variableMatches = Regex.Matches(parameters[1], @"&(\w+)");

                for (int i = 0; i < formatSpecifierMatches.Count; i++)
                {
                    var formatSpecifier = formatSpecifierMatches[i].Value;
                    var variableName = variableMatches[i].Value.Substring(1); 

                    if (formatSpecifier == "%lf")
                    {
                        csCode = ReplaceFirstNOccurrences(csCode, match.Value, $"{variableName} = double.Parse(Console.ReadLine());", 1);
                    }
                    else if (formatSpecifier == "%d")
                    {
                        csCode = ReplaceFirstNOccurrences(csCode, match.Value, $"{variableName} = int.Parse(Console.ReadLine());", 1);
                    }
                }
            }
        }
        return csCode;
    }


    static string ProcessFunctionDeclarations(string csCode)
    {
        var functionDeclarationPattern = @"(double|int|void)\s+(\w+)\s*\(([^)]*)\)\s*\{";
        var functionDeclarations = Regex.Matches(csCode, functionDeclarationPattern);

        foreach (Match match in functionDeclarations)
        {
            var returnType = match.Groups[1].Value;
            var functionName = match.Groups[2].Value;
            var parameters = match.Groups[3].Value;

        
            if (functionName == "main")
            {
                functionName = "Main";
                returnType = "void";
            }

            var parameterList = parameters.Split(',')
                                         .Select(p => p.Trim())
                                         .Where(p => !string.IsNullOrWhiteSpace(p))
                                         .ToArray();

            for (var i = 0; i < parameterList.Length; i++)
            {
                var parts = parameterList[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    parameterList[i] = $"{parts[0]} {parts[1]}"; // keep type and name in correct order
                }
            }

            var newParameters = string.Join(", ", parameterList);
            csCode = csCode.Replace(match.Value, $"static {returnType} {functionName}({newParameters}){{");
        }

        return csCode;
    }

}