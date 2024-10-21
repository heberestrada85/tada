FROM mcr.microsoft.com/dotnet/sdk:8.0
LABEL maintainer="Heber Estrada <heber.estrada@outlook.com>"

ENV PATH $PATH:/root/.dotnet/tools

RUN apt-get update -qq
RUN apt-get install -yq --no-install-recommends nano sudo postgresql-client-common less unzip iputils-ping
#RUN dotnet tool install -g dotnet-ef

EXPOSE 5001
COPY profile-tada.sh /etc/profile.d/
