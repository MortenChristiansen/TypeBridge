using System;

namespace MappingGenerator.Generator
{
    static class NamespaceFormatter
    {
        public static string Format(string @namespace, params string[] classDefinitionas) =>
$@"namespace {@namespace}
{{
    {string.Join($"{Environment.NewLine}{Environment.NewLine}    ", classDefinitionas)}
}}";
    }
}
