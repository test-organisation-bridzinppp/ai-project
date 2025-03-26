FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./src/worker-job/worker-job.csproj", "worker-job/"]
RUN dotnet restore "worker-job/worker-job.csproj"
COPY ./src/worker-job/ ./worker-job/
WORKDIR "/src/worker-job"
RUN dotnet build "worker-job.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "worker-job.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "worker-job.dll"]