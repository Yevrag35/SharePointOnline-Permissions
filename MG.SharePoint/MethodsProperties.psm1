Function Get-TypeMethods()
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [type] $Type,

        [parameter(Mandatory=$false)]
        [System.Reflection.BindingFlags] $Flags = [System.Reflection.BindingFlags]"Instance,Public"
    )
    $bad = @("Equals", "GetType", "ToString", "GetHashCode");

    $methods = $Type.GetMethods($Flags) | Where-Object { $_.Name -notlike "*_*" -and $_.Name -notin $bad };
    $outObj = foreach ($m in $methods)
    {
        $parameters = $m | Get-MethodParameters
        New-Object PSObject -Property @{
            ReturnType = Resolve-Type -Type $m.ReturnType
			Name = $m.Name
            Parameters = $parameters
        }
    }
    $outObj | Select-Object ReturnType, Name, Parameters | Sort-Object Name | Out-GridView
}

Function Get-TypeProperties()
{
	[CmdletBinding()]
	param
	(
        [parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [type] $Type,

        [parameter(Mandatory=$false)]
        [System.Reflection.BindingFlags] $Flags = [System.Reflection.BindingFlags]"Instance,Public"
    )
    $properties = $Type.GetProperties($Flags)
    $outObj = foreach ($p in $properties)
    {
        New-Object PSObject -Property @{
            Type = Resolve-Type -Type $p.PropertyType
            Name = $p.Name
            Get = $p.CanRead
            Set = $p.CanWrite
        }
    }
    $outObj | Select-Object Type, Name, Get, Set | Sort-Object Name | Out-GridView
}

Function Get-MethodParameters()
{
    [CmdletBinding()]
    [OutputType([string])]
    param
    (
        [parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [System.Reflection.MethodInfo] $MethodInfo
    )

    $parameters = @($MethodInfo.GetParameters());
    $strs = New-Object System.String[] $parameters.Count
    for ($i = 0; $i -lt $parameters.Count; $i++)
    {
        $p = $parameters[$i];
        $pType = Resolve-Type -Type $p.ParameterType
        $strs[$i] = "{0} {1}" -f $pType, $p.Name;
    }
    $outStr = $strs -join ', '
    Write-Output -InputObject $outStr;
}

Function Resolve-Type([type]$Type)
{
    switch ($Type.FullName)
    {
        "System.String"
        {
            $pType = "string"
        }
        "System.Void"
        {
            $pType = "void"
        }
        "System.Int32"
        {
            $pType = "int"
        }
        "System.Boolean"
        {
            $pType = "bool"
        }
        "System.Object"
        {
            $pType = "object"
        }
        "System.Guid"
        {
            $pType = $Type.Name
        }
        "System.String[]"
        {
            $pType = "string[]"
        }
        "System.Object[]"
        {
            $pType = "object[]"
        }
        "System.Byte[]"
        {
            $pType = "byte[]"
        }
        "System.Byte"
        {
            $pType = "byte"
        }
        default
        {
            if ([string]::IsNullOrEmpty($Type.FullName))
            {
                $pType = $Type.ToString()
            }
            elseif (-not $Type.IsGenericType)
            {
                $pType = $Type.FullName
            }
            else
            {
                $strs = foreach ($t in $Type.GenericTypeArguments)
                {
                    Resolve-Type -Type $t
                }
                $pType = "{0}[{1}]" -f $Type.GetGenericTypeDefinition().FullName, $($strs -join ', ')
            }
        }
    }
    Write-Output $pType
}

Export-ModuleMember -Function "Get-TypeMethods", "Get-TypeProperties"