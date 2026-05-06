param (
    [Parameter(HelpMessage = "Environment for deploy: dev, stg, prod (dev)")]
    [ValidateSet("dev", "stg", "prod", "")]
    [string]$environment = "dev"
)

Write-Host Building...
Remove-Item -Recurse .\dist\
dotnet publish .\src\Mechanics.Auth.Api -c Release -r linux-x64 --no-self-contained -o dist

Write-Host Creating package...
Compress-Archive -Path .\dist\* -DestinationPath .\dist.zip -Force

Write-Host Uploading...
$bucketName = "fiap-mechanics-$environment-lambda-artifacts-$( aws sts get-access-key-info --access-key-id $( aws configure get aws_access_key_id ) --query Account --output text )"
$tag = New-Guid
aws s3 cp .\dist.zip s3://$bucketName/auth/$tag.zip

Write-Host Updating function...
aws lambda update-function-code --function-name "fiap-mechanics-$environment-auth-token" --s3-bucket $bucketName --s3-key auth/$tag.zip | Out-Null
aws lambda wait function-updated --function-name "fiap-mechanics-$environment-auth-token"
aws lambda publish-version --function-name "fiap-mechanics-$environment-auth-token" | Out-Null

Write-Host Seeding database...
$tableName = "auth-$environment-users"
$users = Get-Content .\seeds\users.json | ConvertFrom-Json
foreach ($user in $users) {
    $item = @{}
    foreach ($prop in $user.PSObject.Properties) {
        $item[$prop.Name] = @{ "S" = $prop.Value.ToString() }
    }

    aws dynamodb put-item `
        --table-name $tableName `
        --item "$($item | ConvertTo-Json -Compress)" `
        --condition-expression "attribute_not_exists(id)"
}

Write-Host -ForegroundColor Green Lambda function deployed.
