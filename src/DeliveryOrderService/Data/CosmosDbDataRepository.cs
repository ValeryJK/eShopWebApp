using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DeliveryOrderService.Configuration.Interfaces;
using DeliveryOrderService.Data.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace DeliveryOrderService.Data;
public abstract class CosmosDbDataRepository<T> : IDataRepository<T> where T : BaseEntity
{
    protected readonly ICosmosDbConfiguration _cosmosDbConfiguration;
    protected readonly CosmosClient _client;
    private readonly ILogger<CosmosDbDataRepository<T>> _logger;

    public abstract string ContainerName { get; }

    public CosmosDbDataRepository(ICosmosDbConfiguration cosmosDbConfiguration,
        CosmosClient client, ILogger<CosmosDbDataRepository<T>> logger)
    {
        _cosmosDbConfiguration = cosmosDbConfiguration ?? throw new ArgumentNullException(nameof(cosmosDbConfiguration));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T> AddAsync(T newEntity)
    {
        try
        {
            Container container = GetContainer();
            ItemResponse<T> createResponse = await container.CreateItemAsync(newEntity);

            return createResponse;
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex.Message);

            if (ex.StatusCode != HttpStatusCode.NotFound)
            {
                throw;
            }

            return default;
        }
    }

    public async Task DeleteAsync(string entityId)
    {
        try
        {
            Container container = GetContainer();

            await container.DeleteItemAsync<T>(entityId, new PartitionKey(entityId));
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex.Message);

            if (ex.StatusCode != HttpStatusCode.NotFound)
            {
                throw;
            }
        }
    }

    public async Task<T> GetAsync(string entityId)
    {
        try
        {
            Container container = GetContainer();

            ItemResponse<T> entityResult = await container.ReadItemAsync<T>(entityId, new PartitionKey(entityId));

            return entityResult;
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex.Message);

            if (ex.StatusCode != HttpStatusCode.NotFound)
            {
                throw;
            }

            return default;
        }
    }

    public async Task<T> UpdateAsync(T entity)
    {
        try
        {
            Container container = GetContainer();

            ItemResponse<BaseEntity> entityResult = await container.ReadItemAsync<BaseEntity>(entity.Id.ToString(), new PartitionKey(entity.Id.ToString()));

            if (entityResult != null)
            {
                await container.ReplaceItemAsync(entity, entity.Id.ToString(), new PartitionKey(entity.Id.ToString()));
            }
            return entity;
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex.Message);

            if (ex.StatusCode != HttpStatusCode.NotFound)
            {
                throw;
            }

            return default;
        }
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        try
        {
            Container container = GetContainer();

            IOrderedQueryable<T> queryable = container.GetItemLinqQueryable<T>();
            using FeedIterator<T> linqFeed = queryable.ToFeedIterator();

            var entities = new List<T>();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<T> response = await linqFeed.ReadNextAsync();
                foreach (T item in response)
                {
                    entities.Add(item);
                }
            }

            return entities;

        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex.Message);

            if (ex.StatusCode != HttpStatusCode.NotFound)
            {
                throw;
            }

            return default;
        }
    }

    protected Container GetContainer()
    {
        var database = _client.GetDatabase(_cosmosDbConfiguration.DatabaseName);
        var container = database.GetContainer(ContainerName);
        return container;
    }
}
