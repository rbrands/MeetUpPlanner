## Enable local HTTPS for .NET Core
dotnet dev-certs https --trust

## Set NuGet
dotnet nuget add source https://nuget.pkg.github.com/rbrands/index.json -n github -u rbrands -p $NUGETPASSWORD --store-password-in-clear-text

## Restore projects
dotnet restore

## Static Web Apps
# npm install -g @azure/static-web-apps-cli
# npm i -g azure-functions-core-tools@4 --unsafe-perm true