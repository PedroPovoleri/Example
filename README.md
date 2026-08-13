# ResumeHexagonalApp

A .NET 10 sample solution implementing a hexagonal architecture with:

- Domain layer with business entity and port contracts
- Application layer with use-case orchestration
- Infrastructure layer with in-memory driven adapters
- Minimal API HTTP surface using handler functions
- xUnit tests for service behavior

## Solution structure

- `ResumeHexagonal.Domain`
- `ResumeHexagonal.Application`
- `ResumeHexagonal.Infrastructure`
- `ResumeHexagonal.Api`
- `ResumeHexagonal.Tests`

## Run locally

```bash
dotnet restore

dotnet build ResumeHexagonalApp.sln --nologo -v minimal

dotnet test ResumeHexagonalApp.sln --nologo --verbosity minimal

dotnet run --project ResumeHexagonal.Api
```

## API endpoints

- `POST /api/resume-events`
- `GET /api/resume-events/{id}`

## Container publishing from the project file

This app is configured for container publishing directly from the .csproj, without creating a Dockerfile.

Example:

```bash
dotnet publish ResumeHexagonal.Api/ResumeHexagonal.Api.csproj -c Release \
  -p:ContainerRegistry=<your-registry-host> \
  -p:ContainerImageName=resumehexagonal-api \
  -p:ContainerImageTag=latest
```

For Azure Container Registry:

```bash
dotnet publish ResumeHexagonal.Api/ResumeHexagonal.Api.csproj -c Release \
  -p:ContainerRegistry=myregistry.azurecr.io \
  -p:ContainerImageName=resumehexagonal-api \
  -p:ContainerImageTag=latest
```

For Amazon ECR, use your ECR registry hostname in the same way, then push the image with your normal AWS CLI workflow.

## Notes

This sample uses an in-memory implementation for the driven ports (`ForCreateEvent`, `ForReadEvent`, `ForLogEvent`) so the architecture is easy to understand and extend with real persistence or messaging adapters.
