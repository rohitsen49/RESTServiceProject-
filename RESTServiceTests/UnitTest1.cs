using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Newtonsoft.Json.Linq;
using FluentAssertions;

namespace RESTServiceTests
{
    public class Tests
    {
        public class UserObject
        {
            public int userID { get; set; }
            public string userEmail { get; set; }
            public string userPassword { get; set; }
            public DateTime createdDate { get; set; }
        }

        HttpClient client;

        [SetUp]
        public void Setup()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5269/api/");
        }

        [Test]
        public async Task TestGetAll()
        {
            var result = await client.GetAsync("Rest");

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task AddNewUserValid()
        {
            var newUser = new UserObject
            {
                userID = 0,
                userEmail = "testemail@gmail.com",
                userPassword = "password"
            };

            var newJson = JsonSerializer.Serialize(newUser);

            var postContent = new StringContent(newJson,
                Encoding.UTF8, "application/json");

            var postResult = await client.PostAsync("Rest", postContent);

            Assert.AreEqual(HttpStatusCode.Created, postResult.StatusCode);
        }

        [Test]
        public async Task UpdateUserPassword()
        {
            var newUser = new UserObject
            {
                userID = 0,
                userEmail = "testemail@gmail.com",
                userPassword = "password"
            };


            var newJson = JsonSerializer.Serialize(newUser);

            var postContent = new StringContent(newJson,
                Encoding.UTF8, "application/json");

            var postResult = await UpdateUser(newUser, "testemail@gmail.com", "newpassword");

            Assert.AreEqual(HttpStatusCode.Created, postResult.StatusCode);
        }

        [Test]
        public async Task UpdateUserEmail()
        {
            var newUser = new UserObject
            {
                userID = 0,
                userEmail = "testemail@gmail.com",
                userPassword = "password"
            };


            var newJson = JsonSerializer.Serialize(newUser);

            var postContent = new StringContent(newJson,
                Encoding.UTF8, "application/json");

            var postResult = await UpdateUser(newUser, "mynewemail@gmail.com", "newpassword");

            Assert.AreEqual(HttpStatusCode.Created, postResult.StatusCode);
        }

        [Test]
        public async Task AddNewUserInvalidFieldPassword()
        {
            var newUser = new UserObject
            {
                userID = 0,
                userEmail = "megaemail@gmail.com",
                userPassword = ""
            };

            var newJson = JsonSerializer.Serialize(newUser);

            var postContent = new StringContent(newJson,
                Encoding.UTF8, "application/json");

            var postResult = await client.PostAsync("Rest", postContent);

            Assert.AreEqual(HttpStatusCode.BadRequest, postResult.StatusCode);
        }

        [Test]
        public async Task AddNewUserInvalidFieldUserEmail()
        {
            var newUser = new UserObject
            {
                userID = 0,
                userEmail = "",
                userPassword = "Megajohn1954"
            };

            var newJson = JsonSerializer.Serialize(newUser);

            var postContent = new StringContent(newJson,
                Encoding.UTF8, "application/json");

            var postResult = await client.PostAsync("Rest", postContent);

            Assert.AreEqual(HttpStatusCode.BadRequest, postResult.StatusCode);
        }

        [Test]
        public async Task TestGetSpecific_Good()
        {
            var postResult = await CreateUser("testemail@gmail.com", "password");

            var json = await postResult.Content.ReadAsStringAsync();

            var user = JsonSerializer.Deserialize<UserObject>(json);

            var result = await client.GetAsync("Rest/" + user.userID);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task TestGetSpecific_Bad()
        {
            var result = await client.GetAsync("Rest/10211");

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task<HttpResponseMessage> CreateUser(string userEmailInput, string userPasswordInput)
        {
            var newUser = new UserObject
            {
                userID = 0,
                userEmail = userEmailInput,
                userPassword = userPasswordInput
            };

            var newJson = JsonSerializer.Serialize(newUser);
            var postContent = new StringContent(newJson, Encoding.UTF8, "application/json");
            var postResult = await client.PostAsync("Rest", postContent);

            return postResult;
        }

        private async Task<HttpResponseMessage> UpdateUser(UserObject currentUser, string userEmailInput, string userPasswordInput)
        {
            currentUser.userEmail = userEmailInput;
            currentUser.userPassword = userPasswordInput;


            var newJson = JsonSerializer.Serialize(currentUser);
            var postContent = new StringContent(newJson, Encoding.UTF8, "application/json");
            var postResult = await client.PostAsync("Rest", postContent);

            return postResult;
        }
    }
}