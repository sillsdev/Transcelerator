call "c:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"

pushd .
MSbuild /target:Build /property:teamcity_build_checkoutDir=..\  /property:teamcity_dotnet_nunitlauncher_msbuild_task="notthere" /property:Platform=x86 /property:BUILD_NUMBER="*.*.6.789" /property:Minor="1"
MSbuild /target:installer /property:teamcity_build_checkoutDir=..\  /property:teamcity_dotnet_nunitlauncher_msbuild_task="notthere" /property:Platform=x86 /property:BUILD_NUMBER="*.*.6.789" /property:Minor="1"
popd
PAUSE

REM #/verbosity:detailed