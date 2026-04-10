// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

#if USE_NEWTONSOFT_JSON

using Newtonsoft.Json.Serialization;
using System;

namespace Jih.Unity.Infrastructure.Json
{
    class JsonSaveSerializationBinder : ISerializationBinder
    {
        public string RuntimeRootNamespace { get; }

        public JsonSaveSerializationBinder(string runtimeRootNamespace)
        {
            RuntimeRootNamespace = runtimeRootNamespace;
        }

        public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            assemblyName = null;
            string fullName = serializedType.FullName ?? throw new InvalidOperationException($"Type '{serializedType}' has not fullname.");
            if (!fullName.StartsWith(RuntimeRootNamespace, StringComparison.Ordinal))
            {
                throw new JsonSaveException($"JSON save type identifier with namespace must starts with runtime-root-namespace '{RuntimeRootNamespace}'. Type: {fullName}", serializedType);
            }
            typeName = fullName[RuntimeRootNamespace.Length..];
        }

        public Type BindToType(string? assemblyName, string typeName)
        {
            string fullName = RuntimeRootNamespace + typeName;
            Type? type = Type.GetType(typeName);
            if (type is null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(fullName);
                    if (type is not null)
                    {
                        return type;
                    }
                }
            }
            throw new JsonSaveException($"JSON save Type '{fullName}' not found.");
        }
    }
}

#endif
