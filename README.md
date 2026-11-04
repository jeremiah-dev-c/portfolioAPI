# portfolioapi

backend voor mijn portfoliosite. je kunt projecten en blogposts aanmaken, opvragen, wijzigen en verwijderen.

## techniek

- .net 10
- asp.net core minimal api (dus geen controllers)
- ef core met sql server
- swagger ui om te testen

## codestructuur

de code is in lagen verdeeld, elke laag heeft een eigen map:

- endpoints: de routes
- services: de logica en het omzetten naar dto's
- data: de dbcontext
- models: de entiteiten (project en blogpost)
- dtos: de invoer en uitvoer van de api

## starten

ik gebruik een mac dus de sql server draait bij mij in docker:

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=jouwwachtwoord" -p 1433:1433 --name sql_server -d mcr.microsoft.com/mssql/server:2022-latest

de connectionstring zet ik in de user-secrets, zodat er geen wachtwoord in de code staat:

dotnet user-secrets set "ConnectionStrings:PortfolioDb" "Server=localhost,1433;Database=PortfolioDb;User Id=sa;Password=jouwwachtwoord;TrustServerCertificate=True"

de database maak ik aan met een ef core migratie:

dotnet ef database update

daarna starten met:

dotnet run

swagger staat dan op http://localhost:5175/swagger (bij mij)
