# Polyfills namespace

In-house declarations of C# compiler or analyzer interoperations.

## Null-state attributes

[Nullable static analysis](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis)  
[MemberNotNullAttribute Class](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.membernotnullattribute)  
[MemberNotNullWhenAttribute Class](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.membernotnullwhenattribute)

### Conditional Compile

Add `INFRASTRUCTURE_USE_NULL_STATES` as symbol.

## External Init

[The init keyword](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init)  
[IsExternalInit Class](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.isexternalinit)

### Conditional Compile

Add `INFRASTRUCTURE_USE_EXTERNAL_INIT` as symbol.

## Skip Locals Init

[SkipLocalsInitAttribute Class](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.skiplocalsinitattribute)

### Conditional Compile

Add `INFRASTRUCTURE_USE_SKIP_LOCALS_INIT` as symbol.

## Caller Argument Expression

[CallerArgumentExpressionAttribute Class](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerargumentexpressionattribute)

### Conditional Compile

Add `INFRASTRUCTURE_USE_CALLER_ARGUMENT_EXPRESSION` as symbol.
