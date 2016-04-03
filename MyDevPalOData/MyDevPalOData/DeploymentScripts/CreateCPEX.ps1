param(
    $inputFolder,
    $outputPath
)

function ZipFiles( $zipfilename, $sourcedir )
{
    Write-Host("ZipFileName: $zipfilename")
    Write-Host("sourceDir: $sourceDir")
    $tempfilename = [System.IO.Path]::GetTempFileName()
    Remove-Item $tempfilename -Force

	[Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" )
	$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
	[System.IO.Compression.ZipFile]::CreateFromDirectory( $sourcedir, $tempfilename, $compressionLevel, $false )
    Copy-Item -Path $tempfilename -Destination $zipfilename -Force

    Remove-Item $tempfilename -Force
}

ZipFiles "$outputPath " "$inputFolder "
