# Dockerfile that would create an image with capability to convert html (mustache spec) to
# PDF 
# How I extracted underlying Dockerfile from  the downloaded image from CARLA OpenSource is 
# listed below
# docker pull chenzj/dfimage
# The above image helps with extraction fo docker file def
# On Windows system mapping looks like
# alias dfimage="docker run -v //var/run/docker.sock:/var/run/docker.sock --rm chenzj/dfimage"
# On Linux the required mapping looks like 
# alias dfimage="docker run -v /var/run/docker.sock:/var/run/docker.sock --rm chenzj/dfimage"
 
#  dfimage  ID_of_image_whose_dockerfile_would_be_created > Dockerfile
# cat Dockerfile
# It works better than docker history imageid --no-trunc 
# Also Docker history shows commands in reverse chronological order
# Converted the extracted Dockerfile (created from s2i strategy) to Docker build strategy equivalent below
# Not tested yet
# It assumes a particular naming and location for the pdf service C# project 

FROM registry.access.redhat.com/ubi8/dotnet-80-runtime:8.0-5.20240327145241 AS base
WORKDIR /opt/app-root/app
#ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_URLS=http://*:8080

ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV DOTNET_gcServer=1
ARG VERSION
ENV VERSION=$VERSION
USER 0
RUN /bin/sh -c "cd /opt     \
    && INSTALL_PKGS=" xorg-x11-fonts-Type1 libpng libpng-devel libjpeg-turbo libjpeg-turbo-devel libX11 libXext libXrender xorg-x11-fonts-Type1 fontconfig psmisc libjpeg62"     \
    && yum --disableplugin=subscription-manager --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder install -y $INSTALL_PKGS     \
    && yum -y install http://mirror.centos.org/centos/8-stream/AppStream/x86_64/os/Packages/xorg-x11-fonts-75dpi-7.5-19.el8.noarch.rpm     \
    && rpm -V $INSTALL_PKGS     \
    && yum --disableplugin=subscription-manager --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder -y install https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6.1-2/wkhtmltox-0.12.6.1-2.almalinux8.x86_64.rpm     \
    && mkdir /usr/local/bin/Linux     \
    && echo $'#!/usr/bin/env sh\n    /usr/local/bin/wkhtmltopdf --zoom 1.3 $@ \n    ' > /usr/local/bin/Linux/wkhtmltopdf     \
    && chmod +x /usr/local/bin/Linux/wkhtmltopdf     \
    && yum --disableplugin=subscription-manager clean all -y     \
    && popd"
USER 1001
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["pdf-service.csproj", "./"]
RUN dotnet restore "pdf-service.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "pdf-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pdf-service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /opt/app-root/app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pdf-service.dll"]
