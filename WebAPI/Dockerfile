#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["keys/", "keys/"]
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["Data/Repositories.csproj", "Data/"]
COPY ["Exceptions/Exceptions.csproj", "Exceptions/"]
COPY ["Model/Model.csproj", "Model/"]
COPY ["Logic/Logic.csproj", "Logic/"]
COPY ["RepositoriesImplementations/RepositoriesImplementations.csproj", "RepositoriesImplementations/"]
RUN dotnet restore "WebAPI/WebAPI.csproj"
COPY . .
WORKDIR "/src/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]

FROM nginx:alpine # use the nginx image that is found on docker hub
COPY nginx.conf /../server/nginx.conf # copy the configuration file from the previous step