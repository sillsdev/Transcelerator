pushd .
MSbuild /target:Build /property:teamcity_build_checkoutDir=..\  /property:teamcity_dotnet_nunitlauncher_msbuild_task="notthere" /property:BUILD_NUMBER="*.*.0.789" /property:Minor="1"
MSbuild /target:Installer;ConvertReleaseNotesToHtml /property:teamcity_build_checkoutDir=..\  /property:teamcity_dotnet_nunitlauncher_msbuild_task="notthere" /property:BUILD_NUMBER="*.*.0.789" /property:Minor="1"
popd
PAUSE

REM #/verbosity:detailed