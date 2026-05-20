FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["TaskManagement.API/TaskManagement.API.csproj",                   "TaskManagement.API/"]
COPY ["TaskManagement.Application/TaskManagement.Application.csproj",   "TaskManagement.Application/"]
COPY ["TaskManagement.Domain/TaskManagement.Domain.csproj",             "TaskManagement.Domain/"]
COPY ["TaskManagement.Infastructure/TaskManagement.Infastructure.csproj","TaskManagement.Infastructure/"]

RUN dotnet restore "TaskManagement.API/TaskManagement.API.csproj"

COPY . .

RUN dotnet publish "TaskManagement.API/TaskManagement.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]