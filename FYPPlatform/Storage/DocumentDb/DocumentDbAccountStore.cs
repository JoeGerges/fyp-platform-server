using FYPPlatform.DataContracts;
using FYPPlatform.Web.Exceptions;
using FYPPlatform.Web.Storage.DocumentDb;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYPPlatform.Web.Storage
{
    public class DocumentDbAccountStore : IAccountStore
    {
        private readonly IDocumentClient _documentClient;
        private readonly IOptions<DocumentDbSettings> _options;
        private Uri DocumentCollectionUri => UriFactory.CreateDocumentCollectionUri(_options.Value.DatabaseName, _options.Value.CollectionName);
        public DocumentDbAccountStore(IDocumentClient documentClient, IOptions<DocumentDbSettings> options)
        {
            _documentClient = documentClient;
            _options = options;
        }

     
        public async Task AddAccount(Account account)
        {
            try
            {
                var entity = ToEntity(account, "email sent");
                await _documentClient.CreateDocumentAsync(DocumentCollectionUri, entity);
            }
            catch (DocumentClientException e)
            {
                Console.WriteLine(e.StackTrace);
                throw new StorageErrorException($"Failed to add account with email {account.Email} to FYP of organization {account.Organization}", (int) e.StatusCode);
            }
        }

      
        public async Task DeleteAccount(string email)
        {
            try
            {
                await _documentClient.DeleteDocumentAsync(CreateDocumentUri(email),
                    new RequestOptions { PartitionKey = new PartitionKey("FYP") });
            }
            catch (DocumentClientException e)
            {
                throw new StorageErrorException($"Failed to delete account with email {email} from FYP", e, (int)e.StatusCode);
            }
        }

    
        public async Task<Account> GetAccount(string email)
        {
            try
            {
                var entity = await _documentClient.ReadDocumentAsync<DocumentDbAccountEntity>(
                    CreateDocumentUri(email),
                    new RequestOptions { PartitionKey = new PartitionKey("FYP") });
                return ToAccount(entity);
            }
            catch(DocumentClientException e)
            {
                throw new StorageErrorException($"Failed to fetch account with email {email}", (int)e.StatusCode);
            }
        }

        public async Task<GetAccountsResult> GetAccounts(string continuationToken, int limit, string role)
        {
            try
            {
                var feedOptions = new FeedOptions
                {
                    MaxItemCount = limit,
                    EnableCrossPartitionQuery = false,
                    RequestContinuation = continuationToken,
                    PartitionKey = new PartitionKey("FYP")
                };

                IQueryable<DocumentDbAccountEntity> query = _documentClient.CreateDocumentQuery<DocumentDbAccountEntity>(DocumentCollectionUri, feedOptions);

                if(!string.IsNullOrWhiteSpace(role))
                {
                    query.Where(x => x.Role.Equals(role));
                }

                FeedResponse<DocumentDbAccountEntity> feedResponse = await query.AsDocumentQuery().ExecuteNextAsync<DocumentDbAccountEntity>();
                return new GetAccountsResult
                {
                    ContinuationToken = feedResponse.ResponseContinuation,
                    Accounts = feedResponse.Select(ToAccount).ToList()
                };
            }
            catch (DocumentClientException e)
            {
                throw new StorageErrorException($"Failed to list accounts", e, (int)e.StatusCode);
            }
        }


        private Account ToAccount(DocumentDbAccountEntity entity)
        {
            return new Account
            {
                Name = entity.Name,
                Email = entity.Email,
                Organization = entity.Organization,
                Role = entity.Role,
                Skills = entity.Skills,
                Interests = entity.Interests,
                State = entity.State,
                Password = entity.Password
            };
        }

        
        public async Task UpdateAccount(Account account)
        {
            try
            {
                var entity = ToEntity(account, "account created");
                await _documentClient.UpsertDocumentAsync(DocumentCollectionUri, entity, new RequestOptions { PartitionKey = new PartitionKey("FYP") });
            }
            catch (DocumentClientException e)
            {
                throw new StorageErrorException($"Failed to update account with email {account.Email}", (int)e.StatusCode);
            }
        }


        private static DocumentDbAccountEntity ToEntity(Account account, string state)
        {
            if (string.IsNullOrWhiteSpace(account.Email))
            {
                throw new BadRequestException("Email cannot be empty");
            }

            return new DocumentDbAccountEntity
            {
                PartitionKey = "FYP",
                Email = account.Email,
                Name = account.Name,
                Organization = account.Organization,
                Role = account.Role,
                Password = account.Password,
                Skills = account.Skills,
                Interests = account.Interests,
                State = state
            };
        }

        private Uri CreateDocumentUri(string email)
        {
            return UriFactory.CreateDocumentUri(_options.Value.DatabaseName, _options.Value.CollectionName, email);
        }
    }
}
