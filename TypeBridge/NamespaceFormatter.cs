using System;

namespace TypeBridge
{
    static class NamespaceFormatter
    {
        public static string Format(string @namespace, params string[] classDefinitionas) =>
$@"using System;
using System.Linq;
using System.Collections.Generic;

namespace {@namespace}
{{
    {string.Join($"{Environment.NewLine}{Environment.NewLine}    ", classDefinitionas)}
}}";
    }
}
