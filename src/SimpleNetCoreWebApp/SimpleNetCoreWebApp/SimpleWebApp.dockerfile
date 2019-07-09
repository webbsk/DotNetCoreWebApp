FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["SimpleNetCoreWebApp/SimpleNetCoreWebApp.csproj", "SimpleNetCoreWebApp/"]
RUN dotnet restore "SimpleNetCoreWebApp/SimpleNetCoreWebApp.csproj"
COPY . .
WORKDIR "/src/SimpleNetCoreWebApp"
RUN dotnet build "SimpleNetCoreWebApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SimpleNetCoreWebApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SimpleNetCoreWebApp.dll"]