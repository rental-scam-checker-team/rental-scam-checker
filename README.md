# Rental Scam Checker

CTEC2713 Agile Development Team Project. Checks a Danish rental listing for common scam signs and gives it a risk score.

## Team

| Table | Owner |
|---|---|
| tblListing | Aman Paudel |
| tblPriceBenchmark | Shrayana Kalika |
| tblSafetyTip | Ambika Pandey |
| tblRiskCheck | Guru Prasad Kandel (Scrum Master) |

## Structure

```
database/           SQL scripts, one folder per table (table + stored procedures + test data)
src/RSC.Classes/    C# classes: data access + validation (one class + collection per table)
src/RSC.Api/        ASP.NET Core Web API (controllers call RSC.Classes)
tests/RSC.Tests/    xUnit tests
frontend/           React (Vite) front end
```

## Run it

Needs: .NET 8 SDK, Node.js 20+, SQL Server (Express or LocalDB).

1. Database: create a database called `RentalScamChecker`, then run the scripts in this order:
   `database/Listing` → `database/RiskCheck` → `database/PriceBenchmark` → `database/SafetyTip`
2. API: `dotnet run --project src/RSC.Api` (Swagger at `/swagger`)
3. Tests: `dotnet test`
4. Front end: `cd frontend`, `npm install`, `npm run dev` (http://localhost:5173)

Change the connection string in `src/RSC.Api/appsettings.json` if your SQL Server is not `localhost`.

## Rules

- Never push straight to `main`. Make a branch, e.g. `RSC-14-riskcheck-table`, then open a pull request.
- Put the Jira key at the start of every commit message, e.g. `RSC-14 add tblRiskCheck stored procedures`.
- Only change files in your own table's folders unless the team agrees.
