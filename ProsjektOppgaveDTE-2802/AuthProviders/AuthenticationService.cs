using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ProsjektOppgaveBlazor.AuthProviders;
using ProsjektOppgaveDTE_2802.Models.ViewModel;
using RegisterResponse = ProsjektOppgaveDTE_2802.Models.ViewModel.RegisterResponse;

namespace ProsjektOppgaveDTE_2802.AuthProviders;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorageService;
    
    public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _authStateProvider = authenticationStateProvider;
        _localStorageService = localStorageService;
        _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
    
    public async Task<RegisterResponse> RegisterUser(LoginViewModel loginViewModel)
    {
        var authResult = _httpClient.PostAsJsonAsync("https://localhost:7022/api/Auth/register", loginViewModel);
        var authContent = authResult.Result.Content.ReadAsStringAsync();
        var jsonAuthContent = JsonSerializer.Deserialize<RegisterResponse>(authContent.Result, _serializerOptions);
        return jsonAuthContent;
    }
    
    public async Task<LoginResponse> Login(LoginViewModel loginViewModel)
    {
        var authResult = await _httpClient.PostAsJsonAsync("https://localhost:7022/api/Auth/Login", loginViewModel);
        var authContent = await authResult.Content.ReadAsStringAsync();
        var jsonAuthContent = JsonSerializer.Deserialize<LoginResponse>(authContent, _serializerOptions);
        
        if (!authResult.IsSuccessStatusCode)
        {
            return jsonAuthContent;
        }
        await _localStorageService.SetItemAsync("authToken", jsonAuthContent.Token);
        ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(loginViewModel.Username);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", jsonAuthContent.Token);
        
        return jsonAuthContent;
    }

    public async Task Logout()
    {
        await _localStorageService.RemoveItemAsync("authToken");
        ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}