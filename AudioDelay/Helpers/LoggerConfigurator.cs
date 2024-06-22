using AudioDelay.Args;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace AudioDelay.Helpers;

public class LoggerConfigurator
{
  public static LoggerConfiguration ConfigureLogger(Arguments arguments)
  {
    var configuration = new LoggerConfiguration()
      .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
      .Enrich.FromLogContext()
      .Enrich.WithProperty("Application", "AudioDelay")
      .WriteTo.Console();

    if (arguments.LoggerName == ValidLoggers.Loki)
      configuration.WriteTo.GrafanaLoki(arguments.LoggerUrl!.ToString(),
        [new LokiLabel { Key = "app", Value = "AudioDelay" }]);

    if (arguments.LoggerName == ValidLoggers.Elasticsearch)
    {
      configuration.WriteTo.Elasticsearch([arguments.LoggerUrl!],
        opts =>
        {
          opts.DataStream = new DataStreamName("logs", "dotnet", "audio-delay");
          opts.BootstrapMethod = BootstrapMethod.Failure;
        });
    }

    return configuration;
  }
}