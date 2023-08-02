BasePath="./Assets/Art"
ThirdPartyPath="./Assets/3rdParty/*"
IncorrectFiles=""

function CollectFilesThatAreNotInFolder()
{
    local anyIncorrect=false
    local fileExtension="*.$1"
    local folderPath=$2"/*"
    local folderPathAlt=$3"/*"
    local errorMessage=""
    
    if [[ $3 != "" ]]
    then
        errorMessage=" is not in << $2 >> or << $3 >> or any sub folder"
    else
        errorMessage=" is not in << $2 >> or any sub folder"
    fi

    find "./" -type f -name "$fileExtension" -print0 | 
    while IFS= read -r -d $'\0' file 
    do
        if [[ $file != $folderPath ]] && [[ $file != $ThirdPartyPath ]] && [[ $file != $folderPathAlt ]]
        then
            if [[ $anyIncorrect == false ]]
            then
                anyIncorrect=true
                echo $'\n\n'"Please consider changes on those $1 files:"
            fi
            file=${file:2:${#file}}
            echo "$file"$errorMessage
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

sed -i '2,$d' BodyFile.txt

if [[ $IncorrectFiles == "" ]]
then
    echo $'Success!\nAll files are in the correct folders!' >> BodyFile.txt
else
    echo $'Failed :c \n'"$IncorrectFiles" >> BodyFile.txt
    exit 1
fi
