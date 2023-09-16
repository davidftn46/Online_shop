using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition.Constants
{
    public enum Feedback
    {
        OK = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        InternalServerError = 500,
        InvalidEmail = 1000,
        InvalidUsername = 1001,
        AccountNotActivated = 1002,
        InvalidPasswordGuid = 1003,
        AccountAlreadyActivated = 1004,
        AllUsersVerified = 1005,
    }
}
