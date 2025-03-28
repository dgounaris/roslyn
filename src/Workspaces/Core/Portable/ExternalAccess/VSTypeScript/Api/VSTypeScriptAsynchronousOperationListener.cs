﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Shared.TestHooks;

namespace Microsoft.CodeAnalysis.ExternalAccess.VSTypeScript.Api;

internal readonly struct VSTypeScriptAsynchronousOperationListener(IAsynchronousOperationListener underlyingObject)
{
    public VSTypeScriptAsyncToken BeginAsyncOperation(string name, object? tag = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        => new(underlyingObject.BeginAsyncOperation(name, tag, filePath, lineNumber));
}
