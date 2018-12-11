rem find the default gateway ip
@For /f "tokens=3" %%* in (
    'route.exe print ^|findstr "\<0.0.0.0\>"'
) Do @Set "defaultGateway=%%*"

rem Delete any existing route to the standard non routable IMDS ip address. It will be resetup later.
route delete 169.254.169.0/24

if NOT ["%errorlevel%"]==["0"] (
    echo "Route deletion failed"
    exit /b %errorlevel%
)
echo "Successfully deleted route for IMDS Ip address"

rem Add new route for IMDS ip to Default Gateway Ip Address
route add 169.254.169.0/24 %defaultGateway%
if NOT ["%errorlevel%"]==["0"] (
    echo "Route addition failed"
    exit /b %errorlevel%
)
echo "Successfully added route for IMDS Ip address"