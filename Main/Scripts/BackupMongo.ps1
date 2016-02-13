########################################################################################
# Backup Mongo DBs
########################################################################################

cd $PSScriptRoot
write-host "current dir:$pwd"

$date = Get-Date -format "yyyyMMdd_HHmmss"
$backupFolder = $date;
$basePath = ".\DBbackup";
$destinationPath = Join-Path $basePath $backupFolder;
$mongoBin = "C:\Program Files\MongoDB 2.6 Standard\bin\mongodump.exe";
if(!(Test-Path -Path $mongoBin)) {
  $mongoBin = "C:\Program Files\MongoDB\server\3.0\bin\mongodump.exe";
}

write-host "destinationPath:$destinationPath"

write-host "---STEP - MongoDB backup ---"
if(!(Test-Path -Path $destinationPath)) {
    New-Item -ItemType directory -Path $destinationPath;
    &$mongoBin --out $destinationPath;
}

write-host "---STEP - Zip ---"
$zipname = "mysitterhub_dbbackup_$date.zip"
$zip = "$destinationPath\$zipname"
write-zip $destinationPath $zip

write-host "---STEP - UPLOADING TO S3 ---"

$accessKey = "AKIAJF3CPB5GCZPAMWGQ"; #DbBackup IAM user
$secretKey = "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";
$creds = New-AWSCredentials -AccessKey $accessKey -SecretKey $secretKey
Set-AWSCredentials -AccessKey $accessKey -SecretKey $secretKey #-StoreAs MysitterhubProfile

#Set-AWSCredentials -ProfileName MysitterhubProfile 

Write-S3Object -BucketName avodbbackups -File $zip -Key $zipname

exit;

