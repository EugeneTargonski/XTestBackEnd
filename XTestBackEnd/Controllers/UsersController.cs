using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.Extensions.Logging;

namespace XTestBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IRepository<User> _userRepository;
        public UsersController(IRepository<User> userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _userRepository.GetList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public User Get(int id)
        {
            return _userRepository.GetById(id);
        }

        // POST: api/Users
        [HttpPost]
        public void Post([FromBody] User value)
        {
            _userRepository.Create(value);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public void Put([FromBody] User value)
        {
            _userRepository.Create(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _userRepository.Delete(id);
        }
    }
}
