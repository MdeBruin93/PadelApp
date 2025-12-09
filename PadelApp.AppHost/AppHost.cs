using Azure.Provisioning.SignalR;
using Azure.Provisioning.Sql;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var signalR = builder.AddAzureSignalR("signalr")
    .ConfigureInfrastructure(infra =>
    {
        var signalR = infra.GetProvisionableResources()
            .OfType<SignalRService>()
            .Single();

        signalR.Sku.Name = "Free_F1";
    })
    .RunAsEmulator();

var azureSql = builder.AddAzureSqlServer("azuresql")
    .ConfigureInfrastructure(infra =>
    {
        var sql = infra.GetProvisionableResources().OfType<SqlDatabase>().Single();
        sql.FreeLimitExhaustionBehavior = FreeLimitExhaustionBehavior.BillOverUsage;
    })
    .RunAsContainer()
    .AddDatabase("database");

builder.AddProject<PadelApp>("padelapp")
    .WithReference(azureSql)
    .WaitFor(azureSql)
    .WithReference(signalR)
    .WaitFor(signalR)
    .WithExternalHttpEndpoints();

builder.Build().Run();
