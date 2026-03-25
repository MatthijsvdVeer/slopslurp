var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SlopSlurp_Web>("web");

builder.Build().Run();
