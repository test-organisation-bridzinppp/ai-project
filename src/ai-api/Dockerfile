FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./src/ai-api/ai-api.csproj", "ai-api/"]
RUN dotnet restore "ai-api/ai-api.csproj"
COPY ./src/ .
WORKDIR "/src/ai-api"
RUN dotnet build "ai-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ai-api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ai-api.dll"]