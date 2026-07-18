FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SmartFoodPlanner.csproj .
RUN dotnet restore SmartFoodPlanner.csproj

COPY . .
RUN dotnet publish SmartFoodPlanner.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/Data

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "SmartFoodPlanner.dll"]
