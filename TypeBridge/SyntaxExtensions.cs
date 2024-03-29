﻿using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace TypeBridge
{
    static class SyntaxExtensions
    {
        public static string GetNamespace(this ITypeSymbol type)
        {
            var ns = new List<string>();
            ISymbol symbol = type;
            while (symbol.ContainingNamespace != null)
            {
                ns.Insert(0, symbol.ContainingNamespace.Name);
                symbol = symbol.ContainingNamespace;
            }
            return string.Join(".", ns).Trim('.');
        }

        public static string GetQualifiedName(this ITypeSymbol type) =>
            $"{type.GetNamespace()}.{type.Name}";

        public static string Decapitalize(this string s) =>
            s.Length == 0 ? "" : s.Length == 1 ? s.ToLower() : s.Substring(0, 1).ToLower() + s.Substring(1);

        public static string GetUngenericizedQualifiedName(this ITypeSymbol type) =>
            type.ToDisplayString().Replace("<", "_").Replace(">", "_");

        public static string GetUngenericizedName(this ITypeSymbol type)
        {
            var name = type.ToDisplayString();
            if (!name.Contains("<"))
                return name;

            return name.Replace("<", "_").Replace(">", "_").Replace(".", "_");
        }
    }
}
