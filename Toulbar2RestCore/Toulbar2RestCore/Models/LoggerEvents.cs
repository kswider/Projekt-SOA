public class LoggerEvents
    {
        public const int RequestPassed = 1000;

        public const int ExampleFound = 1001; //info
        public const int ExampleNotFound = 1002; //warning

        public const int FileLoaded = 1003; //info

        public const int ProblemLoaded = 1004; //info

        public const int ResponseCreated = 1005; //info

        public const int FileCreated = 1006; //info

        public const int Process = 1007;

        public const int RequestFailed = 4000;
        public const int ExampleFileError = 4001;
        public const int FileError = 4002;
        public const int ProblemError = 4004;

    }