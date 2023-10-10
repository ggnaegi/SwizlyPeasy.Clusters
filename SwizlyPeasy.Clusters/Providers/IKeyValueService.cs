namespace SwizlyPeasy.Clusters.Providers
{
    public interface IKeyValueService
    {
        /// <summary>
        ///     Saving data in provider key value store
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task SaveToKeyValueStore(string key, byte[] value);

        /// <summary>
        ///     Retrieving raw data referenced by key from provider
        ///     key value store
        /// </summary>
        /// <param name="key"></param>
        /// <returns>raw data as byte array</returns>
        public Task<byte[]> GetFromKeyValueStore(string key);
    }
}
