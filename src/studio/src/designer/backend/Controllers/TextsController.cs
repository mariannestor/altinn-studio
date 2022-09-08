using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Altinn.Studio.Designer.Helpers;
using Altinn.Studio.Designer.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Altinn.Studio.Designer.Controllers
{
    /// <summary>
    /// Controller containing actions related to text files in the
    /// new format; text.*.json with key:value pairs.
    /// </summary>
    /// <remarks>
    /// NB: Must not be confused with TextController (singular)
    /// which interacts with the text files in the old format.
    /// </remarks>
    [Authorize]
    [AutoValidateAntiforgeryToken]
    [Route("designer/api/v2/{org}/{repo}/texts")]
    public class TextsController : ControllerBase
    {
        private readonly ITextsService _textsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextsController"/> class.
        /// </summary>
        /// <param name="textsService">The texts service.</param>
        public TextsController(ITextsService textsService)
        {
            _textsService = textsService;
        }

        /// <summary>
        /// Endpoint for editing text file for a specific language.
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the app.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="languageCode">Language identifier specifying the text file to read.</param>
        /// <param name="jsonText">New content from request body to be added to the text file.</param>
        /// <remarks>If duplicates of keys are tried added the
        /// deserialization to dictionary will overwrite the first
        /// key:value pair with the last key:value pair</remarks>
        [HttpPut]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{languageCode}")]
        public async Task<ActionResult> Put(string org, string repo, string languageCode, [FromBody] Dictionary<string, string> jsonText)
        {
            string developer = AuthenticationHelper.GetDeveloperUserName(HttpContext);

            await _textsService.EditText(org, repo, developer, languageCode, jsonText);

            return new JsonResult(new
            {
                Success = true,
                Message = "Texts are updated."
            });
        }

        /// <summary>
        /// Endpoint for getting the complete text file for a specific language.
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the app.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="languageCode">Language identifier specifying the text file to read.</param>
        /// <returns>Text file</returns>
        /// <remarks>If duplicates of keys are tried added the
        /// deserialization to dictionary will overwrite the first
        /// key:value pair with the last key:value pair</remarks>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("{languageCode}")]
        public async Task<ActionResult<Dictionary<string, string>>> Get(string org, string repo, [FromRoute] string languageCode)
        {
            string developer = AuthenticationHelper.GetDeveloperUserName(HttpContext);

            Dictionary<string, string> text = await _textsService.GetText(org, repo, developer, languageCode);

            return Ok(text);
        }
    }
}
