<a href="https://www.nuget.org/packages/BlazorRest" rel="nofollow">
 <img src="https://i.ibb.co/K7MM2rz/v142.png">
 </a>

# BlazorRest
blazor rest is a library for sending http requests in Blazor WebAssembly in the simplest way. blazorrest also gives you the ability to intercept requests, response, and errors that you can customize them.
You will no longer have to serialize or deserialize data 
or even add an auhtorization header.

## Installing
To install the package add the following line to you csproj file replacing x.x.x with the latest version number (found at the top of this file

```
<PackageReference Include="BlazorRest" Version="x.x.x" />
```
You can also install via the .NET CLI with the following command:

```
dotnet add package BlazorRest
```

## Setup
In the program.cs file, you can register in the simplest way as follows
```cs
builder.Services.AddBlazorRest(opt =>
{
    opt.BaseUri = new Uri("http://localhost:54240");
});
```
If it is registered above, you can only send http requests without any other functionality
But to send Bearer tokens automatically, you can implement the ‍‍‍‍``IJwtService`` interface
```cs
public interface IJwtService
{
   ValueTask SetTokenAsync(string? jwtToken);

   ValueTask<string?> GetTokenAsync();

   ValueTask SetRefereshTokenAsync(string? refreshToken);

   ValueTask<string?> GetRefereshTokenAsync();
}
```
And register the subclass that it has implemented ``IJwtTokenService`` interface as follows at the time of registration

```cs
builder.Services.AddBlazorRest(opt =>
{
    opt.BaseUri = new Uri("http://localhost:54240");
    opt.UseJwtService<JwtService>();//you can implement IJwtService and use like this
});
```

implement the ‍‍‍‍‍‍``IRequestInterceptor`` interface To intercept requests.

```cs
public interface IRequestInterceptor
{       
   HttpRequestMessage InterceptRequest(HttpRequestMessage request);
}
```

 implement the ‍‍‍‍‍‍``IResponseInterceptor`` interface To intercept response.
  
 ```cs
 public interface IResponseInterceptor
 {        
   HttpResponseMessage InterceptResponse(HttpResponseMessage response);
 }
 ```
 
 
implement the ‍‍‍‍‍‍``IErrorInterceptor`` interface For Intercept requests, their status code not equal to 200.
 
 
```cs
public interface IErrorInterceptor
{ 
   Task InterceptError(ErrorInterceptorModel? error);
}
```
 
 The most complete way to register blazorrest is as follows.
 
```cs
builder.Services.AddBlazorRest(opt =>
{
    opt.BaseUri = new Uri("http://localhost:54240");
    opt.UseJwtService<JwtService>(); //you can implement IJwtService and use like this
    opt.UseRequestInterceptor<RequestInterceptor>();
    opt.UseResponseInterceptor<ResponseInterceptor>();
    opt.UseErrorInterceptor<ErrorInterceptor>();
    opt.jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };
});
```

## sample

#### login
```cs 
public class AccountService : IAccountService
{
   private readonly IBlazorRest _blazorRest;
   private readonly IJwtService _jwt;
     
   public AccountService(IJwtService jwt,IBlazorRest blazorRest)
   {
      _jwt = jwt;      
     _blazorRest = blazorRest;
   }

   public async Task Login(UserLoginDto loginDto)
   {
      var message =new BlazorRestMessage("/auth/login")
      {
         Method = HttpMethod.Post,
         Content = new JsonContent(loginDto)   
      };
         
      var result = await _blazorRest.SendAsync<LoginResponse>(message);

      if (!result.IsSuccessful)
      {
         //do somthing
      }    
           
      if (result?.Data?.Token is null)
      {
         //do somthing
      }  
       
      await _jwt.SetTokenAsync(result?.Data?.Token);           
  }
}
```

#### UploadFile
```cs
public async Task UploadAvatarAsync(IBrowserFile ProfileImage)
{
   var message = new BlazorRestMessage("/user/avatar")
   { 
      Method = HttpMethod.Post,
      Content = new FileContent(ProfileImage, "FileNameInFormData")
   };
    
   var result = await _blazorRest.SendAsync(message);
}
```
### upload file with model

```cs
public async Task EditUserAsync(EditProfileDto editUserDto, IBrowserFile ProfileImage)
{
    var message = new BlazorRestMessage("/user")
    {
       Method = HttpMethod.Post,
       Content = new FileWithModelContent(ProfileImage, nameof(ProfileImage), editUserDto)    
    };
    
    var result = await _blazorRest.SendAsync<EditProfileResponse>(message);
    //or
    //var result = await _blazorRest.SendAsync(message);
}
```
 

