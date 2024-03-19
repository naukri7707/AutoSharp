namespace AutoSharp.Core
{
    public readonly struct OnCompiledArgs
    {
        public OnCompiledArgs(Result result, string assemblyName, string filePath)
        {
            this.result = result;
            this.assemblyName = assemblyName;
            this.filePath = filePath;
        }

        public readonly Result result;

        public readonly string assemblyName;

        public readonly string filePath;

        public enum Result
        {
            Success,

            Fail,
        }
    }
}
