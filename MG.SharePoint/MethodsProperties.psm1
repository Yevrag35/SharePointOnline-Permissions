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

Function New-SPExpression()
{
    [CmdletBinding()]
    [alias("newex")]
    param
    (
        [parameter(Mandatory=$true, Position=0)]
        [string[]] $PropertyName,

        [parameter(Mandatory=$true, Position=1)]
        [type] $Type
    )
    Begin
    {
        $parameterExprType = [System.Linq.Expressions.ParameterExpression].MakeArrayType()
        $lambdaMethod = [System.Linq.Expressions.Expression].GetMethods() | ? { $_.Name -eq "Lambda" -and $_.IsGenericMethod -and $_.GetParameters().Length -eq 2 -and $_.GetParameters()[1].ParameterType -eq $parameterExprType }
        $lambdaGeneric = Invoke-Expression "`$lambdaMethod.MakeGenericMethod([System.Func``2[$($Type.FullName),System.Object]])";
        $list = Invoke-Expression "New-Object 'System.Collections.Generic.List[System.Linq.Expressions.Expression[System.Func``2[$($Type.FullName),System.Object]]]' $($PropertyName.Length)"
    }
    Process
    {
        for ($i = 0; $i -lt $PropertyName.Length; $i++)
        {
            $name = $PropertyName[$i]
            $param1 = [System.Linq.Expressions.Expression]::Parameter($Type, "p")
            $name1 = [System.Linq.Expressions.Expression]::Property($param1, $name)
            $body1 = [System.Linq.Expressions.Expression]::Convert($name1, $([object]))
            $list.Add($lambdaGeneric.Invoke($null, [object[]]@($body1, [System.Linq.Expressions.ParameterExpression[]]@($param1))))
        }
    }
    End
    {
        $out = Invoke-Expression "New-Object 'System.Linq.Expressions.Expression[System.Func``2[$($Type.FullName),System.Object]][]' $($list.Count)"
        $list.CopyTo($out, 0);
        Write-Output -InputObject $out -NoEnumerate;
    }
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

Export-ModuleMember -Function "Get-TypeMethods", "Get-TypeProperties", "New-SPExpression" -Alias "newex"