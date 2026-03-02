Write-Host Building...
rm -Recurse .\dist\
dotnet publish -c Release -r linux-x64 --no-self-contained -o dist

Write-Host Creating package...
Compress-Archive -Path .\dist\* -DestinationPath .\dist.zip -Force

Write-Host Uploading...
$environment = "dev"
$bucketName = "fiap-mechanics-$environment-lambda-artifacts-$(aws sts get-access-key-info --access-key-id $(aws configure get aws_access_key_id) --query Account --output text)"
$tag = New-Guid
aws s3 cp .\dist.zip s3://$bucketName/auth/$tag.zip

Write-Host Updating function...
aws lambda update-function-code --function-name "fiap-mechanics-$environment-auth" --s3-bucket $bucketName --s3-key auth/$tag.zip | Out-Null
