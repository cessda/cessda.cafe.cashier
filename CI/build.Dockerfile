FROM mcr.microsoft.com/dotnet/sdk:6.0-bookworm-slim

# Install Java for SonarScanner
RUN apt-get -qq update
RUN apt-get -qq install default-jre-headless