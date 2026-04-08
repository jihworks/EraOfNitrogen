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
    /// <remarks>
    /// Marked property must have setter.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonSaveAttribute : Attribute
    {
    }

    public class JsonSaveContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<MemberInfo> members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Cast<MemberInfo>()
                .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .Where(m => m.GetCustomAttribute<JsonSaveAttribute>() is not null)
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

    /// <remarks>
    /// Using custom resolver that <see cref="JsonSaveContractResolver"/>.<br/>
    /// Supports only opt-in by <see cref="JsonSaveAttribute"/>.<br/>
    /// If there is only <see cref="JsonPropertyAttribute"/>, it will be <b>excluded</b>.
    /// </remarks>
    public static class JsonSave
    {
        static readonly JsonSerializerSettings _settings = new()
        {
            ContractResolver = new JsonSaveContractResolver(),

            // This option allows private constructors.
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,

            // This option will replace collection fields.
            ObjectCreationHandling = ObjectCreationHandling.Replace,

            PreserveReferencesHandling = PreserveReferencesHandling.Objects,

            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,

            Formatting = Formatting.Indented,
        };

        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }

        public static T? DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void PopulateObject(string json, object target)
        {
            JsonConvert.PopulateObject(json, target, _settings);
        }
    }
}

#endif
