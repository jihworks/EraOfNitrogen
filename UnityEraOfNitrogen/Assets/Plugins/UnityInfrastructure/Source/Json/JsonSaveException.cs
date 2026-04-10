// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

#if USE_NEWTONSOFT_JSON

using System;
using System.Reflection;

namespace Jih.Unity.Infrastructure.Json
{
    public class JsonSaveException : Exception
    {
        public Type? RelatedType { get; }
        public MemberInfo? RelatedMember { get; }

        public JsonSaveException(string message, Type? relatedType = null, MemberInfo? relatedMember = null) : base(message)
        {
            RelatedType = relatedType;
            RelatedMember = relatedMember;
        }
    }
}

#endif
