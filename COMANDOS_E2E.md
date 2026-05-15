# Ejecutar pruebas E2E viendo el navegador

Ejecuta estos comandos desde la carpeta `TestingBackendCourse`.

## Terminal 1: API

```powershell
dotnet run --project src\Course.Api\Course.Api.csproj
```

## Terminal 2: Web

```powershell
dotnet run --project src\Course.Web\Course.Web.csproj
```

## Terminal 3: Pruebas E2E

```powershell
dotnet test Course.Web.E2ETest\Course.Web.E2ETest.csproj --settings Course.Web.E2ETest\.runsettings
```

El archivo `.runsettings` ejecuta Playwright con `Headless=false`, por eso se abre el navegador durante las pruebas.
