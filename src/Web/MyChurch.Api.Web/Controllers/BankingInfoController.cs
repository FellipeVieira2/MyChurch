using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Church.Commands.CreateBankingInfo;
using MyChurch.Application.Church.Commands.DeleteBankingInfo;
using MyChurch.Application.Church.Commands.SetDefaultBankingInfo;
using MyChurch.Application.Church.Commands.UpdateBankingInfo;
using MyChurch.Application.Church.Queries.GetChurchBankingInfos;
using MyChurch.Application.Dtos;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/banking-info")]
    public class BankingInfoController : BaseController
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BankingInfoDto>))]
        public async Task<IActionResult> GetAll()
        {
            var q = AuthorizationRequestCreate<GetChurchBankingInfosQuery>();
            var result = await Mediator.Send(q);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankingInfoDto))]
        public async Task<IActionResult> Create([FromBody] CreateBankingInfoCommand command)
        {
            var cmd = AuthorizationRequestCreate<CreateBankingInfoCommand>();
            cmd.Nickname = command.Nickname;
            cmd.BankName = command.BankName;
            cmd.BankCode = command.BankCode;
            cmd.Agency = command.Agency;
            cmd.Account = command.Account;
            cmd.AccountDigit = command.AccountDigit;
            cmd.AccountType = command.AccountType;
            cmd.HolderName = command.HolderName;
            cmd.HolderDocument = command.HolderDocument;
            cmd.PixKey = command.PixKey;
            cmd.PixKeyType = command.PixKeyType;

            var result = await Mediator.Send(cmd);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankingInfoDto))]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBankingInfoV2Command body)
        {
            var cmd = AuthorizationRequestCreate<UpdateBankingInfoV2Command>();
            cmd.BankingInfoId = id;
            cmd.Nickname = body.Nickname;
            cmd.BankName = body.BankName;
            cmd.BankCode = body.BankCode;
            cmd.Agency = body.Agency;
            cmd.Account = body.Account;
            cmd.AccountDigit = body.AccountDigit;
            cmd.AccountType = body.AccountType;
            cmd.HolderName = body.HolderName;
            cmd.HolderDocument = body.HolderDocument;
            cmd.PixKey = body.PixKey;
            cmd.PixKeyType = body.PixKeyType;

            var result = await Mediator.Send(cmd);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var cmd = AuthorizationRequestCreate<DeleteBankingInfoCommand>();
            cmd.BankingInfoId = id;
            await Mediator.Send(cmd);
            return NoContent();
        }

        [HttpPut("default")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetDefault([FromBody] SetDefaultBankingInfoRequest body)
        {
            var cmd = AuthorizationRequestCreate<SetDefaultBankingInfoCommand>();
            cmd.BankingInfoId = body.BankingInfoId;

            var ok = await Mediator.Send(cmd);
            return Ok(new { success = ok });
        }
    }

    public class SetDefaultBankingInfoRequest
    {
        public int? BankingInfoId { get; set; }
    }
}
