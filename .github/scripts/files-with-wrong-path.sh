BasePath="./Assets/Art"
ThirdPartyPath="./Assets/3rdParty/*"
PluginsFolderPath="./Assets/Plugins/*"
DocsFilesPath="./Assets/Scripts/Shared/Docs/*"
PackagesFilePath="./Packages/*"
IncorrectFiles=""

function CollectFilesThatAreNotInFolder()
{
    local anyIncorrect=false
    local fileExtension="*.$1"
    local path=$2
    local pathAlt=$3
    path=${path:2:${#path}}
    pathAlt=${pathAlt:2:${#pathAlt}}
    local folderPath=$2"/*"
    local folderPathAlt=$3"/*"
    local errorMessage=""
    if [[ $3 != "" ]]
    then
        errorMessage="These \`$1\` files are not in \`$path\` or  \`$pathAlt\` or any sub folder"
    else
        errorMessage="These \`$1\` files are not not in \`$path\` or any sub folder"
    fi

    find "./" -type f -name "$fileExtension" -print0 | 
    while IFS= read -r -d $'\0' file 
    do
        if [[ $file != $folderPath ]] && [[ $file != $ThirdPartyPath ]] && [[ $file != $folderPathAlt ]] && [[ $file != $PluginsFolderPath ]] && [[ $file != $DocsFilesPath ]] && [[ $file != PackagesFilePath ]]
        then
            if [[ $anyIncorrect == false ]]
            then
                anyIncorrect=true
                echo $'\n\n'"$errorMessage"
            fi
            file=${file:2:${#file}}
            echo "- $file"
        fi
    done
}

AnimationFiles="controller"
AnimationFilesPath="$BasePath/Animations"
IncorrectFiles+="$(CollectFilesThatAreNotInFolder $AnimationFiles $AnimationFilesPath)"

FBXFiles="fbx"
FBXFilesPath="$BasePath/FBXs"
IncorrectFiles+="$(CollectFilesThatAreNotInFolder $FBXFiles $FBXFilesPath)"

MatFiles="mat"
MatFilesPath="$BasePath/Materials"
IncorrectFiles+="$(CollectFilesThatAreNotInFolder $MatFiles $MatFilesPath)"

ShaderFiles="shadergraph"
ShaderFilesPath="$BasePath/Shaders"
IncorrectFiles+="$(CollectFilesThatAreNotInFolder $ShaderFiles $ShaderFilesPath)"

TextureFiles="png"
TextureFilesPath="$BasePath/Textures"
TextureFilesPathAlt="$BasePath/UI"
IncorrectFiles+="$(CollectFilesThatAreNotInFolder $TextureFiles $TextureFilesPath $TextureFilesPathAlt)"

sed -i '2,$d' FilesPathBodyFile.txt

if [[ $IncorrectFiles == "" ]]
then
    echo $'Success!\nAll files are in the correct folders!' >> FilesPathBodyFile.txt
else
    echo $'Failed :c \n'"$IncorrectFiles" >> FilesPathBodyFile.txt
fi
