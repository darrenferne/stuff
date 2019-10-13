#
# Script for creating certifcates, binding and unbinding to ports
# Authors: D Murphy
# Date: 25/05/2017
#

#Requires -Version 3.0
#Requires -RunAsAdministrator

<#
.SYNOPSIS
Creates a certificate or uses an existing certificate and binds it to one or more ports.
.DESCRIPTION
Creates a self signed certificate for use with bwf development in the root of the certificate storage. If a certificate already
exists with the name of the domian passed then the existing certificate is used. If more than one certificate exists with the same 
name then you will be asked to select the required certifcate.

The script will clear any certificate bindings for the ports and IP address passed in and then bind the newly created 
or selected existing certifcate to the IP address and ports.

.PARAMETER hostname
-hostname The hostname of the machine, this will be used as the certifcate name. Defaults to localhost

.PARAMETER ipaddress
The IP Address of the machine, this will be used to clear and bind the certifcate to ports. Defaults to 0.0.0.0

.PARAMETER ports
The ports to bind the certificate to. One or more ports can be used, separate values with commas. Defaults to 4323

.PARAMETER forcecreate
This option will force the creation of a certificate, on Windows 10 the certificate will automatically be used for binding unless the -nobinding option is used

.PARAMETER nobinding
This option will not bind the certificate to any ports, use it if you need a certificate to setup a test environment etc.

.EXAMPLE
Default usage, useful for getting started with development on bwf. Will create a certificate called localhost and bind it to 0.0.0.0 port 4323

bwf-self-cert.ps1

.EXAMPLE
Binding to multiple ports. Will bind to 0.0.0.0 ports 4323 and 10001

bwf-self-cert.ps1 -ports 4323,10001
#>

param(
    [string]$hostname = "localhost",
    [string]$ipaddress = "0.0.0.0",
    [string[]]$ports = "4323",
    [switch]$forcecreate,
    [switch]$nobinding
)

function GetExistingCertificate
{
    param(
        $certId,
        $store = "Root",
        $location = "LocalMachine"
    )
    [array]$existingCert = Get-ChildItem -path cert:\$location\$store | Where-Object {$_.Subject -eq $certId }

    If($existingCert.Count -gt 1)
    {
        $title = "Choose certificate"
        $prompt = "Found more than 1 matching certficate, please select certificate to use"

        $choices = @()
            For($index = 0; $index -lt $existingCert.Count; $index++){
                $label = "&" + $index + ": " + ($existingCert[$index]).Thumbprint
                $choices += New-Object System.Management.Automation.Host.ChoiceDescription $label, ($existingCert[$index])
            }

        $chosenCertificateIndex = $host.ui.PromptForChoice($title, $message, $choices, 0) 

        $existingCert[$chosenCertificateIndex]
    }
    
    If($existingCert.Count -eq 1)
    {
        $existingCert[0]
    }
}

function ClearPortBindings($address, [string[]]$ports)
{   
    foreach($port in $ports)
    {			
        $ipport = $address + ":" + $port
        Write-Host "Clearing binding on " $ipport
        $netshParams = 'http delete sslcert ipport=' + $ipport
        & netsh.exe $netshParams.Split(' ')
    }        
}

function BindCertificateToPorts($certificate, $ipaddress, [string[]]$ports)
{
    $guid = "{" + [guid]::NewGuid() + "}"

    foreach($port in $ports)
    {			
        $ipport = $ipaddress + ":" + $port
        Write-Host "Binding certificate to port " $ipport
        $netshParams = 'http add sslcert ipport=' + $ipport + ' certhash=' + $certificate.Thumbprint + ' appid=' + $guid
        & netsh.exe $netshParams.Split(' ')    
    }        
}

function CreateSelfCertOpenSSL 
{
    param(
        [string]$hostname = "localhost"
    )
    
    $SAN = "`n[SAN]`nsubjectAltName=DNS:" + $hostname

    Copy-Item C:\OpenSSL-Win32\bin\openssl.cfg .\$hostname.cnf

    Add-Content .\$hostname.cnf $SAN

    $reqParams = "req -new -x509 -newkey rsa:2048 -sha256 -nodes -subj /CN=$hostname -extensions SAN -keyout $hostname.key -days 1825 -out $hostname.crt -config $hostname.cnf"
    & C:\OpenSSL-Win32\bin\openssl.exe $reqParams.Split(' ')

    $pkcs12Params = "-export -out $hostname.p12 -inkey $hostname.key -in $hostname.crt -passout pass:"
    & C:\OpenSSL-Win32\bin\openssl.exe pkcs12 $pkcs12Params.Split(' ')

    if( Get-Command  "Import-PfxCertificate" -errorAction SilentlyContinue )
    {
        $newCertificate = Import-PfxCertificate -FilePath .\$hostname.p12 -CertStoreLocation Cert:\LocalMachine\My    

        CopyCertificate $newCertificate
    }
    else
    {
        Write-Host "You need to import the certificate manually and then run this script again."    
        exit
    }

    return $newCertificate
}


function CreateCertificate($hostname)
{
	$certificate = New-SelfSignedCertificate -KeyLength 2048 -DnsName $hostname -FriendlyName $hostname `
        -CertStoreLocation "Cert:LocalMachine\My" -HashAlgorithm sha256 -NotAfter (Get-Date).AddYears(5)
    CopyCertificate $certificate
    return $certificate
}

function CopyCertificate($certificate)
{
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store "Root", "LocalMachine"
    $store.Open( "ReadWrite" )
    $store.Add($certificate)
    $store.Close()
}

function NoSelfCertCmdlet()
{
   if( Get-Command  "New-SelfSignedCertificate" -errorAction SilentlyContinue )
   {
       return $false       
   }
   else 
   {
       return $true
   }
}

# start
Write-Host "Creating Certificate for" $hostname, $ipaddress, $ports

# build certificate subject string
$certId = "CN=" + $hostname

$openSSLDownload = 'https://slproweb.com/download/Win32OpenSSL_Light-1_1_0f.exe'

$certificate = $null

# check for existing certificate
if(!$forcecreate)
{
    $certificate = GetExistingCertificate $certId
}

If($certificate)
{
    Write-Host "Using existing certificate"
}
else
{    
    #create new certificate
    Write-Host "Creating new certificate"
    if(NoSelfCertCmdlet)
    {
        # check for open ssl
        if( Test-Path C:\OpenSSL-Win32\bin\openssl.exe)
        {
            $certificate = CreateSelfCertOpenSSL $hostname         
        }
        else 
        {
            Write-Host Cannot create a certificate because OpenSSL cannot be found.    
            Write-Host Download `& install $openSSLDownload.
            exit
        }
    }
    else
    {
        $certificate = CreateCertificate $hostname
    }        
}

if($certificate)
{
    Write-Host "Thumbprint"$certificate.Thumbprint "Subject"$certificate.Subject
    if(!$nobinding)
    {
        ClearPortBindings $ipaddress $ports

        BindCertificateToPorts $certificate $ipaddress $ports
    }
}
else 
{
    Write-Host "Failed to create certificate."
}
