function CreateSearchPath($depth)
{
    $prefix = "<HintPath>";
    $postfix = "Libs\packages";
    $body = "";

    for($i=1; $i -le $depth; $i++)
    {
        $body = $body + "..\";
    }

    return $prefix + $body + $postfix;
}

function ReplaceNugetDir()
{
    Get-ChildItem -Path $currentDirectory -Filter *.csproj -Recurse -File -Name | ForEach-Object { 
        $fileFullPath = Resolve-Path (Join-Path -Path $currentDirectory -ChildPath $_);
        $text = [IO.File]::ReadAllText($fileFullPath, [Text.Encoding]::UTF8)

        for($i=0; $i -le $deep; $i++)
        {
            $replacePath = CreateSearchPath $i;
            $text = $text.Replace($replacePath, '<HintPath>$(NuGetDir)');
        }

        [IO.File]::WriteAllText($fileFullPath, $text, [Text.Encoding]::UTF8);
    }
}

function ReplaceFrameworkVersion()
{
    Get-ChildItem -Path $currentDirectory -Filter *.csproj -Recurse -File -Name | ForEach-Object { 
        $fileFullPath = Resolve-Path (Join-Path -Path $currentDirectory -ChildPath $_);
        $text = [IO.File]::ReadAllText($fileFullPath, [Text.Encoding]::UTF8)
        
        $text = $text.Replace('lib\net45', 'lib\$(NetFrameworkVersion)');

        [IO.File]::WriteAllText($fileFullPath, $text, [Text.Encoding]::UTF8);
    }
}

$currentDirectory = Get-Location;
$deep = 10;

ReplaceNugetDir;
ReplaceFrameworkVersion;