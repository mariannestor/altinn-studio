using System;
using System.Collections.Generic;
using System.Linq;
using Altinn.Authorization.ABAC.Constants;
using Altinn.Platform.Authorization.Constants;
using Altinn.Platform.Authorization.Models;

namespace Altinn.Platform.Authorization.IntegrationTests.Data
{
    public static class TestDataHelper
    {
        public static ResourceAction GetResourceActionModel(string action, IEnumerable<string> roles)
        {
            ResourceAction resourceAction = new ResourceAction // legg til Title
            {
                Match = new AttributeMatch { Id = XacmlConstants.MatchAttributeIdentifiers.ActionId, Value = action },
                RoleGrants = new List<RoleGrant>(),
                Title = new LocalizedText(action, action, action)
            };

            foreach (string role in roles)
            {
                resourceAction.RoleGrants.Add(new RoleGrant { IsDelegable = true, RoleTypeCode = role });
            }

            return resourceAction;
        }

        public static ResourcePolicy GetResourcePolicyModel(string org, string app, string task = null, string endEvent = null)
        {
            string title = string.Empty;
            ResourcePolicy policy = new ResourcePolicy
            {
                Resource = new List<AttributeMatch>
                {
                    new AttributeMatch { Id = XacmlRequestAttribute.OrgAttribute, Value = org },
                    new AttributeMatch { Id = XacmlRequestAttribute.AppAttribute, Value = app }
                }
            };

            if (task != null)
            {
                policy.Resource.Add(new AttributeMatch { Id = XacmlRequestAttribute.TaskAttribute, Value = task });
                title = $"{org}/{app}/{task}";
            }

            if (endEvent != null)
            {
                policy.Resource.Add(new AttributeMatch { Id = XacmlRequestAttribute.EndEventAttribute, Value = endEvent });
                title = $"{org}/{app}/{endEvent}";
            }

            policy.Title = new LocalizedText(title, title, title);
            return policy;
        }
    }
}
