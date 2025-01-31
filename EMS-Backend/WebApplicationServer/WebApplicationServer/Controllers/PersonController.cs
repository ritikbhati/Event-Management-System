using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplicationServer.Models;
using Microsoft.Extensions.Configuration;
using WebApplicationServer.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using WebApplicationServer.Services.IService;
using WebApplicationServer.Services;
using WebApplicationServer.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Azure;

namespace WebApplicationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private string connectionString = "Server=tcp:ems-server.database.windows.net,1433;Initial Catalog=emsdatabase;Persist Security Info=False;User ID=ajaykarode;Password=Emspassword@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly IGetAllPerson _getAllPerson;

        public PersonController(IGetAllPerson getAllPerson)
        {
            _getAllPerson = getAllPerson;
        }

        [HttpGet, Authorize]
        public async Task<GetAllPersonResponseViewModel> GetAllPersons()
        {
            var persons = await _getAllPerson.GetAllPersons();
            return persons;
        }

        [HttpGet("{Id}")]
        public async Task<GetPersonByIdResponseViewModel> GetPersonById(string Id)
        {
            var person = await _getAllPerson.GetPersonById(Id);
            return person;

        }



        //THIS API IS ONLY FOR ADMIN
        [HttpGet("getpersonbyrole/{role}")]
        public async Task<GetAllPersonByAdminResponseViewModel> GetPersonByRole(string role)
        {
            GetAllPersonByAdminResponseViewModel response = new GetAllPersonByAdminResponseViewModel();
            response.Status = 200;
            response.Message = "All Person Fetched Successfully";
            response.AllPersons = (await _getAllPerson.GetPersonByRole(role)).ToList();
            return response;
        }

        [HttpDelete("{Id}")]
        public async Task<ResponseViewModel> DeletePerson(string Id)
        {
            ResponseViewModel response;
            response = await _getAllPerson.DeletePerson(Id);
            return response;
        }


        [HttpPut("updatePerson/{id}")]
        public async Task<ResponseViewModel> UpdatePerson(string id, UpdatePersonViewModel updatePerson)
        {
            ResponseViewModel response;

            if (!ModelState.IsValid)
            {
                response = new ResponseViewModel();
                response.Status = 422;
                response.Message = "Please provide valid Person details.";
                return response;
            }

            response = await _getAllPerson.UpdatePerson(id, updatePerson);
            return response;
        }


        [HttpPost("blockperson/{personId}")]
        public async Task<ResponseViewModel> BlockPerson(string personId)
        {
            ResponseViewModel response = new ResponseViewModel();
            response = await _getAllPerson.BlockPerson(personId);
            response.Status = 200;
            response.Message = "Person Blocked Successfully";
            return response;

        }


        [HttpPost("unblockperson/{personId}")]
        public async Task<ResponseViewModel> UnblockPerson(string personId)
        {
            ResponseViewModel response = new ResponseViewModel();

            response = await _getAllPerson.UnBlockPerson(personId);
            response.Status = 200;
            response.Message = "Person UnBlocked Successfully";
            return response;

        }

    }
}



