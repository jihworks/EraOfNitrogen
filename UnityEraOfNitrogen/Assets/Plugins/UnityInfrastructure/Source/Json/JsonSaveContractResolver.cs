// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

#if USE_NEWTONSOFT_JSON

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jih.Unity.Infrastructure.Json
{
    public class JsonSaveContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<MemberInfo> members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Cast<MemberInfo>()
                .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .Where(m => m.GetCustomAttribute<JsonSaveMemberAttribute>() is not null)
                .ToList();

            foreach (var member in members)
            {
                if (member is not PropertyInfo prop)
                {
                    continue;
                }
                if (!prop.CanRead)
                {
                    throw new InvalidOperationException($"The JSON save property '{prop.Name}' in '{type.FullName}' missing getter.");
                }
                if (!prop.CanWrite)
                {
                    throw new InvalidOperationException($"The JSON save property '{prop.Name}' in '{type.FullName}' missing setter.");
                }
            }

            List<JsonProperty> result = new(members.Count);

            foreach (var member in members)
            {
                JsonProperty jsonProperty = CreateProperty(member, memberSerialization);
                jsonProperty.Writable = true;
                jsonProperty.Readable = true;

                result.Add(jsonProperty);
            }

            return result;
        }
    }
}

#endif
