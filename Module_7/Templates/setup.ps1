# Make sure you use Az module 5.6.0 ot higer
# Update-Module AZ
# Or
# Install-Module -Name Az
$rgName = "testgroup"
Get-AzContext -ErrorAction SilentlyContinue -ErrorVariable notLoggedIn
if ($notLoggedin)
{
    Connect-AzAccount
    $subs = Get-AzSubscription
    for (($i=0); $i -lt $subs.Length;$i++)
    {
      Write-Host $i") " $subs[$i].Name
    }
    $selection = Read-Host "Select Subscription (0,1,2,...)"

    Set-AzContext -SubscriptionName $subs[$selection]
}
$file = $PSScriptRoot+"\resources.bicep"
Write-Host $file
New-AzResourceGroupDeployment -ResourceGroupName $rgName -TemplateFile $file

#$pass = ConvertTo-SecureString -String "Pa@@w0rd" -AsPlainText -Force
#New-AzADApplication  -DisplayName "Module_7" -IdentifierUris "api://vklndlfkbnkldnlbskd" -EndDate (Get-Date).AddMonths(3) -Password $pass



