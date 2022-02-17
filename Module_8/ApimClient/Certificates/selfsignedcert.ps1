# Script should be run aS Administrator
# Start Visual Studio Code as Administrator should do
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Bypass
$cert = New-SelfSignedCertificate -CertStoreLocation Cert:\LocalMachine\My -DnsName acmi.ps.com 
$pass = ConvertTo-SecureString -String "P@ssw0rd" -Force -AsPlainText
$path = "Cert:\LocalMachine\My\"+$cert.Thumbprint
Export-PfxCertificate -Cert $path -FilePath ".\selfcert.pfx" -Password $pass