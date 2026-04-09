// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

#if USE_NEWTONSOFT_JSON

using Newtonsoft.Json;

namespace Jih.Unity.Infrastructure.Json
{
    /// <remarks>
    /// Using custom resolver that <see cref="JsonSaveContractResolver"/>.<br/>
    /// Supports only opt-in by <see cref="JsonSaveMemberAttribute"/>.<br/>
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
