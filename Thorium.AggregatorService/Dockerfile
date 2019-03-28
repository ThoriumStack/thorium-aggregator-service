FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

WORKDIR /app/Thorium.Aggregator/

# Copy everything and restore
COPY . ./
RUN dotnet restore --configfile Nuget.config

# Build
RUN dotnet publish -c Release -o out 
RUN ls
COPY replaceme.pfx out

# Build runtime image
FROM microsoft/dotnet:2.2-sdk
WORKDIR /app
COPY --from=build-env /app/Thorium.Aggregator/out/ .
ENTRYPOINT ["dotnet", "Thorium.Aggregator.dll"]