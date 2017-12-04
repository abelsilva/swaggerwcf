$dllFiles = Get-ChildItem -Filter *.dll | ForEach-Object {$_.FullName}

$dllFiles | ForEach-Object {
    $__ = [Reflection.Assembly]::LoadFile($_)
}

[SwaggerWcf.SwaggerWcfEndpoint]::GenerateSwaggerFile() | Out-File .\swagger.json
