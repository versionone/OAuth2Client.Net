#!/bin/bash -e
. ./build.properties
if [ -d build-tools ]; then cd build-tools && git fetch && git stash && git pull && cd ..; else git clone https://github.com/versionone/openAgile-build-tools.git build-tools; fi
source ./build-tools/common.sh

# ---- Produce .NET Metadata --------------------------------------------------

function create_assemblyinfo() {
  cat > $1 <<EOF
module AssemblyInfo

open System.Reflection

[<assembly: AssemblyTitle("$PRODUCT_NAME")>]
[<assembly: AssemblyDescription("$Configuration")>]
[<assembly: AssemblyConfiguration("$Configuration")>]
[<assembly: AssemblyCompany("$ORGANIZATION_NAME")>]
[<assembly: AssemblyProduct("VersionOne")>]
[<assembly: AssemblyCopyright("Copyright $COPYRIGHT_RANGE, VersionOne, Inc. Please see the LICENSE.MD file.")>]
[<assembly: AssemblyVersion("$VERSION_NUMBER.$REVISION_NUMBER.$BUILD_NUMBER")>]
[<assembly: AssemblyInformationalVersion("$VERSION_NUMBER.$REVISION_NUMBER.$BUILD_NUMBER")>]

ignore ()

EOF
}


# ---- Build solution using msbuild -------------------------------------------
create_assemblyinfo ./OAuth2Client/AssemblyInfo.fs

nuget_packages_refresh

WIN_SIGNING_KEY="`winpath "$SIGNING_KEY"`"

MSBuild.exe $SOLUTION_FILE \
  -p:SignAssembly=$SIGN_ASSEMBLY \
  -p:AssemblyOriginatorKeyFile=$WIN_SIGNING_KEY \
  -p:DownloadNuGetExe=true \
  -p:RequireRestoreConsent=false \
  -p:Configuration="$Configuration" \
  -p:Platform="$Platform" \
  -p:Verbosity=Diagnostic


# ./NuGet.exe install -OutputDirectory packages ./OAuth2Client/packages.config
#/c/windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe //p:DownloadNuGetExe=true  OAuth2Client.sln
