FROM microsoft/dotnet:2.1-runtime
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "IdentitySample.Web.dll"]
