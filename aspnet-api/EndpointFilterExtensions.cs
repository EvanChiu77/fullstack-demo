public static class EndpointFilterExtensions
{
    // 此擴展方法會在請求處理前後分別輸出日誌
    public static TBuilder AddLoggingFilter<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            Console.WriteLine("Before executing endpoint filter");
            var result = await next(context);
            Console.WriteLine("After executing endpoint filter");
            return result;
        });
        return builder;
    }
}