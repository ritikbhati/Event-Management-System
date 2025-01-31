﻿using WebApplicationServer.Models.ResponseModels;
using WebApplicationServer.Models.ViewModels;

namespace WebApplicationServer.Services.IService
{
    public interface IGetAllPerson
    {
        public Task<GetAllPersonResponseViewModel> GetAllPersons();
        public Task<GetPersonByIdResponseViewModel> GetPersonById(string id);
        public Task<List<GetAllPersonByAdminViewModel>> GetPersonByRole(string role);
        public Task<ResponseViewModel> DeletePerson(string id);
        public Task<ResponseViewModel> UpdatePerson(string id, UpdatePersonViewModel updatePerson);
        public Task<ResponseViewModel> BlockPerson(string personId);
        public Task<ResponseViewModel> UnBlockPerson(string personId);
    }
}


