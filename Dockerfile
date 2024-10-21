# Build sdk image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
LABEL maintainer="Heber Estrada <heber.estrada1@gmail.com>"

WORKDIR /app

COPY  . ./app/

RUN ln -sf /usr/share/zoneinfo/America/Chihuahua /etc/localtime
RUN dotnet restore ./app/Tada/Tada.csproj
RUN dotnet publish ./app/Tada/Tada.csproj -c Release --output out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

COPY --from=builder /app/out .

ENV ASPNETCORE_HTTP_PORT=5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "Tada.dll"]
