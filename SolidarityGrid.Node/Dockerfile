#Compilación

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SolidarityGrid.Node.csproj", "./"]
RUN dotnet restore "SolidarityGrid.Node.csproj"


COPY . .
RUN dotnet publish "SolidarityGrid.Node.csproj" -c Release -o /app/out

# Ejecución

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app


COPY --from=build /app/out .


EXPOSE 8080

ENTRYPOINT ["dotnet", "SolidarityGrid.Node.dll"]