// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

#if USE_NEWTONSOFT_JSON

using System;

namespace Jih.Unity.Infrastructure.Json
{
    /// <remarks>
    /// Marked property must have setter.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonSaveMemberAttribute : Attribute
    {
    }
}

#endif
