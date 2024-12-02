using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// CustomLogger class for logging messages with various log levels and output options
/// </summary>
public class CustomLogger
{
    #region Fields and Properties

    /// <summary>
    /// File path for logging output
    /// </summary>
    private readonly string logFilePath;

    /// <summary>
    /// Flag to enable or disable console output
    /// </summary>
    private readonly bool enableConsole;

    /// <summary>
    /// Lock object for thread-safe file writing
    /// </summary>
    private readonly object writeLock = new object();

    /// <summary>
    /// Unique name for the logger instance
    /// </summary>
    public string LoggerName { get; private set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the CustomLogger class
    /// </summary>
    /// <param name="loggerName">Unique name for the logger instance</param>
    /// <param name="enableConsoleOutput">Flag to enable or disable console output (default: false)</param>
    public CustomLogger(string loggerName, bool enableConsoleOutput = false)
    {
        // Set logger name and console output flag
        LoggerName = loggerName;
        enableConsole = enableConsoleOutput;

        // Create a unique file name using timestamp and logger name
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"{loggerName}_{timestamp}.log";

        // Define log directory path
        string logDirectory = Path.Combine(Application.dataPath, "Logs");
        logFilePath = Path.Combine(logDirectory, fileName);

        // Ensure log directory exists
        if (!Directory.Exists(logDirectory))
        {
            Debug.Log("Log directory does not exist. Creating directory: " + logDirectory);
            Directory.CreateDirectory(logDirectory);
        }

        // Check if the file path is valid
        if(isValidFilePath(logFilePath) == false)
        {
            Debug.LogError("Invalid file path. Please check the directory and file name. File Path: " + logFilePath);
        }

        // Write initial log header
        Log($"=== Log Started for {loggerName} at {DateTime.Now} ===");
    }

    #endregion

    #region Enum

    /// <summary>
    /// Enum for log levels
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Informational log level
        /// </summary>
        Info,

        /// <summary>
        /// Warning log level
        /// </summary>
        Warning,

        /// <summary>
        /// Error log level
        /// </summary>
        Error,

        /// <summary>
        /// Debug log level
        /// </summary>
        Debug
    }

    #endregion

    #region Log Methods

    /// <summary>
    /// Logs a message with the specified log level. Error and warning levels will be written to console regardless of the writeToConsole flag.
    /// </summary>
    /// <param name="message">Log message</param>
    /// <param name="writeToConsole">Flag to write to console (default: false)</param>
    /// <param name="level">Log level (default: Info)</param>
    public void Log(string message, bool writeToConsole = false, LogLevel level = LogLevel.Info)
    {
        // Format log message with timestamp, logger name, and log level
        string formattedMessage = FormatLogMessage(message, level);

        switch (level)
        {
            case LogLevel.Warning:
                Debug.LogWarning(formattedMessage);
                break;
            case LogLevel.Error:
                Debug.LogError(formattedMessage);
                break;
            default:
                if(writeToConsole || enableConsole)
                    Debug.Log(formattedMessage);
                break;
        }

        // Write to file asynchronously
        WriteToFileAsync(formattedMessage);
    }

    /// <summary>
    /// Logs a message with a specified log level. Does not write to console.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="level"></param>
    public void Log(string message, LogLevel level)
    {
        Log(message, false, level);
    }


    /// <summary>
    /// Formats a log message with timestamp, logger name, and log level
    /// </summary>
    /// <param name="message">Log message</param>
    /// <param name="level">Log level</param>
    /// <returns>Formatted log message</returns>
    private string FormatLogMessage(string message, LogLevel level)
    {
        return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{LoggerName}] [{level}] {message}";
    }

    #endregion

    #region File Writing

    /// <summary>
    /// Writes a log message to file asynchronously
    /// </summary>
    /// <param name="message">Log message</param>
    private async void WriteToFileAsync(string message)
    {
        // Run task in background thread
        await Task.Run(() =>
        {
            try
            {
                // Use lock to prevent multiple threads from writing simultaneously
                lock (writeLock)
                {
                    using (StreamWriter writer = File.AppendText(logFilePath))
                    {
                        writer.WriteLine(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception to console if enabled
                if (enableConsole)
                {
                    Debug.LogError($"Failed to write to log file: {ex.Message}");
                }
            }
        });
    }

    /// <summary>
    /// Validates if the file path is valid and writable
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private bool isValidFilePath(string filePath)
    {
        try
        {
            using (FileStream fs = File.Create(filePath, 1, FileOptions.DeleteOnClose))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Convenience Methods

    /// <summary>
    /// Logs an informational message
    /// </summary>
    /// <param name="message">Log message</param>
    public void LogInfo(string message) => Log(message, LogLevel.Info);

    /// <summary>
    /// Logs a warning message
    /// </summary>
    /// <param name="message">Log message</param>
    public void LogWarning(string message) => Log(message, LogLevel.Warning);

    /// <summary>
    /// Logs an error message
    /// </summary>
    /// <param name="message">Log message</param>
    public void LogError(string message) => Log(message, LogLevel.Error);

    /// <summary>
    /// Logs a debug message
    /// </summary>
    /// <param name="message">Log message</param>
    public void LogDebug(string message) => Log(message, LogLevel.Debug);

    #endregion

    #region Helper Method

    /// <summary>
    /// Returns the full path to the log file
    /// </summary>
    /// <returns>Log file path</returns>
    public string GetLogFilePath() => logFilePath;

    #endregion
}