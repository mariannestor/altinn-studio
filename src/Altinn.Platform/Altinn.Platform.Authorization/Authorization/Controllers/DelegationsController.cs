using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Constants;
using Altinn.Platform.Authorization.Helpers;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Altinn.Platform.Authorization.Controllers
{
    /// <summary>
    /// Controller responsible for all operations for managing delegations of Altinn Apps
    /// </summary>
    [ApiController]
    public class DelegationsController : ControllerBase
    {
        private readonly IPolicyAdministrationPoint _pap;
        private readonly Services.Interface.IPolicyInformationPoint _pip;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegationsController"/> class.
        /// </summary>
        /// <param name="policyAdministrationPoint">The policy administration point</param>
        /// <param name="policyInformationPoint">The policy information point</param>
        /// <param name="logger">the logger.</param>
        public DelegationsController(IPolicyAdministrationPoint policyAdministrationPoint, IPolicyInformationPoint policyInformationPoint, ILogger<DelegationsController> logger)
        {
            _pap = policyAdministrationPoint;
            _pip = policyInformationPoint;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint for adding one or more rules for the given app/offeredby/coveredby. This updates or creates a new delegated policy of type "DirectlyDelegated". DelegatedByUserId is included to store history information in 3.0.
        /// </summary>
        /// <param name="rules">All rules to be delegated</param>
        /// <response code="201" cref="List{Rule}">Created</response>
        /// <response code="206" cref="List{Rule}">Partial Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Authorize(Policy = AuthzConstants.ALTINNII_AUTHORIZATION)]
        [Route("authorization/api/v1/[controller]/AddRules")]
        public async Task<ActionResult> Post([FromBody] List<Rule> rules)
        {
            if (rules == null || rules.Count < 1)
            {
                return BadRequest("Missing rules in body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }

            try
            {
                List<Rule> delegationResults = await _pap.TryWriteDelegationPolicyRules(rules);

                if (delegationResults.All(r => r.CreatedSuccessfully))
                {
                    return Created("Created", delegationResults);
                }

                if (delegationResults.Any(r => r.CreatedSuccessfully))
                {
                    return StatusCode(206, delegationResults);
                }

                string rulesJson = JsonSerializer.Serialize(rules);
                _logger.LogError("Delegation could not be completed. None of the rules could be processed, indicating invalid or incomplete input:\n{rulesJson}", rulesJson);
                return StatusCode(400, $"Delegation could not be completed");
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to store delegation rules in database. {e}");
                return StatusCode(500, $"Unable to store delegation rules in database. {e}");
            }
        }

        /// <summary>
        /// Endpoint for retrieving delegated rules between parties
        /// </summary>
        [HttpPost]
        [Route("authorization/api/v1/[controller]/GetRules")]
        public async Task<ActionResult<List<Rule>>> GetRules([FromBody] RuleQuery ruleQuery, [FromQuery] bool onlyDirectDelegations = false)
        {
            var ruleMatches = ruleQuery.RuleMatch;
            List<int> coveredByPartyIds = new List<int>();
            List<int> coveredByUserIds = new List<int>();
            List<int> offeredByPartyIds = new List<int>();
            List<string> orgApps = new List<string>();

            foreach (List<AttributeMatch> resource in ruleQuery.RuleMatch.Resources)
            {
                string org = resource.FirstOrDefault(match => match.Id == XacmlRequestAttribute.OrgAttribute)?.Value;
                string app = resource.FirstOrDefault(match => match.Id == XacmlRequestAttribute.AppAttribute)?.Value;
                if (!string.IsNullOrEmpty(org) && !string.IsNullOrEmpty(app))
                {
                    orgApps.Add($"{org}/{app}");
                }
            }

            string coveredByUserId = string.Empty;
            if (ruleQuery.RuleMatch.CoveredBy.Id == XacmlRequestAttribute.PartyAttribute)
            {
                coveredByPartyIds.Add(int.Parse(ruleQuery.RuleMatch.CoveredBy.Value));
            }
            else if (ruleQuery.RuleMatch.CoveredBy.Id == XacmlRequestAttribute.UserAttribute)
            {
                coveredByUserIds.Add(int.Parse(ruleQuery.RuleMatch.CoveredBy.Value));
            }

            if (ruleQuery.KeyRolePartyIds.Any(id => id != 0))
            {
                coveredByPartyIds.AddRange(ruleQuery.KeyRolePartyIds);
            }

            if (ruleQuery.ParentPartyId != 0)
            {
                offeredByPartyIds.Add(ruleQuery.ParentPartyId);
            }

            if (ruleQuery.RuleMatch.OfferedByPartyId != 0)
            {
                offeredByPartyIds.Add(ruleQuery.RuleMatch.OfferedByPartyId);
            }

            try
            {
                return Ok(await _pip.GetRulesAsync(orgApps, offeredByPartyIds, coveredByPartyIds, coveredByUserIds));
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to get rules. {e}");
                return StatusCode(500, $"Unable to get rules. {e}");
            }
        }

        /// <summary>
        /// Endpoint for deleting delegated rules between parties
        /// </summary>
        [HttpPost]
        [Route("authorization/api/v1/[controller]/DeleteRules")]
        public async Task<ActionResult> DeleteRules([FromBody] List<string> ruleIds)
        {
            try
            {
                return StatusCode(404, "Not yet implemented");
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to delete rules. {e}");
                return StatusCode(500, $"Unable to delete rules. {e}");
            }
        }

        /// <summary>
        /// Endpoint for deleting an entire delegated policy between parties
        /// </summary>
        [HttpPost]
        [Route("authorization/api/v1/[controller]/DeletePolicy")]
        public async Task<ActionResult> DeletePolicy([FromBody] RuleMatch policyMatch)
        {
            try
            {
                return StatusCode(404, "Not yet implemented");
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to delete delegated policy. {e}");
                return StatusCode(500, $"Unable to delete delegated policy. {e}");
            }
        }

        /// <summary>
        /// Endpoint for deleting delegated rules between parties
        /// </summary>
        /// <response code="200" cref="List{Rule}">Deleted</response>
        /// <response code="206" cref="List{Rule}">Partial Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Authorize(Policy = AuthzConstants.ALTINNII_AUTHORIZATION)]
        [Route("authorization/api/v1/[controller]/DeleteRules")]
        public async Task<ActionResult> DeleteRule([FromBody] RequestToDeleteRuleList rulesToDelete)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Rule> deletionResults = await _pap.TryDeleteDelegationPolicyRules(rulesToDelete);
            int ruleCountToDelete = DelegationHelper.GetRulesCountToDeleteFromRequestToDelete(rulesToDelete);

            if (deletionResults.Count == ruleCountToDelete)
            {
                _logger.LogInformation("Deletion completed");
                return StatusCode(200, deletionResults);
            }

            if (deletionResults.Count > 0)
            {
                _logger.LogInformation("Partial deletion completed");
                return StatusCode(206, deletionResults);
            }

            _logger.LogInformation("Deletion could not be completed");
            return StatusCode(500, $"Unable to complete deletion");
        }

        /// <summary>
        /// Endpoint for deleting an entire delegated policy between parties
        /// </summary>
        /// <response code="200" cref="List{Rule}">Deleted</response>
        /// <response code="206" cref="List{Rule}">Partial Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Authorize(Policy = AuthzConstants.ALTINNII_AUTHORIZATION)]
        [Route("authorization/api/v1/[controller]/DeletePolicy")]
        public async Task<ActionResult> DeletePolicy([FromBody] RequestToDeletePolicyList policiesToDelete)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                        
            List<Rule> deletionResults = await _pap.TryDeleteDelegationPolicies(policiesToDelete);
            int countPolicies = DelegationHelper.GetPolicyCount(deletionResults);

            if (countPolicies == policiesToDelete.Count)
            {
                return StatusCode(200, deletionResults);
            }

            if (countPolicies > 0)
            {
                _logger.LogInformation("Partial deletion completed");
                return StatusCode(206, deletionResults);
            }

            _logger.LogInformation("Deletion could not be completed all policies failed", policiesToDelete);
            return StatusCode(500, $"Unable to complete deletion");            
        }

        /// <summary>
        /// Test method. Should be deleted?
        /// </summary>
        /// <returns>test string</returns>
        [HttpGet]
        [Route("authorization/api/v1/[controller]")]
        public string Get()
        {
            return "Hello world!";
        }
    }
}
