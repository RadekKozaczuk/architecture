REGEX='^((([P][H])?[A-Z][a-z]+[0-9]*)|([A][N][_]))(([H][D][R])?[_]?[0-9]*[A-Z][a-z]+[0-9]*([H][D][R])?)*$'
ThirdPartyPath="./Assets/3rdParty/*"
PluginsFolderPath="./Assets/Plugins/*"
IncorrectFiles=""

function CollectFilesThatAreNotPascalCase()
{
    local anyIncorrect=false
    local fileExtension="*.$1"
    local extensionLenght=${#1}
    local errorMessage=" is not named with PascalCase(_Variant)"

    find "./" -type f -name "$fileExtension" -print0 | 
    while IFS= read -r -d $'\0' file 
    do
        if [[ $file != $ThirdPartyPath ]] && [[ $file != $PluginsFolderPath ]]
        then
            fileName=$(basename "$file")
            fullLenght=${#fileName}
            fileName=${fileName:0:fullLenght-(extensionLenght+1)}

            if ! [[ $fileName =~ $REGEX ]]
            then
                if [[ $anyIncorrect == false ]]
                then
                    anyIncorrect=true
                    echo $'\n\n'"Please consider changes on those $1 files:"
                fi
                file=${file:2:${#file}}
                echo "$file""$errorMessage"
            fi
        fi
    done
}

AnimationFiles="controller"
IncorrectFiles+=$(CollectFilesThatAreNotPascalCase $AnimationFiles)

FBXFiles="fbx"
IncorrectFiles+=$(CollectFilesThatAreNotPascalCase $FBXFiles)

MatFiles="mat"
IncorrectFiles+=$(CollectFilesThatAreNotPascalCase $MatFiles)

ShaderFiles="shadergraph"
IncorrectFiles+=$(CollectFilesThatAreNotPascalCase $ShaderFiles)

TextureFiles="png"
IncorrectFiles+=$(CollectFilesThatAreNotPascalCase $TextureFiles)

MusicFiles="mp3"
IncorrectFiles+=$(CollectFilesThatAreNotPascalCase $MusicFiles)

#some c# files have special names like UIReferenceHolder.cs that don't match regex
#CsharpFiles="cs"
#IncorrectFiles+=$(CollectFilesThatAreNotPascalCase $CsharpFiles)

sed -i '2,$d' BodyFile.txt

if [[ $IncorrectFiles == "" ]]
then
    echo $'Success!\nAll Files are named correctly!' >> BodyFile.txt
else
    echo $'Failed :c \n'"$IncorrectFiles" >> BodyFile.txt
    exit 1
fi
