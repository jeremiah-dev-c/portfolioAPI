# portfolioapi

backend voor mijn portfoliosite. je kunt projecten en blogposts aanmaken, opvragen, wijzigen en verwijderen. de schrijf-routes zijn beveiligd met ASP.NET Core Identity.

## techniek

* .net 10
* asp.net core minimal api (dus geen controllers)
* ef core met sql server
* asp.net core identity (registratie, login met cookie, rollen)
* xunit voor de tests
* swagger ui om te testen

## codestructuur

de code is in lagen verdeeld, elke laag heeft een eigen map:

* endpoints: de routes (projecten, blogposts en auth)
* services: de logica en het omzetten naar dto's
* data: de dbcontext en het aanmaken van de rollen
* models: de entiteiten (project en blogpost)
* dtos: de invoer en uitvoer van de api

de tests staan in een apart project, PortfolioApi.Tests, in dezelfde solution.

## starten

ik gebruik een mac dus de sql server draait bij mij in docker:

```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=jouwwachtwoord" -p 1433:1433 --name sql_server -d mcr.microsoft.com/mssql/server:2022-latest
```

de connectionstring zet ik in de user-secrets, zodat er geen wachtwoord in de code staat:

```
dotnet user-secrets set "ConnectionStrings:PortfolioDb" "Server=localhost,1433;Database=PortfolioDb;User Id=sa;Password=jouwwachtwoord;TrustServerCertificate=True" --project PortfolioApi
```

de database maak ik aan met de ef core migraties (vanuit de map met PortfolioApi.sln). dit maakt ook de identity-tabellen aan (AspNetUsers, AspNetRoles, AspNetUserRoles):

```
dotnet ef database update --project PortfolioApi
```

daarna starten met:

```
dotnet run --project PortfolioApi
```

swagger staat dan op http://localhost:5175/swagger (bij mij)

## registreren en inloggen

* POST /api/auth/register maakt een gebruiker. iedere nieuwe gebruiker krijgt de rol Gast.
* POST /api/auth/login geeft een authenticatiecookie terug.
* de rollen Gast, Editor en Admin worden bij het starten van de api automatisch aangemaakt.
* na een wijziging van een rol moet de gebruiker opnieuw inloggen, want de rol staat in de cookie.

## wie mag wat

* alle GET-routes van projecten en blogposts zijn voor iedereen.
* POST, PUT en DELETE mogen alleen Editor en Admin.
* zonder login geeft de api 401, met een ingelogde gebruiker zonder Editor- of Admin-rol 403.

## editor en admin maken (lokaal)

editor en admin kun je niet via de api krijgen, dat doe ik handmatig in de lokale database:

1. registreer eerst een gewone gebruiker via /api/auth/register, bijvoorbeeld editor@test.nl.
2. voer dit uit in de database:

```sql
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'editor@test.nl' AND r.Name = 'Editor';
```

3. voor een admin doe je hetzelfde met r.Name = 'Admin'.
4. log daarna opnieuw in.

controleren wie welke rol heeft:

```sql
SELECT u.Email, r.Name AS Rol
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON ur.UserId = u.Id
JOIN AspNetRoles r ON r.Id = ur.RoleId;
```

## tests

de tests staan in PortfolioApi.Tests (xunit). ze starten de api met WebApplicationFactory en sturen echte http-verzoeken. elke test gebruikt een eigen sqlite-database in het geheugen, dus mijn echte database wordt niet aangeraakt. uitvoeren vanuit de map met PortfolioApi.sln:

```
dotnet test
```
