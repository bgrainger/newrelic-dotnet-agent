// This ensures that we use a different logging repository from the rest of the process that we end up in.
[assembly: log4net.Config.Repository("NewRelic Log4Net Repository")]
