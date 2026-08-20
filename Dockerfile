FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj
RUN dotnet build tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj --no-restore
RUN dotnet test tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj --no-build
RUN dotnet publish src/FsharpStakeholder/FsharpStakeholder.fsproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FsharpStakeholder.dll"]
