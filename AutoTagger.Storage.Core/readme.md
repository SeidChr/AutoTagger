## using EF .Net Core and using a Mysql DB

FYI Entity Framework 6.0 läuft nur unter .Net Framework 4.6 oder EF Core mit .Net Core 2.0. Mit .Net Standard kann mit EF nicht nutzen. using following tutorial:
  * https://www.learnentityframeworkcore.com/walkthroughs/existing-database
  * https://docs.microsoft.com/de-de/ef/core/providers/
  * https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql

Go to folder of the proj:

```
dotnet add package Microsoft.EntityFrameworkCore.Tools 
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

add in .csproj file:
```
<ItemGroup>
<DotNetCliToolReference
    Include="Microsoft.EntityFrameworkCore.Tools.DotNet"
    Version="2.0.2" />
</ItemGroup>
```

to restore dependencies:
```	
dotnet restore
```

following file generates the database-first models:
```
dotnet ef dbcontext scaffold "Server=78.46.178.185;User Id=InstaTagger;Password=PASSWORD;Database=instatagger" "Pomelo.EntityFrameworkCore.MySql"
```