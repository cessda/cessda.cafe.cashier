FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster

# Install Java for SonarScanner
RUN apt-get -qq update
RUN apt-get -qq install default-jre-headless

# Install SonarScanner
RUN dotnet tool install --global dotnet-sonarscanner
ENV PATH="${PATH}:/tmp/.dotnet/tools"