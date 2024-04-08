![ApiService Deploy](https://github.com/shindy001/DevBookServer/actions/workflows/deploy-devbook-server.yml/badge.svg)

# DevBook Server

DevBook Server is a backend webapi for [DevBook.Web application](https://github.com/shindy001/DevBook.Web).

Purpose of this repo is to provide backend for my testing and personal endeavors in applications/web space and also to brush up on my rusty skills and to try new languages/technologies/approaches.

## Current state of API Features (more to come)
- [x] Authentication (.net identity)
  - Token based with refresh
  - Registration (no email confirmation at least for now)
- [x] Time Management (for signed in users only)
  - Project Management (CRUD + additional commands)
  - Tasks Management (CRUD + additional commands)
- [x] Sudoku boards api
  - Uses remote api to get a new board
- [ ] Time Management analytics - time tracking / earnings statistics
- [ ] Time Management exports - export to excel stylesheet
- [ ] Server integration tests
- [ ] CI tests pipeline

## Technology stack
#### DevBook Api Service (per user data / other apis)
  - .NET v8
  - ASP.NET Core Webapi (minimal)
  - Entity Framework core v8 (ORM)
  - SQLite (DB)
  - Azure App Service (webapi hosting)

## Dev Requirements
- `Visual Studio` or `VSCode with C# Dev Kit`
- .net 8 SDK

## Deployment
API is deployed by using ```github actions (manually)``` to [Azure App Service](https://azure.microsoft.com/en-gb/products/app-service).

## How to run
1. Open DevBookServer.sln
1. Select "https" configuration and run it.
