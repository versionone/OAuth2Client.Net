module AssemblyInfo


open System.Reflection

[<assembly: AssemblyTitle("VersionOne Structured Query implementation")>]

#if DEBUG
[<assembly: AssemblyDescription("Debug")>]
[<assembly: AssemblyConfiguration("Debug")>]
#else
[<assembly: AssemblyDescription("Release")>]
[<assembly: AssemblyConfiguration("Release")>]
#endif

[<assembly: AssemblyCompany("VersionOne, Inc.")>]
[<assembly: AssemblyProduct("VersionOne")>]
[<assembly: AssemblyCopyright("Copyright 2013, VersionOne, Inc. Please see the LICENSE.MD file.")>]
[<assembly: AssemblyVersion("1.0.0.0")>]
[<assembly: AssemblyInformationalVersion("1.0.0.0")>]


ignore ()

