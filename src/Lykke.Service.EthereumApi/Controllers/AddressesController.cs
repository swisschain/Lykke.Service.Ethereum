﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.EthereumApi.Core.Domain;
using Lykke.Service.EthereumApi.Models;
using Lykke.Service.EthereumApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace Lykke.Service.EthereumApi.Controllers
{
    [PublicAPI, Route("/api/addresses")]
    public class AddressesController : Controller
    {
        private readonly IAddressService _addressService;

        public AddressesController(
            IAddressService addressService)
        {
            _addressService = addressService;
        }


        [HttpGet("{address}/validity")]
        public async Task<ActionResult<AddressValidationResponse>> GetAddressValidity(
            string address)
        {
            return new AddressValidationResponse
            {
                IsValid = await _addressService.ValidateAsync(address)
            };
        }

        [HttpPost("blacklist/{address}")]
        public async Task<IActionResult> AddAddressToBlacklist(
            AddressRequest request)
        {
            var result = await _addressService.AddAddressToBlacklistAsync
            (
                address: request.Address,
                reason: "Blacklisted via API."
            );

            switch (result)
            {
                case AddAddressResult.SuccessResult _:
                    return Ok();
                
                case AddAddressResult.HasAlreadyBeenAddedError _:
                    return Conflict();
                
                default:
                    throw new NotSupportedException(
                        $"{nameof(_addressService.AddAddressToBlacklistAsync)} returned unsupported result.");
            }
        }
        
        [HttpPost("whitelist/{address}")]
        public async Task<IActionResult> AddAddressToWhitelist(
            AddressRequest request)
        {
            var result = await _addressService.AddAddressToWhitelistAsync(request.Address);
            
            switch (result)
            {
                case AddAddressResult.SuccessResult _:
                    return Ok();
                
                case AddAddressResult.HasAlreadyBeenAddedError _:
                    return Conflict();
                
                default:
                    throw new NotSupportedException(
                        $"{nameof(_addressService.AddAddressToWhitelistAsync)} returned unsupported result.");
            }
        }

        [HttpGet("blacklist")]
        public async Task<ActionResult<PaginationResponse<BlacklistedAddressResponse>>> GetBlacklistedAddresses(
            PaginationRequest request)
        {
            var (addresses, continuationToken) = await _addressService.GetBlacklistedAddressesAsync(request.Take, request.Continuation);
            
            var result = new PaginationResponse<BlacklistedAddressResponse>()
            {
                Continuation = continuationToken,
                Items = addresses.Select(x => new BlacklistedAddressResponse
                {
                    Address = x.Address,
                    BlacklistingReason = x.BlacklistingReason
                }).ToImmutableArray()
            };
            
            return Ok(result);
        }
        
        [HttpGet("blacklist/{address}/reason")]
        public async Task<ActionResult<BlacklistingReasonResponse>> GetBlacklistingReason(
            AddressRequest request)
        {
            var reason = await _addressService.TryGetBlacklistingReason(request.Address);

            if (reason != null)
            {
                return new BlacklistingReasonResponse
                {
                    Reason = reason
                };
            }
            else
            {
                return NoContent();
            }
        }
        
        [HttpGet("whitelist")]
        public async Task<ActionResult<PaginationResponse<WhitelistedAddressResponse>>> GetWhitelistedAddresses(
            PaginationRequest request)
        {
            var (addresses, continuationToken) = await _addressService.GetWhitelistedAddressesAsync(request.Take, request.Continuation);
            
            var result = new PaginationResponse<WhitelistedAddressResponse>()
            {
                Continuation = continuationToken,
                Items = addresses.Select(x => new WhitelistedAddressResponse
                {
                    Address = x
                }).ToImmutableArray()
            };
            
            return Ok(result);
        }
        
        [HttpDelete("blacklist/{address}")]
        public async Task<IActionResult> RemoveAddressFromBlacklist(
            AddressRequest request)
        {
            var result = await _addressService.RemoveAddressFromBlacklistAsync(request.Address);
            
            switch (result)
            {
                case RemoveAddressResult.SuccessResult _:
                    return Ok();
                
                case RemoveAddressResult.NotFoundError _:
                    return NoContent();
                
                default:
                    throw new NotSupportedException(
                        $"{nameof(_addressService.RemoveAddressFromBlacklistAsync)} returned unsupported result.");
            }
        }
        
        [HttpDelete("whitelist/{address}")]
        public async Task<IActionResult> RemoveAddressFromWhitelist(
            AddressRequest request)
        {
            var result = await _addressService.RemoveAddressFromWhitelistAsync(request.Address);
            
            switch (result)
            {
                case RemoveAddressResult.SuccessResult _:
                    return Ok();
                
                case RemoveAddressResult.NotFoundError _:
                    return NoContent();
                
                default:
                    throw new NotSupportedException(
                        $"{nameof(_addressService.RemoveAddressFromWhitelistAsync)} returned unsupported result.");
            }
        }

        #region Not Implemented Endpoints
        
        [HttpGet("{address}/balance")]
        public ActionResult<AddressValidationResponse> GetBalance(
            AddressRequest address)
                => StatusCode(StatusCodes.Status501NotImplemented);
        
        [HttpGet("{address}/explorer-url")]
        public ActionResult<AddressValidationResponse> GetExplorerUrl(
            AddressRequest address)
                => StatusCode(StatusCodes.Status501NotImplemented);
        
        [HttpGet("{address}/underlying")]
        public ActionResult<AddressValidationResponse> GetUnderlyingAddress(
            AddressRequest address)
                => StatusCode(StatusCodes.Status501NotImplemented);
        
        [HttpGet("{address}/virtual")]
        public ActionResult<AddressValidationResponse> GetVirtualAddress(
            AddressRequest address)
                => StatusCode(StatusCodes.Status501NotImplemented);
        
        #endregion
    }
}
