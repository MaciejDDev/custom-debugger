using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CDebugger
{
    public class VariableNameValidator
    {
        // Expresión regular para validar nombres de variables en C#
        private static readonly Regex ValidVariableRegex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        // Lista de palabras clave reservadas de C#
        private static readonly HashSet<string> CSharpKeywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
            "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override",
            "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
            "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "while"
        };
        
        public static bool IsValidVariableName(string name)
        {
            // Verificar si el nombre cumple con el formato de un identificador válido
            if (!ValidVariableRegex.IsMatch(name))
                return false;

            // Verificar si el nombre es una palabra clave reservada
            if (CSharpKeywords.Contains(name))
                return false;

            return true;
        }
    }
}