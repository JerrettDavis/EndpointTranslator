FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EndpointTranslator/EndpointTranslator.csproj", "EndpointTranslator/"]
RUN dotnet restore "EndpointTranslator/EndpointTranslator.csproj"
COPY . .
WORKDIR "/src/EndpointTranslator"
RUN dotnet build "EndpointTranslator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EndpointTranslator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EndpointTranslator.dll"]
