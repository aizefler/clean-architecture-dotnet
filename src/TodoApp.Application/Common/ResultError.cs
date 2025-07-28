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
        public const string ErroGenerico = "E000000: Ocorreu um erro geral.";
        public const string UsuarioNaoAutenticado = "E000001: Usuário não autenticado.";
        public const string UsuarioNaoAutorizado = "E000002: Usuário não possui autorização para executar esta operação.";
        public const string ErroAoObterPagina = "E000004: A Página {0} é inválida.";
        public const string CampoObrigatorio = "E000005: O {0} é obrigatório.";
        public const string CampoMaximoCaracteres = "E000006: O {0} deve ter no máximo {1} caracteres.";
        public const string CampoInvalido = "E000007: {0} inválido.";

        // Health Errors (01)
        public const string DatabaseNaoDisponivel = "E000100: Base de dados não disponível.";

        // TodoItem Errors (02)



    }
}
