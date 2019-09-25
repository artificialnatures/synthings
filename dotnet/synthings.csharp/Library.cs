namespace synthings.csharp
{
    using core;
    
    public static class Library
    {
        public static LibraryResolver Load()
        {
            return aggregateLibrary.build();
        }
    }
}