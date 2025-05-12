var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password", secret: true);

var database = builder.AddSqlServer("sql", password: sqlPassword)
    .WithDataVolume()
    .AddDatabase("sqldb");

var token = builder.AddParameter("token-security-key", secret: true);

var adminUsername = builder.AddParameter("admin-username");
var adminPassword = builder.AddParameter("admin-password", secret: true);

var api = builder.AddProject<Projects.ManualProg_Api>("api")
    .WithReference(database)
    .WaitFor(database)
    .WithEnvironment("token-security-key", token)
    .WithEnvironment("admin-username", adminUsername)
    .WithEnvironment("admin-password", adminPassword)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("app", "../ManualProg.App")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
