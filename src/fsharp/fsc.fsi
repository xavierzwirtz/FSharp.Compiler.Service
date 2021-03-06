// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Driver 

open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Build
open Microsoft.FSharp.Compiler.Env

//----------------------------------------------------------------------------
// Public helpers - entry point for fsc.exe

type SigningInfo = SigningInfo of (* delaysign:*) bool * (*signer:*)  string option * (*container:*) string option

[<AbstractClass>]
type ErrorLoggerProvider =
    new : unit -> ErrorLoggerProvider
    abstract CreateErrorLoggerThatQuitsAfterMaxErrors : tcConfigBuilder : TcConfigBuilder * exiter : Exiter -> ErrorLogger

#if NO_COMPILER_BACKEND
#else
/// <summary>fsc.exe calls this</summary>
/// A global variable representing a parameter used to configure the compilation service.
val mainCompile : 
    argv: string[] * 
    bannerAlreadyPrinted: bool * 
    openBinariesInMemory: bool * 
    exiter: Exiter * 
    loggerProvider: ErrorLoggerProvider * 
    tcImportsCapture: (TcImports -> unit) option *
    dynamicAssemblyCreator: (TcConfig * ILGlobals * ErrorLogger * string * string option * ILModuleDef * SigningInfo -> unit) option
      -> unit

val compileOfAst : 
    openBinariesInMemory: bool * 
    assemblyName:string * 
    target:CompilerTarget * 
    targetDll:string * 
    targetPdb:string option * 
    dependencies:string list * 
    noframework:bool *
    exiter:Exiter * 
    loggerProvider: ErrorLoggerProvider * 
    inputs:ParsedInput list *
    tcImportsCapture : (TcImports -> unit) option *
    dynamicAssemblyCreator: (TcConfig * ILGlobals * ErrorLogger * string * string option * ILModuleDef * SigningInfo -> unit) option
      -> unit

val EncodeInterfaceData: tcConfig:TcConfig * tcGlobals:TcGlobals * exportRemapping:Tastops.Remap * generatedCcu: Tast.CcuThunk * outfile: string -> ILAttribute list * ILResource list
val ValidateKeySigningAttributes : tcConfig:TcConfig -> tcGlobals:TcGlobals -> TypeChecker.TopAttribs -> SigningInfo
val GetSigner : SigningInfo -> ILBinaryWriter.ILStrongNameSigner option

//----------------------------------------------------------------------------
// Internal helpers used to implement the SimpleCodeServices API

type ILResource with 
    /// Read the bytes from a resource local to an assembly
    member internal Bytes : byte[]


//----------------------------------------------------------------------------
/// Internal helpers used to implement the Visual Studio hosted CodeServices API.
///
/// The F# project system calls this to pop up type provider security dialog if needed.
val internal runFromCommandLineToImportingAssemblies : (string -> unit) * string[] * string * string * Exiter -> unit

/// Proccess the given set of command line arguments
val internal ProcessCommandLineFlags : TcConfigBuilder * argv:string[] -> string list

#endif
