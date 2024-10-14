using HmsApi;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


await WebHost.CreateDefaultBuilder(args)
    .UseStartup<Startup>()
    .Build()
    .RunAsync();
