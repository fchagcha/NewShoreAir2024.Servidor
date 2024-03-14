namespace NewShoreAir.Business.Application.Extensions
{
    public static class FunctionalExtensions
    {
        public static async Task<TOut> PipeNullCheckAsync<TIn, TOut>(this Task<TIn>? @this,
            Func<TIn, Task<TOut>> functionIfNotNull,
            Func<TIn, Task<TOut>> functionIfNull)
        {
            var result = await @this;
            if (result is not null)
                return await functionIfNotNull(result);
            else
                return await functionIfNull(result);
        }
    }
}