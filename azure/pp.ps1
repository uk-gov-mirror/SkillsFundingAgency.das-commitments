param(
[parameter()]
[string]$StorageConnectionString
)

$ctx = New-AzureStorageContext -ConnectionString "$(StorageConnectionString)"

$date=get-date -UFormat %d-%m-%Y

$Files = Get-ChildItem "$(System.DefaultWorkingDirectory)\das-commitments-automation-suite\Publish\SFA.DAS.Commitments.UI.Tests\Project\Screenshots\$date" -Filter *.png -ErrorAction SilentlyContinue

if ($files.Count -cge 1) {

Foreach ($file in $files) {
$blobname = $file
Set-AzureStorageBlobContent -File "$(System.DefaultWorkingDirectory)\das-commitments-automation-suite\Publish\SFA.DAS.Commitments.UI.Tests\Project\Screenshots\$date\$File" -Container $env:ContainerName -Blob $env:RELEASE_ENVIRONMENTNAME\$date\$BlobName -Context $ctx  -Force 
}
 Write-Error ("An error has been captured")
}
else {
write-host "No errors Capture"