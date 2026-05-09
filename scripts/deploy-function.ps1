param (
    [Parameter(HelpMessage = "Environment for deploy: dev, stg, prod (dev)")]
    [ValidateSet("dev", "stg", "prod", "")]
    [string]$Environment = "dev",
    [Parameter(HelpMessage = "Function to deploy: api, consumer, all")]
    [ValidateSet("api", "consumer", "all")]
    [string]$Function = "all",
    [Parameter(HelpMessage = "Apply seeds for tables")]
    [switch]$ApplySeeds
)

Write-Host "Fetching data for environment '$environment'..."
$bucketName = "fiap-mechanics-$environment-lambda-artifacts-$( aws sts get-access-key-info --access-key-id $( aws configure get aws_access_key_id ) --query Account --output text )"

function Deploy-Function
{
    param(
        [string]$project,
        [string]$functionName,
        [bool]$snapStart = $false
    )

    Write-Host Building...
    Remove-Item -Recurse .\dist\
    dotnet publish .\src\$project -c Release -r linux-x64 --no-self-contained -o dist

    Write-Host Creating package...
    Compress-Archive -Path .\dist\* -DestinationPath .\dist.zip -Force

    Write-Host Uploading...
    $tag = New-Guid
    aws s3 cp .\dist.zip s3://$bucketName/$functionName/$tag.zip

    Write-Host Updating function...
    aws lambda update-function-code --function-name "fiap-mechanics-$environment-$functionName" --s3-bucket $bucketName --s3-key $functionName/$tag.zip | Out-Null

    if ($snapStart)
    {
        aws lambda wait function-updated --function-name "fiap-mechanics-$environment-$functionName"
        aws lambda publish-version --function-name "fiap-mechanics-$environment-$functionName" | Out-Null
    }

    Write-Host -ForegroundColor Green "Lambda function 'fiap-mechanics-$environment-$functionName' deployed."
}

if ($function -eq "api" -or $function -eq "all")
{
    Write-Host -ForegroundColor Yellow "Deploying function 'auth-token'..."
    Deploy-Function -project "Mechanics.Auth.Api" -functionName "auth-token" -snapStart $true
}

if ($function -eq "consumer" -or $function -eq "all")
{
    Write-Host -ForegroundColor Yellow "Deploying function 'auth-consumer'..."
    Deploy-Function -project "Mechanics.Auth.Consumer" -functionName "auth-consumer"
}

if ($applySeeds)
{
    Write-Host "Seeding users table for environment $environment..."
    $tableName = "auth-$environment-users"
    $users = Get-Content .\seeds\users.json | ConvertFrom-Json

    foreach ($user in $users)
    {
        $item = @{ }
        foreach ($prop in $user.PSObject.Properties)
        {
            $item[$prop.Name] = @{ "S" = $prop.Value.ToString() }
        }

        aws dynamodb put-item `
        --table-name $tableName `
        --item "$( $item | ConvertTo-Json -Compress )" `
        --condition-expression "attribute_not_exists(id)" | Out-Null
    }
}
