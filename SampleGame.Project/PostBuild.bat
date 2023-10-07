@echo ON

echo Running PostBuild

REM Store the Project's path that contains this script and the Assets folder we want to create a symlink for
cd %2
set path=%cd%
echo %path%

REM Destination folder
cd %1
echo Current cd = %cd%

REM SETUP path to the assets folder
set assetsPath=%path%\Assets
echo %assetsPath%

if exist %assetsPath% (
    echo folder for Assets is already hooked up
) else (    
    echo mklink /d Assets %assetsPath%
    mklink /d Assets %assetsPath%
)