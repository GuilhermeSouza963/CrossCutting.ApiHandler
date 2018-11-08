# CrossCutting.ApiHandler
Biblioteca responsável por fazer requisições Post e Get autenticadas com jwt bearer ou não autenticadas, facilitando a passagem de paramêtros.

CrossCutting ApiHandler Documentação

Primeiro para usar o ApiHandler você deve inicia-lo com um dos dois construtores :

Primeiro construtor quando a chamada não é autenticada: 

public Handler(string baseUrl) : this() {
    _baseUrl = baseUrl;
    UrlAuth = string.Format("{0}{1}", baseUrl, "/oauth2/token");
}

A baseUrl deve ser a url base que será usada nas requisições http da sua api, exemplo:

var handler = new Handler("https://suaapi");

Segundo construtor quando a chamada é autenticada:

public Handler(string baseUrl, List<Parameters> authorizationParams) : this(baseUrl) {
    AuthorizationParams = authorizationParams;
    listKeyValues = AuthorizationParams.ListKeyValues();
}

A baseUrl deve ser a url base que será usada nas requisições http da sua api, os parametros são os parametros passados
para sua autenticação JWT Bearer, exemplo:

var handler = new Handler("https://suaapi", new List<Parameters> {
                new Parameters("UserName", ""),
                new Parameters("Password", ""),
                new Parameters("client_Id", ""),
                new Parameters("client_secret", "");


Você pode iniciar o construtor via injeção de dependência também caso queira chamá-lo apenas uma vez, exemplo:

public class suaClasse : Handler, IsuaClasse 
{
    public suaClasse() : base("https://suaapi", new List<Parameters> {
            new Parameters("UserName", ""),
            new Parameters("Password", ""),
            new Parameters("client_Id", ""),
            new Parameters("client_secret", "")
    })
    {

    }
}

Após isso é só registrar no container da sua injeção de dependência, nesse exemplo usando simple injector:

container.Register<IsuaClasse, suaClasse>();

Após iniciar o construtor a chamada da api e simples:

POST:
	- Sem parâmetros na URL:
	 var result = handler.Post<Obj>("/exemplo/key", Obj);
	 
	- Com parêmetros na URL:
	 var result = handler.Post<Obj>("/Usuario/Id", new List<Parameters> {
                new Parameters("exemplo","1"),
				new Parameters("exemplo2","2")
            }, Obj);
GET:
	- Sem parêmtros na URL:
	var result = handler.GetAsString("/exemplo/key");
	
	-Com parâmetros na URL:
	var result = handler.GetAsString("/exemplo/key", new List<Parameters> {
                new Parameters("exemplo","1"),
				new Parameters("exemplo2","2")
            });

Para instalar via Packager Manager Console:

Install-Package CrossCutting.ApiHandler -Version 1.0.0

https://www.nuget.org/packages/CrossCutting.ApiHandler/
