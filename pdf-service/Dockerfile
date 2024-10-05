FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["pdf-service.csproj", "./"]
RUN dotnet restore "pdf-service.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "pdf-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pdf-service.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /opt/app-root/app
#ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_URLS=http://*:8080

ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV DOTNET_gcServer=1
ENV LD_LIBRARY_PATH=/usr/lib:/usr/local/lib:$LD_LIBRARY_PATH
ARG VERSION
ENV VERSION=$VERSION
USER 0
RUN apt-get update && ln -s /usr/lib/libgdiplus.so /lib/x86_64-linux-gnu/libgdiplus.so
RUN apt-get install -y --no-install-recommends zlib1g fontconfig libfreetype6 libx11-6 libxext6 libxrender1 wget gdebi
RUN wget https://github.com/wkhtmltopdf/wkhtmltopdf/releases/download/0.12.5/wkhtmltox_0.12.5-1.stretch_amd64.deb
RUN wget http://archive.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.1f-1ubuntu2_amd64.deb
RUN dpkg -i libssl1.1_1.1.1f-1ubuntu2_amd64.deb
RUN gdebi --n wkhtmltox_0.12.5-1.stretch_amd64.deb
RUN ln -s /usr/local/lib/libwkhtmltox.so /usr/lib/libwkhtmltox.so
USER 1001

EXPOSE 8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "pdf-service.dll"]


