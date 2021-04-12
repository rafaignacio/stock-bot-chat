
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY ../ .
RUN dotnet publish -c Release -o /app

FROM  mcr.microsoft.com/dotnet/runtime:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "finance-bot.Worker.dll"]
