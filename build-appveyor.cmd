ECHO APPVEYOR_REPO_BRANCH: %APPVEYOR_REPO_BRANCH%
ECHO APPVEYOR_REPO_TAG: %APPVEYOR_REPO_TAG%
ECHO APPVEYOR_BUILD_NUMBER : %APPVEYOR_BUILD_NUMBER%
ECHO APPVEYOR_BUILD_VERSION : %APPVEYOR_BUILD_VERSION%

CALL build\Tools\NuGet.exe restore development\DesignAgency.BoboFacets.sln
CALL "%programfiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\amd64\MsBuild.exe" build\package.build.xml
pause