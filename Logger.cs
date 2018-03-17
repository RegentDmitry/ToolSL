namespace ToolSL
{
    public static class Logger
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Info(string text)
        {
            _logger.Info(text);
        }

        public static void Error(string error)
        {
            _logger.Info(error);
        }
    }
}
