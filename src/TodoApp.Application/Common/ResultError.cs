namespace TodoApp.Application.Common
{
    public class ResultError
    {
        /*
         * Error Code Pattern:
         * E + Operation + Module + Id
         * 
         * [ Operation  | Code ]
         * [ Generic   | 00   ]
         * [ Insert    | 10   ]
         * [ Update    | 20   ]
         * [ Get       | 30   ]
         * [ Delete    | 40   ]
         * 
         * [ Module    | Code ]
         * [ Generic   | 00   ]
         * [ Health    | 01   ]
         * [ TodoItem    | 02   ]
         * 
         * [ Id - 01 to 99 ]
         */

        // Generic Errors (00)
        public const string GenericError = "E000000: A general error occurred.";
        public const string UserNotAuthenticated = "E000001: User not authenticated.";
        public const string UserNotAuthorized = "E000002: User does not have authorization to execute this operation.";
        public const string InvalidPageError = "E000004: Page {0} is invalid.";
        public const string FieldRequired = "E000005: {0} is required.";
        public const string FieldMaxLength = "E000006: {0} must have at most {1} characters.";
        public const string FieldInvalid = "E000007: {0} is invalid.";

        // Health Errors (01)
        public const string DatabaseUnavailable = "E000100: Database is not available.";

        // TodoItem Errors (02)



    }
}
