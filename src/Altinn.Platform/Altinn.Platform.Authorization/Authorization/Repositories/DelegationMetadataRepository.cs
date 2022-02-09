using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.Extensions;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Repositories.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Altinn.Platform.Authorization.Repositories
{
    /// <summary>
    /// Repository implementation for PostgreSQL operations on delegations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DelegationMetadataRepository : IDelegationMetadataRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly string insertDelegationChangeSql = "call delegation.insert_change(@_altinnAppId, @_offeredByPartyId, @_coveredByUserId, @_coveredByPartyId, @_performedByUserId, @_blobStoragePolicyPath, @_blobStorageVersionId, @_isDeleted, @_delegationChangeId)";
        private readonly string getCurrentDelegationChangeSql = "select * from delegation.get_current_change(@_altinnAppId, @_offeredByPartyId, @_coveredByUserId, @_coveredByPartyId)";
        private readonly string getAllDelegationChangesSql = "select * from delegation.get_all_changes(@_altinnAppId, @_offeredByPartyId, @_coveredByUserId, @_coveredByPartyId)";
        private readonly string getAllCurrentDelegationChangesPartyIdsSql = "select * from delegation.get_all_current_changes_coveredbypartyids(@_altinnAppIds, @_offeredByPartyIds, @_coveredByPartyIds)";
        private readonly string getAllCurrentDelegationChangesUserIdsSql = "select * from delegation.get_all_current_changes_coveredbyuserids(@_altinnAppIds, @_offeredByPartyIds, @_coveredByUserIds)";
        private readonly string getAllCurrentDelegationChangesOnlyOfferedBysSql = "select * from delegation.get_all_current_changes(@_altinnAppIds, @_offeredByPartyIds)";

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegationMetadataRepository"/> class
        /// </summary>
        /// <param name="postgresSettings">The postgreSQL configurations for AuthorizationDB</param>
        /// <param name="logger">logger</param>
        public DelegationMetadataRepository(
            IOptions<PostgreSQLSettings> postgresSettings,
            ILogger<DelegationMetadataRepository> logger)
        {
            _logger = logger;
            _connectionString = string.Format(
                postgresSettings.Value.ConnectionString,
                postgresSettings.Value.AuthorizationDbPwd);
        }

        /// <inheritdoc/>
        public async Task<bool> InsertDelegation(string altinnAppId, int offeredByPartyId, int? coveredByPartyId, int? coveredByUserId, int delegatedByUserId, string blobStoragePolicyPath, string blobStorageVersionId, bool isDeleted = false)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(insertDelegationChangeSql, conn);
                pgcom.Parameters.AddWithValue("_altinnAppId", altinnAppId);
                pgcom.Parameters.AddWithValue("_offeredByPartyId", offeredByPartyId);
                pgcom.Parameters.AddWithValue("_coveredByUserId", coveredByUserId.HasValue ? coveredByUserId.Value : DBNull.Value);
                pgcom.Parameters.AddWithValue("_coveredByPartyId", coveredByPartyId.HasValue ? coveredByPartyId.Value : DBNull.Value);
                pgcom.Parameters.AddWithValue("_performedByUserId", delegatedByUserId);
                pgcom.Parameters.AddWithValue("_blobStoragePolicyPath", blobStoragePolicyPath);
                pgcom.Parameters.AddWithValue("_blobStorageVersionId", blobStorageVersionId);
                pgcom.Parameters.AddWithValue("_isDeleted", isDeleted);

                pgcom.Parameters.AddWithValue("_delegationChangeId", 0); // Must be included since it's specified in stored proc, but so far unable to get inserted value back.

                await pgcom.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Authorization // DelegationMetadataRepository // Insert // Exception");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<DelegationChange> GetCurrentDelegationChange(string altinnAppId, int offeredByPartyId, int? coveredByPartyId, int? coveredByUserId)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(getCurrentDelegationChangeSql, conn);
                pgcom.Parameters.AddWithValue("_altinnAppId", altinnAppId);
                pgcom.Parameters.AddWithValue("_offeredByPartyId", offeredByPartyId);
                pgcom.Parameters.AddWithValue("_coveredByUserId", coveredByUserId.HasValue ? coveredByUserId.Value : DBNull.Value);
                pgcom.Parameters.AddWithValue("_coveredByPartyId", coveredByPartyId.HasValue ? coveredByPartyId.Value : DBNull.Value);

                using NpgsqlDataReader reader = pgcom.ExecuteReader();
                if (reader.Read())
                {
                    return GetDelegationChange(reader);
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Authorization // DelegationMetadataRepository // GetCurrentDelegationChange // Exception");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<List<DelegationChange>> GetAllDelegationChanges(string altinnAppId, int offeredByPartyId, int? coveredByPartyId, int? coveredByUserId)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(getAllDelegationChangesSql, conn);
                pgcom.Parameters.AddWithValue("_altinnAppId", altinnAppId);
                pgcom.Parameters.AddWithValue("_offeredByPartyId", offeredByPartyId);
                pgcom.Parameters.AddWithValue("_coveredByUserId", coveredByUserId.HasValue ? coveredByUserId.Value : DBNull.Value);
                pgcom.Parameters.AddWithValue("_coveredByPartyId", coveredByPartyId.HasValue ? coveredByPartyId.Value : DBNull.Value);

                List<DelegationChange> delegationChanges = new List<DelegationChange>();

                using NpgsqlDataReader reader = pgcom.ExecuteReader();
                while (reader.Read())
                {
                    delegationChanges.Add(GetDelegationChange(reader));
                }

                return delegationChanges;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Authorization // DelegationMetadataRepository // GetAllDelegationChanges // Exception");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<List<DelegationChange>> GetAllCurrentDelegationChanges(List<string> altinnAppIds = null, List<int> offeredByPartyIds = null, List<int> coveredByPartyIds = null, List<int> coveredByUserIds = null)
        {
            List<DelegationChange> delegationChanges = new List<DelegationChange>();
            if ((coveredByPartyIds == null && coveredByUserIds == null) || (coveredByPartyIds?.Count > 0 && coveredByUserIds?.Count > 0))
            {
                delegationChanges.AddRange(await GetAllCurrentDelegationChangesOfferedByPartyIdOnly(altinnAppIds, offeredByPartyIds));
            }
            else
            {
                if (coveredByPartyIds?.Count > 0)
                {
                    delegationChanges.AddRange(await GetAllCurrentDelegationChangesCoveredByPartyIds(altinnAppIds, offeredByPartyIds, coveredByPartyIds));
                }

                if (coveredByUserIds?.Count > 0)
                {
                    delegationChanges.AddRange(await GetAllCurrentDelegationChangesCoveredByUserIds(altinnAppIds, offeredByPartyIds, coveredByUserIds));
                }
            }

            return delegationChanges;
        }

        private static DelegationChange GetDelegationChange(NpgsqlDataReader reader)
        {
            DelegationChange delegationChange = new DelegationChange();
            delegationChange.PolicyChangeId = reader.GetValue<int>("delegationchangeid");
            delegationChange.AltinnAppId = reader.GetValue<string>("altinnappid");
            delegationChange.OfferedByPartyId = reader.GetValue<int>("offeredbypartyid");
            delegationChange.CoveredByPartyId = reader.GetValue<int>("coveredbypartyid");
            delegationChange.CoveredByUserId = reader.GetValue<int>("coveredbyuserid");
            delegationChange.PerformedByUserId = reader.GetValue<int>("performedbyuserid");
            delegationChange.BlobStoragePolicyPath = reader.GetValue<string>("blobstoragepolicypath");
            delegationChange.BlobStorageVersionId = reader.GetValue<string>("blobstorageversionid");
            delegationChange.IsDeleted = reader.GetValue<bool>("isdeleted");
            delegationChange.Created = reader.GetValue<DateTime>("created");
            return delegationChange;
        }

        private async Task<List<DelegationChange>> GetAllCurrentDelegationChangesCoveredByPartyIds(List<string> altinnAppIds = null, List<int> offeredByPartyIds = null, List<int> coveredByPartyIds = null)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(getAllCurrentDelegationChangesPartyIdsSql, conn);
                pgcom.Parameters.AddWithValue("_altinnAppIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text, altinnAppIds?.Count > 0 ? altinnAppIds : DBNull.Value);
                pgcom.Parameters.AddWithValue("_offeredByPartyIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, offeredByPartyIds?.Count > 0 ? offeredByPartyIds : DBNull.Value);
                pgcom.Parameters.AddWithValue("_coveredByPartyIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, coveredByPartyIds?.Count > 0 ? coveredByPartyIds : DBNull.Value);

                List<DelegationChange> delegationChanges = new List<DelegationChange>();

                using NpgsqlDataReader reader = pgcom.ExecuteReader();
                while (reader.Read())
                {
                    delegationChanges.Add(GetDelegationChange(reader));
                }

                return delegationChanges;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Authorization // DelegationMetadataRepository // GetAllCurrentDelegationChangesCoveredByPartyIds // Exception");
                throw;
            }
        }

        private async Task<List<DelegationChange>> GetAllCurrentDelegationChangesCoveredByUserIds(List<string> altinnAppIds = null, List<int> offeredByPartyIds = null, List<int> coveredByUserIds = null)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(getAllCurrentDelegationChangesUserIdsSql, conn);
                pgcom.Parameters.AddWithValue("_altinnAppIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text, altinnAppIds?.Count > 0 ? altinnAppIds : DBNull.Value);
                pgcom.Parameters.AddWithValue("_offeredByPartyIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, offeredByPartyIds?.Count > 0 ? offeredByPartyIds : DBNull.Value);
                pgcom.Parameters.AddWithValue("_coveredByUserIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, coveredByUserIds?.Count > 0 ? coveredByUserIds : DBNull.Value);

                List<DelegationChange> delegationChanges = new List<DelegationChange>();

                using NpgsqlDataReader reader = pgcom.ExecuteReader();
                while (reader.Read())
                {
                    delegationChanges.Add(GetDelegationChange(reader));
                }

                return delegationChanges;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Authorization // DelegationMetadataRepository // GetAllCurrentDelegationChangesCoveredByUserIds // Exception");
                throw;
            }
        }

        private async Task<List<DelegationChange>> GetAllCurrentDelegationChangesOfferedByPartyIdOnly(List<string> altinnAppIds = null, List<int> offeredByPartyIds = null)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(getAllCurrentDelegationChangesOnlyOfferedBysSql, conn);
                pgcom.Parameters.AddWithValue("_altinnAppIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text, altinnAppIds?.Count > 0 ? altinnAppIds : DBNull.Value);
                pgcom.Parameters.AddWithValue("_offeredByPartyIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, offeredByPartyIds?.Count > 0 ? offeredByPartyIds : DBNull.Value);

                List<DelegationChange> delegationChanges = new List<DelegationChange>();

                using NpgsqlDataReader reader = pgcom.ExecuteReader();
                while (reader.Read())
                {
                    delegationChanges.Add(GetDelegationChange(reader));
                }

                return delegationChanges;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Authorization // DelegationMetadataRepository // GetAllCurrentDelegationChangesOfferedByPartyIdOnly // Exception");
                throw;
            }
        }
    }
}
